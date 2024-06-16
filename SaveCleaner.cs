using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipyardExpansion
{

    [HarmonyPatch(typeof(SaveLoadManager), "DoSaveGame")]
    internal static class SaveCleaner
    {
        [HarmonyPrefix]
        internal static void SavePatch()
        {
            if (!Plugin.cleanSave.Value) return;
            if (Plugin.bruteForce.Value)
            {
                for (int i = 0; i < Plugin.modCustomParts.Count; i++)
                {
                    Plugin.modCustomParts[i].availableParts = new List<BoatPart>();
                }
                Plugin.cleanSave.Value = false;
                Plugin.bruteForce.Value = false;
                return;
            }
           
            for (int i = 0; i < Plugin.modCustomParts.Count; i++)
            {
                var parts = Plugin.modCustomParts[i];
                for (int j = 0; j < parts.availableParts.Count;)
                {
                    var part = parts.availableParts[j];
                    if (Plugin.modParts.Contains(part))
                    {
                        parts.availableParts.Remove(part);
                        continue;
                    }
                    for (int k = 0; k < part.partOptions.Count;)
                    {
                        var option = part.partOptions[k];
                        if(Plugin.modPartOptions.Contains(option))
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
            Plugin.cleanSave.Value = false;

        }
    }

}
