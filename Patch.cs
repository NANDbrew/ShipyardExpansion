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
        [HarmonyPatch(typeof(SaveableBoatCustomization))]
        internal static class PartsPatch
        {

            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Adder(SaveableBoatCustomization __instance, BoatCustomParts ___parts, BoatRefs ___refs)
            {
                foreach (var part in ___parts.availableParts)
                {
                    Plugin.stockParts.Add(part, part.activeOption);
                    foreach (var option in part.partOptions)
                    {
                        Plugin.stockPartOptions.Add(option);
                    }
                }
                foreach (var mast in ___refs.GetComponentsInChildren<Mast>())
                {
                    Plugin.stockMasts.Add(mast);
                }
                Plugin.moddedBoats.Add(___parts);

                Array.Resize(ref ___refs.masts, 64);// = ___refs.masts.AddRangeToArray(new Mast[33]);
                //Debug.Log(___refs.masts);

                if (__instance.name == "BOAT dhow small (10)") DhowPatches.Patch(__instance.transform, ___parts);
                if (__instance.name == "BOAT dhow medium (20)") SanbuqPatches.Patch(__instance.transform, ___parts);
                if (__instance.name == "BOAT junk small singleroof(90)") KakamPatches.Patch(__instance.transform, ___parts);
                if (__instance.name == "BOAT junk medium (80)") JunkPatches.Patch(__instance.transform, ___parts);
                if (__instance.name == "BOAT medi small (40)") CogPatches.Patch(__instance.transform, ___parts);
                if (__instance.name == "BOAT medi medium (50)") BrigPatches.Patch(__instance.transform, ___parts);

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
