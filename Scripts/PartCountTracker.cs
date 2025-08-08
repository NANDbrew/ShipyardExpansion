using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{
    internal static class PartCountTracker
    {

        public static void Write()
        {
            
            foreach (var boat in Plugin.stockParts)
            {
                string key = Plugin.PLUGIN_ID + "." + boat.Key.GetComponent<SaveableObject>().sceneIndex.ToString() + ".partCount";
                string value = boat.Value.Count().ToString();
                if (GameState.modData.ContainsKey(key))
                {
                    GameState.modData[key] = value;
                }
                else
                {
                    GameState.modData.Add(key, value);
                }

            }
        }
/*
        public static void ReadSaveVersion()
        {
            if (GameState.modData.ContainsKey(Plugin.PLUGIN_ID))
            {
                string text = new string((from a in GameState.modData[Plugin.PLUGIN_ID] where char.IsNumber(a) select a).ToArray());
                saveVersion = ConvertSave.ToInt32(text);

                string[] b = GameState.modData[Plugin.PLUGIN_ID].Split('.');
                saveVersion2 = new int[b.Length];
                for (int i = 0; i < b.Length; i++) saveVersion2[i] = ConvertSave.ToInt32(b[i]);

                if (Plugin.convertSave.Value)
                {
                    foreach (GameObject obj in Plugin.converted.Keys)
                    {
                        obj.GetComponent<SaveableBoatCustomization>().LoadData(Plugin.converted[obj]);
                    }
                    Debug.Log("SE version manager: reapplied SaveableBoatCustomization");
                }
            }
            else
            {
                saveVersion2 = new int[]{ 0, 0, 0 };
                saveVersion = 0;

                foreach (GameObject obj in Plugin.converted.Keys)
                {
                    obj.GetComponent<SaveableBoatCustomization>().LoadData(Plugin.converted[obj]);
                }
                Debug.Log("SE version manager: reapplied SaveableBoatCustomization");
            }
            Debug.Log("SE save version read: " + saveVersion);
            Debug.Log("SE save version2: " + saveVersion2[0].ToString() + " " + saveVersion2[1] + " " + saveVersion2[2]);
        }*/
    }
}
