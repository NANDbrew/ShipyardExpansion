using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{

    [HarmonyPatch(typeof(ShipyardUI))]
    internal static class ShipyardUIPatches2
    {

        static int[] partCounts;
        static List<GameObject> pageContainers;
        static TextMesh currentPageText;
        static int currentPage = 0;
        static int currentPageCount = 0;
        static int pageSize = 8;

        [HarmonyPatch("RefreshPartsPanel")]
        [HarmonyPrefix]
        public static void RefreshPatch(ShipyardUI __instance, TextMesh[] ___partOptionsTexts, int category, bool freshStart)
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
            currentPageCount = Mathf.CeilToInt((float)partCounts[category] / pageSize);
            PageButton.UpdatePage();
        }

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePatch2(ShipyardUI __instance, GameObject ___sailMenu, ref ShipyardButton[] ___partOptionsLeftButtons, ref ShipyardButton[] ___partOptionsRightButtons, ref TextMesh[] ___partOptionsTexts, GameObject ___newPartsMenu)
        {
            
            var thing = UnityEngine.Object.Instantiate(AssetTools.bundle2.LoadAsset<GameObject>("Assets/ShipyardExpansion/page_buttons.prefab"), ___newPartsMenu.transform).transform;
            currentPageText = thing.Find("text").GetComponent<TextMesh>();
            thing.Find("page_left").gameObject.AddComponent<PageButton>().buttonType = 0;
            thing.Find("page_right").gameObject.AddComponent<PageButton>().buttonType = 1;

            pageSize = ___partOptionsTexts.Length;
            pageContainers = new List<GameObject>();
            var cont0 = new GameObject("PageContainer_0");
            cont0.transform.SetParent(___newPartsMenu.transform, false);

            for (int i = 0; i < pageSize; i++)
            {
                ___partOptionsLeftButtons[i].transform.SetParent(cont0.transform);
            }
            for (int i = 0; i < pageSize; i++)
            {
                ___partOptionsRightButtons[i].transform.SetParent(cont0.transform);
            }
            for (int i = 0; i < pageSize; i++)
            {
                ___partOptionsTexts[i].transform.SetParent(cont0.transform);
            }

            pageContainers.Add(cont0);
            for (int i = 1; i < Plugin.maxPartsPages; i++) // starting at 1 because 0 is already created
            {
                GameObject pageContainer = new GameObject($"PageContainer_{i}");
                pageContainer.transform.SetParent(___newPartsMenu.transform, false);

                ShipyardButton[] newLefts = new ShipyardButton[pageSize];
                for (int j = 0; j < pageSize; j++)
                {
                    newLefts[j] = UnityEngine.Object.Instantiate(___partOptionsLeftButtons[j], pageContainer.transform);
                }
                ___partOptionsLeftButtons = ___partOptionsLeftButtons.Concat(newLefts).ToArray();

                ShipyardButton[] newRights = new ShipyardButton[pageSize];
                for (int j = 0; j < pageSize; j++)
                {
                    newRights[j] = UnityEngine.Object.Instantiate(___partOptionsRightButtons[j], pageContainer.transform);
                }
                ___partOptionsRightButtons = ___partOptionsRightButtons.Concat(newRights).ToArray();

                TextMesh[] newTexts = new TextMesh[pageSize];
                for (int j = 0; j < pageSize; j++)
                {
                    newTexts[j] = UnityEngine.Object.Instantiate(___partOptionsTexts[j], pageContainer.transform);
                }
                ___partOptionsTexts = ___partOptionsTexts.Concat(newTexts).ToArray();
                
                pageContainers.Add(pageContainer);
                
            }
            //PageButton.UpdatePage();

        }

        public class PageButton : GoPointerButton
        {
            public int buttonType;

            public override void OnActivate()
            {
                UISoundPlayer.instance.PlayUISound(UISounds.buttonHover, 0.33f, 3f);
                ShipyardUI.instance.HideNewSailButtons();
                if (buttonType == 0)
                {
                    if (currentPage > 0)
                    {
                        currentPage--;
                        UpdatePage();
                    }
                }
                else if (buttonType == 1)
                {
                    if (currentPage < currentPageCount - 1)
                    {
                        currentPage++;
                        UpdatePage();
                    }
                }
            }
            public static void UpdatePage()
            {
                if (currentPage >= currentPageCount)
                {
                    currentPage = currentPageCount - 1;
                }
                for (int i = 0; i < pageContainers.Count; i++)
                {
                    pageContainers[i].SetActive(i == currentPage);
                }
                currentPageText.text = (currentPage + 1).ToString() + " / " + currentPageCount.ToString();
            }
        }
        
    }
}
