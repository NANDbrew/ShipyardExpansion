using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(SaveLoadManager), "SaveModData")]
    internal static class VersionPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!GameState.modData.ContainsKey(Plugin.PLUGIN_ID))
            {
                GameState.modData.Add(Plugin.PLUGIN_ID, Plugin.PLUGIN_VERSION);
            }
            else
            {
                GameState.modData[Plugin.PLUGIN_ID] = Plugin.PLUGIN_VERSION;
            }
        }
    }
}
