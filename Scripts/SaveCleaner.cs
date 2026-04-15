using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShipyardExpansion
{
    public static class SaveCleaner
    {
        public static void CleanSaveOld(List<BoatCustomParts> boats, Dictionary<BoatPart, int> stockParts, List<BoatPartOption> stockPartOptions, List<Mast> stockMasts)
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
                    Debug.LogWarning("SaveCleaner: removing sail " + sail.prefabIndex);

                    data.sails.Remove(sail);
                    continue;
                }
                i++;
            }

            return data;
        }

        public static void CleanBoatParts(ref BoatCustomParts parts)
        {
            Debug.Log("SaveCleaner: commencing cleaning...");
            for (int i = 0; i < parts.availableParts.Count;)
            {
                BoatPart part = parts.availableParts[i];
                if (!Plugin.stockParts[parts.gameObject.GetComponent<BoatRefs>()].TryGetValue(part, out part.activeOption)) 
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

        public static void ConvertSave(ref SaveBoatCustomizationData data, BoatRefs refs)
        {
            //if (GameState.playing && !GameState.justStarted) return;
            int index = refs.gameObject.GetComponent<SaveableObject>().sceneIndex;

            if (!Plugin.stockParts.ContainsKey(refs)) return;
            // initial conversion from vanilla (move sails on topmast forestays to new indices, pad part options)
            if (!Plugin.converted.ContainsKey(refs.gameObject))
            {
                List<int> partCounts = PartCountTracker.Read(index);

                // check vanilla part count and pad as needed
                if (Plugin.stockParts.ContainsKey(refs) && partCounts.Count > 0)
                {
                    int partCount = 0;

                    if (partCounts.Count == 1)
                    {
                        partCount = partCounts[0];
                    }
                    else if (partCounts.Count > 1)
                    {
                        partCount = partCounts.Count;
                    }
                    Debug.Log("SE: part count for " + refs.gameObject.name + " = " + partCount);
                    if (Plugin.stockParts[refs].Count > partCount)
                    {
                        //data.partActiveOptions.InsertRange(partCount - 1, new int[Plugin.stockParts[refs].Count - partCount]);
                        for (int i = partCount; i < Plugin.stockParts[refs].Count; i++)
                        {
                            Debug.Log("SE: padding partActiveOptions for " + refs.gameObject.name + " at index " + (i));
                            data.partActiveOptions.Insert(i, Plugin.stockParts[refs].ElementAt(i).Value);
                        }
                    }

                    if (partCounts.Count > 1)
                    {
                        int j = 0;
                        foreach (var part in Plugin.stockParts[refs].Keys)
                        {
                            if (j >= partCounts.Count) break;
                            if (part.partOptions.Count > partCounts[j] && data.partActiveOptions[j] >= partCounts[j])
                            {
                                data.partActiveOptions[j] += (part.partOptions.Count - partCounts[j]);
                            }
                            j++;
                        }
                    }
                }
                // check modded part count and pad as needed
                if (data.partActiveOptions.Count < refs.GetComponent<BoatCustomParts>().availableParts.Count)
                {
                    data.partActiveOptions.AddRange(new int[refs.GetComponent<BoatCustomParts>().availableParts.Count - data.partActiveOptions.Count]);

                }
                Plugin.converted.Add(refs.gameObject, data);
            }



            // if we don't know the file version yet, bug out
            if (VersionManager.saveVersion < 0) return;
            if (VersionManager.saveVersion == 0)
            {
                // convert sanbuq topmast forestays
                if (index == 20 && data.partActiveOptions[3] > 3)
                {
                    data.partActiveOptions[11] = data.partActiveOptions[3] - 3;
                    data.partActiveOptions[3] = 4;
                }
                // adjust installed sail heights for new mast heights
                foreach (var sail in data.sails)
                {
                    var mast = refs.masts[sail.mastIndex];
                    if (Plugin.mastHeights.ContainsKey(mast))
                    {
                        sail.installHeight += mast.mastHeight - Plugin.mastHeights[mast];
                    }
                }
            }
            if (VersionManager.saveVersion2[0] > 0) return;
            // convert Jong shrouds. having this here could be a problem
            if (VersionManager.saveVersion2[1] < 7 && VersionManager.saveVersion2[2] < 90 && index == 70)
            {
                if (data.partActiveOptions[13] > 1) data.partActiveOptions[13] = 1;
                if (data.partActiveOptions[14] > 1) data.partActiveOptions[14] = 1;
            }

            // if the convert setting is off, bug out
            if (!Plugin.convertSave.Value) return;

            // convert to SE v0.5
            if (VersionManager.saveVersion2[1] < 5)
            {
                //Debug.Log("brig??");
                foreach (SaveSailData sailData in data.sails)
                {
                    Debug.Log("sail data: " + sailData.prefabIndex + ", mast: " + sailData.mastIndex);
                    if (sailData.mastIndex > 30 && sailData.mastIndex < 50)
                    {
                        sailData.mastIndex += 20;
                        Debug.Log("new mast index = " + sailData.mastIndex);
                    }
                }
                if (index == 20)
                {
                    if (data.partActiveOptions[19] == 3) data.partActiveOptions[19] = 0;

                }
                else if (index == 40)
                {
                    if (data.partActiveOptions[1] == 5)
                    {
                        data.partActiveOptions[1] = 4;
                        data.partActiveOptions[11] = 3;
                    }
                    //else if (data.partActiveOptions[1] == 6) data.partActiveOptions[1] = 5;
                    if (data.partActiveOptions[6] == 2)
                    {
                        data.partActiveOptions[6] = 0;
                        data.partActiveOptions[11] = 1;
                    }
                    else if (data.partActiveOptions[6] == 3) data.partActiveOptions[6] = 2;
                }

                SailDataManager.SaveSailConfig(refs); // this assumes the sails have been installed, I think
            }

            // convert to SE v0.6.93. This is just sail position adjustments
            if (VersionManager.saveVersion2[1] < 7 && VersionManager.saveVersion2[2] < 93)
            {
                if (index == 20 || index == 50)
                {
                    int[] mastIndices = new int[0];
                    if (index == 20) mastIndices = new int[5] { 13, 14, 64, 59, 70 };
                    else if (index == 50) mastIndices = new int[6] { 55, 56, 57, 58, 59, 60 };

                    foreach (var sail in data.sails)
                    {
                        if (mastIndices.Contains(sail.mastIndex))
                        {
                            sail.installHeight += 1f;
                        }
                    }
                }
            }

        }
    }
}
