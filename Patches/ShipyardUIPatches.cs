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
        static GameObject increaseWidthButton;
        static GameObject decreaseWidthButton;
        static GameObject rotateForwardButton;
        static GameObject rotateBackwardButton;

        static List<BoatPart>[] partCounts;
        static bool extraParts;
        //static List<ShipyardButton> buttonList;

        [HarmonyPatch("RefreshPartsPanel")]
        [HarmonyPostfix]
        public static void RefreshPatch(ShipyardUI __instance, TextMesh[] ___partOptionsTexts, bool freshStart)
        {
            if (!Plugin.extra.Value) return;
            if (freshStart)
            {
                extraParts = false;
                partCounts = new List<BoatPart>[4] { new List<BoatPart>(), new List<BoatPart>(), new List<BoatPart>(), new List<BoatPart>(), };
                BoatCustomParts component = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();
                foreach (var part in component.availableParts)
                {
                    partCounts[part.category].Add(part);
                    if (partCounts[part.category].Count > ___partOptionsTexts.Length)
                    {
                        part.category = 3;
                        extraParts = true;
                    }
                }

            }
            if (partCounts[3].Count > 0) extraParts = true;
            extraButton.SetActive(extraParts);
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
            newButton.localPosition = oldButton.localPosition + new Vector3(1.67f, 0, 0);
            newButton.GetComponent<ShipyardButton>().index = 3;
            newButton.name = "mode button Parts Extra";
            newButton.GetComponent<ShipyardButton>().SetText("Extra");
            newButton.gameObject.SetActive(false);
            extraButton = newButton.gameObject;

            scaleUpButton = UnityEngine.Object.Instantiate(___moveUpButton, ___moveUpButton.transform.parent);
            scaleUpButton.transform.localPosition += new Vector3(-1.54f, 0.02f, 0);
            scaleUpButton.name = "button Scale Up";
            UnityEngine.Object.Destroy(scaleUpButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonUp = scaleUpButton.AddComponent<SailScaleButton>();
            buttonUp.buttonType = SailScaleButton.ButtonType.scaleUp;
            buttonUp.SetText("Scale\nUp");

            scaleDownButton = UnityEngine.Object.Instantiate(___moveDownButton, ___moveDownButton.transform.parent);
            scaleDownButton.transform.localPosition += new Vector3(-1.54f, 0.02f, 0);
            scaleDownButton.name = "button Scale Down";
            UnityEngine.Object.Destroy(scaleDownButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonDown = scaleDownButton.AddComponent<SailScaleButton>();
            buttonDown.buttonType = SailScaleButton.ButtonType.scaleDown;
            buttonDown.SetText("Scale\nDown");

            increaseHeightButton = UnityEngine.Object.Instantiate(___moveUpButton, ___moveUpButton.transform.parent);
            increaseHeightButton.transform.localPosition += new Vector3(-3.08f, 0.04f, 0);
            increaseHeightButton.name = "button Increase Height";
            UnityEngine.Object.Destroy(increaseHeightButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonTaller = increaseHeightButton.AddComponent<SailScaleButton>();
            buttonTaller.buttonType = SailScaleButton.ButtonType.increaseHeight;
            buttonTaller.SetText("Increase\nHeight");

            decreaseHeightButton = UnityEngine.Object.Instantiate(___moveDownButton, ___moveDownButton.transform.parent);
            decreaseHeightButton.transform.localPosition += new Vector3(-3.08f, 0.04f, 0);
            decreaseHeightButton.name = "button Decrease Height";
            UnityEngine.Object.Destroy(decreaseHeightButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonShorter = decreaseHeightButton.AddComponent<SailScaleButton>();
            buttonShorter.buttonType = SailScaleButton.ButtonType.decreaseHeight;
            buttonShorter.SetText("Decrease\nHeight");

            increaseWidthButton = UnityEngine.Object.Instantiate(___moveUpButton, ___moveUpButton.transform.parent);
            increaseWidthButton.transform.localPosition += new Vector3(-3.08f, 0.04f, 0);
            increaseWidthButton.name = "button Increase Width";
            UnityEngine.Object.Destroy(increaseWidthButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonWider = increaseWidthButton.AddComponent<SailScaleButton>();
            buttonWider.buttonType = SailScaleButton.ButtonType.increaseWidth;
            buttonWider.SetText("Increase\nWidth");

            decreaseWidthButton = UnityEngine.Object.Instantiate(___moveDownButton, ___moveDownButton.transform.parent);
            decreaseWidthButton.transform.localPosition += new Vector3(-3.08f, 0.04f, 0);
            decreaseWidthButton.name = "button Decrease Width";
            UnityEngine.Object.Destroy(decreaseWidthButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonNarrower = decreaseWidthButton.AddComponent<SailScaleButton>();
            buttonNarrower.buttonType = SailScaleButton.ButtonType.decreaseWidth;
            buttonNarrower.SetText("Decrease\nWidth");


            rotateForwardButton = UnityEngine.Object.Instantiate(___moveUpButton, ___moveUpButton.transform.parent);
            rotateForwardButton.transform.localPosition += new Vector3(-3.08f, 0.04f, 0);
            rotateForwardButton.name = "button Rotate Forward";
            UnityEngine.Object.Destroy(rotateForwardButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonrotFwd = rotateForwardButton.AddComponent<SailScaleButton>();
            buttonrotFwd.buttonType = SailScaleButton.ButtonType.rotateForward;
            buttonrotFwd.SetText("Rotate\nForward");

            rotateBackwardButton = UnityEngine.Object.Instantiate(___moveDownButton, ___moveDownButton.transform.parent);
            rotateBackwardButton.transform.localPosition += new Vector3(-3.08f, 0.04f, 0);
            rotateBackwardButton.name = "button Rotate Backward";
            UnityEngine.Object.Destroy(rotateBackwardButton.GetComponent<ShipyardButton>());
            SailScaleButton buttonrotBkwd = rotateBackwardButton.AddComponent<SailScaleButton>();
            buttonrotBkwd.buttonType = SailScaleButton.ButtonType.rotateBackward;
            buttonrotBkwd.SetText("Rotate\nBackward");

        }
        [HarmonyPatch("UpdateMoveButtons")]
        [HarmonyPostfix]
        public static void UpdateMoveButtonsPatch()
        {
            bool active = GameState.currentShipyard.sailInstaller.GetCurrentSail() != null && !GameState.currentShipyard.sailInstaller.GetCurrentSail().IsInstalled();
            SailScaler currentSail = null;
            if (active) currentSail = GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>();
            bool rotatable = active && currentSail != null && currentSail.rotatablePart != null;
            bool heightable = active && currentSail != null && (currentSail.GetScaleType().Equals(ScaleType.Square));
            bool widthable = active && currentSail != null && (currentSail.GetScaleType().Equals(ScaleType.Jib));
            scaleUpButton.SetActive(active);
            scaleDownButton.SetActive(active);

            increaseHeightButton.SetActive(heightable);
            decreaseHeightButton.SetActive(heightable);

            increaseWidthButton.SetActive(widthable);
            decreaseWidthButton.SetActive(widthable);

            rotateForwardButton.SetActive(rotatable);
            rotateBackwardButton.SetActive(rotatable);
        }
    }
}
