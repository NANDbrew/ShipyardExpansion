using ShipyardExpansion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NANDBrewShipDataParser
{   //made by NANDBrew: https://gist.github.com/NANDbrew/01ec847ecd91b8ba544dc87626e3177c#file-compressedconfigutility-cs
    internal class ShareableConfigUtility
    {
        public static readonly Dictionary<int, string> boatNames = new Dictionary<int, string>
        {
            { 10, "D" },
            { 20, "S" },
            { 40, "C" },
            { 50, "B" },
            { 80, "J" },
            { 90, "K" },
        };
        public static readonly Dictionary<string, int> boatIndexes = new Dictionary<string, int>
        {
            { "D", 10 },
            { "S", 20 },
            { "C", 40 },
            { "B", 50 },
            { "J", 80 },
            { "K", 90 },
        };
        static readonly char modIndicator = '@';
        static readonly char opener = '(';
        static readonly string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_";
        // is shipyard expansion present?
        private static bool? _SEenabled;
        public static bool SEenabled
        {
            get
            {
                if (_SEenabled == null)
                {
                    _SEenabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.nandbrew.shipyardexpansion");
                }
                return (bool)_SEenabled;
            }
        }

        public static string CreateShipConfig(SaveBoatCustomizationData data, GameObject boat, bool compressPartOptions)
        {
            int boatIndex = boat.GetComponent<SaveableObject>().sceneIndex;
            string text = "";
            // check if Shipyard Expansion mod
            if (SEenabled) text += modIndicator;
            text += boatNames[boatIndex];

            for (int i = 0; i < data.partActiveOptions.Count;)
            {
                int opt = data.partActiveOptions[i];
                if (compressPartOptions)
                {
                    // compress 1-4 contiguous part options into 1
                    int j = i + 1;
                    // if the next item shares the value of the current one, transpose current value up (3 => 19 => 35 => 51) and skip ahead
                    while (j < data.partActiveOptions.Count && data.partActiveOptions[j] == data.partActiveOptions[i] && opt + 16 < chars.Length)
                    {
                        opt += 16;
                        j++;
                    }
                    text += IntToBase64(opt, false);
                    i = j;
                }
                else
                {
                    text += opt;
                    i++;
                }
            }
            string output = "";
            Mast[] masts = boat.GetComponent<BoatRefs>().masts;
            text += opener;
            foreach (Mast mast in masts)
            {
                if (!mast || !mast.gameObject.activeInHierarchy)
                {
                    continue;
                }

                foreach (GameObject sail in mast.sails)
                {
                    SailScaler comp2 = sail.GetComponent<SailScaler>();
                    output += IntToBase64(Mathf.RoundToInt(comp2.scale.x * 100), true);
                    output += IntToBase64(Mathf.RoundToInt(comp2.scale.y * 100), true);
                    //output += IntToBase64(Mathf.RoundToInt(comp2.angle, true));
                }
            }
            foreach (SaveSailData sailData in data.sails)
            {
                int prefabIndex = sailData.prefabIndex;
                int mastIndex = sailData.mastIndex;
                int installHeight = Mathf.RoundToInt(sailData.installHeight * 100);
                text += IntToBase64(prefabIndex, true);
                text += IntToBase64(mastIndex, false);
                text += IntToBase64(installHeight, true);
                text += output;
                // angles are skipped because we want to make the shipyard validate them anyway
                // new format doesn't support colors
                //if (includeColors) text += seperator + sailData.sailColor.ToString(CultureInfo.InvariantCulture);
            }
            return text;
        }

        // this assumes short values will fit in 1 char, and long ones will fit in 2
        static string IntToBase64(int value, bool isLong)
        {
            if (isLong)
            {
                int mod = value % chars.Length;
                int main = (int)Mathf.Floor(value / chars.Length);
                string text = "";
                text += chars[main];
                text += chars[mod];

                return text;
            }
            else return chars[value].ToString();
        }
        // as above, assumes all inputs are 2 or less in length
        static int Base64ToInt(string value)
        {
            int num = 0;
            if (value.Length == 2)
            {
                num += chars.IndexOf(value[0]) * chars.Length;
                num += chars.IndexOf(value[1]);
            }
            else if (value.Length == 1)
            {
                num += chars.IndexOf(value[0]);
            }
            return num;
        }
        public static SaveBoatCustomizationData ParseShipConfig(string shipConfig, GameObject boat, bool includeOptional)
        {
            BoatRefs boatRefs = boat.GetComponent<BoatRefs>();
            SaveBoatCustomizationData result = new SaveBoatCustomizationData();
            if (shipConfig.IndexOf(modIndicator) == 0)
            {
                //this is only needed to display a warning. Everything vanilla compatible will be installed
                if (SEenabled)
                {
                    NotificationUi.instance.ShowNotification("This code was saved\nwith the ShipyardExpansion mod.\nOnly vanilla parts will be loaded.");
                }
            }

            List<BoatPart> availableParts = boat.GetComponent<BoatCustomParts>().availableParts;
            // trim off mod indicator
            shipConfig = shipConfig.TrimStart(modIndicator);
            // remove check for correct boat; it's being done elsewhere
            /*if (boatIndexes[shipConfig.First().ToString()] != boat.GetComponent<SaveableObject>().sceneIndex)
            {
                //check moved to the LoadCode() method in Main.cs
                return null;
            }*/
            shipConfig = shipConfig.Substring(1);
            string[] split1 = shipConfig.Split(opener);

            var partOptions = split1.First();
            for (int j = 0; j < partOptions.Length && j < availableParts.Count; j++)
            {
                int opt = Base64ToInt(partOptions[j].ToString());
                int val = opt % 16;
                int count = (int)Mathf.Floor(opt / 16) + 1;
                for (int k = 0; k < count; k++)
                {
                    // includeOptional is also being checked elsewhere, but we'll leave it for completeness
                    if (includeOptional || availableParts[j].category != 1 && opt < availableParts[j].partOptions.Count)
                    {
                        result.partActiveOptions.Add(val);
                    }
                    else
                    {
                        result.partActiveOptions.Add(availableParts[j].activeOption);
                    }
                }

            }
            
            string sails = split1.Last();
            while (sails.Length >= 5)
            {
                int mastIndex = Base64ToInt(sails.Substring(2, 1));
                if (mastIndex < boatRefs.masts.Length && boatRefs.masts[mastIndex] != null)
                {
                    result.sails.Add(new SaveSailData
                    {
                        prefabIndex = Base64ToInt(sails.Substring(0, 2)),
                        mastIndex = mastIndex,
                        installHeight = (float)Base64ToInt(sails.Substring(3, 2)) / 100f,
                    });
                }
                sails = sails.Substring(5);
            }
            return result;
        }
    }
}
