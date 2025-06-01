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
            SailScaler component = sailObject.GetComponent<SailScaler>();
            component.UpdateInstallHeight(___currentMast.transform);

            Sail component2 = sailObject.GetComponent<Sail>();
            if (Plugin.autoFit.Value && component2.installHeight > ___currentMast.mastHeight)
            {
                float extraHeight = component2.UseExtendedMastHeight()? ___currentMast.extraBottomHeight : 0;
                component.SetScaleRel((___currentMast.mastHeight + extraHeight - 0.1f) / component.GetBaseHeight());

            }
        }
        public static void Postfix(ref Sail ___selectedSail)
        {
            SailScaler component = ___selectedSail.GetComponent<SailScaler>();
            if (Plugin.vertLateens.Value && ___selectedSail.category == SailCategory.lateen)
            {
                VertifySail(component);
            }
            if (Plugin.vertFins.Value && ___selectedSail.category == SailCategory.other && ___selectedSail.name.Contains("junklateen"))
            {
                VertifySail(component);

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

    // --- lug sail hacks ---
    [HarmonyPatch(typeof(ShipyardSailColChecker), "RunColCheck")]
    internal static class ColCheckPatch
    {
        public static void Prefix(ShipyardSailColChecker __instance, Sail ___sail, ref Quaternion ___initialRot, ref Vector3 ___initialLocalPos, ref Vector3 ___sailModelOffset)
        {
            if (___sail.GetComponent<SailScaler>().rotatablePart)
            {
                ___initialRot.eulerAngles = new Vector3(___sail.transform.localEulerAngles.x, ___sail.GetComponent<SailScaler>().rotatablePart.localEulerAngles.y, ___sail.transform.localEulerAngles.z);
            }

            if (__instance.GetComponent<SailPartLocations>() is SailPartLocations partLocations)
            {
                foreach (ShipyardSailColCheckerSub sub in __instance.GetComponentsInChildren<ShipyardSailColCheckerSub>())
                {
                    sub.transform.localPosition = partLocations.locations[sub.transform.GetSiblingIndex()];
                }
            }
        }
    }
}
