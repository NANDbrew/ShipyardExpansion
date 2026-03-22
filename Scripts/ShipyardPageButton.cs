using System.Collections.Generic;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{

    public class ShipyardPageButton : GoPointerButton
    {
        public int buttonType;
        public static List<GameObject> pageContainers = new List<GameObject>();
        public static TextMesh currentPageText;
        public static int currentPage = 0;
        public static int currentPageCount = 0;
        public static int pageSize = 8;

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
            if (currentPage < 0) currentPage = 0;
            for (int i = 0; i < pageContainers.Count; i++)
            {
                pageContainers[i].SetActive(i == currentPage);
            }
            currentPageText.text = (currentPage + 1).ToString() + " / " + currentPageCount.ToString();
        }
    }
}
