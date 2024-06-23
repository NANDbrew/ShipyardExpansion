using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{

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
        public static void AwakePatch(GameObject ___partsOtherMenu, ShipyardUI __instance, GameObject ___ui)
        {
            oldButton = ___ui.transform.Find("mode button Parts Other");
            Transform newButton = UnityEngine.Object.Instantiate(oldButton, ___ui.transform);
            newButton.localPosition = oldButton.localPosition + new Vector3(0, 1.26f, 0);
            newButton.GetComponent<ShipyardButton>().index = 3;
            newButton.name = "mode button Parts Extra";
            newButton.GetComponent<ShipyardButton>().SetText("Extra");
            newButton.gameObject.SetActive(false);
            extraButton = newButton.gameObject;
        }

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePatch2(ShipyardUI __instance, ref GameObject[] ___mastButtons)
        {

            GameObject[] newButtons = new GameObject[Plugin.mastListSize];
            for (int i = 0; i < newButtons.Length; i++)
            {
                if (i < ___mastButtons.Length)
                {
                    newButtons[i] = ___mastButtons[i];
                }
                else
                {
                    var button = UnityEngine.Object.Instantiate(___mastButtons[0], ___mastButtons[0].transform.parent).gameObject;
                    button.name = "shipyard ui mast button (" + i + ")";
                    button.GetComponent<ShipyardButton>().index = i;
                    newButtons[i] = button;
                }
            }
            ___mastButtons = newButtons;
        }
    }
}
