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
        public const string PLUGIN_VERSION = "0.1.0";

        internal const int mastListSize = 64;

        public static List<BoatCustomParts> moddedBoats;
        public static List<BoatPartOption> stockPartOptions;
        public static Dictionary<BoatPart, int> stockParts;
        public static List<Mast> stockMasts;

        //--settings--
        internal static ConfigEntry<bool> cleanSave;
        //internal static ConfigEntry<bool> cleanLoad;
        internal static ConfigEntry<bool> vertLateens;
        internal static ConfigEntry<bool> convertSave;


        private void Awake()
        {

            stockPartOptions = new List<BoatPartOption>();
            stockParts = new Dictionary<BoatPart, int>();
            stockMasts = new List<Mast>();

            //modPartOptions = new List<BoatPartOption>();
            //modParts = new List<BoatPart>();
            moddedBoats = new List<BoatCustomParts>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            //cleanSave = Config.Bind("Settings", "Clean load", false);
            //bruteForce = Config.Bind("Settings", "Brute force", false);
            vertLateens = Config.Bind("Settings", "Vertical lateens", true);
            convertSave = Config.Bind("Fixers", "Convert saves", true);
            cleanSave = Config.Bind("Fixers", "Clean save", false);
        }
    }
}
