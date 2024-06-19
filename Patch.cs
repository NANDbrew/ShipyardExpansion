using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipyardExpansion
{
    internal class Patch
    {
        [HarmonyPatch(typeof(SaveableBoatCustomization))]
        internal static class PartsPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Adder(SaveableBoatCustomization __instance, BoatCustomParts ___parts)
            {
                if (__instance.name == "BOAT dhow small (10)") DhowPatches.Patch(__instance.transform, ___parts.availableParts);
                if (__instance.name == "BOAT dhow medium (20)") SanbuqPatches.Patch(__instance.transform, ___parts.availableParts);
                if (__instance.name == "BOAT junk small singleroof(90)") KakamPatches.Patch(__instance.transform, ___parts.availableParts);
                if (__instance.name == "BOAT junk medium (80)") JunkPatches.Patch(__instance.transform, ___parts.availableParts);
                if (__instance.name == "BOAT medi small (40)") CogPatches.Patch(__instance.transform, ___parts.availableParts);
                if (__instance.name == "BOAT medi medium (50)") BrigPatches.Patch(__instance.transform, ___parts.availableParts);

            }
        }
    }
}
