using HarmonyLib;
using ShipyardExpansion.ShipPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    internal class Patch
    {
        [HarmonyPatch(typeof(SaveableBoatCustomization),"Awake")]
        internal static class PartsPatch
        {
            [HarmonyPostfix]
            public static void Adder(SaveableBoatCustomization __instance, BoatCustomParts ___parts, BoatRefs ___refs)
            {
                if (!AssetTools.bundle || !AssetTools.bundle2) AssetTools.LoadAssetBundles();

                foreach (var mast in ___refs.masts)
                {
                    if (mast != null) Plugin.mastHeights.Add(mast, mast.mastHeight);
                }
                Array.Resize(ref ___refs.masts, Plugin.mastListSize);

                // add references for save cleaner


                int sceneIndex = __instance.GetComponent<SaveableObject>().sceneIndex;

                if (sceneIndex == 10)
                {
                    CacheStockParts(___refs, ___parts);
                    DhowPatches.Patch(__instance.transform, ___parts, ___refs);
                }
                else if (sceneIndex == 20)
                {
                    CacheStockParts(___refs, ___parts);
                    SanbuqPatches.Patch(__instance.transform, ___parts, ___refs);
                }
                else if (sceneIndex == 90)
                {
                    CacheStockParts(___refs, ___parts);
                    KakamPatches.Patch(__instance.transform, ___parts, ___refs);
                }
                else if (sceneIndex == 80)
                {
                    CacheStockParts(___refs, ___parts);
                    JunkPatches.Patch(__instance.transform, ___parts, ___refs);
                }
                else if (sceneIndex == 40)
                {
                    CacheStockParts(___refs, ___parts);
                    CogPatches.Patch(__instance.transform, ___parts, ___refs);
                }
                else if (sceneIndex == 50)
                {
                    CacheStockParts(___refs, ___parts);
                    BrigPatches.Patch(__instance.transform, ___parts, ___refs);
                }
                else if (sceneIndex == 70)
                {
                    CacheStockParts(___refs, ___parts);
                    JongPatches.Patch(__instance.transform, ___parts, ___refs);
                }
            }
            private static void CacheStockParts(BoatRefs refs, BoatCustomParts parts)
            {
                Plugin.stockParts.Add(refs, new Dictionary<BoatPart, int>());
                foreach (var part in parts.availableParts)
                {
                    Plugin.stockParts[refs].Add(part, part.activeOption);
                }
            }

        }
        [HarmonyPatch(typeof(SaveBoatCustomizationData), MethodType.Constructor)]
        internal class GetDataPatch
        {
            [HarmonyPostfix]
            public static void Postfix(SaveBoatCustomizationData __instance)
            {
                __instance.masts = new bool[Plugin.mastListSize];
            }
        }
    }
}
