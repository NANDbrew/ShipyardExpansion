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
            Array.Resize(ref ___sails, 160);
            /*var modSail1 = SailAdder.CopySail(___sails, 37, new Vector3(2f, 0.25f, 0), new Vector3(90, 356, 0), "lug huge", "balanced lug 10yd", 159);
            modSail1.transform.Find("sail M gaff huge").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail1.transform.Find("sail M gaff huge").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail1.GetComponent<Sail>().category = SailCategory.other;*/

            var modSail2 = SailAdder.CopySail(___sails, 16, new Vector3(1.55f, 0.25f, 0), new Vector3(90, 354, 0), "lug medium", "balanced lug 6yd", 158);
            modSail2.transform.Find("sail M small gaff").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail2.transform.Find("sail M small gaff").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail2.GetComponent<Sail>().category = SailCategory.other;

            var modSail3 = SailAdder.CopySail(___sails, 75, new Vector3(2.35f, 0.25f, 0), new Vector3(90, 354, 0), "lug giant", "balanced lug 11yd", 157);
            modSail3.transform.Find("sail M small gaff 3").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail3.transform.Find("sail M small gaff 3").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail3.GetComponent<Sail>().category = SailCategory.other;
            var modSail4 = SailAdder.CopySail(___sails, 5, new Vector3(1.5f, 0.2f, 0), new Vector3(90, 356, 0), "lug tiny", "balanced lug 4yd", 156);
            modSail4.transform.Find("sail A tiny gaff").Find("SAIL_tiny_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail4.transform.Find("sail A tiny gaff").Find("SAIL_tiny_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail4.GetComponent<Sail>().category = SailCategory.other;

            //var lateen20 = SailAdder.CopySail(___sails, 121, Vector3.zero, new Vector3(90, 0, 0), 1.6f, "M lateen big 1.6", "lateen 20yd", 155);
        }
    }
    internal static class SailAdder
    {

        public static GameObject CopySail(GameObject[] sailPrefabs, int prefabIndex, Vector3 position, Vector3 eulerAngles, string name, string prettyName, int newIndex)
        {
            Transform sailObject = sailPrefabs[prefabIndex].GetComponentInChildren<Animator>().transform;
            return CopySail(sailPrefabs, prefabIndex, position, eulerAngles, sailObject.localScale.x, name, prettyName, newIndex);
        }
        public static GameObject CopySail(GameObject[] sailPrefabs, int prefabIndex, Vector3 position, Vector3 eulerAngles, float scale, string name, string prettyName, int newIndex)
        {
            //Debug.Log("thing");
            Transform sailBase = UnityEngine.Object.Instantiate(sailPrefabs[prefabIndex]).transform;
            var sail = sailBase.GetComponent<Sail>();
            sail.prefabIndex = newIndex;
            sail.sailName = prettyName;
            sailBase.name = newIndex + " SAIL " + name;
            var windShadow = sailBase.Find("wind shadow col");

            Transform sailObject = sailBase.GetComponentInChildren<Animator>().transform;
            sail.windcenter.parent = sailObject;
            windShadow.parent = sailObject;
            sailObject.localPosition = position;
            sailObject.localEulerAngles = eulerAngles;
            sail.windcenter.parent = sailBase;
            windShadow.parent = sailBase;
            sailObject.SetAsFirstSibling();
            SailPartLocations offsetStore = sailObject.gameObject.AddComponent<SailPartLocations>();
            offsetStore.forwardOffset = position.x / scale;
            var col_parent = sailBase.GetComponent<SailConnections>().colChecker.transform;
            UnityEngine.Object.Destroy(col_parent.GetComponent<Rigidbody>());
            var locs = col_parent.gameObject.AddComponent<SailPartLocations>();

            foreach (ShipyardSailColCheckerSub child in col_parent.GetComponentsInChildren<ShipyardSailColCheckerSub>())
            {
                child.transform.localPosition += position;
                UnityEngine.Object.Destroy(child.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(child.GetComponent<ShipyardSailColCheckerSub>());
                locs.locations.Add(child.transform.localPosition);
            }
            sail.installHeight = (float)Math.Round(sail.installHeight * (scale / sailObject.localScale.y), 2);
            sailObject.localScale = new Vector3(scale, scale, scale);
            sailBase.gameObject.SetActive(false);
            sailPrefabs[newIndex] = sailBase.gameObject;
            return sailBase.gameObject;
        }
    }

    public class SailPartLocations : MonoBehaviour
    {
        public List<Vector3> locations = new List<Vector3>();
        public float forwardOffset = 0f;
    }

}
