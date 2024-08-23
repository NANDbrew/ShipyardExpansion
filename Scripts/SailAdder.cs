using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

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
            Array.Resize(ref ___sails, 160);
            var modSail1 = SailAdder.CopySail(___sails, 37, new Vector3(3f, 0.25f, 0), new Vector3(90, 354, 0), "lug huge", "balanced lug 10yd", 159);
            modSail1.transform.Find("sail M gaff huge").Find("SAIL_small_gaff").Find("boom_brace").gameObject.SetActive(false);
            modSail1.transform.Find("sail M gaff huge").Find("SAIL_small_gaff").Find("boom_brace_001").gameObject.SetActive(false);
            modSail1.GetComponent<Sail>().category = SailCategory.other;

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

            var lateen20 = SailAdder.CopySail(___sails, 121, Vector3.zero, new Vector3(90, 0, 0), 1.6f, "M lateen big 1.6", "lateen 20yd", 155);
        }
    }
    internal static class SailAdder
    {
        //public static Material badMat;
        public static bool hasRun = false;
        public static GameObject ModSail(GameObject[] sailPrefabs)
        {
            //List<GameObject> list = new List<GameObject>();

            Transform sailBase = UnityEngine.Object.Instantiate(sailPrefabs[37]).transform;
            var windCenter = sailBase.Find("wincenter & sail audio");
            var sailObject = sailBase.Find("sail M gaff huge");
            var windShadow = sailBase.Find("wind shadow col");
            windCenter.parent = sailObject;
            windShadow.parent = sailObject;
            sailObject.localPosition = new Vector3(3, 0.25f, 0);
            sailObject.localEulerAngles = new Vector3(90, 355, 0);
            var sailObjectSub = sailObject.Find("SAIL_small_gaff");
            sailObjectSub.Find("boom_brace").gameObject.SetActive(false);
            sailObjectSub.Find("boom_brace_001").gameObject.SetActive(false);
            windCenter.parent = sailBase;
            windShadow.parent = sailBase;
            /* windCenter.SetAsFirstSibling();
             windShadow.SetAsLastSibling();*/
            sailObject.SetSiblingIndex(2);

            var sail = sailBase.GetComponent<Sail>();
            sail.prefabIndex = 131;
            sail.sailName = "balanced lug 10yd";
            sail.category = SailCategory.other;
            sailBase.name = "131 SAIL lug huge";
            //list.Add(sailBase.gameObject);

            //hasRun = true;
            return sailBase.gameObject;
        }
        public static GameObject ModSail2(GameObject[] sailPrefabs)
        {
            //List<GameObject> list = new List<GameObject>();

            Transform sailBase = UnityEngine.Object.Instantiate(sailPrefabs[16]).transform;
            var windCenter = sailBase.Find("wincenter & sail audio");
            var sailObject = sailBase.Find("sail M small gaff");
            var windShadow = sailBase.Find("wind shadow col");
            windCenter.parent = sailObject;
            windShadow.parent = sailObject;
            sailObject.localPosition = new Vector3(1.5f, 0.2f, 0);
            sailObject.localEulerAngles = new Vector3(90, 355, 0);
            var sailObjectSub = sailObject.Find("SAIL_small_gaff");
            sailObjectSub.Find("boom_brace").gameObject.SetActive(false);
            sailObjectSub.Find("boom_brace_001").gameObject.SetActive(false);
            windCenter.parent = sailBase;
            windShadow.parent = sailBase;
            //windCenter.SetAsFirstSibling();
            //windShadow.SetAsLastSibling();
            sailObject.SetAsFirstSibling();
            sailObject.tag = "sailObject";
            var sail = sailBase.GetComponent<Sail>();
            sail.prefabIndex = 132;
            sail.sailName = "balanced lug 6yd";
            sail.category = SailCategory.other;
            sailBase.name = "132 SAIL lug small";

            var col_parent = sailObjectSub.Find("col_parent");
            UnityEngine.Object.Destroy(col_parent.GetComponent<Rigidbody>());
            for (int i = 0; i < col_parent.childCount; i++)
            {
                UnityEngine.Object.Destroy(col_parent.GetChild(i).GetComponent<Rigidbody>());
            }

            return sailBase.gameObject;
        }
        public static GameObject ModSail3(GameObject[] sailPrefabs)
        {
            //List<GameObject> list = new List<GameObject>();

            Transform sailBase = UnityEngine.Object.Instantiate(sailPrefabs[5]).transform;
            var windCenter = sailBase.Find("wincenter & sail audio");
            var sailObject = sailBase.Find("sail A tiny gaff");
            var windShadow = sailBase.Find("wind shadow col");
            windCenter.parent = sailObject;
            windShadow.parent = sailObject;
            sailObject.localPosition = new Vector3(1.5f, 0.2f, 0);
            sailObject.localEulerAngles = new Vector3(90, 355, 0);
            var sailObjectSub = sailObject.Find("SAIL_tiny_gaff");
            sailObjectSub.Find("boom_brace").gameObject.SetActive(false);
            sailObjectSub.Find("boom_brace_001").gameObject.SetActive(false);
            windCenter.parent = sailBase;
            windShadow.parent = sailBase;
            //windCenter.SetAsFirstSibling();
            //windShadow.SetAsLastSibling();
            sailObject.SetAsFirstSibling();
            sailObject.tag = "sailObject";
            var sail = sailBase.GetComponent<Sail>();
            sail.prefabIndex = 133;
            sail.sailName = "balanced lug 4yd";
            sail.category = SailCategory.other;
            sailBase.name = "133 SAIL lug tiny";

            var col_parent = sailObjectSub.Find("col_parent");
            UnityEngine.Object.Destroy(col_parent.GetComponent<Rigidbody>());
            for (int i = 0; i < col_parent.childCount; i++)
            {
                UnityEngine.Object.Destroy(col_parent.GetChild(i).GetComponent<Rigidbody>());
            }

            return sailBase.gameObject;
        }
        public static GameObject ModSail4(GameObject[] sailPrefabs)
        {
            //List<GameObject> list = new List<GameObject>();

            Transform sailBase = UnityEngine.Object.Instantiate(sailPrefabs[75]).transform;
            var windCenter = sailBase.Find("wincenter & sail audio");
            var sailObject = sailBase.Find("sail M small gaff 3");
            var windShadow = sailBase.Find("wind shadow col");
            windCenter.parent = sailObject;
            windShadow.parent = sailObject;
            sailObject.localPosition = new Vector3(2.35f, 0.25f, 0);
            sailObject.localEulerAngles = new Vector3(90, 354, 0);
            var sailObjectSub = sailObject.Find("SAIL_small_gaff");
            sailObjectSub.Find("boom_brace").gameObject.SetActive(false);
            sailObjectSub.Find("boom_brace_001").gameObject.SetActive(false);
            windCenter.parent = sailBase;
            windShadow.parent = sailBase;
            //windCenter.SetAsFirstSibling();
            //windShadow.SetAsLastSibling();
            sailObject.SetAsFirstSibling();
            sailObject.tag = "sailObject";
            var sail = sailBase.GetComponent<Sail>();
            sail.prefabIndex = 134;
            sail.sailName = "balanced lug 11yd";
            sail.category = SailCategory.other;
            sailBase.name = "134 SAIL lug huger";

            var col_parent = sailObjectSub.Find("col_parent");
            UnityEngine.Object.Destroy(col_parent.GetComponent<Rigidbody>());
            for (int i = 0; i < col_parent.childCount; i++)
            {
                UnityEngine.Object.Destroy(col_parent.GetChild(i).GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(col_parent.GetChild(i).GetComponent<ShipyardSailColCheckerSub>());
            }

            return sailBase.gameObject;
        }
        public static GameObject CopySail(GameObject[] sailPrefabs, int prefabIndex, Vector3 position, Vector3 eulerAngles, string name, string prettyName, int newIndex)
        {
            Transform sailObject = sailPrefabs[prefabIndex].GetComponentInChildren<Animator>().transform;
            return CopySail(sailPrefabs, prefabIndex, position, eulerAngles, sailObject.localScale.x, name, prettyName, newIndex);
        }
        public static GameObject CopySail(GameObject[] sailPrefabs, int prefabIndex, Vector3 position, Vector3 eulerAngles, float scale, string name, string prettyName, int newIndex)
        {
            //List<GameObject> list = new List<GameObject>();
            //Debug.Log("thing");
            Transform sailBase = UnityEngine.Object.Instantiate(sailPrefabs[prefabIndex]).transform;
            var sail = sailBase.GetComponent<Sail>();
            sail.prefabIndex = newIndex;
            sail.sailName = prettyName;
            //sail.category = SailCategory.other;
            sailBase.name = newIndex + " SAIL " + name;
            var windShadow = sailBase.Find("wind shadow col");

            //var sailObject = sailBase.Find(subName);
            Transform sailObject = sailBase.GetComponentInChildren<Animator>().transform;
            sail.windcenter.parent = sailObject;
            windShadow.parent = sailObject;
            sailObject.localPosition = position;
            sailObject.localEulerAngles = eulerAngles;
            sail.windcenter.parent = sailBase;
            windShadow.parent = sailBase;
            sailObject.SetAsFirstSibling();

            var col_parent = sailBase.GetComponent<SailConnections>().colChecker.transform;
            //col_parent.localScale = scale;
            UnityEngine.Object.Destroy(col_parent.GetComponent<Rigidbody>());
            for (int i = 0; i < col_parent.childCount; i++)
            {
                UnityEngine.Object.Destroy(col_parent.GetChild(i).GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(col_parent.GetChild(i).GetComponent<ShipyardSailColCheckerSub>());
            }
            sail.installHeight = (float)Math.Round(sail.installHeight * (scale / sailObject.localScale.y), 2);
            sailObject.localScale = new Vector3(scale, scale, scale);
            sailBase.gameObject.SetActive(false);
            //sailObject.GetChild(0).GetComponent<Renderer>().material = sailPrefabs[prefabIndex].GetComponent<Renderer>().sharedMaterial;
            //sail.sailArea = Mathf.Pow(Mathf.Sqrt(sail.sailArea) * scale, 2f);
            sailPrefabs[newIndex] = sailBase.gameObject;
            return sailBase.gameObject;
        }
    }
}
