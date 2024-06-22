using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class JunkPatches
    {
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            //Mast[] masts = __instance.GetComponent<BoatRefs>().masts;
            Transform container = boat.Find("junk medium (actual)");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast_mid_0");
            Transform mainMast2 = structure.Find("mast_mid_1");
            Transform mizzenMast1 = structure.Find("mast_mizzen_0");
            Transform mizzenMast2 = structure.Find("mast_mizzen_1");
            Transform foreMast = structure.Find("mast_front_");

            #region adjustments
            mainMast1.GetComponent<Mast>().mastHeight += 1f;//= 17.5f;
            mainMast2.GetComponent<Mast>().mastHeight += 1.8f;//= 18.5f;
            mizzenMast1.GetComponent<Mast>().mastHeight += 3f;//= 12.6f;
            mizzenMast2.GetComponent<Mast>().mastHeight += 0.8f;//= 12.6f;
            foreMast.GetComponent<Mast>().mastHeight += 2f;//= 11.6f;

            mainMast1.GetComponent<Mast>().startSailHeightOffset += 1f;//= 17.5f;
            mainMast2.GetComponent<Mast>().startSailHeightOffset += 1.8f; //= 18.5f;
            mizzenMast1.GetComponent<Mast>().startSailHeightOffset += 3f;//= 12.6f;
            mizzenMast2.GetComponent<Mast>().startSailHeightOffset += 0.8f;//= 12.6f;
            foreMast.GetComponent<Mast>().startSailHeightOffset += 2f;//= 11.6f;

            partsList.availableParts[4].category = 2;
            partsList.availableParts[5].category = 2;
            partsList.availableParts[6].category = 2;
            partsList.availableParts[7].category = 2;

            #endregion
        }
    }

}
