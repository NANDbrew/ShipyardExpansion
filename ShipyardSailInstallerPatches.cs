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

            float tilt = 0;
            if (!Plugin.vertLateens.Value) return;

            if (__instance.category == SailCategory.lateen)
            {
                Debug.Log("sail \"" + __instance.name + "\" updated install position");
                if (__instance.prefabIndex == 61) tilt = -5.6f;

                __instance.transform.eulerAngles = new Vector3(270, 0, 0); // new Vector3(tilt, __instance.transform.eulerAngles.y, __instance.transform.eulerAngles.z);
                __instance.transform.localEulerAngles = new Vector3(0, __instance.transform.localEulerAngles.y + tilt, 0);
                //__instance.GetComponent<SailConnections>().colChecker.transform.localRotation = __instance.transform.localRotation;
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
