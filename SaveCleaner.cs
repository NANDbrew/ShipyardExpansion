using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
/*    [HarmonyPatch(typeof(SaveableBoatCustomization), "LoadData")]
    internal static class CustomizationCleaner
    {
        [HarmonyPrefix]
        public static bool LoadData(SaveBoatCustomizationData data, BoatRefs ___refs, BoatCustomParts ___parts)
        {
            if (!Plugin.cleanLoad.Value) return true;
            //Debug.Log("Loading data for " + base.gameObject.name);
            for (int i = 0; i < data.masts.Length; i++)
            {
                if (___refs.masts.Length > i && ___refs.masts[i] != null)
                {
                    ___refs.masts[i].RemoveAllSails();
                }
            }

            if (data.partActiveOptions != null)
            {
                for (int j = 0; j < data.partActiveOptions.Count; j++)
                {
                    if (___parts.availableParts.Count > j)
                    {
                        ___parts.availableParts[j].activeOption = data.partActiveOptions[j];
                    }
                }
            }

            if ((bool)___parts)
            {
                ___parts.RefreshParts();
            }

            foreach (SaveSailData sail in data.sails)
            {
                int mastIndex = sail.mastIndex;
                if (___refs.masts[mastIndex] == null)
                {
                    //Debug.LogError("___refs.masts[index] is null at index " + mastIndex + " / GameObject name: " + base.gameObject.name);
                }
                else
                {
                    ___refs.masts[sail.mastIndex].LoadSail(sail);
                }
            }
            Debug.Log("SaveableBoatCustomization: LoadData finished successfully.");
            return false;
       }
    }

*/



    [HarmonyPatch(typeof(SaveLoadManager), "DoSaveGame")]
    internal static class SaveCleaner
    {
        static readonly string[] boats = new string[] { "BOAT medi medium (50)", "BOAT dhow medium (20)", "BOAT junk medium (80)", "BOAT medi small (40)", "BOAT dhow small (10)", "BOAT junk small singleroof (90)" };

        [HarmonyPrefix]
        internal static void SavePatch()
        {
            if (!Plugin.cleanSave.Value) return;



            for (int i = 0; i < Plugin.moddedBoats.Count; i++)
            {
                var ___partsList = Plugin.moddedBoats[i];
                for (int j = 0; j < ___partsList.availableParts.Count;)
                {
                    var part = ___partsList.availableParts[j];
                    if (!Plugin.stockParts.ContainsKey(part))
                    {
                        ___partsList.availableParts.Remove(part);
                        continue;
                    }
                    for (int k = 0; k < part.partOptions.Count;)
                    {
                        var option = part.partOptions[k];
                        if (!Plugin.stockPartOptions.Contains(option))
                        {
                            part.partOptions.Remove(option);
                            continue;
                        }
                        k++;
                    }
                    if (part.activeOption >= part.partOptions.Count) part.activeOption = Plugin.stockParts[part];
                    //if (part.activeOption >= part.partOptions.Count) part.activeOption = part.partOptions.Count - 1;
                    j++;
                }
                var mastList = ___partsList.gameObject.GetComponent<BoatRefs>().masts;
                for (int l = 0; l < mastList.Length; l++)
                {
                    if (mastList[l] != null && !Plugin.stockMasts.Contains(mastList[l]))
                    {
                        mastList[l].RemoveAllSails();
                        mastList[l] = null;
                    }    
                }
            }

            Plugin.cleanSave.Value = false;

        }
    }

}
