using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipyardExpansion
{
    /*    [HarmonyPatch(typeof(SaveableBoatCustomization), "GetData")]
        internal static class CustomizationCleaner
        {
            [HarmonyPostfix]
            public static void Postfix(ref SaveBoatCustomizationData __result)
            {
                if (!Plugin.bruteForce.Value) return;
                __result = new SaveBoatCustomizationData();
                Plugin.bruteForce.Value = false;
            }
        }*/


    [HarmonyPatch(typeof(SaveLoadManager), "DoSaveGame")]
    internal static class SaveCleaner
    {
        static readonly string[] boats = new string[] { "BOAT medi medium (50)", "BOAT dhow medium (20)", "BOAT junk medium (80)", "BOAT medi small (40)", "BOAT dhow small (10)", "BOAT junk small singleroof (90)" };

        [HarmonyPrefix]
        internal static void SavePatch()
        {
            if (!Plugin.cleanSave.Value) return;

            for (int i = 0; i < Plugin.modCustomParts.Count; i++)
            {
                var partsList = Plugin.modCustomParts[i];
                for (int j = 0; j < partsList.Count;)
                {
                    var part = partsList[j];
                    if (Plugin.modParts.Contains(part))
                    {
                        partsList.Remove(part);
                        continue;
                    }
                    for (int k = 0; k < part.partOptions.Count;)
                    {
                        var option = part.partOptions[k];
                        if (Plugin.modPartOptions.Contains(option))
                        {
                            part.partOptions.Remove(option);
                            continue;
                        }
                        k++;
                    }
                    if (part.activeOption >= part.partOptions.Count) part.activeOption = part.partOptions.Count - 1;
                    j++;
                }
            }

            /*   if (Plugin.bruteForce.Value)
               {
                   foreach (var boat in boats)
                   {
                       Refs.shiftingWorld.Find(boat).GetComponent<BoatCustomParts>().availableParts = new List<BoatPart>();
                   }
                   *//*for (int i = 0; i < Plugin.modCustomParts.Count; i++)
                   {
                       Plugin.modCustomParts[i].availableParts = new List<BoatPart>();
                   }*//*
               }*/

            //Plugin.bruteForce.Value = false;
            Plugin.cleanSave.Value = false;

        }
    }

}
