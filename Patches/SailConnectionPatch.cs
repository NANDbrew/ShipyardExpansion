using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(SailConnections), "Awake")]
    internal static class SailConnectionPatch
    {
        internal static void Postfix(SailConnections __instance)
        {
            if (__instance.mastReefAttachment == null || __instance.mastReefAttExtension == null) return;
            if (__instance.mastReefAttachment.GetComponent<RopeEffect>()?.attachment == __instance.mastReefAttExtension)
            {
                // magical reference swapping
                (__instance.mastReefAttExtension, __instance.mastReefAttachment) = (__instance.mastReefAttachment, __instance.mastReefAttExtension);

                // what it's doing:
                //var att = __instance.mastReefAttachment;
                //__instance.mastReefAttachment = __instance.mastReefAttExtension;
                //__instance.mastReefAttExtension = att;
            }
        }
    }
}
