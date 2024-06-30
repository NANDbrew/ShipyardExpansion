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
        static int conversionCounter = 0;

        [HarmonyPrefix]
        internal static void Convert(SaveBoatCustomizationData data, BoatRefs ___refs)
        {
            if (!Plugin.convertSave.Value) return;
            foreach (SaveSailData sailData in data.sails)
            {
                if (mastIndices.ContainsKey(sailData.mastIndex) && ___refs.masts[mastIndices[sailData.mastIndex]] != null && ___refs.masts[sailData.mastIndex] == null)
                {
                    mastIndices.TryGetValue(sailData.mastIndex, out sailData.mastIndex);
                }
            }
            conversionCounter++;
            Debug.Log("ShipyardExpansion converted " + conversionCounter + " ships");
            //Plugin.convertSave.Value = false;
        }
    }

}
