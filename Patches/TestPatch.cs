/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(BoatPart), "SetOptionEnabled")]
    internal static class TestPatch
    {
        public static bool Prefix(int i, bool state, BoatPart __instance)
        {
            try 
            {
                __instance.partOptions[i].gameObject.SetActive(state);
                GameObject[] childOptions = __instance.partOptions[i].childOptions;
                for (int j = 0; j < childOptions.Length; j++)
                {
                    childOptions[j].SetActive(state);
                }

                if (__instance.partOptions[i].walkColObject == null)
                {
                    Debug.LogError(__instance.partOptions[i].optionName + ": no walkColObject set.");
                }
                else
                {
                    __instance.partOptions[i].walkColObject.SetActive(state);
                }
            }
            catch
            {
                Debug.LogError("something went wrong with " + __instance.partOptions[i].name); 
            }

            return false;
        }
    }
}
*/