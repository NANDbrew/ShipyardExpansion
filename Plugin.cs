using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardExpansion
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.nandbrew.shipyardexpansion";
        public const string PLUGIN_NAME = "Shipyard Expansion";
        public const string PLUGIN_VERSION = "0.8.92";

        internal const int mastListSize = 128;
        internal const int sailListSize = 512;
        internal const int maxPartsPages = 3;

        internal static Plugin instance;
        public static List<BoatCustomParts> moddedBoats;
        public static Dictionary<BoatRefs, Dictionary<BoatPart, int>> stockParts;
        public static int stockSailsListSize;
        public static Dictionary<GameObject, SaveBoatCustomizationData> converted;
        public static Transform prefabContainer;
        public static Dictionary<Mast, float> mastHeights = new Dictionary<Mast, float>();

        //--settings--
        internal static ConfigEntry<bool> cleanSave;
        internal static ConfigEntry<bool> convertSave;
        internal static ConfigEntry<bool> vertLateens;
        internal static ConfigEntry<bool> vertFins;
        internal static ConfigEntry<bool> unrollSails;
        internal static ConfigEntry<bool> addSails;
        internal static ConfigEntry<bool> cleanLoad;
        internal static ConfigEntry<bool> percentSailNames;
        internal static ConfigEntry<bool> autoFit;
        internal static ConfigEntry<bool> skipSailData;
        internal static ConfigEntry<bool> starterSetFix;
        internal static ConfigEntry<int> climbSpeed;
        internal static ConfigEntry<bool> topsailPatch;
        internal static ConfigEntry<bool> combinedScale;

        private void Awake()
        {
            instance = this;
            stockParts = new Dictionary <BoatRefs, Dictionary<BoatPart, int>>();
            moddedBoats = new List<BoatCustomParts>();
            converted = new Dictionary<GameObject, SaveBoatCustomizationData>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            convertSave = Config.Bind("Fixers", "Convert saves", false, new ConfigDescription("Enable this before loading a save from a version of Shipyard Expansion before v0.5"));
            cleanSave = Config.Bind("Fixers", "Clean save", false, new ConfigDescription("Enable this before saving if you want to uninstall this mod (will disable itself when done)"));
            cleanLoad = Config.Bind("Fixers", "Clean load", true, new ConfigDescription("Enable this before loading a broken save"));
            skipSailData = Config.Bind("zDebug", "skip sail data", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            starterSetFix = Config.Bind("Fixers", "starter set fix", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            vertLateens = Config.Bind("Slant", "Vertical lateens", true, new ConfigDescription("Install lateens vertical instead of slanting with the mast"));
            vertFins = Config.Bind("Slant", "Vertical fins", true, new ConfigDescription("Install fin sails vertical instead of slanting with the mast"));
            unrollSails = Config.Bind("Settings", "Unfurl sails in shipyard", true, new ConfigDescription("Unfurl existing sails when entering the shipyard"));
            addSails = Config.Bind("Settings", "Add lug sails", true, new ConfigDescription("Adds new sails in the 'Other' category. (requires a restart)", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            percentSailNames = Config.Bind("Settings", "Show percent scale in sail name", true, new ConfigDescription(""));
            autoFit = Config.Bind("Settings", "Auto-fit sails", true, new ConfigDescription("Automatically scale too-big sails to fit the mast"));
            climbSpeed = Config.Bind("Settings", "Climb speed", 10, new ConfigDescription("Speed when climbing up to tops/crow's nests", new AcceptableValueRange<int>(2, 15)));
            topsailPatch = Config.Bind("Settings", "Link topmasts", true, new ConfigDescription("Link square sail angles on topmasts to the ones on the mast below (requires a restart)"));
            combinedScale = Config.Bind("Settings", "Combined scaling", false, new ConfigDescription("scale square sails uniformly and by width (if disabled, scale height and width separately)"));
            
            AssetTools.LoadAssetBundles();
        }

    }
}
