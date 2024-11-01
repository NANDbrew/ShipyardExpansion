using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static void Prefix(GameObject sailObject, ref Sail ___selectedSail, Mast ___currentMast, Shipyard ___shipyard, ShipyardSailInstaller __instance)
        {
            SailScaler component = sailObject.GetComponent<SailScaler>();
            component.UpdateInstallHeight(___currentMast.transform);

        }
        public static void Postfix(GameObject sailObject, ref Sail ___selectedSail, Mast ___currentMast, Shipyard ___shipyard, ShipyardSailInstaller __instance)
        {
            SailScaler component = ___selectedSail.GetComponent<SailScaler>();
            //float tilt = 0;
            if (Plugin.vertLateens.Value && ___selectedSail.category == SailCategory.lateen)
            {
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

            if (Plugin.autoFit.Value && ___selectedSail.installHeight > ___currentMast.mastHeight)
            {
                component.SetScaleRel((___currentMast.mastHeight - 0.1f) / component.GetBaseHeight());
                __instance.MoveHeldSail(___selectedSail.installHeight - component.GetBaseHeight());
            }
        }

    }

    [HarmonyPatch(typeof(ShipyardSailColChecker), "RunColCheck")]
    internal static class ColCheckPatch
    {
        public static void Prefix(ShipyardSailColChecker __instance, Sail ___sail, ref Quaternion ___initialRot, ref Vector3 ___initialLocalPos, ref Vector3 ___sailModelOffset)
        {
            if (___sail.GetComponent<SailScaler>().rotatablePart)
            {
                ___initialRot.eulerAngles = new Vector3(___sail.transform.localEulerAngles.x, ___sail.GetComponent<SailScaler>().rotatablePart.localEulerAngles.y, ___sail.transform.localEulerAngles.z);

            }

            if (___sail.name.Contains("lug"))
            {
                //___initialRot.eulerAngles = new Vector3(___sail.transform.localEulerAngles.x, ___sail.GetComponent<SailScaler>().scaleablePart.localEulerAngles.y, ___sail.transform.localEulerAngles.z);

                foreach (ShipyardSailColCheckerSub sub in __instance.GetComponentsInChildren<ShipyardSailColCheckerSub>())
                {
                    sub.transform.localPosition = __instance.GetComponent<SailPartLocations>().locations[sub.transform.GetSiblingIndex()];
                }
            }
        }
    }
}
