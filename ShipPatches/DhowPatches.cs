using HarmonyLib;
using System;
using System.Collections;
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
            Transform lowForestay = container.Find("forestay");

            PartRefs.dhow = container;
            PartRefs.dhowCol = walkCols;

            Debug.Log("Dhow adjustments");
            #region adjustments
            partsList.availableParts[1].category = 2;
            partsList.availableParts[4].category = 2;

            shortForestay.localScale = new Vector3(1, 1, 1.02f);
            shortForestay.GetComponent<BoatPartOption>().requires.Remove(container.Find("bowsprit").GetComponent<BoatPartOption>());
            container.Find("empty low forestay").GetComponent<BoatPartOption>().optionName = "(no lower forestay)";

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
            mainMast.GetComponent<Mast>().midAngleWinch[0].rope = null;
            mainMast.GetComponent<Mast>().midAngleWinch[1].rope = null;
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

            shortForestay.GetComponent<Mast>().mastReefAtt = lowForestay.GetComponent<Mast>().mastReefAtt;
            #endregion

            Debug.Log("Dhow mizzenMast");
            #region mizzenMast
            Mast mizzen_new = Util.CopyMast(mainMast, new Vector3(-4.87f, 6.16f, 0), mizzen_old.localEulerAngles, mainMast.localScale, "mast_mizzen_1", "mizzen mast", 31);
            mizzen_new.reefWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().reefWinch, mainMast.localPosition, mizzen_new.transform.localPosition + new Vector3(0.47f, -0.2f, 0));
            mizzen_new.midAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().midAngleWinch, mainMast.localPosition, mainMast.localPosition + new Vector3(-1.44f, 0, 0));
            mizzen_new.leftAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().leftAngleWinch, Vector3.zero, Vector3.zero);
            mizzen_new.leftAngleWinch[0].transform.localPosition = new Vector3(-6.4f, 1.26f, 1.5f);
            mizzen_new.leftAngleWinch[0].transform.localEulerAngles = new Vector3(288, 99, 75);
            mizzen_new.rightAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().rightAngleWinch, Vector3.zero, Vector3.zero);
            mizzen_new.rightAngleWinch[0].transform.localPosition = new Vector3(-6.4f, 1.26f, -1.5f);
            mizzen_new.rightAngleWinch[0].transform.localEulerAngles = new Vector3(288, 59, 120);
            var rope_holder_aft = UnityEngine.Object.Instantiate(container.Find("rope_holder"), mizzen_new.transform);
            rope_holder_aft.transform.position = mizzen_old.Find("rope_holder_002").position;
            rope_holder_aft.transform.rotation = mizzen_old.Find("rope_holder_002").rotation;
            mizzen_new.midRopeAtt[0] = UnityEngine.Object.Instantiate(container.Find("rope_att_angle_extension"), rope_holder_aft.transform);
            mizzen_new.midRopeAtt[0].transform.localPosition = Vector3.zero;
            mizzen_new.midRopeAtt[1] = mizzen_new.midRopeAtt[0];
            var mizzenReefAtt = mizzen_new.transform.Find("rope_holder_001");
            mizzen_new.mastReefAtt[0] = mizzenReefAtt.Find("att");
            mizzen_new.mastReefAtt[1] = mizzenReefAtt.Find("att");


            BoatPartOption emptyMizzen = Util.CreatePartOption(container, "(empty mizzen)", "(no mizzen mast)");
            BoatPart mizzenPart = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { emptyMizzen, mizzen_new.GetComponent<BoatPartOption>() });

            Debug.Log("Dhow mizzen shrouds");
            var mizzenShrouds = mizzen_new.transform.Find(newCont.name);
            mizzenShrouds.localPosition = new Vector3(-0.07f, 0f, 0f);
            mizzenShrouds.localEulerAngles = new Vector3(359.5f, 10, 0);
            mizzenShrouds.localScale = new Vector3(-1, 1, 0.95f);
            var mizShroudsCols = mizzen_new.walkColMast.GetChild(0);
            mizShroudsCols.localPosition = mizzenShrouds.localPosition;
            mizShroudsCols.localEulerAngles = mizzenShrouds.localEulerAngles;
            mizShroudsCols.localScale = mizzenShrouds.localScale;

            #endregion

            Debug.Log("Dhow raked mast");
            #region raked mainMast
            Mast rakedMain = Util.CopyMast(mainMastTall, new Vector3(5.2f, 9.2f, 0f), new Vector3(287, 90, 270), mainMastTall.localScale, "mast_raked", "raked mast", 32);
            rakedMain.reefWinch = Util.CopyWinches(mainMastTall.GetComponent<Mast>().reefWinch, Vector3.zero, Vector3.zero);
            rakedMain.reefWinch[0].transform.localPosition = new Vector3(2.4f, 0.66f, 0f);
            rakedMain.reefWinch[0].transform.localEulerAngles = new Vector3(343, 270, 90);
            rakedMain.reefWinch[1].transform.localPosition = new Vector3(2.55f, 0.66f, 0.21f);
            rakedMain.reefWinch[1].transform.localEulerAngles = new Vector3(0, 0, 90);
            partsList.availableParts[0].partOptions.Add(rakedMain.GetComponent<BoatPartOption>());
            var rakedShrouds = rakedMain.transform.Find(newCont2.name);
            rakedShrouds.localPosition = new Vector3(0.04f, -0.01f, -3.2f);
            rakedShrouds.localEulerAngles = new Vector3(2, 352.4f, 9);
            rakedShrouds.Find("static_rig").localPosition = new Vector3(0.0643f, -1.5089f, -5.6900f);
            rakedShrouds.Find("static_rig").localEulerAngles = new Vector3(359.16f, 345.62f, 91.4780f);
            rakedShrouds.Find("static_rig_001").localPosition = new Vector3(-0.47f, -1.523f, -5.82f);
            rakedShrouds.Find("static_rig_001").localEulerAngles = new Vector3(359.16f, 343.67f, 87.42f);
            var rakedShroudsCols = rakedMain.walkColMast.GetChild(0);
            rakedShroudsCols.localPosition = rakedShrouds.localPosition;
            rakedShroudsCols.localEulerAngles = rakedShrouds.localEulerAngles;
            rakedShroudsCols.localScale = rakedShrouds.localScale;

            #endregion


            Debug.Log("Dhow forestay");
            #region raked forestay
            Mast rakedForestay = Util.CopyMast(highForestay, new Vector3(5.4f, 9.7f, 0f), new Vector3(309f, 270f, 90f), new Vector3(1f, 1f, 0.78f), "forestay_raked", "high forestay 2", 33);
            rakedForestay.reefWinch = Util.CopyWinches(rakedForestay.reefWinch, Vector3.zero, Vector3.zero);
            rakedForestay.reefWinch[0].transform.localPosition = new Vector3(2.55f, 0.66f, -0.21f);
            //rakedForestay.mastHeight = 9f;
            rakedForestay.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { rakedMain.GetComponent<BoatPartOption>(), container.Find("bowsprit").GetComponent<BoatPartOption>() };
            partsList.availableParts[1].partOptions.Add(rakedForestay.GetComponent<BoatPartOption>());
            #endregion

            #region midstay
            Mast midstay = Util.CopyMast(lowForestay, new Vector3(-4.88f, 7.1f, 0f), new Vector3(324.18f, 270f, 90f), new Vector3(1f, 1f, 0.83f), "midstay", "middlestay", 34);
            midstay.reefWinch = Util.CopyWinches(midstay.reefWinch, mainMast.localPosition + new Vector3(-0.45f, 0, 0), mizzen_new.transform.localPosition);
            //midstay.reefWinch[0].transform.localPosition = new Vector3(2.65f, 0.76f, -0.21f);
            //midstay.mastHeight = 9f;
            midstay.leftAngleWinch = new GPButtonRopeWinch[1] { mainMast.GetComponent<Mast>().leftAngleWinch[1] };
            midstay.rightAngleWinch = new GPButtonRopeWinch[1] { mainMast.GetComponent<Mast>().rightAngleWinch[1] };
            var midstayOpt = midstay.GetComponent<BoatPartOption>();
            midstayOpt.requires = new List<BoatPartOption> { mizzen_new.GetComponent<BoatPartOption>() };
            midstayOpt.requiresDisabled = new List<BoatPartOption> { rakedMain.GetComponent<BoatPartOption>() };
            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { Util.CreatePartOption(container, "(no midstay)", "(no middlestay)"), midstayOpt});
            #endregion

            #region telltales
            BoatPartOption telltaleNone = Util.CreatePartOption(container, "telltale_none", "no telltale");
            BoatPartOption telltaleMain = Util.CreatePartOption(container, "telltale_main", "mainmast telltale");
            telltaleMain.basePrice = 10;
            BoatPartOption telltaleMizzen = Util.CreatePartOption(container, "telltale_mizzen", "mizzen telltale");
            telltaleMizzen.basePrice = 10;
            telltaleMizzen.requires = new List<BoatPartOption> { mizzen_new.GetComponent<BoatPartOption>() };
            GameObject flagMain0 = mainMast.Find("shrouds").Find("flag_low").gameObject;
            GameObject flagMain1 = mainMastTall.Find("shrouds").Find("flag_low").gameObject;
            GameObject flagMain2 = rakedMain.transform.Find("shrouds").Find("flag_low").gameObject;
            GameObject flagMizzen = mizzenShrouds.Find("flag_low").gameObject;
            telltaleMain.childOptions = new GameObject[] { flagMain0, flagMain1, flagMain2 };
            telltaleMizzen.childOptions = new GameObject[] { flagMizzen };
            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption> { telltaleMain, telltaleMizzen, telltaleNone });
            #endregion

            #region late adjustments
            //highForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
            lowForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
            shortForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());

            Array.Resize(ref mainMast.GetComponent<Mast>().leftAngleWinch, 1);
            Array.Resize(ref mainMast.GetComponent<Mast>().rightAngleWinch, 1);
            #endregion
            partsList.StartCoroutine(AddRoof(container, walkCols, partsList));
        }
        private static IEnumerator AddRoof(Transform structure, Transform walkCols, BoatCustomParts partsList)
        {
            Debug.Log("waiting for cog roof...");
            yield return new WaitUntil(() => PartRefs.cog != null);
            Debug.Log("found cog roof");
            #region roof
            Transform newRoof = UnityEngine.Object.Instantiate(PartRefs.cog.Find("struct_var_1__low_roof_"), structure);
            newRoof.name = "cabin";
            newRoof.localScale = new Vector3(0.9f, 0.75f, 1);
            newRoof.localPosition = new Vector3(0.6f, -0.85f, 0);
            newRoof.Find("trim_008").gameObject.SetActive(false);
            newRoof.Find("trim_013").gameObject.SetActive(false);
            for (int i = 0; i < newRoof.childCount; i++)
            {
                newRoof.GetChild(i).GetComponent<Renderer>().material.color = new Color(0.36f, 0.37f, 0.37f);
                newRoof.GetChild(i).GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.2f);
            }
            newRoof.Find("trim_000").GetComponent<Renderer>().material.color = new Color(0.51f, 0.5f, 0.53f);
            Transform supportsLeft = UnityEngine.Object.Instantiate(new GameObject().transform, newRoof);
            supportsLeft.name = "supports_left";
            Transform support_0L = newRoof.Find("trim_018");
            support_0L.name = "support_0";
            support_0L.transform.SetParent(supportsLeft);
            support_0L.localScale = new Vector3(1.85f, 1.22f, 0.6f);
            support_0L.localEulerAngles = new Vector3(10, 0, 270);
            support_0L.localPosition = new Vector3(-2.49f, -14.7f, -1.4f);
            Transform support_1L = UnityEngine.Object.Instantiate(support_0L, supportsLeft);
            support_1L.name = "support_1";
            support_1L.localScale = new Vector3(1.85f, 1.22f, 0.53f);
            support_1L.localEulerAngles = new Vector3(0, 0, 270);
            support_1L.localPosition = new Vector3(-7.49f, -14.9f, 1.47f);
            Transform support_2L = UnityEngine.Object.Instantiate(support_0L, supportsLeft);
            support_2L.name = "support_2";
            //support_2L.localScale = new Vector3(1.85f, 1.22f, 0.5f);
            support_2L.localEulerAngles = new Vector3(8, 0, 270);
            support_2L.localPosition = new Vector3(-3.58f, -14.76f, -0.9f);
            Transform support_3L = UnityEngine.Object.Instantiate(support_0L, supportsLeft);
            support_3L.name = "support_3";
            support_3L.localScale = new Vector3(1.85f, 1.22f, 0.55f);
            support_3L.localEulerAngles = new Vector3(1, 0, 270);
            support_3L.localPosition = new Vector3(-5.22f, -14.94f, 0.88f);
            Transform support_4L = UnityEngine.Object.Instantiate(support_0L, supportsLeft);
            support_4L.name = "support_4";
            support_4L.localScale = new Vector3(1.85f, 1.22f, 0.53f);
            support_4L.localEulerAngles = new Vector3(0, 0, 270);
            support_4L.localPosition = new Vector3(-6.52f, -14.94f, 1.30f);
            supportsLeft.localScale = new Vector3(1, 1, 1.03f);
            supportsLeft.localPosition = new Vector3(0, 0, -0.1f);
            Transform supportsRight = UnityEngine.Object.Instantiate(supportsLeft, newRoof);
            supportsRight.name = "supports_right";
            supportsRight.localScale = new Vector3(supportsLeft.localScale.x, -supportsLeft.localScale.y, supportsLeft.localScale.z);
            #endregion

            #region walkCol
            Transform newRoofCol = UnityEngine.Object.Instantiate(PartRefs.cogCol.Find("struct_var_1__low_roof_"), walkCols);
            newRoofCol.localPosition = newRoof.localPosition;
            newRoofCol.localScale = newRoof.localScale;
            newRoofCol.name = newRoof.name;
            newRoofCol.Find("trim_008").gameObject.SetActive(false);
            newRoofCol.Find("trim_013").gameObject.SetActive(false);
