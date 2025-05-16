using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Mast), "TopsailCheckAndAttach")]
    internal static class TopsailPatch
    {
        private static void Postfix(GameObject squareSail, Mast __instance, ref bool __result)
        {
            if (!Plugin.topsailPatch.Value) return;
            if (__instance.GetComponent<BoatPartOption>()?.childMast is Mast childMast)
            {
                for (int num = childMast.sails.Count - 1; num >= 0; num--)
                {
                    if (childMast.sails[num] != null && childMast.sails[num].GetComponent<Sail>().squareSail)
                    {
                        Debug.Log($"SE topsail patch => {__instance.gameObject.name}.{squareSail.name}: adding topsail component.");
                        squareSail.AddComponent<SquareTopsailAngleMirror>().sailBelow = childMast.sails[num].GetComponent<HingeJoint>();
                        __result = true;
                        break;
                    }
                }
            }

        }
    }
    [HarmonyPatch(typeof(Mast), "Awake")]
    internal static class ChildMastPatch
    {
        private static void Postfix(Mast __instance)
        {
            if (!Plugin.topsailPatch.Value) return;
            AssetTools.AutoLinkMast(__instance);
        }
    }
}
