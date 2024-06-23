using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class DhowPatches
    {
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.transform.Find("dhow");
            Transform mainMast = container.Find("mast");
            Mast mainMastM = mainMast.GetComponent<Mast>();
            Transform walkCols = mainMastM.walkColMast.parent;

            Transform mainMastTall = container.Find("mast_taller");
            Mast mainMastTallM = mainMastTall.GetComponent<Mast>();
            Transform mizzen_old = container.Find("mast_mizzen");
            Transform shortForestay = container.Find("forestay_low");
            Transform highForestay = container.Find("forestay_tall");

            Debug.Log("Dhow adjustments");
            #region adjustments
            partsList.availableParts[1].category = 2;
            partsList.availableParts[4].category = 2;


            var newCont = UnityEngine.Object.Instantiate(new GameObject(), mainMast).transform;
            newCont.name = "shrouds";

            var newContCols = UnityEngine.Object.Instantiate(new GameObject(), mainMastM.walkColMast).transform;
            newContCols.name = newCont.name;
            walkCols.Find("Cylinder_002").parent = newContCols;

            //newCont.localPosition = mainMast.localPosition;
            container.Find("Cylinder_002").parent = newCont;
            container.Find("rig_col").parent = newCont;
            container.Find("static_rig").parent = newCont;
            container.Find("static_rig_001").parent = newCont;
            container.Find("flag_low").parent = newCont;

            container.Find("Cylinder").gameObject.SetActive(false);
            mainMast.GetComponent<Mast>().reefWinch[0].rope = null;
            container.Find("rope_holder_001").parent = mainMast;
            container.Find("flag").parent = mainMast;

            var tallFlag = UnityEngine.Object.Instantiate(mainMast.Find("flag"), mainMastTall);
            tallFlag.localPosition = new Vector3(tallFlag.localPosition.x, tallFlag.localPosition.y, 0.7f);

            var newCont2 = UnityEngine.Object.Instantiate(newCont, mainMastTall, true);
            newCont2.name = newCont.name;
            var newCont2Cols = UnityEngine.Object.Instantiate(newContCols, mainMastTallM.walkColMast, true);
            var tallMastReefAtt = UnityEngine.Object.Instantiate(mainMast.Find("rope_holder_001"), mainMastTall, false).GetChild(0);
            mainMastTall.GetComponent<Mast>().mastReefAtt[0] = tallMastReefAtt;
            mainMastTall.GetComponent<Mast>().mastReefAtt[1] = tallMastReefAtt;
            #endregion

            Debug.Log("Dhow mizzenMast");
            #region mizzenMast
            Mast mizzen_new = Util.CopyMast(mainMast, mizzen_old.localPosition, mizzen_old.localEulerAngles, mainMast.localScale, "mast_mizzen_1", "mizzen mast", 31);
            mizzen_new.reefWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().reefWinch, mainMast.localPosition, mizzen_new.transform.localPosition + new Vector3(0.47f, -0.2f, 0));
            //mizzen_new.reefWinch[0].name = "";
            //mizzen_new.reefWinch[0].rope = null;
            //mizzen_new.reefWinch[1].name = "";
            mizzen_new.midAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().midAngleWinch, mainMast.localPosition, mainMast.localPosition + new Vector3(-1.45f, 0, 0));
            mizzen_new.leftAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().leftAngleWinch, Vector3.zero, Vector3.zero);
            mizzen_new.leftAngleWinch[0].transform.localPosition = new Vector3(-6.4f, 1.26f, 1.5f);
            mizzen_new.leftAngleWinch[0].transform.localEulerAngles = new Vector3(288, 99, 75);
            mizzen_new.rightAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().rightAngleWinch, Vector3.zero, Vector3.zero);
            mizzen_new.rightAngleWinch[0].transform.localPosition = new Vector3(-6.4f, 1.26f, -1.5f);
            mizzen_new.rightAngleWinch[0].transform.localEulerAngles = new Vector3(288, 59, 120);
            var rope_holder_aft = UnityEngine.Object.Instantiate(container.Find("rope_holder"), mizzen_new.transform);
            rope_holder_aft.transform.localPosition = mizzen_old.Find("rope_holder_002").localPosition;
            rope_holder_aft.transform.localRotation = mizzen_old.Find("rope_holder_002").localRotation;
            mizzen_new.midRopeAtt[0] = UnityEngine.Object.Instantiate(container.Find("rope_att_angle_extension"), rope_holder_aft.transform);
            mizzen_new.midRopeAtt[0].transform.localPosition = Vector3.zero;
            mizzen_new.midRopeAtt[1] = mizzen_new.midRopeAtt[0];
            //var mizzenReefAtt = UnityEngine.Object.Instantiate(container.Find("rope_holder_001"), mizzen_new.transform);
            var mizzenReefAtt = mizzen_new.transform.Find("rope_holder_001");
            //mizzenReefAtt.transform.localPosition = new Vector3(-0.2f, 0.03f, 0.6f);
            //mizzenReefAtt.transform.localEulerAngles = new Vector3(90, 97, 0);
            mizzen_new.mastReefAtt[0] = mizzenReefAtt.Find("att");
            mizzen_new.mastReefAtt[1] = mizzenReefAtt.Find("att");


            BoatPartOption emptyMizzen = Util.CreatePartOption(container, "(empty mizzen)", "(no mizzen mast)");
            BoatPart mizzenPart = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { emptyMizzen, mizzen_new.GetComponent<BoatPartOption>() });

            Debug.Log("Dhow mizzen shrouds");
            var mizzenShrouds = mizzen_new.transform.Find(newCont.name);
            mizzenShrouds.localPosition = new Vector3(-0.1f, -0.02f, -0.07f);
            mizzenShrouds.localEulerAngles = new Vector3(0, 10, 0);
            mizzenShrouds.localScale = new Vector3(-1, 1, 1);
            var mizShroudsCols = mizzen_new.walkColMast.GetChild(0);
            mizShroudsCols.localPosition = mizzenShrouds.localPosition;
            mizShroudsCols.localEulerAngles = mizzenShrouds.localEulerAngles;
            mizShroudsCols.localScale = mizzenShrouds.localScale;

            #endregion

            Debug.Log("Dhow raked mast");
            #region raked mainMast
            Mast rakedMain = Util.CopyMast(mainMastTall, new Vector3(5.2f, 9.2f, 0f), new Vector3(287, 90, 270), mainMastTall.localScale, "mast_raked", "raked mast", 32);
            rakedMain.reefWinch = Util.CopyWinches(mainMastTall.GetComponent<Mast>().reefWinch, Vector3.zero, Vector3.zero);
            rakedMain.reefWinch[0].transform.localPosition = new Vector3(2.43f, 0.76f, 0f);
            rakedMain.reefWinch[0].transform.localEulerAngles = new Vector3(343, 270, 90);
            rakedMain.reefWinch[1].transform.localPosition = new Vector3(2.65f, 0.76f, 0.21f);
            rakedMain.reefWinch[1].transform.localEulerAngles = new Vector3(0, 0, 90);
            partsList.availableParts[0].partOptions.Add(rakedMain.GetComponent<BoatPartOption>());
            var rakedShrouds = rakedMain.transform.Find(newCont2.name);
            rakedShrouds.localPosition = new Vector3(0.03f, -0.01f, -3.9f);
            rakedShrouds.localEulerAngles = new Vector3(2, 354, 9);
            rakedShrouds.Find("static_rig").localEulerAngles = new Vector3(359.4f, 343.62f, 91.478f);
            rakedShrouds.Find("static_rig_001").localEulerAngles = new Vector3(359.16f, 343.67f, 87.42f);
            var rakedShroudsCols = rakedMain.walkColMast.GetChild(0);
            rakedShroudsCols.localPosition = rakedShrouds.localPosition;
            rakedShroudsCols.localEulerAngles = rakedShrouds.localEulerAngles;
            rakedShroudsCols.localScale = rakedShrouds.localScale;

            #endregion


            Debug.Log("Dhow forestay");
            #region raked forestay
            Mast rakedForestay = Util.CopyMast(highForestay, new Vector3(5.4f, 9.7f, 0f), new Vector3(309f, 270f, 90f), new Vector3(1f, 1f, 0.78f), "forestay_raked", "high forestay", 33);
            rakedForestay.reefWinch = Util.CopyWinches(rakedForestay.reefWinch, Vector3.zero, Vector3.zero);
            rakedForestay.reefWinch[0].transform.localPosition = new Vector3(2.65f, 0.76f, 0.21f);
            rakedForestay.mastHeight = 9f;
            rakedForestay.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { rakedMain.GetComponent<BoatPartOption>(), container.Find("bowsprit").GetComponent<BoatPartOption>() };
            partsList.availableParts[4].partOptions.Add(rakedForestay.GetComponent<BoatPartOption>());
            #endregion

            //highForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
            container.Find("forestay").GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
            container.Find("forestay_low").GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
        }

    }
}
