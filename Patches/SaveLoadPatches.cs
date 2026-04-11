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
/*        public static void Prefix(ref SaveBoatCustomizationData data, BoatRefs ___refs, BoatCustomParts ___parts)
        {
            SaveCleaner.ConvertSave(ref data, ___refs);

            if (Plugin.cleanLoad.Value)
            {
                data = SaveCleaner.CleanLoad(data, ___refs, ___parts);
            }
        }*/
        public static void Postfix(BoatRefs ___refs, BoatCustomParts ___parts)
        {
            SailDataManager.LoadSailConfig(___refs);
            SailDataManager.SaveSailConfig(___refs);
        }
    }

    [HarmonyPatch(typeof(SaveableBoatCustomization), "GetData")]
    internal static class CustomizationCleaner2
    { 
        public static void Postfix(BoatRefs ___refs, BoatCustomParts ___parts)
        {
            if (GameState.currentShipyard == null) return;
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
                for (int i = 0; i < Plugin.moddedBoats.Count; i++)
                {
                    BoatCustomParts parts = Plugin.moddedBoats[i];
                    SaveCleaner.CleanBoatParts(ref parts);
                }
                //SaveCleaner.CleanSaveOld(Plugin.moddedBoats, Plugin.stockParts, Plugin.stockPartOptions, Plugin.stockMasts);
                Plugin.cleanSave.Value = false;
            }
        }
    }

/*    [HarmonyPatch(typeof(SaveLoadManager), "LoadModData")]
    internal class LoadPatch
    {
        public static void Postfix()
        {
            *//*            VersionManager.ReadSaveVersion();
                        foreach (BoatRefs boat in GameObject.FindObjectsOfType<BoatRefs>())
                        {
                            if (boat.GetComponent<SaveableObject>().extraSetting)
                            {
                                SailDataManager.LoadSailConfig(boat);
                            }
                        }*//*
            //PartCountTracker.Write();

            //VersionManager.WriteSaveVersion();
        }

    }
    [HarmonyPatch(typeof(SaveLoadManager), "SaveModData")]
    internal class SavePatch
    {
        public static void Prefix()
        {
            //VersionManager.WriteSaveVersion();
            //PartCountTracker.Write();
        }

    }*/

    [HarmonyPatch(typeof(SaveLoadManager), "LoadNeeds")]
    internal class LoadNeedsPatch
    {
        public static void Postfix(SaveContainer save)
        {
            GameState.modData = save.modData;
            VersionManager.ReadSaveVersion();

            for (int i = 0; i < save.savedObjects.Count; i++)
            {
                SaveObjectData data = save.savedObjects[i];
                BoatRefs refs = SaveLoadManager.instance.GetCurrentObjects()[data.sceneIndex]?.GetComponent<BoatRefs>();
                if (refs != null)
                {
                    SaveCleaner.ConvertSave(ref data.customization, refs);

                    if (Plugin.cleanLoad.Value)
                    {
                        data.customization = SaveCleaner.CleanLoad(data.customization, refs, refs.GetComponent<BoatCustomParts>());
                    }
                }
            }
            PartCountTracker.Write();

            VersionManager.WriteSaveVersion();
            Debug.Log("SE: Vanilla Save version = " + save.gameVersion);
        }

    }
}
