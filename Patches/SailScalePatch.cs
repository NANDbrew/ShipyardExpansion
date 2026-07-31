using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Sail))]
    public static class SailScalePatch
    {
        [HarmonyPatch("GetScaleZ")]
        [HarmonyPostfix]
        public static void GetScaleZPatch(Sail __instance, ref float __result, Cloth ___cloth)
        {
            if (__instance.GetComponent<SailScaler>()?.scaleType == ScaleType.Jib)
            {
                __result = ___cloth.transform.parent.localScale.x;
            }
/*            else if (__instance.GetComponent<SailScaler>() == null)
            {
                string text = __instance.name;
                Transform parent = __instance.transform.parent;
                for (int i = 0; i < 10; i++)
                {
                    if (parent == null) break;

                    text = parent.name + "." + text;
                    parent = parent.parent;

                }
                Debug.Log("SE: Missing Sail Scaler at " + text);
            }*/
        }

        [HarmonyPatch("LoadScale")]
        [HarmonyPostfix]
        public static void LoadScalePatch(Sail __instance, float y, float z)
        {
            __instance.GetComponent<SailScaler>().SetScaleAbs(z, y);
        }

        [HarmonyPatch("ChangeScale")]
        [HarmonyPrefix]
        public static bool ChangeScalePatch(Sail __instance, ref string ___sailName, float changeY, float changeZ)
        {
            if (Plugin.overrideScaling.Value)
            {
                __instance.GetComponent<SailScaler>().SetScaleAbs(__instance.GetScaleZ() + changeZ, __instance.GetScaleY() + changeY);
            }
            ___sailName = NameSail(___sailName, __instance.GetScaleY(), __instance.GetScaleZ());
            return !Plugin.overrideScaling.Value;
        }
        private static string NameSail(string oldName, float y, float z)
        {
            string baseName = oldName.Contains("(") ? oldName.Substring(0, oldName.IndexOf('(')) : oldName;
            baseName = baseName.Trim();

            if (Plugin.percentSailNames.Value)
            {
                string size2 = "";
                if (!Mathf.Approximately(z, y)) size2 = "x" + Mathf.RoundToInt((z) * 100) + "%";
                return $"{baseName} ({Mathf.RoundToInt((y) * 100)}%{size2})";
            }
            return baseName;
        }

    }

}
