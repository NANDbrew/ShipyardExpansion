using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(SaveableBoatCustomization), "LoadData")]
    internal static class SaveConvert
    {
        static readonly Dictionary<int, int> mastIndices = new Dictionary<int, int>() { { 29, 31 }, { 28, 32 }, { 27, 33 }, { 26, 34 }, { 25, 35 }, { 24, 36 }, { 23, 37 }, { 22, 38 }, { 21, 39 }, { 20, 40 }, { 19, 41 } };
        static readonly Dictionary<int, int> mastIndicesJunk30 = new Dictionary<int, int>() { { 29, 31 } };
        static int conversionCounter = 0;
        public static int saveVersion;

        [HarmonyPrefix]
        internal static void Prefix(SaveBoatCustomizationData data, BoatRefs ___refs)
        {
            //if (!Plugin.convertSave.Value) return;

            if (saveVersion >= 30) return;
            /*if (___refs.GetComponent<SaveableObject>().sceneIndex == 80)
            {
                foreach (SaveSailData sailData in data.sails)
                {
                    if (mastIndicesJunk30.ContainsKey(sailData.mastIndex) && ___refs.masts[mastIndicesJunk30[sailData.mastIndex]] != null && ___refs.masts[sailData.mastIndex] == null)
                    {
                        mastIndicesJunk30.TryGetValue(sailData.mastIndex, out sailData.mastIndex);
                    }
                }
                conversionCounter++;
            }*/
            Debug.Log("ShipyardExpansion converted " + conversionCounter + " ships");
            //Plugin.convertSave.Value = false;
        }
    }

}
