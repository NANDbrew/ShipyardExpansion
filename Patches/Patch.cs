using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class Patch
    {
        [HarmonyPatch(typeof(SaveableBoatCustomization),"Awake")]
        internal static class PartsPatch
        {
            [HarmonyPostfix]
            public static void Adder(SaveableBoatCustomization __instance, BoatCustomParts ___parts, BoatRefs ___refs)
            {
                if (!AssetTools.bundle) AssetTools.LoadAssetBundles();
                
                Array.Resize(ref ___refs.masts, Plugin.mastListSize);
                int sceneIndex = __instance.GetComponent<SaveableObject>().sceneIndex;

                if (sceneIndex == 10) DhowPatches.Patch(__instance.transform, ___parts);
                else if (sceneIndex == 20) SanbuqPatches.Patch(__instance.transform, ___parts);
                else if (sceneIndex == 90) KakamPatches.Patch(__instance.transform, ___parts);
                else if (sceneIndex == 80) JunkPatches.Patch(__instance.transform, ___parts);
                else if (sceneIndex == 40) CogPatches.Patch(__instance.transform, ___parts);
                else if (sceneIndex == 50) BrigPatches.Patch(__instance.transform, ___parts);

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
