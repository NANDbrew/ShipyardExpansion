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

            float tilt = Plugin.tiltOffset.Value;

            if (Plugin.vertLateens.Value && __instance.category == SailCategory.lateen)
            {
                //Debug.Log("sail \"" + __instance.name + "\" updated install position");
                if (__instance.prefabIndex == 61) tilt = -5.6f;

                __instance.transform.eulerAngles = new Vector3(270, 0, 0); // new Vector3(tilt, __instance.transform.eulerAngles.y, __instance.transform.eulerAngles.z);
                __instance.transform.localEulerAngles = new Vector3(0, -__instance.transform.parent.localEulerAngles.y + tilt, 0);
                //__instance.GetComponent<SailConnections>().colChecker.transform.localRotation = __instance.transform.localRotation;
            }
            if (Plugin.vertFins.Value && __instance.category == SailCategory.other)
            {                
                Transform child = __instance.transform.GetChild(2);
                Transform child0 = __instance.transform.GetChild(0);
                Transform child1 = __instance.transform.GetChild(1);
                child0.parent = child;
                child1.parent = child;
                child.localEulerAngles = new Vector3(child.localEulerAngles.x, -__instance.transform.parent.localEulerAngles.y, child.localEulerAngles.z);
                child0.parent = __instance.transform;
                child1.parent = __instance.transform;
                child0.SetSiblingIndex(0);
                child1.SetSiblingIndex(1);
                child.SetSiblingIndex(2);
            }
        }

    }
    [HarmonyPatch(typeof(ShipyardSailColChecker), "RunColCheck")]
    internal static class ColRemover
    {
        public static void Prefix(ShipyardSailColChecker __instance, Sail ___sail, ref Quaternion ___initialRot)
        {
            if (___sail.category == SailCategory.lateen)
            {
                ___initialRot = ___sail.transform.localRotation;

                __instance.transform.Find("col_001").gameObject.SetActive(!Plugin.lenientLateens.Value);

            }
            else if (___sail.category == SailCategory.other)
            {
                ___initialRot = Quaternion.Euler(-___sail.transform.parent.localEulerAngles);
                //__instance.transform.Find("col_001").gameObject.SetActive(!Plugin.lenientLateens.Value);

            }
            else if (___sail.category == SailCategory.square && !___sail.name.Contains("junk"))
            {
                for (int i = 0; i < __instance.transform.childCount; i++)
                {
                    var child = __instance.transform.GetChild(i);
                    if (child.name != "Cube")
                    {
                        child.gameObject.SetActive(!Plugin.lenientSquares.Value);
                    }
                }
            }
        }
    }
}
