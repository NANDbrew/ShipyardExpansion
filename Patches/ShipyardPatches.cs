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
                ShipyardCutawayButton.SetState(true);

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

            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Patch5(Shipyard __instance, ref GameObject[] ___sailPrefabs)
            {
                if (PrefabsDirectory.instance == null)
                {
                    Debug.LogError($"SE: Prefabs directory is null! Shipyard ({__instance.name}) is awake too early?");
                    return;
                }
                if (___sailPrefabs != null && !___sailPrefabs.Contains(PrefabsDirectory.instance.sails[158]))
                {
/*                    if (PrefabsDirectory.instance.sails[156] == null || PrefabsDirectory.instance.sails[157] == null || PrefabsDirectory.instance.sails[158] == null)
                    {
                        Debug.LogError("SE: sails not initialized");
                    }*/
                    ___sailPrefabs = ___sailPrefabs.AddRangeToArray(new GameObject[3] { PrefabsDirectory.instance.sails[156], PrefabsDirectory.instance.sails[157], PrefabsDirectory.instance.sails[158] });

                }
            }

            [HarmonyPatch("AdmitShip")]
            [HarmonyPrefix]
            public static void Patch6(GameObject ship)
            {
                Debug.Log("finding renderers...");
                ShipyardCutawayButton.FindRenderers(ship);
            }
        }

    }
}
