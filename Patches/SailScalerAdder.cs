using HarmonyLib;
using ShipyardExpansion.Scripts;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    [HarmonyPatch(typeof(PrefabsDirectory), "Start")]
    internal static class CompAdder2
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(GameObject[] ___sails)
        {

            for (int i = 0; i < ___sails.Length; i++)
            {
                if (___sails[i] != null)
                {
                    if (___sails[i].GetComponent<SailScaler>() == null)
                    {
                        ___sails[i].gameObject.AddComponent<SailScaler>();
                    }
                    if (___sails[i].GetComponent<SailTextureChanger>() == null)
                    {
                        var tc = ___sails[i].gameObject.AddComponent<SailTextureChanger>();
                        tc.Setup();
                    }
                }
            }
        }
    }
}
