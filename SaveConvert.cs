using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(SaveableBoatCustomization), "LoadData")]
    internal static class SaveConvert
    {
        static int conversionCounter = 0;
        public static int saveVersion;

        [HarmonyPrefix]
        internal static void Prefix(SaveBoatCustomizationData data, BoatRefs ___refs)
        {
            //if (!Plugin.convertSave.Value) return;

            if (saveVersion >= 30) return;
            if (___refs.GetComponent<SaveableObject>().sceneIndex == 80)
            {
                bool flag = false;
                if (data.partActiveOptions[12] == 1)
                {
                    data.partActiveOptions[12] = 0;
                    data.partActiveOptions[3] = 3;
                    flag = true;
                }
                foreach (SaveSailData sailData in data.sails)
                {
                    if (sailData.mastIndex == 33)
                    {
                        sailData.mastIndex = 37;
                        sailData.installHeight = 17f;
                        flag = true;
                    }
                    else if (sailData.mastIndex == 13 && flag)
                    {
                        sailData.mastIndex = 37;
                        sailData.installHeight -= 1;
                    }
                }
                conversionCounter++;
            }
            else if (___refs.GetComponent<SaveableObject>().sceneIndex == 20)
            {
                while (data.partActiveOptions.Count < ___refs.GetComponent<BoatCustomParts>().availableParts.Count)
                {
                    data.partActiveOptions.Add(0);
                }
                foreach (SaveSailData sailData in data.sails)
                {
                    if (sailData.mastIndex == 38) // main topstay
                    {
                        data.partActiveOptions[19] = 1;
                    }
                    else if (sailData.mastIndex == 37) // mainstay
                    {
                        data.partActiveOptions[20] = 1;
                    }
                }
                conversionCounter++;
            }
            Debug.Log("ShipyardExpansion converted " + conversionCounter + " ships");
            //Plugin.convertSave.Value = false;
        }
    }

}
