using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(SaveLoadManager), "DoSaveGame")]
    internal static class SaveCleanerPatch
    {
        [HarmonyPrefix]
        internal static void SavePatch()
        {
            if (!Plugin.cleanSave.Value) return;
            SaveCleaner.CleanSave(Plugin.moddedBoats, Plugin.stockParts, Plugin.stockPartOptions, Plugin.stockMasts);
            Plugin.cleanSave.Value = false;
        }
    }

    [HarmonyPatch(typeof(SaveableBoatCustomization), "LoadData")]
    internal static class CustomizationCleaner
    {
        [HarmonyPrefix]
        public static void Prefix(ref SaveBoatCustomizationData data, BoatRefs ___refs, BoatCustomParts ___parts)
        {
            SaveCleaner.Convert(data, ___refs);

            if (!Plugin.cleanLoad.Value) return;
            data = SaveCleaner.CleanLoad(data, ___refs, ___parts);
        }
    }
}
