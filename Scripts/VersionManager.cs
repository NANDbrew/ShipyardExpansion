using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{
    internal static class VersionManager
    {
        public static void WriteSaveVersion()
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

        public static void ReadSaveVersion()
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
