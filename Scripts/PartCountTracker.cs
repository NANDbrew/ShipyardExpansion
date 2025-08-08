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
    }
}
