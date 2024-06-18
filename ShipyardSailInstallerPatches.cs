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
    internal class SailRefs
    {
        public static Dictionary<GameObject, Quaternion> sails = new Dictionary<GameObject, Quaternion>();

    }
    [HarmonyPatch(typeof(Sail), "UpdateInstallPosition")]
    internal static class ShipyardSailPatches
    {
        static GameObject boatPointer;

        [HarmonyPrefix]
        public static void Prefix(Sail __instance)
        {
            Util.AddGizmo(__instance.transform);
            if (boatPointer == null)
            {
                boatPointer = Util.AddGizmo(__instance.GetComponentInParent<BoatCustomParts>().transform);
                boatPointer.transform.position += new Vector3(0, 5, 0);
            }
            if (!Plugin.vertLateens.Value) return;
            if (__instance.category == SailCategory.lateen && __instance.category == SailCategory.junk)
            {
                /*if(SailRefs.sails.ContainsKey(__instance.gameObject))
                {
                    __instance.transform.rotation = SailRefs.sails[__instance.gameObject];
                    __instance.transform.localEulerAngles = new Vector3(0, __instance.transform.localEulerAngles.y, 0);
                }*/
                __instance.transform.eulerAngles = new Vector3(270, 0, 0);
                __instance.transform.localEulerAngles = new Vector3(0, __instance.transform.localEulerAngles.y, 0);

            }


        }

    }
}
