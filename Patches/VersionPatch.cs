using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(SaveLoadManager), "SaveModData")]
    internal static class VersionPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (GameState.modData.ContainsKey(Plugin.PLUGIN_ID))
            {
                GameState.modData[Plugin.PLUGIN_ID] = Plugin.PLUGIN_VERSION;
            }
            else
            {
                GameState.modData.Add(Plugin.PLUGIN_ID, Plugin.PLUGIN_VERSION);
            }
        }
    }
    [HarmonyPatch(typeof(SaveLoadManager), "LoadModData")]
    internal static class VersionPatch2
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (GameState.modData.ContainsKey(Plugin.PLUGIN_ID))
            {
                string text = new string((from a in GameState.modData[Plugin.PLUGIN_ID] where char.IsNumber(a) select a).ToArray());
                Debug.Log("save version = " + text);
                SaveCleaner.saveVersion = Convert.ToInt32(text);
            }
            else
            {
                SaveCleaner.saveVersion = 0;
            }
        }
    }
}
