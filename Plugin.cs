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
        public const string PLUGIN_VERSION = "0.2.3";

        internal const int mastListSize = 64;

        public static List<BoatCustomParts> moddedBoats;
        public static List<BoatPartOption> stockPartOptions;
        public static Dictionary<BoatPart, int> stockParts;
        public static List<Mast> stockMasts;

        /*public static Transform topmastRef;
        public static Transform spritRef;
        public static Transform spritColRef;*/

        //--settings--
        internal static ConfigEntry<bool> cleanSave;
        internal static ConfigEntry<bool> convertSave;
        internal static ConfigEntry<bool> lenientLateens;
        internal static ConfigEntry<bool> lenientSquares;
        internal static ConfigEntry<bool> vertLateens;
        internal static ConfigEntry<bool> vertFins;

        internal static ConfigEntry<bool> unrollSails;
        internal static ConfigEntry<int> tiltOffset;


        private void Awake()
        {

            stockPartOptions = new List<BoatPartOption>();
            stockParts = new Dictionary<BoatPart, int>();
            stockMasts = new List<Mast>();

            moddedBoats = new List<BoatCustomParts>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            convertSave = Config.Bind("Fixers", "Convert saves", false, new ConfigDescription("Enable this before loading a save from a version of Shipyard Expansion before v0.1.0"));
            cleanSave = Config.Bind("Fixers", "Clean save", false, new ConfigDescription("Enable this before saving if you want to uninstall this mod (will disable itself when done)"));

            vertLateens = Config.Bind("Settings", "Vertical lateens", true, new ConfigDescription("Keep lateens vertical instead of slanting with the mast"));
            vertFins = Config.Bind("Settings", "Vertical fins", true, new ConfigDescription("Keep fin sails vertical instead of slanting with the mast"));
            lenientLateens = Config.Bind("Settings", "Lenient lateens", false, new ConfigDescription("Ignore collisions with the back edge of lateen sails"));
            lenientSquares = Config.Bind("Settings", "Lenient squares", false, new ConfigDescription("Ignore collisions with the sides of square sails"));
            unrollSails = Config.Bind("Settings", "Unfurl sails in shipyard", true, new ConfigDescription("Unfurl existing sails when entering the shipyard"));
            tiltOffset = Config.Bind("Settings", "Tilt Offset", 0, new ConfigDescription("", new AcceptableValueRange<int>(-25, 25)));
        }
    }
}
