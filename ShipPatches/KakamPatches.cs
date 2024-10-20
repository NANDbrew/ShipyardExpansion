﻿using HarmonyLib;
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
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.Find("junk small");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast");
            Transform mainMast2 = structure.Find("mast_center");
            Transform mizzenMast = structure.Find("mast_001");
            Mast mizzenMastM = mizzenMast.GetComponent<Mast>();
            //Transform shrouds = container.Find("Cylinder_002");
            //var shroudAnchor = container.Find("static_rig_001");
            Transform forestay = container.Find("forestay");
            Mast forestayM = forestay.GetComponent<Mast>();
            Transform lowerForestay = container.Find("forestay_002");
            Mast lowerForestayM = lowerForestay.GetComponent<Mast>();
            var angleWinches = new GPButtonRopeWinch[2] { mizzenMastM.leftAngleWinch[0], mizzenMastM.rightAngleWinch[0] };
            //var aftAngleWinches = new GPButtonRopeWinch[2] { mizzenMastM.leftAngleWinch[1], mizzenMastM.rightAngleWinch[1] };
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;

            PartRefs.kakam = container;
            PartRefs.kakamCol = walkCol;

            #region adjustments
            partsList.availableParts[0].category = 2;
            partsList.availableParts[7].category = 2;
            mainMast1.GetComponent<Mast>().mastHeight = 9.7f;
            mainMast2.GetComponent<Mast>().mastHeight = 9.7f;
            //mainMast1.GetComponent<Mast>().startSailHeightOffset -= 0.4f;
            mizzenMastM.leftAngleWinch = new GPButtonRopeWinch[1] { mizzenMastM.leftAngleWinch[1] };
            mizzenMastM.rightAngleWinch = new GPButtonRopeWinch[1] { mizzenMastM.rightAngleWinch[1] };
            mizzenMastM.GetComponent<Mast>().mastHeight = 8.4f;
            mizzenMastM.GetComponent<Mast>().extraBottomHeight = 0.6f;
            var m1brace = mizzenMast.Find("mast_holder_001");
            m1brace.localPosition = new Vector3(-1.29f, 0, -10.12f);
            m1brace.localScale = new Vector3(0.87f, 0.87f, 0.94f);
            //mizzenMastM.GetComponent<Mast>().startSailHeightOffset -= 0.5f;
            var ropeHolder_003 = mainMast1.Find("rope_holder_003");
            mainMast1.GetComponent<Mast>().midRopeAtt[0].parent = ropeHolder_003;
            ropeHolder_003.localPosition = new Vector3(-3f, 0f, -11.3f);
            ropeHolder_003.localEulerAngles = new Vector3(270f, 0f, 0f);

            container.Find("Cube_002").parent = mizzenMast;
            container.Find("Cube_003").parent = mizzenMast;
            container.Find("Cube_004").parent = structure.Find("struct_var_1__long_roof_");
            structure.Find("mast_003").parent = container.Find("hammock_001");
            #endregion
            
            #region midstays
            //var midstayReefs = Util.CopyWinches(mizzenMast.GetComponent<Mast>().reefWinch, Vector3.zero, Vector3.zero);
            //var angleWinches2 = Util.CopyWinches(angleWinches, Vector3.zero, new Vector3(0.3f, -0.05f, 0));

            /*Mast midstay_lower = Util.CopyMast(lowerForestay, new Vector3(-2f, 7f, 0f), lowerForestay.localEulerAngles, new Vector3(1f, 1f, 1.37f), "midstay_lower", "lower midstay", 31);
            midstay_lower.mastReefAtt = mizzenMastM.mastReefAtt;
            midstay_lower.reefWinch = new GPButtonRopeWinch[1] { midstayReefs[0] };
            midstay_lower.reefWinch[0].transform.localPosition = new Vector3(-2.87f, midstayReefs[0].transform.localPosition.y, 0f);
            midstay_lower.reefWinch[0].transform.localEulerAngles = new Vector3(0, 90, 90);
            midstay_lower.reefWinch[0].rope = null;
            midstay_lower.leftAngleWinch = new GPButtonRopeWinch[1] { angleWinches[0] };
            midstay_lower.rightAngleWinch = new GPButtonRopeWinch[1] { angleWinches[1] };
            midstay_lower.mastHeight = 7.5f;
            midstay_lower.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mainMast1.GetComponent<BoatPartOption>(), mizzenMast.GetComponent<BoatPartOption>() };
            partsList.availableParts[7].partOptions.Add(midstay_lower.GetComponent<BoatPartOption>());
*/
            //Mast midstay_upper = Util.CopyMast(lowerForestay, new Vector3(-2f, 10.5f, 0f), lowerForestay.localEulerAngles, new Vector3(1f, 1f, 1.654f), "midstay_upper", "top midstay", 32);
            Mast midstay_upper = Util.CopyMast(forestay, new Vector3(-2.4f, 10.6f, 0f), forestay.localEulerAngles, new Vector3(1, 1, 1.14f), "midstay", "middlestay", 32);
            midstay_upper.mastReefAtt = mizzenMastM.mastReefAtt;
            midstay_upper.reefWinch = new GPButtonRopeWinch[1] { Util.CopyWinch(mizzenMast.GetComponent<Mast>().reefWinch[1], new Vector3(-3.27f, mizzenMast.GetComponent<Mast>().reefWinch[1].transform.localPosition.y, 0f)) };
            midstay_upper.reefWinch[0].transform.localEulerAngles = new Vector3(0, 270, 90);
            midstay_upper.reefWinch[0].rope = null;
            midstay_upper.leftAngleWinch = new GPButtonRopeWinch[1] { angleWinches[0] };
            midstay_upper.rightAngleWinch = new GPButtonRopeWinch[1] { angleWinches[1] };
            //midstay_upper.mastHeight = 10f;
            midstay_upper.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mainMast1.GetComponent<BoatPartOption>(), mizzenMast.GetComponent<BoatPartOption>() };
            var midstay_upper_none = Util.CreatePartOption(container, "(no midstay)", "(no middlestay)");
            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { midstay_upper_none, midstay_upper.GetComponent<BoatPartOption>() });

            #endregion

        }

    }
}
