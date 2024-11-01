using HarmonyLib;
using ShipyardExpansion.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{

    [HarmonyPatch(typeof(SaveableBoatCustomization), "LoadData")]
    internal static class CustomizationCleaner
    {
        public static void Prefix(ref SaveBoatCustomizationData data, BoatRefs ___refs, BoatCustomParts ___parts)
        {
            SaveCleaner.Convert(data, ___refs);

            if (Plugin.cleanLoad.Value)
            {
                data = SaveCleaner.CleanLoad(data, ___refs, ___parts);
            }
            
        }
        public static void Postfix(BoatRefs ___refs, BoatCustomParts ___parts)
        {
            SailDataManager.LoadSailConfig(___refs);

        }
    }

    [HarmonyPatch(typeof(SaveableBoatCustomization), "GetData")]
    internal static class CustomizationCleaner2
    { 
        public static void Postfix(BoatRefs ___refs, BoatCustomParts ___parts)
        {
            SailDataManager.SaveSailConfig(___refs);


        }
    }
    [HarmonyPatch(typeof(SaveLoadManager), "DoSaveGame")]
    internal static class SaveCleanerPatch
    {
        [HarmonyPrefix]
        internal static void SavePatch()
        {
            if (Plugin.cleanSave.Value)
            {
                foreach (BoatCustomParts parts in Plugin.moddedBoats)
                {
                    SaveCleaner.CleanSaveData(parts);
                }
                //SaveCleaner.CleanSave(Plugin.moddedBoats, Plugin.stockParts, Plugin.stockPartOptions, Plugin.stockMasts);
                Plugin.cleanSave.Value = false;
            }
        }
    }

    [HarmonyPatch(typeof(SaveLoadManager), "LoadModData")]
    internal class SavePatch
    {
        public static void Postfix()
        {
            VersionManager.ReadSaveVersion();
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Boat"))
            {
                if (gameObject.GetComponent<BoatRefs>() != null)
                {
                    SailDataManager.LoadSailConfig(gameObject.GetComponent<BoatRefs>());
                }
            }
        }

    }
    [HarmonyPatch(typeof(SaveLoadManager), "SaveModData")]
    internal class LoadPatch
    {
        public static void Prefix()
        {
            VersionManager.WriteSaveVersion();
        }

    }
}
