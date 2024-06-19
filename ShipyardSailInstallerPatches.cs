using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Mono.Cecil;
using UnityEngine;

namespace ShipyardExpansion
{

    [HarmonyPatch(typeof(Sail), "UpdateInstallPosition")]
    internal static class ShipyardSailPatches
    {
        [HarmonyPrefix]
        public static void Prefix(Sail __instance)
        {
            if (!Plugin.vertLateens.Value) return;

            float tilt = 270;

            if (__instance.category == SailCategory.lateen || __instance.category == SailCategory.junk || __instance.category == SailCategory.other)
            {
                Debug.Log("sail \"" + __instance.name + "\" updated install position");
                if (__instance.prefabIndex == 61) tilt = 275f;

                __instance.transform.eulerAngles = new Vector3(tilt, __instance.transform.eulerAngles.y, __instance.transform.eulerAngles.z);
                //__instance.transform.localEulerAngles = new Vector3(0, __instance.transform.localEulerAngles.y, 0);
            }
        }

    }
/*    [HarmonyPatch(typeof(Sail), "RunColCheck")]
    internal static class ShipyardColCheckerPatches
    {
        [HarmonyPrefix]
        public static void Prefix(Sail __instance)
        {
            if (!(bool)GameState.currentShipyard) return;
            if (__instance.GetComponent<SailConnections>() is SailConnections con)
            { 
                con.reefController.currentLength = 1; 
            }

        }
    }*/
}
