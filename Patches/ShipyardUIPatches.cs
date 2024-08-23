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

        static GameObject scaleUpButton;
        static GameObject scaleDownButton;
        static GameObject increaseHeightButton;
        static GameObject decreaseHeightButton;
        static GameObject rotateForwardButton;
        static GameObject rotateBackwardButton;


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
        [HarmonyPrefix]
        public static void AwakePatch2(ShipyardUI __instance, ref GameObject[] ___mastButtons)
        {
            // mast buttons (clickable ball things for selecting masts)
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

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(GameObject ___partsOtherMenu, ShipyardUI __instance, GameObject ___ui, GameObject ___moveUpButton, GameObject ___moveDownButton)
        {
            oldButton = ___ui.transform.Find("mode button Parts Other");
            Transform newButton = UnityEngine.Object.Instantiate(oldButton, ___ui.transform);
            newButton.localPosition = oldButton.localPosition + new Vector3(0, 1.26f, 0);
            newButton.GetComponent<ShipyardButton>().index = 3;
            newButton.name = "mode button Parts Extra";
            newButton.GetComponent<ShipyardButton>().SetText("Extra");
            newButton.gameObject.SetActive(false);
            extraButton = newButton.gameObject;

            scaleUpButton = UnityEngine.Object.Instantiate(___moveUpButton, ___moveUpButton.transform.parent);
            scaleUpButton.transform.localPosition = new Vector3(4.2f, ___moveUpButton.transform.localPosition.y, ___moveUpButton.transform.localPosition.z);
            scaleUpButton.name = "button Scale Up";
            UnityEngine.Object.Destroy(scaleUpButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonUp = scaleUpButton.AddComponent<SailScaleButton>();
            buttonUp.buttonType = SailScaleButton.ButtonType.scaleUp;
            buttonUp.SetText("Scale\nUp");

            scaleDownButton = UnityEngine.Object.Instantiate(___moveDownButton, ___moveDownButton.transform.parent);
            scaleDownButton.transform.localPosition = new Vector3(4.2f, ___moveDownButton.transform.localPosition.y, ___moveDownButton.transform.localPosition.z);
            scaleDownButton.name = "button Scale Down";
            UnityEngine.Object.Destroy(scaleDownButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonDown = scaleDownButton.AddComponent<SailScaleButton>();
            buttonDown.buttonType = SailScaleButton.ButtonType.scaleDown;
            buttonDown.SetText("Scale\nDown");

            increaseHeightButton = UnityEngine.Object.Instantiate(___moveUpButton, ___moveUpButton.transform.parent);
            increaseHeightButton.transform.localPosition = new Vector3(2.7f, ___moveUpButton.transform.localPosition.y, ___moveUpButton.transform.localPosition.z);
            increaseHeightButton.name = "button Increase Height";
            UnityEngine.Object.Destroy(increaseHeightButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonTaller = increaseHeightButton.AddComponent<SailScaleButton>();
            buttonTaller.buttonType = SailScaleButton.ButtonType.increaseHeight;
            buttonTaller.SetText("Increase\nHeight");

            decreaseHeightButton = UnityEngine.Object.Instantiate(___moveDownButton, ___moveDownButton.transform.parent);
            decreaseHeightButton.transform.localPosition = new Vector3(2.7f, ___moveDownButton.transform.localPosition.y, ___moveDownButton.transform.localPosition.z);
            decreaseHeightButton.name = "button Decrease Height";
            UnityEngine.Object.Destroy(decreaseHeightButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonShorter = decreaseHeightButton.AddComponent<SailScaleButton>();
            buttonShorter.buttonType = SailScaleButton.ButtonType.decreaseHeight;
            buttonShorter.SetText("Decrease\nHeight");


            rotateForwardButton = UnityEngine.Object.Instantiate(___moveUpButton, ___moveUpButton.transform.parent);
            rotateForwardButton.transform.localPosition = new Vector3(2.7f, ___moveUpButton.transform.localPosition.y, ___moveUpButton.transform.localPosition.z);
            rotateForwardButton.name = "button Rotate Forward";
            UnityEngine.Object.Destroy(rotateForwardButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonrotBkwd = rotateForwardButton.AddComponent<SailScaleButton>();
            buttonrotBkwd.buttonType = SailScaleButton.ButtonType.rotateForward;
            buttonrotBkwd.SetText("Rotate\nForward");

            rotateBackwardButton = UnityEngine.Object.Instantiate(___moveDownButton, ___moveDownButton.transform.parent);
            rotateBackwardButton.transform.localPosition = new Vector3(2.7f, ___moveDownButton.transform.localPosition.y, ___moveDownButton.transform.localPosition.z);
            rotateBackwardButton.name = "button Rotate Backward";
            UnityEngine.Object.Destroy(rotateBackwardButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonrotFwd = rotateBackwardButton.AddComponent<SailScaleButton>();
            buttonrotFwd.buttonType = SailScaleButton.ButtonType.rotateBackward;
            buttonrotFwd.SetText("Rotate\nBackward");

        }
        [HarmonyPatch("UpdateMoveButtons")]
        [HarmonyPostfix]
        public static void UpdateMoveButtonsPatch()
        {
            bool active = GameState.currentShipyard.sailInstaller.GetCurrentSail() != null && !GameState.currentShipyard.sailInstaller.GetCurrentSail().IsInstalled();
            SailScaler currentSail = null;
            if (active) currentSail = GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>();
            bool rotatable = active && currentSail != null && currentSail.rotatablePart != null;
            bool heightable = active && currentSail != null && (currentSail.GetScaleType().Equals(ScaleType.Square) || currentSail.GetScaleType().Equals(ScaleType.Jib));
            scaleUpButton.SetActive(active);
            scaleDownButton.SetActive(active);

            increaseHeightButton.SetActive(heightable);
            decreaseHeightButton.SetActive(heightable);

            rotateForwardButton.SetActive(rotatable);
            rotateBackwardButton.SetActive(rotatable);
        }
    }
}
