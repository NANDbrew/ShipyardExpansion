using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    public static class SaveCleaner
    {
        public static int saveVersion = 0;
        public static void CleanSave(List<BoatCustomParts> boats, Dictionary<BoatPart, int> stockParts, List<BoatPartOption> stockPartOptions, List<Mast> stockMasts)
        {
            for (int i = 0; i < boats.Count; i++)
            {
                var partsList = boats[i];
                for (int j = 0; j < partsList.availableParts.Count;)
                {
                    var part = partsList.availableParts[j];
                    if (!stockParts.ContainsKey(part))
                    {
                        partsList.availableParts.Remove(part);
                        continue;
                    }
                    for (int k = 0; k < part.partOptions.Count;)
                    {
                        var option = part.partOptions[k];
                        if (!stockPartOptions.Contains(option))
                        {
                            part.partOptions.Remove(option);
                            continue;
                        }
                        k++;
                    }
                    if (part.activeOption >= part.partOptions.Count) part.activeOption = stockParts[part];
                    //if (part.activeOption >= part.partOptions.Count) part.activeOption = part.partOptions.Count - 1;
                    j++;
                }
                var mastList = partsList.gameObject.GetComponent<BoatRefs>().masts;
                for (int l = 0; l < mastList.Length; l++)
                {
                    if (mastList[l] != null && !stockMasts.Contains(mastList[l]))
                    {
                        mastList[l].RemoveAllSails();
                        mastList[l] = null;
                    }
                    for (int m = mastList[l].sails.Count - 1; m >= 0; m--)
                    {
                        var sail = mastList[l].sails[m].GetComponent<Sail>();
                        if (sail.prefabIndex > Plugin.stockSailsListSize - 1 || PrefabsDirectory.instance.sails[sail.prefabIndex])
                        {
                            mastList[l].DetachSailFromMast(sail.gameObject);
                        }
                    }
                }
            }
        }

        public static SaveBoatCustomizationData CleanLoad(SaveBoatCustomizationData data, BoatRefs boatRefs, BoatCustomParts parts)
        {
            //Debug.Log("SaveCleaner: commencing cleaning...");
            if (data.masts.Length > boatRefs.masts.Length)
            {
                Array.Resize(ref data.masts, boatRefs.masts.Length);
                Debug.LogWarning("SaveCleaner: shortened mast list");

            }

            if (data.partActiveOptions != null)
            {
                //Debug.Log("SaveCleaner: cleaning activeOptions list");
                var activeOptions = new List<int>();

                for (int i = 0; i < parts.availableParts.Count; i++)
                {
                    if (i >= data.partActiveOptions.Count) { Debug.Log("out of saved parts"); break; }
                    var option = data.partActiveOptions[i];
                    if (option < parts.availableParts[i].partOptions.Count)
                    {
                        activeOptions.Add(option);
                        //Debug.Log("SaveCleaner: activeOption " + i + " was safe");
                    }
                    else
                    {
                        activeOptions.Add(parts.availableParts[i].activeOption);
                        Debug.LogWarning("SaveCleaner: fixed activeOption " + i);

                    }
                }
                data.partActiveOptions = activeOptions;
            }

            //Debug.Log("SaveCleaner: cleaning sails list");
            for (int i = 0; i < data.sails.Count;)
            {
                var sail = data.sails[i];
                if (sail.mastIndex >= boatRefs.masts.Length || boatRefs.masts[sail.mastIndex] == null || sail.prefabIndex >= PrefabsDirectory.instance.sails.Length || PrefabsDirectory.instance.sails[sail.prefabIndex] == null)
                {
                    Debug.LogWarning("SaveCleaner: removing sail " + sail);

                    data.sails.Remove(sail);
                    continue;
                }
                i++;
            }

            return data;
        }

        public static void Convert(SaveBoatCustomizationData data, BoatRefs refs)
        {
            if (saveVersion == 0 && data.partActiveOptions.Count < refs.GetComponent<BoatCustomParts>().availableParts.Count)
            {
                data.partActiveOptions.AddRange(new int[refs.GetComponent<BoatCustomParts>().availableParts.Count - data.partActiveOptions.Count]);

                if (refs.GetComponent<SaveableObject>().sceneIndex == 20 && data.partActiveOptions[3] > 3)
                {
                    data.partActiveOptions[11] = data.partActiveOptions[3] - 3;
                    data.partActiveOptions[3] = 4;
                }
            }
            if (GameState.currentShipyard != null) return;

            if (!Plugin.convertSave.Value) return;

            if (saveVersion >= 43) return;
            foreach (SaveSailData sailData in data.sails)
            {
                sailData.installHeight /= refs.masts[sailData.mastIndex].transform.localScale.z;
            }

            if (saveVersion >= 30) return;
            if (refs.GetComponent<SaveableObject>().sceneIndex == 80)
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
                //conversionCounter++;
            }
            else if (refs.GetComponent<SaveableObject>().sceneIndex == 20)
            {
                while (data.partActiveOptions.Count < refs.GetComponent<BoatCustomParts>().availableParts.Count)
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
                //conversionCounter++;
            }
            //Debug.Log("ShipyardExpansion converted " + conversionCounter + " ships");
            //Plugin.convertSave.Value = false;
        }
    }
}
