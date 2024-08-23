using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{
    internal static class BermudaMastStuff
    {
        [HarmonyPatch(typeof(ShipyardSailInstaller), "AddNewSail")]
        public static class AddSailPatches
        {
            public static void Prefix(GameObject sailObject, Mast ___currentMast, ref bool __state)
            {
                if (___currentMast.onlySquareSails && sailObject.GetComponent<Sail>().category != SailCategory.square)
                {
                    ___currentMast.onlySquareSails = false;
                    __state = true;
                }
            }
            public static void Postfix(GameObject sailObject, Mast ___currentMast, bool __state)
            {
                if (__state && ___currentMast.onlyStaysails)
                {
                    sailObject.GetComponent<Sail>().ChangeInstallHeight(___currentMast.extraBottomHeight);
                    ___currentMast.onlySquareSails = true;
                }
            }
        }

        [HarmonyPatch(typeof(ShipyardSailInstaller), "MastNotTallEnough")]
        public static class MastTall
        {
            public static void Postfix(Mast mast, Sail sail, ref bool __result)
            {
                if (sail.category == SailCategory.staysail)
                {
                    if (sail.installHeight < mast.mastHeight + mast.extraBottomHeight * 2)
                    {
                        __result = false;
                    }
                }
            }
        }


        [HarmonyPatch(typeof(ShipyardSailInstaller), "MoveHeldSail")]
        public static class MoveSailPatches
        {
            public static bool Prefix(ref float distance, ShipyardSailInstaller __instance, Sail ___selectedSail, Mast ___currentMast, ref bool __state)
            {
                if (___currentMast.onlySquareSails && ___currentMast.onlyStaysails)
                {
                    __state = true;
                    ___currentMast.onlySquareSails = false;
                    if (___selectedSail && !___selectedSail.IsInstalled())
                    {
                        float num = 0;
                        if (___selectedSail.category == SailCategory.staysail)
                        {
                            num = ___currentMast.extraBottomHeight;
                        }
                        float currentInstallHeight = ___selectedSail.GetCurrentInstallHeight();
                        Debug.Log("current height: " + currentInstallHeight);
                        if (distance > 0f && currentInstallHeight + distance > ___currentMast.mastHeight + 0.1f + num)
                        {
                            distance = ___currentMast.mastHeight - currentInstallHeight;
                            Debug.Log("max mast height reached");
                        }

                        if (distance < 0f && currentInstallHeight + distance - ___selectedSail.installHeight < -0.1f - num)
                        {
                            distance = ___selectedSail.installHeight - currentInstallHeight;
                            Debug.Log("min mast height reached");
                        }

                        ___selectedSail.ChangeInstallHeight(distance);
                        AccessTools.Method(__instance.GetType(), "ApplySailPosition").Invoke(__instance, null);
                        return false;
                    }
                }
                return true;
            }
            public static void Postfix(Mast ___currentMast, bool __state)
            {
                if (___currentMast.onlyStaysails && __state) 
                {
                    ___currentMast.onlySquareSails = true;
                }
            }
        }
       /* [HarmonyPatch(typeof(ShipyardSailInstaller),"CheckSailOverlap")]
        public static class SailOverlapCheckPatch
        {
            [HarmonyPostfix]
            public static void CheckOverlap(Mast mast, Sail sailToCheck, ref bool __result)
            {
                if (__result == true)
                {
                    foreach (GameObject sail in mast.sails)
                    {
                        Sail component = sail.GetComponent<Sail>();
                        if (component == sailToCheck)
                        {
                            continue;
                        }
                        if (sailToCheck.category == SailCategory.staysail && component.category == SailCategory.square)
                        {
                            __result = false;
                        }
                        else if (sailToCheck.category == SailCategory.square && component.category == SailCategory.staysail)
                        {
                            __result = false;
                        }
                    }

                }
            }
        }*/
        [HarmonyPatch(typeof(ShipyardUI), "SailMastCompatible")]
        public static class MastCompatiblePatch
        {
            [HarmonyPostfix]
            public static void SailCompatible(GameObject sailPrefab, ref bool __result)
            {
                if (__result || !sailPrefab) { return; }
                //SailCategory category = sailPrefab.GetComponent<Sail>().category;
                if (GameState.currentShipyard.sailInstaller.GetCurrentMast().onlySquareSails && GameState.currentShipyard.sailInstaller.GetCurrentMast().onlyStaysails)
                {
                    /*if (sailPrefab.GetComponent<Sail>().category == SailCategory.staysail || sailPrefab.GetComponent<Sail>().category == SailCategory.square || sailPrefab.GetComponent<Sail>().category == SailCategory.lateen)
                    {
                        __result = true;
                    }*/
                    __result = true;
                }
            }
        }
        [HarmonyPatch(typeof(Sail), "UseExtendedMastHeight")]
        public static class ExtendHeightPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Sail __instance, ref bool __result)
            {
                //SailCategory category = sailPrefab.GetComponent<Sail>().category;
                if (__instance.category == SailCategory.staysail)
                {
                    __result = true;
                }
            }
        }
    }
}