/*            newRoofCol.Find("trim_012").localScale = new Vector3(0.94f, 1, 1.05f);
            newRoofCol.Find("trim_004").localScale = new Vector3(0.95f, 1, 1.05f);
            newRoofCol.Find("trim_000").localScale = new Vector3(0.94f, 0.97f, 1.05f);
*/
            Transform supportColsLeft = UnityEngine.Object.Instantiate(new GameObject().transform, newRoofCol);
            supportColsLeft.name = supportsLeft.name;
            Transform supportCol_0L = newRoofCol.Find("trim_018");
            supportCol_0L.transform.SetParent(supportColsLeft);
            supportCol_0L.localScale = support_0L.localScale;
            supportCol_0L.localEulerAngles = support_0L.localEulerAngles;
            supportCol_0L.localPosition = support_0L.localPosition;
            Transform supportCol_1L = UnityEngine.Object.Instantiate(supportCol_0L, supportColsLeft);
            supportCol_1L.localScale = support_1L.localScale;
            supportCol_1L.localEulerAngles = support_1L.localEulerAngles;
            supportCol_1L.localPosition = support_1L.localPosition;
            Transform supportCol_2L = UnityEngine.Object.Instantiate(supportCol_0L, supportColsLeft);
            supportCol_2L.localScale = support_2L.localScale;
            supportCol_2L.localEulerAngles = support_2L.localEulerAngles;
            supportCol_2L.localPosition = support_2L.localPosition;
            Transform supportCol_3L = UnityEngine.Object.Instantiate(supportCol_0L, supportColsLeft);
            supportCol_3L.localScale = support_3L.localScale;
            supportCol_3L.localEulerAngles = support_3L.localEulerAngles;
            supportCol_3L.localPosition = support_3L.localPosition;
            Transform supportCol_4L = UnityEngine.Object.Instantiate(supportCol_0L, supportColsLeft);
            supportCol_4L.localScale = support_4L.localScale;
            supportCol_4L.localEulerAngles = support_4L.localEulerAngles;
            supportCol_4L.localPosition = support_4L.localPosition;
            supportColsLeft.localPosition = supportsLeft.localPosition;
            supportColsLeft.localScale = supportsLeft.localScale;

            Transform supportColsRight = UnityEngine.Object.Instantiate(supportColsLeft, newRoofCol);
            supportColsRight.localScale = new Vector3(supportsRight.localScale.x, -supportsRight.localScale.y, supportsRight.localScale.z);
            supportColsRight.name = supportsRight.name;
            #endregion

            BoatPartOption newRoofOpt = newRoof.GetComponent<BoatPartOption>();
            //newRoofOpt.optionName = "hard roof 2";
            newRoofOpt.walkColObject = newRoofCol.gameObject;
            newRoofOpt.basePrice = 1400;
            newRoofOpt.installCost = 300;
            newRoofOpt.mass = 45;
            partsList.availableParts[3].partOptions.Add(newRoofOpt);
            partsList.RefreshParts();

        }
    }
}
