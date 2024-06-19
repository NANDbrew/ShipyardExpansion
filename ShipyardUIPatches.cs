using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{
/*    [HarmonyPatch(typeof(ShipyardButton))]
    internal static class ShipyardButtonPatches
    {
        internal static int menuCategory;

        [HarmonyPatch("ExtraLateUpdate")]
        [HarmonyPostfix]
        public static void Patch(ShipyardButton __instance, int ___index, ShipyardButton.ButtonFunction ___function, ref bool ___overrideEnableOutline)
        {
            if (___function == ShipyardButton.ButtonFunction.changeCategory && ___index == menuCategory)
            {
                ___overrideEnableOutline = true;
            }
            else
            {
                ___overrideEnableOutline = false;
            }
        }
    }*/

    [HarmonyPatch(typeof(ShipyardUI))]
    internal static class ShipyardUIPatches
    {
        static GameObject extraButton;
        static Transform oldButton;
        //static List<ShipyardButton> buttonList;

        [HarmonyPatch("RefreshPartsPanel")]
        [HarmonyPostfix]
        public static void RefreshPatch(ShipyardUI __instance, TextMesh[] ___partOptionsTexts, int category, GameObject ___partsOtherMenu)
        {
            //if (!Plugin.showGizmos.Value) return;
            BoatCustomParts component = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();
            int partCount = 0;
            bool extraParts = false;
            foreach (var part in component.availableParts)
            {
                if (part.category == category)
                {
                    partCount++;
                    if(partCount > ___partOptionsTexts.Length)
                    {
                        part.category = 3;
                    }
                }
                if (part.category == 3) extraParts = true;
            }
            if (extraParts) extraButton.SetActive(true);
            else extraButton.SetActive(false);
            extraButton.transform.localPosition = oldButton.localPosition + new Vector3(1.67f, 0, 0);
            //ShipyardButtonPatches.menuCategory = category;
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(GameObject ___partsOtherMenu, ShipyardUI __instance)
        {
            //if (!Plugin.showGizmos.Value) return;
            /*          buttonList = new List<ShipyardButton>();
                      var buttons = __instance.GetComponentsInChildren<ShipyardButton>();
                      foreach (var button in buttons)
                      {
                          if (button.function == ShipyardButton.ButtonFunction.changeCategory)
                          {
                              buttonList.Add(button);
                          }
                      }*/

            oldButton = __instance.transform.GetChild(0).transform.Find("mode button Parts Other");
            Transform newButton = UnityEngine.Object.Instantiate(oldButton, __instance.transform.GetChild(0));
            newButton.localPosition = oldButton.localPosition + new Vector3(0, 1.26f, 0);
            newButton.GetComponent<ShipyardButton>().index = 3;
            newButton.name = "mode button Parts Extra";
            newButton.GetChild(0).GetComponent<TextMesh>().text = "Extra";
            newButton.gameObject.SetActive(false);
            extraButton = newButton.gameObject;
        }
    }
}
