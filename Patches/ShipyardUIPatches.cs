using BepInEx.Bootstrap;
using HarmonyLib;
using ShipyardExpansion.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShipyardExpansion.Patches
{

    [HarmonyPatch(typeof(ShipyardUI))]
    internal static class ShipyardUIPatches
    {
        static GameObject scaleUpButton;
        static GameObject scaleDownButton;
        static GameObject increaseHeightButton;
        static GameObject decreaseHeightButton;
        static GameObject increaseWidthButton;
        static GameObject decreaseWidthButton;
        static GameObject rotateForwardButton;
        static GameObject rotateBackwardButton;
        static GameObject flipButton;
        static GameObject flipButton2;
        static GameObject textureButton;

        static int[] partCounts;

        [HarmonyPatch("RefreshPartsPanel")]
        [HarmonyPrefix]
        public static void RefreshPatch(int category, bool freshStart)
        {
            if (freshStart)
            {
                partCounts = new int[3];
                BoatCustomParts component = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();
                foreach (var part in component.availableParts)
                {
                    partCounts[part.category]++;
                }
            }
            ShipyardPageButton.currentPageCount = Mathf.CeilToInt((float)partCounts[category] / ShipyardPageButton.pageSize);
            if (ShipyardPageButton.currentPageCount == 0) ShipyardPageButton.currentPageCount = 1;
            ShipyardPageButton.UpdatePage();
        }

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePatch2(ref GameObject[] ___mastButtons, ref ShipyardButton[] ___partOptionsLeftButtons, ref ShipyardButton[] ___partOptionsRightButtons, ref TextMesh[] ___partOptionsTexts, GameObject ___newPartsMenu)
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

            var thing = UnityEngine.Object.Instantiate(AssetTools.bundle2.LoadAsset<GameObject>("Assets/ShipyardExpansion/page_buttons.prefab"), ___newPartsMenu.transform).transform;
            ShipyardPageButton.currentPageText = thing.Find("text").GetComponent<TextMesh>();
            thing.Find("page_left").gameObject.AddComponent<ShipyardPageButton>().buttonType = 0;
            thing.Find("page_right").gameObject.AddComponent<ShipyardPageButton>().buttonType = 1;

            ShipyardPageButton.pageSize = ___partOptionsTexts.Length;
            ShipyardPageButton.pageContainers = new List<GameObject>();
            var cont0 = new GameObject("PageContainer_0");
            cont0.transform.SetParent(___newPartsMenu.transform, false);

            for (int i = 0; i < ShipyardPageButton.pageSize; i++)
            {
                ___partOptionsLeftButtons[i].transform.SetParent(cont0.transform);
            }
            for (int i = 0; i < ShipyardPageButton.pageSize; i++)
            {
                ___partOptionsRightButtons[i].transform.SetParent(cont0.transform);
            }
            for (int i = 0; i < ShipyardPageButton.pageSize; i++)
            {
                ___partOptionsTexts[i].transform.SetParent(cont0.transform);
            }

            ShipyardPageButton.pageContainers.Add(cont0);
            for (int i = 1; i < Plugin.maxPartsPages; i++) // starting at 1 because 0 is already created
            {
                GameObject pageContainer = new GameObject($"PageContainer_{i}");
                pageContainer.transform.SetParent(___newPartsMenu.transform, false);

                ShipyardButton[] newLefts = new ShipyardButton[ShipyardPageButton.pageSize];
                for (int j = 0; j < ShipyardPageButton.pageSize; j++)
                {
                    newLefts[j] = UnityEngine.Object.Instantiate(___partOptionsLeftButtons[j], pageContainer.transform);
                }
                ___partOptionsLeftButtons = ___partOptionsLeftButtons.Concat(newLefts).ToArray();

                ShipyardButton[] newRights = new ShipyardButton[ShipyardPageButton.pageSize];
                for (int j = 0; j < ShipyardPageButton.pageSize; j++)
                {
                    newRights[j] = UnityEngine.Object.Instantiate(___partOptionsRightButtons[j], pageContainer.transform);
                }
                ___partOptionsRightButtons = ___partOptionsRightButtons.Concat(newRights).ToArray();

                TextMesh[] newTexts = new TextMesh[ShipyardPageButton.pageSize];
                for (int j = 0; j < ShipyardPageButton.pageSize; j++)
                {
                    newTexts[j] = UnityEngine.Object.Instantiate(___partOptionsTexts[j], pageContainer.transform);
                }
                ___partOptionsTexts = ___partOptionsTexts.Concat(newTexts).ToArray();

                ShipyardPageButton.pageContainers.Add(pageContainer);

            }
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(GameObject ___moveUpButton, GameObject ___sailMenu)
        {
            GameObject scalingButtons = UnityEngine.GameObject.Instantiate(AssetTools.bundle2.LoadAsset("sail scaling buttons.prefab"), ___moveUpButton.transform.parent) as GameObject;
            
#if DEBUG   // REMOVE THIS BEFORE RELEASE
            scalingButtons.transform.Translate(-1f, 0, 0);
#endif
            scaleUpButton = scalingButtons.transform.Find("button scale up").gameObject;
            SailScaleButton buttonUp = scaleUpButton.AddComponent<SailScaleButton>();
            buttonUp.buttonType = SailScaleButton.ButtonType.scaleUp;

            scaleDownButton = scalingButtons.transform.Find("button scale down").gameObject;
            SailScaleButton buttonDown = scaleDownButton.AddComponent<SailScaleButton>();
            buttonDown.buttonType = SailScaleButton.ButtonType.scaleDown;

            increaseHeightButton = scalingButtons.transform.Find("button increase height").gameObject;
            SailScaleButton buttonTaller = increaseHeightButton.AddComponent<SailScaleButton>();
            buttonTaller.buttonType = SailScaleButton.ButtonType.increaseHeight;

            decreaseHeightButton = scalingButtons.transform.Find("button decrease height").gameObject;
            SailScaleButton buttonShorter = decreaseHeightButton.AddComponent<SailScaleButton>();
            buttonShorter.buttonType = SailScaleButton.ButtonType.decreaseHeight;

            increaseWidthButton = scalingButtons.transform.Find("button increase width").gameObject;
            SailScaleButton buttonWider = increaseWidthButton.AddComponent<SailScaleButton>();
            buttonWider.buttonType = SailScaleButton.ButtonType.increaseWidth;

            decreaseWidthButton = scalingButtons.transform.Find("button decrease width").gameObject;
            SailScaleButton buttonNarrower = decreaseWidthButton.AddComponent<SailScaleButton>();
            buttonNarrower.buttonType = SailScaleButton.ButtonType.decreaseWidth;

            rotateForwardButton = scalingButtons.transform.Find("button rotate forward").gameObject;
            SailScaleButton buttonrotFwd = rotateForwardButton.AddComponent<SailScaleButton>();
            buttonrotFwd.buttonType = SailScaleButton.ButtonType.rotateForward;

            rotateBackwardButton = scalingButtons.transform.Find("button rotate backward").gameObject;
            SailScaleButton buttonrotBkwd = rotateBackwardButton.AddComponent<SailScaleButton>();
            buttonrotBkwd.buttonType = SailScaleButton.ButtonType.rotateBackward;

            flipButton = scalingButtons.transform.Find("button flip").gameObject;
            SailScaleButton buttonflip = flipButton.AddComponent<SailScaleButton>();
            buttonflip.buttonType = SailScaleButton.ButtonType.flip;

            flipButton2 = scalingButtons.transform.Find("button flip p2").gameObject;
            SailScaleButton buttonflip2 = flipButton2.AddComponent<SailScaleButton>();
            buttonflip2.buttonType = SailScaleButton.ButtonType.flip;


            var textureB1 = scalingButtons.transform.Find("button sail texture").gameObject;
            var textureB2 = scalingButtons.transform.Find("button sail texture p2").gameObject;
            if (Chainloader.PluginInfos.ContainsKey("net.lilith.allsailcolors"))
            {
                textureButton = textureB2;
                textureB1.SetActive(false);
            }
            else
            {
                textureButton = textureB1;
                textureB2.SetActive(false);
            }
            textureButton.AddComponent<TextureButton>();

            GameObject furlButton = UnityEngine.GameObject.Instantiate(AssetTools.bundle2.LoadAsset("button sail toggle.prefab"), ___sailMenu.transform) as GameObject;
            furlButton.AddComponent<ShipyardUnfurlButton>();

        }
        [HarmonyPatch("UpdateMoveButtons")]
        [HarmonyPostfix]
        public static void UpdateMoveButtonsPatch()
        {
            bool active = GameState.currentShipyard.sailInstaller.GetCurrentSail() != null && !GameState.currentShipyard.sailInstaller.GetCurrentSail().IsInstalled();
            SailScaler currentSail = active? GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>() : null;
            bool rotatable = active && currentSail != null && currentSail.rotatablePart != null;
            bool heightable = active && currentSail != null && currentSail.GetScaleType().Equals(ScaleType.Square) && !Plugin.combinedScale.Value;
            bool widthable = active && currentSail != null && (currentSail.GetScaleType().Equals(ScaleType.Jib) || currentSail.GetScaleType().Equals(ScaleType.Square));
            bool flippable = active && currentSail != null && currentSail.flippable;
            
            scaleUpButton.SetActive(active && !heightable);
            scaleDownButton.SetActive(active && !heightable);

            increaseHeightButton.SetActive(heightable);
            decreaseHeightButton.SetActive(heightable);

            increaseWidthButton.SetActive(widthable);
            decreaseWidthButton.SetActive(widthable);

            rotateForwardButton.SetActive(rotatable);
            rotateBackwardButton.SetActive(rotatable);

            flipButton.SetActive(flippable && !widthable);
            flipButton2.SetActive(flippable && widthable);

            textureButton.SetActive(GameState.currentShipyard.sailInstaller.GetCurrentSail() != null);
        }
    }

    [HarmonyPatch(typeof(ShipyardButton), "SetText")]
    public static class ButtonPatch
    {
        public static void Prefix(ShipyardButton __instance, ref string text)
        {
            if (__instance.function == ShipyardButton.ButtonFunction.selectSail && text.Contains("%"))
            {
                text = text.Insert(text.IndexOf('('), "\n");
            }
        }

    }
}
