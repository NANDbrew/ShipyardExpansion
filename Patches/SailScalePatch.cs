using HarmonyLib;
using UnityEngine;
using static ShipyardButton;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Sail))]
    public static class SailScalePatch
    {
        [HarmonyPatch("LoadScale")]
        [HarmonyPostfix]
        public static void LoadPatch(Sail __instance, float y, float z)
        {
            __instance.GetComponent<SailScaler>().SetScaleAbs(z, y);
        }

        [HarmonyPatch("ChangeScale")]
        [HarmonyPostfix]
        public static void Postfix(Sail __instance, ref string ___sailName)
        {
            ___sailName = NameSail(___sailName, __instance.GetScaleY(), __instance.GetScaleZ());
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
