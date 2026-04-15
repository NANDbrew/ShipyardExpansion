using System;
using UnityEngine;
using HarmonyLib;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(WindCloth), "Awake")]
    internal static class SailMaterialPatches
    {
        public static Material refMat;
        public static void Postfix(ref Renderer ___renderer)
        {
            if (refMat == null && ___renderer.material.name.StartsWith("sail cloth test"))
            {
                refMat = ___renderer.material;
            }
            if (refMat != null && (___renderer.material.name.StartsWith("item empty outline material") || ___renderer.material.name.StartsWith("placeholder mat")))
            {
                ___renderer.material = refMat;
            }
        }
    }

    [HarmonyPatch(typeof(PrefabsDirectory), "Start")]
    internal static class SailAdderPatches
    {
        public static void Prefix(PrefabsDirectory __instance)
        {
            Plugin.stockSailsListSize = __instance.sails.Length;
            if (__instance.sails.Length < Plugin.sailListSize)
            {
                Array.Resize(ref __instance.sails, Plugin.sailListSize);
            }

        }
        public static void Postfix(ref GameObject[] ___sails)
        {

            if (!Plugin.addSails.Value) return;
            Plugin.prefabContainer = new GameObject { name = "SEprefabContainer" }.transform;
            Plugin.prefabContainer.gameObject.SetActive(false);


            var modSail2 = Util.CopySail(___sails, 30, new Vector3(1.55f, 0.25f, 0), new Vector3(90, 354, 0), 1f, "lug medium", "balanced lug wide", 158);
            modSail2.transform.Find("sail M small gaff").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail2.transform.Find("sail M small gaff").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail2.GetComponent<Sail>().category = SailCategory.other;

            var modSail3 = Util.CopySail(___sails, 75, new Vector3(2.35f, 0.25f, 0), new Vector3(90, 354, 0), 1f, "lug giant", "balanced lug tall", 157);
            modSail3.transform.Find("sail M small gaff 3").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail3.transform.Find("sail M small gaff 3").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail3.GetComponent<Sail>().category = SailCategory.other;
            var modSail4 = Util.CopySail(___sails, 5, new Vector3(1.5f, 0.2f, 0), new Vector3(90, 356, 0), 1f, "lug tiny", "balanced lug low", 156);
            modSail4.transform.Find("sail A tiny gaff").Find("SAIL_tiny_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail4.transform.Find("sail A tiny gaff").Find("SAIL_tiny_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail4.GetComponent<Sail>().category = SailCategory.other;

        }
    }


}
