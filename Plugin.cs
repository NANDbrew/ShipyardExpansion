using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardExpansion
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    //[BepInDependency("com.app24.sailwindmoddinghelper", "2.0.3")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.nandbrew.shipyardexpansion";
        public const string PLUGIN_NAME = "Shipyard Expansion";
        public const string PLUGIN_VERSION = "0.4.5";

        internal const int mastListSize = 64;
        internal const int sailListSize = 160;

        public static List<BoatCustomParts> moddedBoats;
        //public static List<BoatPartOption> stockPartOptions;
        public static Dictionary<BoatPart, int> stockParts;
        //public static List<Mast> stockMasts;
        public static int stockSailsListSize;
        //public static Dictionary<int, SaveBoatCustomizationData> stockConfigs;
        public static Dictionary<GameObject, bool> converted;
        public static Transform prefabContainer;

        //--settings--
        internal static ConfigEntry<bool> cleanSave;
        internal static ConfigEntry<bool> convertSave;
        internal static ConfigEntry<bool> lenientLateens;
        internal static ConfigEntry<bool> lenientSquares;
        internal static ConfigEntry<bool> vertLateens;
        internal static ConfigEntry<bool> vertFins;

        internal static ConfigEntry<bool> unrollSails;

        internal static ConfigEntry<bool> addSails;
        internal static ConfigEntry<bool> cleanLoad;
        internal static ConfigEntry<bool> percentSailNames;
        internal static ConfigEntry<bool> autoFit;


        private void Awake()
        {
            //stockPartOptions = new List<BoatPartOption>();
            stockParts = new Dictionary<BoatPart, int>();
            //stockMasts = new List<Mast>();
            moddedBoats = new List<BoatCustomParts>();
            converted = new Dictionary<GameObject, bool>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            convertSave = Config.Bind("Fixers", "Convert saves", true, new ConfigDescription("Enable this before loading a save from a version of Shipyard Expansion before v0.4.3"));
            cleanSave = Config.Bind("Fixers", "Clean save", false, new ConfigDescription("Enable this before saving if you want to uninstall this mod (will disable itself when done)"));
            cleanLoad = Config.Bind("Fixers", "Clean load", true, new ConfigDescription("Sanitize ship customizations on load"));

            vertLateens = Config.Bind("Slant", "Vertical lateens", true, new ConfigDescription("Install lateens vertical instead of slanting with the mast"));
            vertFins = Config.Bind("Slant", "Vertical fins", true, new ConfigDescription("Install fin sails vertical instead of slanting with the mast"));
            unrollSails = Config.Bind("Settings", "Unfurl sails in shipyard", true, new ConfigDescription("Unfurl existing sails when entering the shipyard"));
            addSails = Config.Bind("Settings", "Add lug sails", true, new ConfigDescription("Adds new sails in the 'Other' category. (requires a restart)"));
            percentSailNames = Config.Bind("Settings", "Show percent scale in sail name", true, new ConfigDescription(""));
            autoFit = Config.Bind("Settings", "Downsize sails to fit", true, new ConfigDescription("Automatically scale too-big sails to fit before installing"));


        }
    }
}
