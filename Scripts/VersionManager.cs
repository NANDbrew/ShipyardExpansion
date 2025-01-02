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
        public static int saveVersion = -1;
        public static void WriteSaveVersion()
        {
            //string text = new string((from a in Plugin.PLUGIN_ID where char.IsNumber(a) select a).ToArray());
            string text = Plugin.PLUGIN_VERSION.Replace(".", string.Empty);
            Debug.Log("SE save version updated: " + text);
            saveVersion = Convert.ToInt32(text);

            if (GameState.modData.ContainsKey(Plugin.PLUGIN_ID))
            {
                GameState.modData[Plugin.PLUGIN_ID] = Plugin.PLUGIN_VERSION;
            }
            else
            {
                GameState.modData.Add(Plugin.PLUGIN_ID, Plugin.PLUGIN_VERSION);
            }
            //Debug.Log("SE save version = " + saveVersion);
        }

        public static void ReadSaveVersion()
        {
            if (GameState.modData.ContainsKey(Plugin.PLUGIN_ID))
            {
                string text = new string((from a in GameState.modData[Plugin.PLUGIN_ID] where char.IsNumber(a) select a).ToArray());
                //Debug.Log("SE save version = " + text);
                saveVersion = Convert.ToInt32(text);
                foreach (GameObject obj in Plugin.converted.Keys)
                {
                    obj.GetComponent<SaveableBoatCustomization>().LoadData(Plugin.converted[obj]);
                }
            }
            else
            {
                saveVersion = 0;
            }
            Debug.Log("SE save version read: " + saveVersion);
        }
    }
}
