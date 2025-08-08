/*using HarmonyLib;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Mast), "Awake")]
    internal static class MastPatches
    {
        internal static void Prefix(Mast __instance)
        {
            if (__instance.shipRigidbody == null)
            {
                Debug.Log("Trying to find shipRigidbody for " + __instance.name);
                Debug.Log("Ship root: " + __instance.transform.root.name);
                __instance.shipRigidbody = __instance.transform.root.GetComponent<Rigidbody>();
            }

        }
    }
}
*/