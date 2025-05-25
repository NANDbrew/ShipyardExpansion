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
            if (!__result && AutoLinkMast(__instance) is Mast linkMast)
            {
                Debug.Log($"SE topsail patch => {__instance.name} checking {linkMast.name} for linkable sails");
                for (int num = linkMast.sails.Count - 1; num >= 0; num--)
                {
                    if (linkMast.sails[num] != null && linkMast.sails[num].GetComponent<Sail>().squareSail)
                    {
                        Debug.Log($"SE topsail patch => {__instance.gameObject.name}.{squareSail.name}: adding topsail component.");
                        squareSail.AddComponent<SquareTopsailAngleMirror>().sailBelow = linkMast.sails[num].GetComponent<HingeJoint>();
                        __result = true;
                        break;
                    }
                }
            }

        }
        public static Mast AutoLinkMast(Mast mast)
        {
            if (mast.GetComponent<BoatPartOption>() is BoatPartOption opt)
            {
                if (opt.childMast is Mast childMast)
                {
                    return childMast;
                }
                foreach (var req in opt.requires)
                {
                    if (req.GetComponentInChildren<Mast>() is Mast foundMast)
                    {
                        return foundMast;
                    }
                }
            }
            return null;
        }
    }
/*    [HarmonyPatch(typeof(Mast), "Awake")]
    internal static class ChildMastPatch
    {
        private static void Postfix(Mast __instance)
        {
            if (!Plugin.topsailPatch.Value) return;
            AssetTools.AutoLinkMast(__instance);
        }
    }*/


}
