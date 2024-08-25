using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class SanbuqPatches
    {
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            //Mast[] masts = boat.GetComponent<BoatRefs>().masts;
            //var boatRefs = boat.GetComponent<BoatRefs>();
            Transform container = boat.transform.Find("dhow medium new");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast");
            Transform mainMast2 = structure.Find("mast_1");
            Transform mizzenMast1 = structure.Find("mast_002");
            Transform topMast1 = structure.Find("mast_0_extension");
            Transform topMast2 = structure.Find("mast_1_extension");
            Transform topmastStay1 = container.Find("forestay_0_extension_long");
            Transform topmastStay2 = container.Find("forestay_1_extension_long");
            Transform topmastStay3 = container.Find("forestay_1_extension_short");
            Mast topmastStay1_mast = topmastStay1.GetComponent<Mast>();
            Mast topmastStay2_mast = topmastStay2.GetComponent<Mast>();
            Mast topmastStay3_mast = topmastStay3.GetComponent<Mast>();
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;

           // Plugin.topmastRef = topMast1;
            PartRefs.sanbuq = container;
            PartRefs.sanbuqCol = walkCol;

            #region adjustments
            // move main mast aft to make room
            Util.MoveMast(mainMast2, mainMast2.localPosition + new Vector3(-0.2f, 0, 0), true);
            //Util.MoveWinches(topmastStay2_mast.reefWinch, mainMast2.localPosition, mainMast2.localPosition + new Vector3(-0.2f, 0, 0));
            //Util.MoveWinches(container.Find("forestay_1_inner").GetComponent<Mast>().reefWinch, mainMast2.localPosition, mainMast2.localPosition + new Vector3(-0.2f, 0, 0));
            Util.MoveMast(container.Find("forestay_1_inner"), container.Find("forestay_1_inner").localPosition + new Vector3(-0.2f, -0.06f, 0), true);
            Util.MoveMast(container.Find("forestay_1_short"), container.Find("forestay_1_short").localPosition + new Vector3(-0.2f, -0.06f, 0), true);
            Util.MoveMast(topMast2, topMast2.localPosition + new Vector3(-0.2f, 0, 0), true);

            //Util.MoveWinches(topmastStay3_mast.reefWinch, mainMast2.localPosition, new Vector3(-0.2f, 0, 0));
            topmastStay3.GetComponent<BoatPartOption>().requiresDisabled.Add(container.Find("forestay_1_long").GetComponent<BoatPartOption>());

            mainMast1.gameObject.GetComponent<BoatPartOption>().optionName = "main mast 1";
            mainMast2.gameObject.GetComponent<BoatPartOption>().optionName = "main mast 2";
            BoatPartOption mainMastNone = Util.CreatePartOption(container, "(empty mainmast)", "(no main mast)");
            partsList.availableParts[0].partOptions.Add(mainMastNone);
            
            BoatPartOption forestayNone = Util.CreatePartOption(container, "(empty forestay)", "(no forestay)");
            partsList.availableParts[3].partOptions.Add(forestayNone);
            //partsList.availableParts[3].activeOption = partsList.availableParts[3].partOptions.Count - 1;
            
            BoatPartOption bowspritNone = Util.CreatePartOption(container, "(empty bowsprit)", "(no bowsprit)");
            partsList.availableParts[2].partOptions.Add(bowspritNone);
            //partsList.availableParts[2].activeOption = partsList.availableParts[2].partOptions.Count - 1;
            #endregion

            #region topmastStay

            topmastStay1_mast.reefWinch = Util.CopyWinches(topmastStay1_mast.reefWinch, Vector3.zero, Vector3.up);
            topmastStay1_mast.leftAngleWinch = Util.CopyWinches(topmastStay1_mast.leftAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, -0.17f));
            topmastStay1_mast.rightAngleWinch = Util.CopyWinches(topmastStay1_mast.rightAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, 0.13f));
            foreach (GPButtonRopeWinch winch in topmastStay1_mast.leftAngleWinch)
            {
                winch.transform.localEulerAngles = new Vector3(0, 186, 0);
            }
            foreach (GPButtonRopeWinch winch in topmastStay1_mast.rightAngleWinch)
            {
                winch.transform.localEulerAngles = new Vector3(0, 356, 0);
            }
            topmastStay2_mast.reefWinch = Util.CopyWinches(topmastStay2_mast.reefWinch, Vector3.zero, Vector3.up);
            topmastStay2_mast.leftAngleWinch = Util.CopyWinches(topmastStay2_mast.leftAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, -0.15f));
            topmastStay2_mast.rightAngleWinch = Util.CopyWinches(topmastStay2_mast.rightAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, 0.14f));
            foreach (GPButtonRopeWinch winch in topmastStay2_mast.leftAngleWinch)
            {
                winch.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            foreach (GPButtonRopeWinch winch in topmastStay2_mast.rightAngleWinch)
            {
                winch.transform.localEulerAngles = Vector3.zero;
            }

            topmastStay3_mast.reefWinch = topmastStay2_mast.reefWinch;
            topmastStay3_mast.leftAngleWinch = topmastStay2_mast.leftAngleWinch;
            topmastStay3_mast.rightAngleWinch = topmastStay2_mast.rightAngleWinch;

            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { Util.CreatePartOption(container, "outer_forestay_empty", "(no outer forestay)"), topmastStay1.GetComponent<BoatPartOption>(), topmastStay2.GetComponent<BoatPartOption>(), topmastStay3.GetComponent<BoatPartOption>(), });

            partsList.availableParts[3].partOptions.Remove(topmastStay1.GetComponent<BoatPartOption>());
            partsList.availableParts[3].partOptions.Remove(topmastStay2.GetComponent<BoatPartOption>());
            partsList.availableParts[3].partOptions.Remove(topmastStay3.GetComponent<BoatPartOption>());



            #endregion

            #region hammock
            BoatPartOption hammockNone = Util.CreatePartOption(container, "(empty hammock)", "(no bed)");
            BoatPartOption hammock = Util.AddPartOption(container.Find("hammock").gameObject, "hammock");

            hammock.optionName = "hammock";
            hammock.childOptions = new GameObject[2] { container.Find("hammock_001").gameObject, walkCol.Find("hammock_001").gameObject };
            hammock.basePrice = 200;
            hammock.installCost = 100;
            hammock.mass = 5;
            hammock.requires = new List<BoatPartOption>();
            hammock.requiresDisabled = new List<BoatPartOption>();
            hammock.walkColObject = walkCol.Find("hammock").gameObject;

            BoatPart hammockPart = Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>(2) { hammock, hammockNone });
            #endregion

            #region foremast
            BoatPartOption foremastNone = Util.CreatePartOption(container, "(empty foremast)", "(no foremast)");
            /*var foremast = Util.CopyMast(mizzenMast1, new Vector3(8.877f, 0, 13.1f), Vector3.zero, mizzenMast1.localScale, "mast_004", "foremast");
            foremast.transform.Find("Cylinder_006").gameObject.SetActive(false);
            foremast.reefWinch = Util.CopyWinches(foremast.reefWinch, mizzenMast1.localPosition, foremast.transform.localPosition);*/
            var foremast = Util.CopyMast(mainMast1, new Vector3(8.87f, 0f, 15.865f), mainMast1.localEulerAngles, new Vector3(1f, 1f, 0.975f), "mast_04", "foremast", 31);
            foremast.reefWinch = Util.CopyWinches(foremast.reefWinch, mainMast1.localPosition, foremast.transform.localPosition);
            foremast.rightAngleWinch = Util.CopyWinches(foremast.rightAngleWinch, foremast.rightAngleWinch[0].transform.localPosition, new Vector3(3.6f, 1.98f, -2.27f));
            foremast.leftAngleWinch = Util.CopyWinches(foremast.leftAngleWinch, foremast.leftAngleWinch[0].transform.localPosition, new Vector3(3.6f, 1.98f, 2.27f));
            foremast.midAngleWinch = Util.CopyWinches(foremast.midAngleWinch, mainMast1.localPosition, foremast.transform.localPosition);
            foremast.midAngleWinch[0].transform.localPosition = new Vector3(-1.87f, 2.5f, -0.8f);
            foremast.midAngleWinch[1].transform.localPosition = new Vector3(-1.87f, 2.5f, 0.8f);
            foremast.midAngleWinch[2].transform.localPosition = new Vector3(-1.8f, 2.51f, -0.37f);
            foremast.midRopeAtt[0].parent.localPosition = new Vector3(-6.6f, -0.05f, -15.03f);

            var foremastPart = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { foremastNone, foremast.GetComponent<BoatPartOption>() });

            /*var source1 = structure.Find("part_shrouds_main_back").GetChild(0).gameObject;
            GameObject shrouds_back = UnityEngine.GameObject.Instantiate(source1, foremast.transform);
            shrouds_back.transform.localPosition = new Vector3(0f, 0f, 1.5f);
            shrouds_back.transform.localScale = new Vector3(1.1f, 0.90f, 1.1f);
            BoatPartOption shrouds_back_opt = shrouds_back.AddComponent<BoatPartOption>();
            shrouds_back_opt.optionName = "fore shrouds 2";
            shrouds_back_opt.basePrice = 1100;
            shrouds_back_opt.installCost = 400;
            shrouds_back_opt.mass = 40;
            shrouds_back_opt.walkColObject = UnityEngine.Object.Instantiate(source1.GetComponentInParent<BoatPartOption>().walkColObject.transform.GetChild(0).gameObject, foremast.walkColMast);
            shrouds_back_opt.walkColObject.transform.localPosition = shrouds_back.transform.localPosition;
            shrouds_back_opt.walkColObject.transform.localRotation = shrouds_back.transform.localRotation;
            shrouds_back_opt.walkColObject.name = shrouds_back.name;
            shrouds_back_opt.requires = new List<BoatPartOption>();
            shrouds_back_opt.requiresDisabled = new List<BoatPartOption>();
            shrouds_back_opt.childOptions = new GameObject[0];*/

            var shrouds_back = UnityEngine.Object.Instantiate(mizzenMast1.transform.Find("part_shrouds_mizzen_back"), foremast.transform);
            var shrouds_back_opt = shrouds_back.GetComponent<BoatPartOption>();
            shrouds_back_opt.optionName = "fore shrouds 2";
            shrouds_back.localPosition = new Vector3(1.2f, 0f, 5.65f);
            shrouds_back.localScale = new Vector3(1f, 0.86f, 1.495f);
            shrouds_back.localEulerAngles = new Vector3(0, 4, 0);
            shrouds_back.GetChild(0).localPosition = new Vector3(5.2f, 0, -15f);
            shrouds_back.GetChild(0).localScale = new Vector3(1f, 1f, 0.7f);
            shrouds_back_opt.walkColObject = UnityEngine.Object.Instantiate(shrouds_back_opt.walkColObject, foremast.walkColMast);
            shrouds_back_opt.walkColObject.transform.localPosition = new Vector3(1.2f, 0f, 5.65f);
            shrouds_back_opt.walkColObject.transform.localEulerAngles = new Vector3(0, 4, 0);
            shrouds_back_opt.walkColObject.transform.localScale = new Vector3(1f, 0.86f, 1.47f);
            var second_shroud = UnityEngine.Object.Instantiate(shrouds_back, shrouds_back);
            second_shroud.localPosition = new Vector3(-0.04f, 0f, 0.15f);
            second_shroud.localScale = new Vector3(1f, 0.91f, 1f);
            second_shroud.localEulerAngles = new Vector3(0, 356, 0);
            second_shroud.GetComponent<BoatPartOption>().enabled = false;
            var second_walkCol = UnityEngine.Object.Instantiate(shrouds_back_opt.walkColObject, shrouds_back_opt.walkColObject.transform);
            second_walkCol.transform.localPosition = second_shroud.localPosition;
            second_walkCol.transform.localEulerAngles = new Vector3(0f, 357f, 0f);
            second_walkCol.transform.localScale = new Vector3(1f, 0.84f, 1.43f);
            shrouds_back.SetAsFirstSibling();
            shrouds_back_opt.walkColObject.transform.SetAsFirstSibling();

            var source2 = structure.Find("part_shrouds_main_side").GetChild(0).gameObject;
            GameObject shrouds_side = UnityEngine.GameObject.Instantiate(source2, foremast.transform);
            shrouds_side.transform.localPosition = new Vector3(0f, 0f, 1.48f);
            shrouds_side.transform.localEulerAngles = new Vector3(6f, 174.6f, 348f);
            shrouds_side.transform.localScale = new Vector3(1.1f, 1.1f, 1.115f);
            BoatPartOption shrouds_side_opt = shrouds_side.AddComponent<BoatPartOption>();
            shrouds_side_opt.optionName = "fore shrouds 1";
            shrouds_side_opt.basePrice = 800;
            shrouds_side_opt.installCost = 400;
            shrouds_side_opt.mass = 20;
            shrouds_side_opt.walkColObject = UnityEngine.Object.Instantiate(source2.GetComponentInParent<BoatPartOption>().walkColObject.transform.GetChild(0).gameObject, foremast.walkColMast);
            shrouds_side_opt.walkColObject.transform.localPosition = shrouds_side.transform.localPosition;
            shrouds_side_opt.walkColObject.transform.localRotation = shrouds_side.transform.localRotation;
            shrouds_side_opt.walkColObject.name = shrouds_side.name;
            shrouds_side_opt.requires = new List<BoatPartOption>();
            shrouds_side_opt.requiresDisabled = new List<BoatPartOption>();
            shrouds_side_opt.childOptions = new GameObject[0];
            shrouds_side.transform.SetSiblingIndex(1);
            shrouds_side_opt.walkColObject.transform.SetSiblingIndex(1);
            //Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { shrouds_side_opt, shrouds_back_opt });
            #endregion

            #region foreTopmast // alt pos = (9.256f, 0, 22.9653f), alt scale = (1, 1, 0.7f)
            Mast foreTopmast = Util.CopyMast(topMast1, (topMast1.localPosition - mainMast1.localPosition) + foremast.transform.localPosition, "mast_04_extension", "fore topmast", 44);
            //foreTopmast.mastHeight = 5f;
            //foreTopmast.extraBottomHeight = 0.6f;
            BoatPartOption foreTopmastNone = Util.CreatePartOption(structure, "(no fore topmast)", "(no fore topmast)");
            Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { foreTopmastNone, foreTopmast.GetComponent<BoatPartOption>() });
            foreTopmast.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foremast.GetComponent<BoatPartOption>() };
            foreTopmast.reefWinch = Util.CopyWinches(foreTopmast.reefWinch, mainMast1.localPosition, foremast.transform.localPosition);
            foreTopmast.midAngleWinch = Util.CopyWinches(topMast1.GetComponent<Mast>().midAngleWinch, topMast1.GetComponent<Mast>().midAngleWinch[0].transform.localPosition, new Vector3(-1.8f, 2.51f, 0.37f));
            foreTopmast.midAngleWinch[0].transform.localEulerAngles = new Vector3(274, 180, 180);
            foreTopmast.leftAngleWinch = Util.CopyWinches(foremast.leftAngleWinch, Vector3.zero, new Vector3(-0.4f, -0.02f, 0.04f));
            foreTopmast.rightAngleWinch = Util.CopyWinches(foremast.rightAngleWinch, Vector3.zero, new Vector3(-0.4f, -0.02f, -0.04f));
            for (int i = 0; i < foreTopmast.midRopeAtt.Length; i++)
            {
                foreTopmast.midRopeAtt[i] = foremast.midRopeAtt[i];
            }
            #endregion
            #region foremast2
            /*var foremast3 = Util.CopyMast(mizzenMast1, new Vector3(11.87f, 0f, 12.565f), new Vector3(0, 15, 0), new Vector3(1f, 1f, 0.975f), "mast_05", "raked foremast", 41);
            foremast3.reefWinch = Util.CopyWinches(mizzenMast1.GetComponent<Mast>().reefWinch, mizzenMast1.localPosition, foremast3.transform.localPosition + new Vector3(-3.62f, 0, 0));
            foremast3.reefWinch[0].transform.localPosition = new Vector3(9.2f, 2.59f, 0.36f);
            foremast3.reefWinch[1].transform.localPosition = new Vector3(9.2f, 2.59f, -0.36f);
            foremast3.reefWinch[1].transform.localEulerAngles = new Vector3(0, 180, 0);
            foremast3.midAngleWinch = foremast.midAngleWinch;
            foremast3.leftAngleWinch = foremast.leftAngleWinch;
            foremast3.rightAngleWinch = foremast.rightAngleWinch;
            var foremast3_ropeHolder = foremast3.transform.Find("rope_holder");
            foremast3_ropeHolder.localPosition = new Vector3(-3.2f, 0f, -12.65f);
            foremast3_ropeHolder.localEulerAngles = new Vector3(270f, 350f, 0f);
            var foremast3_shrouds = foremast3.transform.Find("part_shrouds_mizzen_back");
            foremast3_shrouds.GetChild(0).localPosition = new Vector3(-7.76f, 0f, -14.84f);
            foremast3_shrouds.GetChild(0).localEulerAngles = new Vector3(0, 5, 90);
            foremast3_shrouds.GetChild(0).localScale = new Vector3(1f, -1f, 1f);
            foremast3_shrouds.localScale = new Vector3(1.1f, 0.86f, 1.17f);
            foremast3_shrouds.GetComponent<BoatPartOption>().enabled = false;
            foremast3.transform.Find("part_shrouds_mizzen_side").gameObject.SetActive(false);
            foremast3.transform.Find("Cylinder_006").gameObject.SetActive(false);
            foremast3.walkColMast.Find("part_shrouds_mizzen_side").gameObject.SetActive(false);
            //foremast3_shrouds.tag = "foremast_shrouds";
            //To do: add foremast 2 shroud cols
            var foremast3_shrouds_col = foremast3.walkColMast.Find("part_shrouds_mizzen_back");
            foremast3_shrouds_col.gameObject.SetActive(true);
            foremast3_shrouds_col.localScale = foremast3_shrouds.localScale;

            var foremast3_opt = foremast3.GetComponent<BoatPartOption>();
            foremastPart.partOptions.Add(foremast3_opt);*/
            #endregion

            #region foremast3
            //Debug.Log("sanbuq foremast3");
            Mast foremast3 = Util.CopyMast(foremast.transform, new Vector3(12.65f, 0, 15.56f), new Vector3(0, 15, 0), foremast.transform.localScale, "mast_05_raked", "raked foremast", 42);
            var rakedBrace = UnityEngine.Object.Instantiate(foremast3.transform.Find("mast_003"), foremast3.transform);
            rakedBrace.localPosition = new Vector3(-0.003f, -0.01f, -18.8f);
            rakedBrace.localScale = new Vector3(0.99f, 0.99f, 1.01f);
            foremast3.reefWinch = Util.CopyWinches(foremast.reefWinch, Vector3.zero, Vector3.zero);
            foremast3.reefWinch[0].transform.localPosition = new Vector3(8.63f, 2.217f, 0);
            foremast3.reefWinch[0].transform.localEulerAngles = new Vector3(345, 270, 270);
            foremast3.reefWinch[1].transform.localPosition = new Vector3(9.05f, 2.217f, 0.48f);
            foremast3.reefWinch[2].transform.localPosition = new Vector3(9.05f, 2.217f, -0.48f);

            var foremast3_shrouds = foremast3.transform.GetChild(0);// Find("part_shrouds_mizzen_back(Clone)");
            foremast3.midRopeAtt[0].parent.localPosition = new Vector3(-6.6f, 0f, -17.05f);

            //Debug.Log("did we find a thing?");
            //Debug.Log(foremast3_shrouds.name);
            foremast3_shrouds.transform.localScale = new Vector3(1, 0.84f, 1.535f);
            foremast3_shrouds.transform.GetChild(1).localScale = new Vector3(1, 0.9f, 0.99f);
            var foremast3_shrouds_col = foremast3.walkColMast.GetChild(0);// Find(foremast3_shrouds.name);
            foremast3_shrouds_col.transform.localScale = foremast3_shrouds.localScale;
            foremast3_shrouds_col.transform.GetChild(1).localScale = new Vector3(1f, 0.84f, 1.42f);
            foremast3_shrouds_col.transform.GetChild(1).localPosition = new Vector3(-0.04f, 0f, -0.4f);

            //Debug.Log("did we find another thing?");
            var foremast3_shrouds_s = foremast3.transform.GetChild(1);// Find("part_shrouds_0_side(Clone)");
            Debug.Log(foremast3_shrouds_s.name);
            foremast3_shrouds_s.transform.localEulerAngles = new Vector3(5.5f, 175f, 347f);
            foremast3_shrouds_s.transform.localScale = new Vector3(1.1f, 1.1f, 1.16f);
            foremast3_shrouds_s.transform.GetChild(0).localPosition = new Vector3(8.34f, 2.9f, 16.3f);
            foremast3_shrouds_s.transform.GetChild(0).localEulerAngles = new Vector3(0f, 174.2f, 88f);
            foremast3_shrouds_s.transform.GetChild(0).localScale = new Vector3(1.1f, 1f, 1.15f);

            //Debug.Log("side shrouds");
            var foremast3_shrouds_s_col = foremast3.walkColMast.Find(foremast3_shrouds_s.name);
            foremast3_shrouds_s_col.transform.localEulerAngles = foremast3_shrouds_s.localEulerAngles;
            foremast3_shrouds_s_col.transform.localScale = foremast3_shrouds_s.localScale;
            foremast3_shrouds_s_col.transform.GetChild(0).localPosition = foremast3_shrouds_s.GetChild(0).localPosition;
            foremast3_shrouds_s_col.transform.GetChild(0).localEulerAngles = foremast3_shrouds_s.GetChild(0).localEulerAngles;
            foremast3_shrouds_s_col.transform.GetChild(0).localScale = foremast3_shrouds_s.GetChild(0).localScale;


            foremastPart.partOptions.Add(foremast3.GetComponent<BoatPartOption>());

            //Debug.Log("sanbuq foremast shrouds");
            #region foremast shrouds2

            shrouds_back_opt.enabled = false;
            shrouds_side_opt.enabled = false;
            foremast3_shrouds.GetComponent<BoatPartOption>().enabled = false;
            foremast3_shrouds_s.GetComponent<BoatPartOption>().enabled = false;

            BoatPartOption fore_shrouds_back = Util.CreatePartOption(structure, "part_shrouds_fore_back", "foremast shrouds 2");
            fore_shrouds_back.basePrice = 1100;
            fore_shrouds_back.installCost = 400;
            fore_shrouds_back.mass = 30;

            shrouds_back.transform.parent = fore_shrouds_back.transform;
            foremast3_shrouds.transform.parent = fore_shrouds_back.transform;

            Transform fore_shrouds_back_col = UnityEngine.Object.Instantiate(new GameObject(), foremast.walkColMast.transform.parent).transform;
            fore_shrouds_back_col.name = fore_shrouds_back.name;
            shrouds_back_opt.walkColObject.transform.parent = fore_shrouds_back_col;
            foremast3_shrouds_col.parent = fore_shrouds_back_col;
            fore_shrouds_back.walkColObject = fore_shrouds_back_col.gameObject;

            BoatPartOption fore_shrouds_side = Util.CreatePartOption(structure, "part_shrouds_fore_side", "foremast shrouds 1");
            fore_shrouds_side.basePrice = 1100;
            fore_shrouds_side.installCost = 400;
            fore_shrouds_side.mass = 20;
            shrouds_side.transform.parent = fore_shrouds_side.transform;
            foremast3_shrouds_s.transform.parent = fore_shrouds_side.transform;

            Transform fore_shrouds_side_col = UnityEngine.Object.Instantiate(new GameObject(), foremast.walkColMast.transform.parent).transform;
            fore_shrouds_side_col.name = fore_shrouds_side.name;
            shrouds_side_opt.walkColObject.transform.parent = fore_shrouds_side_col;
            foremast3_shrouds_s_col.parent = fore_shrouds_side_col;
            fore_shrouds_side.walkColObject = fore_shrouds_side_col.gameObject;

            foremast.GetComponent<BoatPartOption>().childOptions = new GameObject[4] { shrouds_side.gameObject, shrouds_back.gameObject, shrouds_side_opt.walkColObject, shrouds_back_opt.walkColObject };
            foremast3.GetComponent<BoatPartOption>().childOptions = new GameObject[4] { foremast3_shrouds_s.gameObject, foremast3_shrouds.gameObject, foremast3_shrouds_col.gameObject, foremast3_shrouds_s_col.gameObject };

            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { fore_shrouds_side, fore_shrouds_back });
            Debug.Log("sanbuq foremast shrouds DONE");

            #endregion
            #endregion

            #region mizzenMast
            Mast mizzenMast2 = Util.CopyMast(mainMast2, mainMast2.localPosition + new Vector3(-7, 0, 0), mainMast1.localEulerAngles, new Vector3(1, 1, 0.965f), "mast_003", "mizzen mast 2", 35);
            UnityEngine.Object.Instantiate(mizzenMast1.transform.Find("Cylinder_006"), mizzenMast2.transform, true);
            UnityEngine.Object.Instantiate(mizzenMast1.GetComponent<Mast>().walkColMast.Find("Cylinder_006"), mizzenMast2.walkColMast, true);
            mizzenMast2.mastHeight = 12.5f;
            var brace = UnityEngine.Object.Instantiate(mizzenMast2.transform.Find("mast_004"), mizzenMast2.transform);
            brace.localPosition = new Vector3(-0.005f, -0.01f, -19.11f);
            brace.localScale = new Vector3(0.98f, 0.99f, 1.2f);
            var mizzenMast2_rope_holder = UnityEngine.Object.Instantiate(mizzenMast1.transform.Find("rope_holder"), mizzenMast2.transform, true).GetChild(0);
            mizzenMast2.reefWinch = Util.CopyWinches(mainMast2.GetComponent<Mast>().reefWinch, mainMast2.localPosition, mizzenMast2.transform.localPosition + new Vector3(0, 0.6f, 0));
            mizzenMast2.midAngleWinch = Util.CopyWinches(mizzenMast1.GetComponent<Mast>().midAngleWinch, mizzenMast1.localPosition, new Vector3(mizzenMast2.transform.localPosition.x - 1, mizzenMast1.localPosition.y, mizzenMast1.localPosition.z));
            mizzenMast2.midAngleWinch = mizzenMast2.midAngleWinch.AddToArray(UnityEngine.Object.Instantiate(mizzenMast2.midAngleWinch[1], container));
            mizzenMast2.midAngleWinch[2].transform.localPosition = mizzenMast2.midAngleWinch[1].transform.localPosition + new Vector3(0, 0, 0.376f);
            mizzenMast2.midAngleWinch[2].transform.localEulerAngles = new Vector3(1, 306, 358);
            mizzenMast2.midAngleWinch[1].transform.localEulerAngles = new Vector3(1, 246, 358);
            mizzenMast2.midAngleWinch[0].transform.localEulerAngles = mainMast1.GetComponent<Mast>().midAngleWinch[0].transform.localEulerAngles;
            mizzenMast2.midAngleWinch[0].transform.localPosition = mainMast1.GetComponent<Mast>().midAngleWinch[0].transform.localPosition + new Vector3(-0.36f, 0, 0);
            mizzenMast2.leftAngleWinch = Util.CopyWinches(mizzenMast1.GetComponent<Mast>().leftAngleWinch, mizzenMast1.GetComponent<Mast>().leftAngleWinch[0].transform.localPosition, mizzenMast1.GetComponent<Mast>().leftAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.06f, 0));
            mizzenMast2.rightAngleWinch = Util.CopyWinches(mizzenMast1.GetComponent<Mast>().rightAngleWinch, mizzenMast1.GetComponent<Mast>().rightAngleWinch[0].transform.localPosition, mizzenMast1.GetComponent<Mast>().rightAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.06f, 0));
            mizzenMast2.midRopeAtt[0].transform.parent.gameObject.SetActive(false);
            
            for (int i = 0; i < mizzenMast2.midRopeAtt.Length; i++)
            {
                mizzenMast2.midRopeAtt[i] = mizzenMast2_rope_holder;
            }
            //mizzenMast2.orderIndex = 25;
            //mizzenMast2.Awake();
            partsList.availableParts[1].partOptions.Add(mizzenMast2.GetComponent<BoatPartOption>());
            

            BoatPartOption mizShrBackOld = mizzenMast1.Find("part_shrouds_mizzen_back").GetComponent<BoatPartOption>();
            BoatPartOption mizzenShrouds = Util.CreatePartOption(structure, "part_shrouds_mizzen_back", "mizzen shrouds 2");
            mizzenShrouds.basePrice = mizShrBackOld.basePrice;
            mizzenShrouds.installCost = mizShrBackOld.installCost;
            mizzenShrouds.mass = mizShrBackOld.mass;
            var mizzenShrouds_1 = UnityEngine.Object.Instantiate(structure.Find("part_shrouds_main_back").Find("part_shrouds_1_back"), mizzenShrouds.transform);
            mizzenShrouds_1.localPosition = new Vector3(mizzenMast2.transform.localPosition.x, 0.01f, 17.1f);
            mizzenShrouds_1.localScale = new Vector3(1.1f, 1.1f, 1.04f);
            mizShrBackOld.transform.parent = mizzenShrouds.transform;

            var mizzenShrouds_walkCols = UnityEngine.Object.Instantiate(new GameObject(), mizzenMast2.walkColMast.parent);
            mizzenShrouds_walkCols.name = mizzenShrouds.name;
            var mizzenShrouds_1_walkCol = UnityEngine.Object.Instantiate(structure.Find("part_shrouds_main_back").GetComponent<BoatPartOption>().walkColObject.transform.Find("part_shrouds_1_back"), mizzenShrouds_walkCols.transform);
            mizzenShrouds_1_walkCol.localPosition = mizzenShrouds_1.localPosition;
            mizzenShrouds_1_walkCol.localScale = mizzenShrouds_1.localScale;
            mizzenShrouds_1_walkCol.localEulerAngles = mizzenShrouds_1.localEulerAngles;
            mizShrBackOld.walkColObject.transform.parent = mizzenShrouds_walkCols.transform;
            mizzenShrouds.walkColObject = mizzenShrouds_walkCols;


            BoatPartOption mizShrSideOld = mizzenMast1.Find("part_shrouds_mizzen_side").GetComponent<BoatPartOption>();
            BoatPartOption mizzenShrouds2 = Util.CreatePartOption(structure, "part_shrouds_mizzen_side", "mizzen shrouds 1");
            mizzenShrouds2.basePrice = mizShrSideOld.basePrice;
            mizzenShrouds2.installCost = mizShrSideOld.installCost;
            mizzenShrouds2.mass = mizShrSideOld.mass;
            var mizzenShrouds2_1 = UnityEngine.Object.Instantiate(structure.Find("part_shrouds_main_side").Find("part_shrouds_1_side"), mizzenShrouds2.transform);
            mizzenShrouds2_1.localPosition = new Vector3(mizzenMast2.transform.localPosition.x, 0.05f, 17.1f);
            mizzenShrouds2_1.localScale = new Vector3(1.1f, 1.1f, 1.05f);
            mizzenShrouds2_1.localEulerAngles = new Vector3(11.4f, 174.96f, 4f);
            mizShrSideOld.transform.parent = mizzenShrouds2.transform;

            var mizzenShrouds2_walkCols = UnityEngine.Object.Instantiate(new GameObject(), mizzenMast2.walkColMast.parent);
            mizzenShrouds2_walkCols.name = mizzenShrouds2.name;
            var mizzenShrouds2_1_walkCol = UnityEngine.Object.Instantiate(structure.Find("part_shrouds_main_side").GetComponent<BoatPartOption>().walkColObject.transform.Find("part_shrouds_1_side"), mizzenShrouds2_walkCols.transform);
            mizzenShrouds2_1_walkCol.localPosition = mizzenShrouds2_1.localPosition;
            mizzenShrouds2_1_walkCol.localScale = mizzenShrouds2_1.localScale;
            mizzenShrouds2_1_walkCol.localEulerAngles = mizzenShrouds2_1.localEulerAngles;
            mizShrSideOld.walkColObject.transform.parent = mizzenShrouds2_walkCols.transform;
            mizzenShrouds2.walkColObject = mizzenShrouds2_walkCols;

            mizzenMast1.gameObject.GetComponent<BoatPartOption>().childOptions = new GameObject[4] {
                mizShrSideOld.gameObject,
                mizShrSideOld.walkColObject,
                mizShrBackOld.gameObject,
                mizShrBackOld.walkColObject,
            };
            mizzenMast2.GetComponent<BoatPartOption>().childOptions = new GameObject[4] {
                mizzenShrouds2_1.gameObject,
                mizzenShrouds2_1_walkCol.gameObject,
                mizzenShrouds_1.gameObject,
                mizzenShrouds_1_walkCol.gameObject,
            };
            partsList.availableParts[6].partOptions = new List<BoatPartOption>() { mizzenShrouds2, mizzenShrouds };
            
            mizShrBackOld.GetComponent<BoatPartOption>().enabled = false;
            mizShrSideOld.GetComponent<BoatPartOption>().enabled = false;
            #endregion

            #region mizzenTopmast // alt pos = (-6.7964f, 0.01f, 22.9662f), alt scale = (1, 1, 0.7f)
            Mast mizzenTopmast = Util.CopyMast(topMast2, (topMast2.localPosition - mainMast2.localPosition) + mizzenMast2.transform.localPosition, "mast_003_extension", "mizzen topmast", 39);
            //mizzenTopmast.mastHeight = 5f;
            //mizzenTopmast.extraBottomHeight = 0.6f;
            BoatPartOption mizzenTopmastNone = Util.CreatePartOption(structure, "(no mizzen topmast)", "(no mizzen topmast)");
            Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { mizzenTopmastNone, mizzenTopmast.GetComponent<BoatPartOption>() });
            mizzenTopmast.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzenMast2.GetComponent<BoatPartOption>() };
            mizzenTopmast.reefWinch = Util.CopyWinches(mizzenTopmast.reefWinch, mainMast2.localPosition, mizzenMast2.transform.localPosition + new Vector3(0, 1, 0));
            mizzenTopmast.midAngleWinch = Util.CopyWinches(topMast1.GetComponent<Mast>().midAngleWinch, topMast1.GetComponent<Mast>().midAngleWinch[0].transform.localPosition, new Vector3(-12.39f, 2.81f, 0f));
            mizzenTopmast.midAngleWinch[0].transform.localEulerAngles = new Vector3(274, 90, 0);
            mizzenTopmast.leftAngleWinch = mizzenMast1.GetComponent<Mast>().leftAngleWinch;
            mizzenTopmast.rightAngleWinch = mizzenMast1.GetComponent<Mast>().rightAngleWinch;
            for (int i = 0; i < mizzenTopmast.midRopeAtt.Length; i++)
            {
                mizzenTopmast.midRopeAtt[i] = mizzenMast2_rope_holder;
            }
            #endregion

            #region forestayOuter
            var src3 = container.Find("forestay_0_short");
            Mast forestayOuter = Util.CopyMast(src3, src3.localPosition + new Vector3(4, 0, 0), new Vector3(311, 270, 90), new Vector3(1, 1, 0.94f), "foremast_stay", "foremast stay", 32);
            forestayOuter.reefWinch = Util.CopyWinches(forestayOuter.reefWinch, src3.localPosition, forestayOuter.transform.localPosition);
            forestayOuter.mastHeight = 16;
            BoatPartOption forestayOuterOpt = forestayOuter.GetComponent<BoatPartOption>();
            forestayOuterOpt.requires = new List<BoatPartOption> {
                foremast.GetComponent<BoatPartOption>(),
                container.Find("part_bowsprit_long_gfx").GetComponent<BoatPartOption>() };
            forestayOuter.mastReefAtt[0] = foremast.transform.Find("rope_holder_jibs_front").Find("att");
            forestayOuter.mastReefAtt[1] = foremast.transform.Find("rope_holder_jibs_front").Find("att");
            partsList.availableParts[3].partOptions.Add(forestayOuterOpt);
            #endregion

            #region raked forestay
            Mast foremast_stay_raked = Util.CopyMast(forestayOuter.transform, new Vector3(13.53f, 17.41f, 0f), new Vector3(298.8f, 270f, 90f), new Vector3(1f, 1f, 0.81f), "forestay_raked", "foremast stay 2", 43);
            foremast_stay_raked.mastHeight = 13.7f;
            foremast_stay_raked.reefWinch = Util.CopyWinches(forestayOuter.reefWinch, forestayOuter.reefWinch[0].transform.localPosition, Vector3.one);
            foremast_stay_raked.reefWinch[0].transform.localPosition = new Vector3(8.82f, 2.9f, 0f);
            foremast_stay_raked.reefWinch[0].transform.localEulerAngles = new Vector3(345, 270, 270);
            foremast_stay_raked.reefWinch[1].transform.localPosition = new Vector3(8.86f, 2.55f, 0.234f);
            foremast_stay_raked.reefWinch[1].transform.localEulerAngles = new Vector3(350, 320, 260);
            BoatPartOption foremast_stay_raked_opt = foremast_stay_raked.GetComponent<BoatPartOption>();
            foremast_stay_raked_opt.requires = new List<BoatPartOption> { foremast3.GetComponent<BoatPartOption>(), container.Find("part_bowsprit_long_gfx").GetComponent<BoatPartOption>() };
            partsList.availableParts[3].partOptions.Add(foremast_stay_raked_opt);
            #endregion

            #region forestayInner
            var src4 = container.Find("forestay_0_short_inner");
            Mast forestayInner = Util.CopyMast(src4, new Vector3(9.53f, 13.09f, 0), new Vector3(311, 270, 90), src4.transform.localScale, "foremast_stay_inner", "lower foremast stay", 33);
            //Mast forestayInner = Util.CopyMast(src4, new Vector3(9.53f, 13f, 0), new Vector3(300, 270, 90), new Vector3(1, 1, 1.03f), "foremast_stay_inner", "lower foremast stay");
            forestayInner.reefWinch = Util.CopyWinches(forestayInner.reefWinch, src4.localPosition, forestayInner.transform.localPosition);
            BoatPartOption forestayInnerOpt = forestayInner.GetComponent<BoatPartOption>();
            forestayInnerOpt.requires = new List<BoatPartOption>() {
                foremast.GetComponent<BoatPartOption>(),
                container.Find("part_bowsprit_long_gfx").GetComponent<BoatPartOption>() };
            var ropeHolder = UnityEngine.Object.Instantiate(foremast.transform.Find("rope_holder_jibs_front"), foremast.transform);
            ropeHolder.localPosition += new Vector3(0, 0, -4.2f);
            forestayInner.mastReefAtt[0] = ropeHolder.GetChild(0);
            //forestayInner.mastReefAtt[0] = forestayOuter.mastReefAtt[0];
            //forestayInner.GetComponent<Mast>().orderIndex = 27;
            //forestayInner.GetComponent<Mast>().Awake();
            partsList.availableParts[4].partOptions.Add(forestayInnerOpt);
            #endregion

            #region middleStays
            var src5 = container.Find("part_stay_mid_0");
            var middlestay_2 = Util.CopyMast(src5, new Vector3(-6.75f, 17.8f, 0), new Vector3(315.8f, 270, 90), new Vector3(1, 1, 0.845f), "part_stay_mid_2", "mizzen stay 1-2", 34);
            middlestay_2.reefWinch = Util.CopyWinches(middlestay_2.reefWinch, src5.localPosition, new Vector3(-7.778f, src5.localPosition.y, 0));
            middlestay_2.reefWinch[0].transform.localEulerAngles = new Vector3(0, 90, 0);
            middlestay_2.transform.GetChild(0).localScale = new Vector3(3.6f, 3.24f, 3.8f);
            middlestay_2.transform.GetChild(1).localScale = new Vector3(2.4f, 2.2f, 2.4f);
            middlestay_2.mastHeight = 15;
            middlestay_2.mastReefAtt[0] = mizzenMast2.transform.Find("rope_holder_jibs_center").GetChild(0);
            middlestay_2.transform.Find("rope_holder_004 (2)").gameObject.SetActive(false);


            middlestay_2.GetComponent<BoatPartOption>().requires[1] = mizzenMast2.GetComponent<BoatPartOption>();
            partsList.availableParts[8].partOptions.Add(middlestay_2.GetComponent<BoatPartOption>());
            
            var middlestay_3 = Util.CopyMast(src5, new Vector3(-9.86f, 14.57f, 0), new Vector3(312f, 270, 90), new Vector3(1, 1, 0.75f), "part_stay_mid_3", "mizzen stay 2-1", 36);
            //middlestay_3.reefWinch = Util.CopyWinches(middlestay_3.reefWinch, src5.localPosition, new Vector3(-6.55f, src5.localPosition.y, 0));
            middlestay_3.transform.GetChild(0).localScale = new Vector3(4.5f, 3.96f, 4.5f);
            middlestay_3.transform.GetChild(1).localScale = new Vector3(2.28f, 1.99f, 2.33f);
            middlestay_3.mastHeight = 14;
            middlestay_3.GetComponent<BoatPartOption>().requires[0] = structure.Find("mast_1").GetComponent<BoatPartOption>();
            partsList.availableParts[8].partOptions.Add(middlestay_3.GetComponent<BoatPartOption>());

            Mast middlestay_4 = Util.CopyMast(middlestay_2.transform, new Vector3(-6.89f, 17.8f, 0f), new Vector3(305, 270, 90), new Vector3(1, 1, 0.6f), "part_stay_mid_4", "mizzen stay 2-2", 46);
            middlestay_4.GetComponent<BoatPartOption>().requires.Remove(mainMast1.GetComponent<BoatPartOption>());
            middlestay_4.GetComponent<BoatPartOption>().requires.Add(mainMast2.GetComponent<BoatPartOption>());
            partsList.availableParts[8].partOptions.Add(middlestay_4.GetComponent<BoatPartOption>());

            // topmast midstay // alt pos = (-6.4f, 0, 22.99f)
            var middlestayTopmast = Util.CopyMast(middlestay_2.transform, new Vector3(-6.4f, 25.4f, 0), middlestay_2.transform.localEulerAngles, new Vector3(1, 1, 0.818f), "part_stay_mid_topmast", "mizzen topstay 1-2", 40);
            middlestayTopmast.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzenTopmast.GetComponent<BoatPartOption>(), mainMast1.GetComponent<BoatPartOption>() };
            middlestayTopmast.reefWinch = Util.CopyWinches(middlestayTopmast.reefWinch, Vector3.zero, new Vector3(0f, 0.6f, 0f));
            middlestayTopmast.leftAngleWinch = Util.CopyWinches(middlestayTopmast.leftAngleWinch, middlestayTopmast.leftAngleWinch[0].transform.localPosition, middlestayTopmast.leftAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.05f, 0));
            middlestayTopmast.rightAngleWinch = Util.CopyWinches(middlestayTopmast.rightAngleWinch, middlestayTopmast.rightAngleWinch[0].transform.localPosition, middlestayTopmast.rightAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.05f, 0));

            Mast middlestayTopmast2 = Util.CopyMast(middlestayTopmast.transform, new Vector3(-6.54f, 25.4f, 0), new Vector3(303.6f, 270, 90), new Vector3(1, 1, 0.59f), "stay_mid_topmast", "mizzen topstay 2-2", 47); // alt pos = (-6.5f, 23, 0)
            middlestayTopmast2.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzenTopmast.GetComponent<BoatPartOption>(), mainMast2.GetComponent<BoatPartOption>() };
            
            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption>() { Util.CreatePartOption(container, "(empty fore midstay)", "(no mizzen topstay)"), middlestayTopmast.GetComponent<BoatPartOption>(), middlestayTopmast2.GetComponent<BoatPartOption>() });
            #endregion


            #region telltale
            var flagSource = mizzenMast1.Find("flag");
            //flagSource.localScale = new Vector3(0.8f, 1f, 0.5f);
            //flagSource.GetComponent<MeshRenderer>().material.color = Color.green;
            BoatPartOption noFlag = Util.CreatePartOption(container, "(flag empty)", "(no telltale)");

            BoatPartOption flags_main = Util.CreatePartOption(container, "flag_main", "mainmast telltale");
            flags_main.basePrice = 50;
            flags_main.installCost = 10;

            Transform flags_main_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_0" }.transform, flags_main.transform);
            var flag_main_0_side = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_side.name = "flag_main_0_side";
            flag_main_0_side.localPosition = new Vector3(6.15f, 3.3f, 2.02f);
            flag_main_0_side.localEulerAngles = new Vector3(80, 20, 0);
            //flag_main_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);
            var flag_main_0_back = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_back.name = "flag_main_0_back";
            flag_main_0_back.localPosition = new Vector3(4.4f, 3.3f, 2.28f);
            flag_main_0_back.localEulerAngles = new Vector3(79, 340, 0);
            flag_main_0_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            Transform flags_main_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_1" }.transform, flags_main.transform);
            var flag_main_1_side = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_side.name = "flag_main_1_side";
            flag_main_1_side.localPosition = new Vector3(0.7f, 3.3f, 2.28f);
            flag_main_1_side.localEulerAngles = new Vector3(80, 20, 0);
            flag_main_1_side.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var flag_main_1_back = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_back.name = "flag_main_1_back";
            flag_main_1_back.localPosition = new Vector3(-0.63f, 3.3f, 2.4f);
            flag_main_1_back.localEulerAngles = new Vector3(80, 340, 0);
            flag_main_1_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);


            BoatPartOption flags_fore = Util.CreatePartOption(container, "flag_fore", "foremast telltale");
            flags_fore.basePrice = 50;
            flags_fore.installCost = 10;

            Transform flags_fore_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_fore_0" }.transform, flags_fore.transform);
            var flag_fore_0_side = UnityEngine.Object.Instantiate(flagSource, flags_fore_0);
            flag_fore_0_side.name = "flag_fore_0_side";
            flag_fore_0_side.localPosition = new Vector3(9.93f, 3.5f, 1.05f);
            flag_fore_0_side.localEulerAngles = new Vector3(272, 190, 0);
            flag_fore_0_side.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var flag_fore_0_back = UnityEngine.Object.Instantiate(flagSource, flags_fore_0);
            flag_fore_0_back.name = "flag_fore_0_back";
            flag_fore_0_back.localPosition = new Vector3(8.63f, 3.5f, 1.7f);
            flag_fore_0_back.localEulerAngles = new Vector3(82, 340, 0);
            flag_fore_0_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            Transform flags_fore_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_fore_1" }.transform, flags_fore.transform);
            var flag_fore_1_side = UnityEngine.Object.Instantiate(flagSource, flags_fore_1);
            flag_fore_1_side.name = "flag_fore_1_side";
            flag_fore_1_side.localPosition = new Vector3(10.6f, 3.5f, 1.1f);
            flag_fore_1_side.localEulerAngles = new Vector3(80, 300, 0);
            flag_fore_1_side.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var flag_fore_1_back = UnityEngine.Object.Instantiate(flagSource, flags_fore_1);
            flag_fore_1_back.name = "flag_fore_1_back";
            flag_fore_1_back.localPosition = new Vector3(9.1f, 3.5f, 1.74f);
            flag_fore_1_back.localEulerAngles = new Vector3(70, 302, 0);
            flag_fore_1_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            BoatPartOption flags_mizzen = Util.CreatePartOption(container, "flag_mizzen", "mizzen telltale");
            flags_mizzen.basePrice = 50;
            flags_mizzen.installCost = 10;

            Transform flags_mizzen_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_0" }.transform, flags_mizzen.transform);
            var flag_mizzen_0_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
            flag_mizzen_0_side.name = "flag_mizzen_0_side";
            flag_mizzen_0_side.localPosition = new Vector3(-10.2f, 3.3f, 2.45f);
            flag_mizzen_0_side.localEulerAngles = new Vector3(290, 180, 150);
            flag_mizzen_0_side.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var flag_mizzen_0_back = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
            flag_mizzen_0_back.name = "flag_mizzen_0_back";
            flag_mizzen_0_back.localPosition = new Vector3(-10.2f, 3.3f, 2.45f);
            flag_mizzen_0_back.localEulerAngles = new Vector3(290, 180, 150);
            flag_mizzen_0_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            Transform flags_mizzen_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_1" }.transform, flags_mizzen.transform);
            var flag_mizzen_1_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
            flag_mizzen_1_side.name = "flag_mizzen_1_side";
            flag_mizzen_1_side.localPosition = new Vector3(-8.36f, 3.3f, 2.6f);
            flag_mizzen_1_side.localEulerAngles = new Vector3(280, 160, 150);
            flag_mizzen_1_side.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var flag_mizzen_1_back = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
            flag_mizzen_1_back.name = "flag_mizzen_1_back";
            flag_mizzen_1_back.localPosition = new Vector3(-8.68f, 3.3f, 2.58f);
            flag_mizzen_1_back.localEulerAngles = new Vector3(80, 333, 0);
            flag_mizzen_1_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            //UnityEngine.Object.Destroy(flagSource);

            flags_fore.requiresDisabled.Add(foremastNone);
            flags_main.requiresDisabled.Add(mainMastNone);
            flags_mizzen.requiresDisabled.Add(structure.Find("(empty mizzen)").GetComponent<BoatPartOption>());
            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { noFlag, flags_fore, flags_main, flags_mizzen });


            mizzenShrouds.childOptions = mizzenShrouds.childOptions.AddRangeToArray(new GameObject[2] { flag_mizzen_1_side.gameObject, flag_mizzen_0_side.gameObject });
            mizzenShrouds2.childOptions = mizzenShrouds2.childOptions.AddRangeToArray(new GameObject[2] { flag_mizzen_1_back.gameObject, flag_mizzen_0_back.gameObject });
            mizzenMast1.GetComponent<BoatPartOption>().childOptions = mizzenMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_0.gameObject);
            mizzenMast2.GetComponent<BoatPartOption>().childOptions = mizzenMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_1.gameObject);

            var mainSideOption = structure.Find("part_shrouds_main_side").gameObject.GetComponent<BoatPartOption>();
            var mainBackOption = structure.Find("part_shrouds_main_back").gameObject.GetComponent<BoatPartOption>();
            mainSideOption.childOptions = mainSideOption.childOptions.AddRangeToArray(new GameObject[2] { flag_main_1_side.gameObject, flag_main_0_side.gameObject });
            mainBackOption.childOptions = mainBackOption.childOptions.AddRangeToArray(new GameObject[2] { flag_main_1_back.gameObject, flag_main_0_back.gameObject });
            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_0.gameObject);
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_1.gameObject);

            var foreSideOption = structure.Find("part_shrouds_fore_side").gameObject.GetComponent<BoatPartOption>();
            var foreBackOption = structure.Find("part_shrouds_fore_back").gameObject.GetComponent<BoatPartOption>();
            foreSideOption.childOptions = foreSideOption.childOptions.AddRangeToArray(new GameObject[2] { flag_fore_1_side.gameObject, flag_fore_0_side.gameObject });
            foreBackOption.childOptions = foreBackOption.childOptions.AddRangeToArray(new GameObject[2] { flag_fore_1_back.gameObject, flag_fore_0_back.gameObject });
            foremast.GetComponent<BoatPartOption>().childOptions = foremast.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_fore_0.gameObject);
            foremast3.GetComponent<BoatPartOption>().childOptions = foremast3.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_fore_1.gameObject);

            #endregion

            #region fore topmast forestay // alternate pos = (9.7f, 22.624f, 0), alt rot = (301.8f, 270, 90), alt scale = (1, 1, 0.84f)
            Mast foreTopmastForestay = Util.CopyMast(topmastStay1, new Vector3(9.7f, topmastStay1.localPosition.y, topmastStay1.localPosition.z), new Vector3(297.8f, topmastStay1.localEulerAngles.y, topmastStay1.localEulerAngles.z), new Vector3(1, 1, 0.92f), "forestay_foremast_extension", "fore topmast forestay", 45);
            foreTopmastForestay.reefWinch = Util.CopyWinches(foreTopmastForestay.reefWinch, topmastStay1.localPosition, foreTopmastForestay.transform.localPosition + new Vector3(0.15f, 0, 0));
            foreTopmastForestay.mastReefAtt = foreTopmast.mastReefAtt;
            BoatPartOption foreTopmastForestayOpt = foreTopmastForestay.GetComponent<BoatPartOption>();
            foreTopmastForestayOpt.requires.Remove(topMast1.GetComponent<BoatPartOption>());
            foreTopmastForestayOpt.requires.Add(foreTopmast.GetComponent<BoatPartOption>());
            partsList.availableParts[11].partOptions.Add(foreTopmastForestayOpt);

            #endregion

            #region main topmast midstay
            Mast topmastStay4_mast = Util.CopyMast(container.Find("forestay_0_long_inner"), new Vector3(0.7f, 25.27f, 0f), new Vector3(318, 270, 90), new Vector3(1, 1, 0.79f), "midstay_f_upper", "main topstay", 38);
            topmastStay4_mast.reefWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.reefWinch[0] };
            topmastStay4_mast.leftAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.leftAngleWinch[0] };
            topmastStay4_mast.rightAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.rightAngleWinch[0] };
            topmastStay4_mast.mastReefAtt = new Transform[1] { topmastStay2_mast.mastReefAtt[0] };
            topmastStay4_mast.mastHeight = 11;
            BoatPartOption topmastStayNone = Util.CreatePartOption(container, "(no topmast midstay)", "(no main topstay)");

            Mast topmastStay6_mast = Util.CopyMast(topmastStay4_mast.transform, new Vector3(0.7f, 25.27f, 0f), new Vector3(322, 270, 90), new Vector3(1, 1, 1.1f), "midstay_f_upper3", "main topstay 2", 48);
            topmastStay6_mast.reefWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.reefWinch[0] };
            topmastStay6_mast.leftAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.leftAngleWinch[0] };
            topmastStay6_mast.rightAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.rightAngleWinch[0] };
            topmastStay6_mast.mastReefAtt = new Transform[1] { topmastStay2_mast.mastReefAtt[0] };

            Mast topmastStay5_mast = Util.CopyMast(topmastStay4_mast.transform, new Vector3(5.7f, 25.27f, 0f), new Vector3(315, 270, 90), new Vector3(1, 1, 0.75f), "midstay_f_upper2", "main topstay 3", 49);
            topmastStay5_mast.reefWinch = new GPButtonRopeWinch[1] { topmastStay1_mast.reefWinch[0] };
            topmastStay5_mast.leftAngleWinch = new GPButtonRopeWinch[1] { topmastStay1_mast.leftAngleWinch[0] };
            topmastStay5_mast.rightAngleWinch = new GPButtonRopeWinch[1] { topmastStay1_mast.rightAngleWinch[0] };
            topmastStay5_mast.mastReefAtt = new Transform[1] { topmastStay1_mast.mastReefAtt[0] };
            //topmastStay5_mast.mastHeight = 11;
            // BoatPartOption topmastStayNone = Util.CreatePartOption(container, "(no topmast midstay)", "(no main topstay)");


            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { topmastStayNone, topmastStay4_mast.GetComponent<BoatPartOption>(), topmastStay6_mast.GetComponent<BoatPartOption>(), topmastStay5_mast.GetComponent<BoatPartOption>() });

            var middlestay_fore = Util.CopyMast(src5, new Vector3(0.2f, 17.7f, 0), new Vector3(308.7f, 270, 90), new Vector3(1, 1, 0.71f), "part_stay_mid_fore", "mainstay", 37);
            middlestay_fore.reefWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.reefWinch[1] };
            middlestay_fore.leftAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.leftAngleWinch[1] };
            middlestay_fore.rightAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.rightAngleWinch[1] };
            middlestay_fore.transform.GetChild(0).localScale = new Vector3(4.5f, 3.96f, 4.5f);
            middlestay_fore.transform.GetChild(1).localScale = new Vector3(2.28f, 1.99f, 2.33f);
            //middlestay_fore.mastHeight = 13;
            middlestay_fore.GetComponent<BoatPartOption>().requires = new List<BoatPartOption>() { mainMast2.GetComponent<BoatPartOption>(), foremast.GetComponent<BoatPartOption>() };
            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { Util.CreatePartOption(container, "main_stay_empty", "(no mainstay)"), middlestay_fore.GetComponent<BoatPartOption>() });

            #endregion

            #region late adjustments
            // partsList.availableParts[3].category = 2;
            foreach (BoatPartOption stay in partsList.availableParts[3].partOptions)
            {
                if (stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    stay.requires.Add(foremastNone);
                    stay.requiresDisabled.Add(bowspritNone);

                }
            }
            // partsList.availableParts[4].category = 2;
            foreach (BoatPartOption stay in partsList.availableParts[4].partOptions)
            {
                if (stay.optionName.StartsWith("topmast") || stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    stay.requiresDisabled.Add(foremast.GetComponent<BoatPartOption>());

                }
            }
            topmastStay1.GetComponent<BoatPartOption>().requires.Add(foremastNone);
            topmastStay2.GetComponent<BoatPartOption>().requires.Add(foremastNone);
            topmastStay4_mast.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foremast.GetComponent<BoatPartOption>(), mainMast2.GetComponent<BoatPartOption>() };

            var flag1 = UnityEngine.Object.Instantiate(mizzenMast1.Find("flag"), topMast1, false);
            flag1.localPosition = new Vector3(0, 0, 0.5f);
            flag1.name = "flagT";
            UnityEngine.Object.Instantiate(flag1, topMast2, false);
            UnityEngine.Object.Instantiate(flag1, foreTopmast.transform, false);
            UnityEngine.Object.Instantiate(flag1, mizzenTopmast.transform, false);

            container.Find("(empty topmast)").GetComponent<BoatPartOption>().childOptions = topMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { mainMast1.Find("flag (2)").gameObject, mainMast2.Find("flag (1)").gameObject });
            foreTopmastNone.childOptions = foreTopmastNone.childOptions.AddToArray(foremast.transform.Find("flag (2)").gameObject);
            mizzenTopmastNone.childOptions = mizzenTopmastNone.childOptions.AddToArray(mizzenMast2.transform.Find("flag (1)").gameObject);

            #endregion
            //add boat to list of modified boats

        }
    }
}
