using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(SaveableBoatCustomization), "GetData")]
    internal class SaveCleaner
    {
        [HarmonyPrefix]
        internal static void CleanerPatch(BoatCustomParts ___parts)
        {
            if (!Plugin.cleanSave.Value) return;
            foreach (BoatPart part in Plugin.modParts)
            {
                ___parts.availableParts.Remove(part);
            }
            foreach (BoatPart part in ___parts.availableParts)
            {
                foreach (BoatPartOption option in part.partOptions)
                {
                    if (Plugin.modPartOptions.Contains(option))
                    {
                        part.partOptions.Remove(option);
                    }
                    part.activeOption = 0;
                }
            }
        }
/*        [HarmonyPatch(typeof(SaveLoadManager), "DoSaveGame")]
        [HarmonyPostfix]
        internal static void SettingResetter()
        {
            if (Plugin.cleanSave.Value == true) Plugin.cleanSave.Value = false;
        }*/
    }
}
