using HarmonyLib;
using ShipyardExpansion.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.Patches
{

    [HarmonyPatch(typeof(SaveableBoatCustomization), "LoadData")]
    internal static class CustomizationCleaner
    {
        public static void Prefix(ref SaveBoatCustomizationData data, BoatRefs ___refs, BoatCustomParts ___parts)
        {
            SaveCleaner.ConvertSave(data, ___refs);

            if (Plugin.cleanLoad.Value)
            {
                data = SaveCleaner.CleanLoad(data, ___refs, ___parts);
            }
            
        }
        public static void Postfix(BoatRefs ___refs, BoatCustomParts ___parts)
        {
            if (___refs.GetComponent<PurchasableBoat>().isPurchased())
            {
                SailDataManager.LoadSailConfig(___refs);
            }
        }
    }

    [HarmonyPatch(typeof(SaveableBoatCustomization), "GetData")]
    internal static class CustomizationCleaner2
    { 
        public static void Postfix(BoatRefs ___refs, BoatCustomParts ___parts)
        {
            if (___refs.GetComponent<PurchasableBoat>().isPurchased())
            {
                SailDataManager.SaveSailConfig(___refs);
            }

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
                    SaveCleaner.CleanBoatParts(parts);
                }
                //SaveCleaner.CleanSaveOld(Plugin.moddedBoats, Plugin.stockParts, Plugin.stockPartOptions, Plugin.stockMasts);
                Plugin.cleanSave.Value = false;
            }
        }
    }

    [HarmonyPatch(typeof(SaveLoadManager), "LoadModData")]
    internal class LoadPatch
    {
        public static void Postfix()
        {
/*            VersionManager.ReadSaveVersion();
            foreach (BoatRefs boat in GameObject.FindObjectsOfType<BoatRefs>())
            {
                if (boat.GetComponent<SaveableObject>().extraSetting)
                {
                    SailDataManager.LoadSailConfig(boat);
                }
            }*/
            VersionManager.WriteSaveVersion();
        }

    }
    [HarmonyPatch(typeof(SaveLoadManager), "SaveModData")]
    internal class SavePatch
    {
        public static void Prefix()
        {
            VersionManager.WriteSaveVersion();
            PartCountTracker.Write();
        }

    }

    [HarmonyPatch(typeof(SaveLoadManager), "LoadNeeds")]
    internal class LoadNeedsPatch
    {
        public static void Postfix(SaveContainer save)
        {
            GameState.modData = save.modData;
            VersionManager.ReadSaveVersion();
            //VersionManager.WriteSaveVersion();
            Debug.Log("Vanilla Save version = " + save.gameVersion);
        }

    }
}
