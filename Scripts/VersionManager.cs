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
        public static int[] saveVersion2 = { -1, 0, 0 };
        public static int saveVersion = -1;
        public static void WriteSaveVersion()
        {
            //string text = new string((from a in Plugin.PLUGIN_ID where char.IsNumber(a) select a).ToArray());
            string text = Plugin.PLUGIN_VERSION.Replace(".", string.Empty);
            Debug.Log("SE save version updated: " + text);
            saveVersion = Convert.ToInt32(text);
            string[] a = Plugin.PLUGIN_VERSION.Split('.');
            saveVersion2 = new int[a.Length];
            for (int i = 0; i < a.Length; i++) saveVersion2[i] = Convert.ToInt32(a[i]);
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
                saveVersion = Convert.ToInt32(text);

                string[] b = GameState.modData[Plugin.PLUGIN_ID].Split('.');
                saveVersion2 = new int[b.Length];
                for (int i = 0; i < b.Length; i++) saveVersion2[i] = Convert.ToInt32(b[i]);

                if (Plugin.convertSave.Value)
                {
                    foreach (GameObject obj in Plugin.converted.Keys)
                    {
                        obj.GetComponent<SaveableBoatCustomization>().LoadData(Plugin.converted[obj]);
                    }
                }
            }
            else
            {
                saveVersion2 = new int[]{ 0, 0, 0};
                saveVersion = 0;

                if (Plugin.convertSave.Value)
                {
                    foreach (GameObject obj in Plugin.converted.Keys)
                    {
                        obj.GetComponent<SaveableBoatCustomization>().LoadData(Plugin.converted[obj]);
                    }
                }
            }
            Debug.Log("SE save version read: " + saveVersion);
            Debug.Log("SE save version2: " + saveVersion2[0].ToString() + " " + saveVersion2[1] + " " + saveVersion2[2]);
        }
    }
}
