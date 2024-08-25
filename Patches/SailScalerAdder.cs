using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Sail), "Start")]
    internal static class CompAdder
    {
        public static void Postfix(Sail __instance)
        {
            if (__instance.GetComponent<SailScaler>() != null || __instance.GetComponentInChildren<Animator>() == null) return;
            __instance.gameObject.AddComponent<SailScaler>();

        }
    }

    [HarmonyPatch(typeof(PrefabsDirectory), "Start")]
    internal static class CompAdder2
    {
        public static void Postfix(GameObject[] ___sails)
        {
            for (int i = 0; i < ___sails.Length; i++)
            {
                if (___sails[i] == null || ___sails[i].GetComponent<SailScaler>()) continue;
                ___sails[i].gameObject.AddComponent<SailScaler>();
            }
        }
    }
}
