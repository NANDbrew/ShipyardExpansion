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
    /*[HarmonyPatch(typeof(ShipyardSailInstaller), "AddNewSail")]
    internal static class ShipyardSailInstallerPatches
    {
        public static bool Prefix(GameObject sailObject, ref Sail ___selectedSail, Mast ___currentMast, Shipyard ___shipyard, ShipyardSailInstaller __instance)
        {

            ___selectedSail = sailObject.GetComponent<Sail>();
            ___selectedSail.enabled = false;
            ___selectedSail.SetSailArea();
            ___selectedSail.GetComponent<Rigidbody>().isKinematic = true;
            ___selectedSail.ChangeSailColor(___shipyard.availableSailColors[0]);
            if (!___currentMast.onlySquareSails)
            {
                if (___selectedSail.UseExtendedMastHeight())
                {
                    ___selectedSail.ChangeInstallHeight(___selectedSail.installHeight - ___currentMast.extraBottomHeight);
                }
                else
                {
                    ___selectedSail.ChangeInstallHeight(___selectedSail.installHeight);
                }
            }
            else
            {
                ___selectedSail.ChangeInstallHeight(___currentMast.mastHeight);
            }

            sailObject.transform.position = ___currentMast.transform.position;
            //sailObject.transform.rotation = ___currentMast.transform.rotation;
            sailObject.transform.eulerAngles = new Vector3(0, 30, 0);
            ___currentMast.sails.Add(sailObject);
            sailObject.transform.parent = ___currentMast.transform;
            ShipyardUI.instance.UpdateMastSailsList();
            ShipyardUI.instance.UpdateDescriptionText();
            ___currentMast.UpdateSailOrder();
            GameState.currentShipyard.UpdateOrder();
            ___selectedSail.UpdateInstallPosition();
            __instance.StartCoroutine(ColPosFix(__instance));
            return false;
        }
        private static IEnumerator ColPosFix(ShipyardSailInstaller obj)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            AccessTools.Method(obj.GetType(), "ApplySailPosition").Invoke(obj, new object[1]);
        }
    }
*/
    [HarmonyPatch(typeof(Sail), "UpdateInstallPosition")]
    internal static class ShipyardSailPatches
    {
        [HarmonyPrefix]
        public static void Prefix(Sail __instance)
        {

            float tilt = Plugin.tiltOffset.Value;
            //float tilt = __instance.GetComponent<SailScaler>().angle;
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
        }

    }
    [HarmonyPatch(typeof(ShipyardSailColChecker), "RunColCheck")]
    internal static class ColRemover
    {
        public static void Prefix(ShipyardSailColChecker __instance, Sail ___sail, ref Quaternion ___initialRot, ref Vector3 ___initialLocalPos, ref Vector3 ___sailModelOffset)
        {
            if (___sail.category == SailCategory.lateen)
            {
                ___initialRot = ___sail.transform.localRotation;

                __instance.transform.Find("col_001").gameObject.SetActive(!Plugin.lenientLateens.Value);

            }
            else if (___sail.category == SailCategory.other)
            {
                ___initialRot.eulerAngles = new Vector3(___sail.transform.localEulerAngles.x, ___sail.transform.GetChild(0).localEulerAngles.y, ___sail.transform.localEulerAngles.z);
                if (___sail.sailName.Contains("lug"))
                {
                    //___sailModelOffset = new Vector3(1.5f, 0.2f, 0);
                    //__instance.transform.localPosition = new Vector3(1.5f, 0.2f, 0);
                }
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
 /*    [HarmonyPatch(typeof(Shipyard))]
    internal static class ShipyardPatch
    {

    }
   [HarmonyPatch(typeof(ShipyardSailColChecker), "Awake")]
    internal static class LugColAdjuster
    {
        public static void Postfix(ShipyardSailColChecker __instance, ref Vector3 ___initialLocalPos, ref Quaternion ___initialRot)
        {
            //Vector3 baseRot = ___initialRot.eulerAngles = new Vector3(1, 1, 1);
            ___initialLocalPos = __instance.transform.parent.parent.localPosition;
            ___initialRot.eulerAngles = new Vector3(___initialRot.eulerAngles.x, __instance.transform.parent.parent.localRotation.y, ___initialRot.eulerAngles.z);

        }
    }*/

}
