using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(GPButtonRopeWinch), "Awake")]
    internal static class WinchFixer
    {
        public static void Postfix(GPButtonRopeWinch __instance)
        {
            if (__instance.transform.localEulerAngles.x % 90 == 0)
            {
                __instance.transform.Rotate(0.1f, 0f, 0f, Space.Self);
                Debug.Log("SE: rotated " + __instance.name);
            }
        }
    }
}
