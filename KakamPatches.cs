using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class KakamPatches
    {
        public static void Patch(Transform boat, List<BoatPart> partsList)
        {
            Transform container = boat.Find("junk small");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast");
            Transform mainMast2 = structure.Find("mast_center");
            Transform mizzenMast = structure.Find("mast_001");
            //Transform shrouds = container.Find("Cylinder_002");
            //var shroudAnchor = container.Find("static_rig_001");

            #region adjustments
            partsList[1].category = 2;
            partsList[4].category = 2;
            #endregion

        }

    }
}
