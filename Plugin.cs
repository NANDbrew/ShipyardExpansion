using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ShipyardExpansion
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    //[BepInDependency("com.app24.sailwindmoddinghelper", "2.0.3")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.nandbrew.shipyardexpansion";
        public const string PLUGIN_NAME = "Shipyard Expansion";
        public const string PLUGIN_VERSION = "0.0.7";

        public static List<BoatPartOption> modPartOptions;
        public static List<BoatPart> modParts;
        public static List<List<BoatPart>> modCustomParts;

/*        public static Dictionary<BoatCustomParts, List<BoatPart>> stockParts;
        public static Dictionary<BoatRefs, Mast[]> stockMasts;
        public static Dictionary<SaveableBoatCustomization, SaveBoatCustomizationData> stockBoatData;
*/

        //--settings--
        internal static ConfigEntry<bool> cleanSave;
        //internal static ConfigEntry<bool> bruteForce;
        internal static ConfigEntry<bool> vertLateens;
        //internal static ConfigEntry<bool> showGizmos;


        private void Awake()
        {
/*            stockParts = new Dictionary<BoatCustomParts, List<BoatPart>>();
            stockMasts = new Dictionary<BoatRefs, Mast[]>();
            stockBoatData = new Dictionary<SaveableBoatCustomization, SaveBoatCustomizationData>();
*/

            modPartOptions = new List<BoatPartOption>();
            modParts = new List<BoatPart>();
            modCustomParts = new List<List<BoatPart>>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            cleanSave = Config.Bind("Settings", "Clean save", false);
            //bruteForce = Config.Bind("Settings", "Brute force", false);
            vertLateens = Config.Bind("Settings", "Vertical lateens", true);
            //showGizmos = Config.Bind("Dev tools", "Show gizmos", false);
        }
    }
}
