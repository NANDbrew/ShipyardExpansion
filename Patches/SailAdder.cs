using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using Unity.Collections.LowLevel.Unsafe;

namespace ShipyardExpansion
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
            if (refMat != null && ___renderer.material.name.StartsWith("item empty outline material"))
            {
                ___renderer.material = refMat;
            }
        }
    }

    [HarmonyPatch(typeof(PrefabsDirectory), "Start")]
    internal static class SailAdderPatches
    {
        public static void Postfix(ref GameObject[] ___sails)
        {
            if (!Plugin.addSails.Value) return;
            Plugin.stockSailsListSize = ___sails.Length;
            Array.Resize(ref ___sails, Plugin.sailListSize);


            var modSail2 = Util.CopySail(___sails, 30, new Vector3(1.55f, 0.25f, 0), new Vector3(90, 354, 0), "lug medium", "balanced lug 6yd", 158);
            modSail2.transform.Find("sail M small gaff").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail2.transform.Find("sail M small gaff").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail2.GetComponent<Sail>().category = SailCategory.other;

            var modSail3 = Util.CopySail(___sails, 75, new Vector3(2.35f, 0.25f, 0), new Vector3(90, 354, 0), "lug giant", "balanced lug 11yd", 157);
            modSail3.transform.Find("sail M small gaff 3").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail3.transform.Find("sail M small gaff 3").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail3.GetComponent<Sail>().category = SailCategory.other;
            var modSail4 = Util.CopySail(___sails, 5, new Vector3(1.5f, 0.2f, 0), new Vector3(90, 356, 0), "lug tiny", "balanced lug 4yd", 156);
            modSail4.transform.Find("sail A tiny gaff").Find("SAIL_tiny_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail4.transform.Find("sail A tiny gaff").Find("SAIL_tiny_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail4.GetComponent<Sail>().category = SailCategory.other;

            //var lateen20 = Util.CopySail(___sails, 121, Vector3.zero, new Vector3(90, 0, 0), 1.6f, "M lateen big 1.6", "lateen 20yd", 155);
        }
    }


}
