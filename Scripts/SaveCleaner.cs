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
        public static int saveVersion = -1;
        public static void CleanSave(List<BoatCustomParts> boats, Dictionary<BoatPart, int> stockParts, List<BoatPartOption> stockPartOptions, List<Mast> stockMasts)
        {
            foreach (BoatCustomParts partsList in boats)
            {
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
                        if (sail.prefabIndex >= Plugin.stockSailsListSize)
                        {
                            mastList[l].DetachSailFromMast(sail.gameObject);
                        }
                    }
                }

                GameState.modData.Remove("SEboatSails." + partsList.gameObject.GetComponent<SaveableObject>().sceneIndex);

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

        public static void CleanSaveData(BoatCustomParts parts)
        {
            Debug.Log("SaveCleaner: commencing cleaning...");
            for (int i = 0; i < parts.availableParts.Count;)
            {
                BoatPart part = parts.availableParts[i];
                if (!Plugin.stockParts.TryGetValue(part, out part.activeOption)) 
                { 
                    parts.availableParts.Remove(part);
                    continue;
                }
                i++;
            }
            parts.RefreshParts();
            foreach (Mast mast in parts.gameObject.GetComponent<BoatRefs>().masts)
            {
                if (mast != null)
                {
                    /*if (mast.orderIndex > 29) mast.RemoveAllSails();
                    else if (mast.sails.Count == 0) mast.Start();*/
                    mast.RemoveAllSails();
                    mast.Start();
                }
            }
        }


        public static void Convert(SaveBoatCustomizationData data, BoatRefs refs)
        {
            //int index = refs.gameObject.GetComponent<SaveableObject>().sceneIndex;
            Debug.Log("converter: converting index " + refs.gameObject.name);
            if (!Plugin.converted.ContainsKey(refs.gameObject))
            {
                if (data.partActiveOptions.Count < refs.GetComponent<BoatCustomParts>().availableParts.Count)
                {
                    data.partActiveOptions.AddRange(new int[refs.GetComponent<BoatCustomParts>().availableParts.Count - data.partActiveOptions.Count]);

                    if (refs.GetComponent<SaveableObject>().sceneIndex == 20 && data.partActiveOptions[3] > 3)
                    {
                        data.partActiveOptions[11] = data.partActiveOptions[3] - 3;
                        data.partActiveOptions[3] = 4;
                    }
                }
                Plugin.converted.Add(refs.gameObject, false);
            }
            if (!Plugin.convertSave.Value || saveVersion < 0) return;
            if (saveVersion >= 43 || Plugin.converted[refs.gameObject] == true)
            {
                Debug.Log("Save converter: skipped-- v" + saveVersion);
            }
            else
            {
                Debug.Log("Save converter: converting-- v" + saveVersion + "...");
                foreach (SaveSailData sailData in data.sails)
                {
                    sailData.installHeight /= refs.masts[sailData.mastIndex].transform.localScale.z;
                }

                if (saveVersion >= 30)
                {
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
                }
                Plugin.converted[refs.gameObject] = true;
            }
            //Debug.Log("ShipyardExpansion converted " + conversionCounter + " ships");
            //Plugin.convertSave.Value = false;
        }
    }
}
