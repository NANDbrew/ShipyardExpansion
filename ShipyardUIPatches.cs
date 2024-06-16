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
    internal class ShipyardUIPatches
    {
        static GameObject extraButton;
/*        [HarmonyPatch("ShowUI")]
        [HarmonyPostfix]
        public static void ShowUIPatch(ShipyardUI __instance, TextMesh[] ___partOptionsTexts, GameObject ___partsOtherMenu)
        {
*//*            BoatCustomParts component = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();

            for (int i = 0; i < component.availableParts.Count; i++)
            {
                var part = component.availableParts[i];
                if (part.category == category && i >= ___partOptionsTexts.Length)
                {
                    part.category = 3;
                }
            }*//*
            if (newButton == null)
            {
                newButton = UnityEngine.Object.Instantiate(___partsOtherMenu, ___partsOtherMenu.transform.parent);
                newButton.transform.localPosition = newButton.transform.localPosition + new Vector3(0, 1.26f, 0);
                //newButton.GetComponent<ShipyardButton>().index = 3;

            }
        }*/
        [HarmonyPatch("RefreshPartsPanel")]
        [HarmonyPostfix]
        public static void RefreshPatch(ShipyardUI __instance, TextMesh[] ___partOptionsTexts, int category, GameObject ___partsOtherMenu)
        {
            BoatCustomParts component = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();
            int partCount = 0;
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
            }
            if (partCount > ___partOptionsTexts.Length) extraButton.SetActive(true);
            else extraButton.SetActive(false);
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(GameObject ___partsOtherMenu, ShipyardUI __instance)
        {
            Transform newButton = UnityEngine.Object.Instantiate(__instance.transform.GetChild(0).transform.Find("mode button Parts Other"), __instance.transform.GetChild(0));
            newButton.localPosition += new Vector3(0, 1.26f, 0);
            newButton.GetComponent<ShipyardButton>().index = 3;
            newButton.name = "mode button Parts Contd";
            newButton.GetChild(0).GetComponent<TextMesh>().text = "Cont.";
            newButton.gameObject.SetActive(false);
            extraButton = newButton.gameObject;
        }
    }
}
