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
        public const string PLUGIN_VERSION = "0.0.4";

        public static List<BoatPartOption> modPartOptions;
        public static List<BoatPart> modParts;
        public static List<BoatCustomParts> modCustomParts;

        //--settings--
        internal static ConfigEntry<bool> cleanSave;
        internal static ConfigEntry<bool> bruteForce;


        private void Awake()
        {
            modPartOptions = new List<BoatPartOption>();
            modParts = new List<BoatPart>();
            modCustomParts = new List<BoatCustomParts>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            cleanSave = Config.Bind("Settings", "Clean save", false);
            bruteForce = Config.Bind("Settings", "Brute force", false);
        }
    }
}
