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

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(ShipyardSailInstaller), "AddNewSail")]
    internal static class ShipyardSailInstallerPatches
    {
        public static void Prefix(GameObject sailObject, Mast ___currentMast)
        {
            SailScaler component = sailObject.GetComponent<SailScaler>();// ?? sailObject.AddComponent<SailScaler>();
            component.UpdateInstallHeight(___currentMast.transform);

            Sail component2 = sailObject.GetComponent<Sail>();
            if (Plugin.autoFit.Value && component2.installHeight > ___currentMast.mastHeight)
            {
                float extraHeight = component2.UseExtendedMastHeight()? ___currentMast.extraBottomHeight : 0;
                component.SetScaleRel((___currentMast.mastHeight + extraHeight - 0.1f) / component.GetBaseHeight());
                //__instance.MoveHeldSail(component2.installHeight - component.GetBaseHeight() - extraHeight);

            }
        }
        public static void Postfix(ref Sail ___selectedSail)
        {
            SailScaler component = ___selectedSail.GetComponent<SailScaler>();
            //float tilt = 0;
            if (Plugin.vertLateens.Value && ___selectedSail.category == SailCategory.lateen)
            {
                VertifySail(component);
/*                ___selectedSail.transform.eulerAngles = new Vector3(270, 0, 0);
                ___selectedSail.transform.localEulerAngles = new Vector3(0, ___selectedSail.transform.localEulerAngles.y, 0);
                component.SetAngle(___selectedSail.transform.localEulerAngles.y);*/
            }
            if (Plugin.vertFins.Value && ___selectedSail.category == SailCategory.other && ___selectedSail.name.Contains("junklateen"))
            {
                VertifySail(component);

/*                ___selectedSail.transform.eulerAngles = new Vector3(270, 0, 0);
                ___selectedSail.transform.localEulerAngles = new Vector3(0, ___selectedSail.transform.localEulerAngles.y, 0);

                component.SetAngle(___selectedSail.transform.localEulerAngles.y);
                if (component.rotatablePart != ___selectedSail.transform)
                {
                    ___selectedSail.transform.localEulerAngles = Vector3.zero;
                }*/

            }

        }
        private static void VertifySail(SailScaler sail)
        {
            sail.transform.eulerAngles = new Vector3(270, 0, 0);
            sail.transform.localEulerAngles = new Vector3(0, sail.transform.localEulerAngles.y, 0);
            sail.SetAngle(sail.transform.localEulerAngles.y);
            if (sail.rotatablePart != sail.transform)
            {
                sail.transform.localEulerAngles = Vector3.zero;
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

            if (__instance.GetComponent<SailPartLocations>() is SailPartLocations partLocations/*___sail.name.Contains("lug")*/)
            {
                //___initialRot.eulerAngles = new Vector3(___sail.transform.localEulerAngles.x, ___sail.GetComponent<SailScaler>().scaleablePart.localEulerAngles.y, ___sail.transform.localEulerAngles.z);

                foreach (ShipyardSailColCheckerSub sub in __instance.GetComponentsInChildren<ShipyardSailColCheckerSub>())
                {
                    sub.transform.localPosition = partLocations.locations[sub.transform.GetSiblingIndex()];
                }
            }
        }
    }
}
