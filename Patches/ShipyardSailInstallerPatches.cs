using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Mono.Cecil;
using UnityEngine;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(ShipyardSailInstaller), "AddNewSail")]
    internal static class ShipyardSailInstallerPatches
    {
        public static void Postfix(GameObject sailObject, ref Sail ___selectedSail, Mast ___currentMast, Shipyard ___shipyard, ShipyardSailInstaller __instance)
        {
            SailScaler component = ___selectedSail.GetComponent<SailScaler>();
            //float tilt = 0;
            if (Plugin.vertLateens.Value && ___selectedSail.category == SailCategory.lateen)
            {
                //Debug.Log("sail \"" + __instance.name + "\" updated install position");
                //if (___selectedSail.prefabIndex == 61) tilt = -5.6f;
                //Debug.Log("sail installer?? " + __instance.GetComponent<SailScaler>().angle);
                ___selectedSail.transform.eulerAngles = new Vector3(270, 0, 0); // new Vector3(tilt, __instance.transform.eulerAngles.y, __instance.transform.eulerAngles.z);
                ___selectedSail.transform.localEulerAngles = new Vector3(0, ___selectedSail.transform.localEulerAngles.y, 0);
                component.SetAngle(___selectedSail.transform.localEulerAngles.y);
            }
            if (Plugin.vertFins.Value && ___selectedSail.category == SailCategory.other && !___selectedSail.sailName.Contains("lug"))
            {
                Transform child = component.rotatablePart;
                Vector3 oldRot = child.localEulerAngles;
                child.eulerAngles = new Vector3(0, 0, 0);
                child.localEulerAngles = new Vector3(oldRot.x, child.localEulerAngles.y + 90, oldRot.z);
                component.SetAngle(child.localEulerAngles.y);
            }

            if (___selectedSail.installHeight > ___currentMast.mastHeight)
            {
                component.SetScaleRel(___currentMast.mastHeight / component.GetBaseHeight());
            }
        }

    }

 /*   [HarmonyPatch(typeof(Sail), "UpdateInstallPosition")]
    internal static class ShipyardSailPatches
    {
        [HarmonyPrefix]
        public static void Prefix(Sail __instance)
        {

            //float tilt = Plugin.tiltOffset.Value;
            float tilt = __instance.GetComponent<SailScaler>().angle;
            if (__instance.category == SailCategory.lateen && Plugin.vertLateens.Value)
            {
                //Debug.Log("sail \"" + __instance.name + "\" updated install position");
                if (__instance.prefabIndex == 61) tilt = -5.6f;
                //Debug.Log("sail installer?? " + __instance.GetComponent<SailScaler>().angle);
                if (Plugin.vertLateens.Value) __instance.transform.eulerAngles = new Vector3(270, 0, 0); // new Vector3(tilt, __instance.transform.eulerAngles.y, __instance.transform.eulerAngles.z);
                __instance.transform.localEulerAngles = new Vector3(0, __instance.transform.localEulerAngles.y + tilt, 0);
                //__instance.GetComponent<SailConnections>().colChecker.transform.localRotation = __instance.transform.localRotation;
            }
            if (__instance.category == SailCategory.other && !__instance.sailName.Contains("lug") && Plugin.vertFins.Value)
            {
                Transform child = __instance.transform.GetChild(2);
                Transform child0 = __instance.transform.GetChild(0);
                Transform child1 = __instance.transform.GetChild(1);
                child0.parent = child;
                child1.parent = child;
                float angle = tilt;
                if (Plugin.vertFins.Value)
                {
                    child.eulerAngles = new Vector3(0, 0, 0);
                    angle += child.localEulerAngles.y + 90;
                }

                child.localEulerAngles = new Vector3(90, angle, 0);
                child0.parent = __instance.transform;
                child1.parent = __instance.transform;
                child0.SetSiblingIndex(0);
                child1.SetSiblingIndex(1);
                child.SetSiblingIndex(2);
                //child.tag = "sailObject";
            }
            __instance.GetComponent<SailScaler>().SetAngle(tilt);

        }

    }*/
    [HarmonyPatch(typeof(ShipyardSailColChecker), "RunColCheck")]
    internal static class ColCheckPatch
    {
        public static void Prefix(ShipyardSailColChecker __instance, Sail ___sail, ref Quaternion ___initialRot, ref Vector3 ___initialLocalPos, ref Vector3 ___sailModelOffset)
        {
            if (___sail.GetComponent<SailScaler>().rotatablePart)
            {
                ___initialRot.eulerAngles = new Vector3(___sail.transform.localEulerAngles.x, ___sail.GetComponent<SailScaler>().rotatablePart.localEulerAngles.y, ___sail.transform.localEulerAngles.z);

            }

            /*if (___sail.category == SailCategory.lateen)
            {

                __instance.transform.Find("col_001").gameObject.SetActive(!Plugin.lenientLateens.Value);

            }
            else if (___sail.category == SailCategory.square && !___sail.name.Contains("junk"))
            {
                foreach (Transform child in ___sail.GetComponentsInChildren<Transform>())
                {
                    if (child.name != "Cube")
                    {
                        child.gameObject.SetActive(!Plugin.lenientSquares.Value);
                    }
                }
            }*/

            if (___sail.name.Contains("lug"))
            {
                ___initialRot.eulerAngles = new Vector3(___sail.transform.localEulerAngles.x, ___sail.GetComponent<SailScaler>().scaleablePart.localEulerAngles.y, ___sail.transform.localEulerAngles.z);

                foreach (ShipyardSailColCheckerSub sub in __instance.GetComponentsInChildren<ShipyardSailColCheckerSub>())
                {
                    sub.transform.localPosition = __instance.GetComponent<SailPartLocations>().locations[sub.transform.GetSiblingIndex()];
                }
            }
        }
    }
}
