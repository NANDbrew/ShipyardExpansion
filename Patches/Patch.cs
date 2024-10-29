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
                foreach (var part in ___parts.availableParts)
                {
                    Plugin.stockParts.Add(part, part.activeOption);
                    /*foreach (var option in part.partOptions)
                    {
                        Plugin.stockPartOptions.Add(option);
                    }*/
                }
                /*foreach (var mast in ___refs.GetComponentsInChildren<Mast>())
                {
                    Plugin.stockMasts.Add(mast);
                }*/
                Plugin.moddedBoats.Add(___parts);
                //Plugin.stockConfigs.Add(__instance.GetComponent<SaveableObject>().sceneIndex, __instance.GetData());
                Array.Resize(ref ___refs.masts, 64);// = ___refs.masts.AddRangeToArray(new Mast[33]);
                //Debug.Log(___refs.masts);
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
