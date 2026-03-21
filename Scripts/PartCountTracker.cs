using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShipyardExpansion
{
    internal static class PartCountTracker
    {
        public static void Write()
        {
            foreach (var boat in Plugin.stockParts)
            {
                string str = Plugin.PLUGIN_ID + "." + boat.Key.GetComponent<SaveableObject>().sceneIndex.ToString();
/*                if (GameState.modData.ContainsKey(str + ".partCount"))
                {
                    GameState.modData.Remove(str + ".partCount");
                }*/
                string key = str + ".partCounts";
                string value = "";
                foreach (var part in boat.Value.Keys)
                {
                    value += part.partOptions.Count.ToString();
                    value += ",";
                }
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

        public static List<int> Read(int index)
        {
            List<int> result = new List<int>();
            if (GameState.modData.TryGetValue(Plugin.PLUGIN_ID + "." + index + ".partCounts", out string value))
            {
                string[] splits = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in splits)
                {
                    result.Add(int.Parse(s));
                }
            }
            else if (GameState.modData.TryGetValue(Plugin.PLUGIN_ID + "." + index + ".partCount", out string oldNum))
            {
                result.Add(Convert.ToInt32(oldNum));
                //GameState.modData.Remove(GameState.modData[Plugin.PLUGIN_ID + "." + index + ".partCount"]);
            }
            
            return result;
        }
    }
}
