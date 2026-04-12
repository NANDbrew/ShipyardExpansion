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
        public static void LoadPatch(Sail __instance, ref string ___sailName)
        {
            ___sailName = NameSail(___sailName, __instance.GetScaleY(), __instance.GetScaleZ());
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


    /*
        [HarmonyPatch(typeof(ShipyardSailInstaller), "ChangeSailScale")]
        public static class SailInstallerPatche99
        {
            public static bool Prefix(float changeY, float changeZ, ShipyardSailInstaller __instance, Sail ___selectedSail)
            {
                if (!___selectedSail)
                {
                    Debug.LogError("ShipyardSailInstaller: Can't change sail scale because selected sail is null.");
                    return false;
                }

                if (___selectedSail.IsInstalled())
                {
                    Debug.Log("Cannot scale sail: already installed.");
                    return false;
                }
                float startY = ___selectedSail.GetScaleY();
                float startZ = ___selectedSail.GetScaleZ();
                if (Mathf.Approximately(changeY, changeZ))
                {
                    changeZ *= startZ / startY;
                }
                var newY = Mathf.Max(Mathf.Min(startY + changeY, 3f), 0.2f);
                var newZ = Mathf.Max(Mathf.Min(startZ + changeZ, 3f), 0.2f);
                ___selectedSail.LoadScale(newY, newZ);
                __instance.MoveHeldSail(0f);
                string baseName = ___selectedSail.sailName.Contains("(") ? ___selectedSail.sailName.Substring(0, ___selectedSail.sailName.IndexOf('(')) : ___selectedSail.sailName;
                baseName = baseName.Trim();

                if (Plugin.percentSailNames.Value)
                {
                    string size2 = "";
                    if (newZ != newY) size2 = "x" + Mathf.RoundToInt((newZ) * 100) + "%";
                    ___selectedSail.sailName = $"{baseName} ({Mathf.RoundToInt((newY) * 100)}%{size2})";
                }
                else ___selectedSail.sailName = baseName;
                ShipyardUI.instance.UpdateDescriptionText();
                ShipyardUI.instance.UpdateMastSailsList();
                return false;
            }

        }


        [HarmonyPatch(typeof(ShipyardButton), "OnActivate")]
        public static class ScaleButtonPatch
        {
            public static bool Prefix(ShipyardButton __instance, ButtonFunction ___function, ref ShipyardButton ___clickHeldButton, ref float ___heldTimer)
            {
                if (___function == ButtonFunction.sizeDown)
                {
                    if (Plugin.combinedScale.Value)
                    {
                        GameState.currentShipyard.ChangeSailScale(-0.05f, -0.05f);
                    }
                    else
                    {
                        GameState.currentShipyard.ChangeSailScale(-0.05f, 0f);
                    }
                    HoldButton(__instance, ref ___clickHeldButton, ref ___heldTimer);
                    return false;
                }
                else if (___function == ButtonFunction.sizeUp)
                {
                    if (Plugin.combinedScale.Value)
                    {
                        __instance.SetText("+\nSize");
                        GameState.currentShipyard.ChangeSailScale(0.05f, 0.05f);
                    }
                    else
                    {
                        __instance.SetText("+\nHeight");
                        GameState.currentShipyard.ChangeSailScale(0.05f, 0f);
                    }
                    HoldButton(__instance, ref ___clickHeldButton, ref ___heldTimer);
                    return false;
                }
                else if (___function == ButtonFunction.widthDown)
                {
                    GameState.currentShipyard.ChangeSailScale(0f, -0.05f);
                    HoldButton(__instance, ref ___clickHeldButton, ref ___heldTimer);
                    return false;
                }
                else if (___function == ButtonFunction.widthUp)
                {
                    GameState.currentShipyard.ChangeSailScale(0f, 0.05f);
                    HoldButton(__instance, ref ___clickHeldButton, ref ___heldTimer);
                    return false;
                }
                return true;
            }
            private static void HoldButton(ShipyardButton caller, ref ShipyardButton clickHeldButton, ref float heldTimer)
            {
                if (clickHeldButton != caller)
                {
                    clickHeldButton = caller;
                    heldTimer = 0.16f;
                }
            }

        }*/

}
