using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
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
                if (!Plugin.unrollSails.Value || discharging) return;

                __instance.StartCoroutine(UnrollSails(___currentShip, waitTime));
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
                if (___sailPrefabs.Last() != PrefabsDirectory.instance.sails[158])
                {
                    ___sailPrefabs = ___sailPrefabs.AddRangeToArray(new GameObject[3] { PrefabsDirectory.instance.sails[156], PrefabsDirectory.instance.sails[157], PrefabsDirectory.instance.sails[158] });

                }
            }

        }
        public static IEnumerator UnrollSails(GameObject ship, float wait)
        {
            yield return new WaitForSeconds(wait);
            foreach (var mast in ship.GetComponent<BoatRefs>().masts)
            {
                if (mast == null) continue;
                foreach (var sail in mast.sails)
                {
                    sail.GetComponent<Sail>().currentUnroll = 1f;
                    //sail.GetComponent<Sail>().enabled = false;
                    sail.transform.localEulerAngles = new Vector3(0f, sail.transform.localEulerAngles.y, sail.transform.localEulerAngles.z);
                }
            }
            Debug.Log("unfurled sails on " + ship.name);
        }
    }
}
