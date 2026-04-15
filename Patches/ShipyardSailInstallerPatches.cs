using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(ShipyardSailInstaller), "AddNewSail")]
    internal static class ShipyardSailInstallerPatches
    {
        public static void Prefix(GameObject sailObject, Mast ___currentMast, ref bool __state)
        {

            Sail component2 = sailObject.GetComponent<Sail>();
            if (Plugin.autoFit.Value && component2.installHeight > ___currentMast.mastHeight)
            {
                float newHeight;
                if (component2.UseExtendedMastHeight())
                { // extra logic to avoid making the sail bigger
                    if (component2.installHeight > ___currentMast.mastHeight + ___currentMast.extraBottomHeight)
                    {
                        newHeight = (___currentMast.mastHeight + ___currentMast.extraBottomHeight - 0.1f) / component2.installHeight;
                        component2.LoadScale(newHeight, newHeight);
                    }
                }
                else
                {
                    newHeight = (___currentMast.mastHeight - 0.1f) / component2.installHeight;
                    component2.LoadScale(newHeight, newHeight);
                }
                __state = true;
            }
        }
        public static void Postfix(Sail ___selectedSail, ShipyardSailInstaller __instance, bool __state)
        {
            SailScaler component = ___selectedSail.GetComponent<SailScaler>();
            if (Plugin.vertLateens.Value && ___selectedSail.category == SailCategory.lateen || component != null)
            {
                VertifySail(component);
            }
            if (Plugin.vertFins.Value && ___selectedSail.category == SailCategory.other && ___selectedSail.name.Contains("junklateen"))
            {
                VertifySail(component);

            }
            if (__state) __instance.MoveHeldSail(0.1f);
            else Debug.Log("boo!");
            //__instance.MoveHeldSail(___selectedSail.GetCurrentInstallHeight() - ___currentMast.mastHeight);

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

    [HarmonyPatch(typeof(ShipyardSailInstaller))]
    public static class SailInstallerHeightClamp
    {
        [HarmonyPatch("MoveHeldSail")]
        [HarmonyPrefix]
        public static bool Patch(ref float distance, Sail ___selectedSail, Mast ___currentMast, ShipyardSailInstaller __instance)
        {
            if (!___selectedSail || !___selectedSail.UseExtendedMastHeight())
            {
                return true;
            }

            if (distance <= 0f && ___selectedSail.GetCurrentInstallHeight() + distance - ___selectedSail.GetScaledHeight() < -0.1f - ___currentMast.extraBottomHeight)
            {
                distance = ___selectedSail.GetScaledHeight() - ___selectedSail.GetCurrentInstallHeight() - ___currentMast.extraBottomHeight;
                Debug.Log("SE: clamped sail mover distance");
            }

            if (___selectedSail.GetScaledHeight() > ___currentMast.mastHeight + ___currentMast.extraBottomHeight)
            {
                ___selectedSail.ChangeInstallHeight(___currentMast.mastHeight - ___selectedSail.GetCurrentInstallHeight());
                //___selectedSail.ChangeInstallHeight(___currentMast.mastHeight);
                //Debug.Log("SE: prevented sail move");
                AccessTools.Method(__instance.GetType(), "ApplySailPosition").Invoke(__instance, null);
                return false;
            }
            return true;
        }
    }

    // --- lug sail hacks ---
    [HarmonyPatch(typeof(ShipyardSailColChecker), "RunColCheck")]
    internal static class ColCheckPatch
    {
        public static void Prefix(ShipyardSailColChecker __instance, Sail ___sail, ref Quaternion ___initialRot)
        {
            if (___sail.GetComponent<SailScaler>()?.rotatablePart)
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

    [HarmonyPatch(typeof(Mast), "AttachSailToMast")]
    internal static class ClothColPatch
    {
        public static void Postfix(Mast __instance, GameObject sailObject)
        {
            Sail sail = sailObject.GetComponent<SailConnections>().sail;
            List<CapsuleCollider> cols = new List<CapsuleCollider>();
            for (int i = 0; i < __instance.mastCols.Length; i++)
            {
                if (__instance.mastCols[i] == null)
                {
                    Debug.LogWarning("SE ClothColPatch: null in mast_cols at " + __instance.name);
                    continue;
                }
                if (__instance.mastCols[i].gameObject.activeInHierarchy)
                {
                    cols.Add(__instance.mastCols[i]);
                }
            }

            sail.cloth.capsuleColliders = cols.ToArray();
        }
    }


}
