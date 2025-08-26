using HarmonyLib;
using ShipyardExpansion.Scripts;
using System.Linq;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    internal class ShipyardPatches
    {
        public static float waitTime = 0f;
        [HarmonyPatch(typeof(Shipyard))]
        private static class UnrollSailsPatch
        {
            private static bool discharging;

            [HarmonyPatch("ResetOrder")]
            [HarmonyPostfix]
            public static void Patch2(Shipyard __instance, GameObject ___currentShip)
            {
                if (!Plugin.unrollSails.Value || discharging)
                {
                    ShipyardUnfurlButton.SetState(true);
                    return;
                }
                __instance.StartCoroutine(Scripts.ShipyardUnfurlButton.UnrollSails(___currentShip, waitTime, 1f));
            }

            [HarmonyPatch("DischargeShip")]
            [HarmonyPrefix]
            public static void Patch3()
            {
                discharging = true;
            }

            [HarmonyPatch("DischargeShip")]
            [HarmonyPostfix]
            public static void Patch4()
            {
                discharging = false;
            }

            [HarmonyPatch("ActivateDocuments")]
            [HarmonyPostfix]
            public static void Patch5(ref GameObject[] ___sailPrefabs)
            {
                if (!___sailPrefabs.Contains(PrefabsDirectory.instance.sails[158]))
                {
                    ___sailPrefabs = ___sailPrefabs.AddRangeToArray(new GameObject[3] { PrefabsDirectory.instance.sails[156], PrefabsDirectory.instance.sails[157], PrefabsDirectory.instance.sails[158] });

                }
            }

        }

    }
}
