using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class AltWinch : MonoBehaviour
    {
        public GPButtonRopeWinch altWinch;
        public GPButtonRopeWinch winch;

        private void Awake()
        {
            winch = GetComponent<GPButtonRopeWinch>();
        }

        private void OnDisable()
        {
            Debug.LogWarning("disabled " + this.name + " for " + altWinch.name);
            if (winch.rope)
            {
                //altWinch.AttachToController(winch.rope);
                altWinch.StartCoroutine(SwitchWinch(altWinch, winch, false));
            }
        }
        private void OnEnable()
        {
            Debug.LogWarning("disabled " + altWinch.name + " for " + this.name);
            if (altWinch.rope)
            {
                //winch.AttachToController(altWinch.rope);
                winch.StartCoroutine(SwitchWinch(winch, altWinch, true));
            }
        }

        public static IEnumerator SwitchWinch(GPButtonRopeWinch toWinch, GPButtonRopeWinch fromWinch, bool disableFrom)
        {
            yield return new WaitForEndOfFrame();
            toWinch.AttachToController(fromWinch.rope);
            toWinch.ShowWinch(true);
            fromWinch.ShowWinch(false);
            fromWinch.rope = null;
        }
    }

 /*   [HarmonyPatch(typeof(GPButtonRopeWinch), "AttachToController")]
    public static class WinchPatch
    {
        public static bool Prefix(RopeController controller, GPButtonRopeWinch __instance)
        { 
            if (!__instance.gameObject.activeInHierarchy && __instance.GetComponent<AltWinch>() is AltWinch altWinch)
            {
                altWinch.altWinch.AttachToController(controller);
                altWinch.altWinch.ShowWinch(true);
                return false;
            }
            return true;
        }
    }*/
   /* [HarmonyPatch(typeof(GPButtonRopeWinch), "ShowWinch")]
    public static class WinchPatch2
    {
        public static void Postfix(bool state, GPButtonRopeWinch __instance, RopeController ___rope)
        {
            if (!__instance.gameObject.activeInHierarchy && __instance.GetComponent<AltWinch>() is AltWinch altWinch)
            {
                altWinch.altWinch.ShowWinch(state);
                if (!state)
                {
                    altWinch.altWinch.rope = null;
                    altWinch.altWinch.ShowWinch(false);
                }
            }
        }
    }*/
}
