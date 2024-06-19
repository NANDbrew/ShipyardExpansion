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
        public static void Patch(Transform boat, List<BoatPart> partsList)
        {

            //Mast[] masts = boat.GetComponent<BoatRefs>().masts;
            var boatRefs = boat.GetComponent<BoatRefs>();
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

            #region adjustments
            // move main mast aft to make room
            Util.MoveMast(mainMast2, mainMast2.localPosition + new Vector3(-0.2f, 0, 0), true);
            Util.MoveWinches(topmastStay2_mast.reefWinch, mainMast2.localPosition, mainMast2.localPosition + new Vector3(-0.2f, 0, 0));
            Util.MoveWinches(container.Find("forestay_1_inner").GetComponent<Mast>().reefWinch, mainMast2.localPosition, mainMast2.localPosition + new Vector3(-0.2f, 0, 0));
            //Util.MoveWinches(topmastStay3_mast.reefWinch, mainMast2.localPosition, new Vector3(-0.2f, 0, 0));

            Util.MoveMast(topMast2, topMast2.localPosition + new Vector3(-0.2f, 0, 0), true);
            mainMast1.gameObject.GetComponent<BoatPartOption>().optionName = "main mast 1";
            mainMast2.gameObject.GetComponent<BoatPartOption>().optionName = "main mast 2";
            BoatPartOption mainMastNone = Util.CreatePartOption(container, "(empty mainmast)", "(no main mast)");
            partsList[0].partOptions.Add(mainMastNone);
            Plugin.modPartOptions.Add(mainMastNone);

            BoatPartOption forestayNone = Util.CreatePartOption(container, "(empty forestay)", "(no forestay)");
            partsList[3].partOptions.Add(forestayNone);
            //partsList[3].activeOption = partsList[3].partOptions.Count - 1;
            Plugin.modPartOptions.Add(forestayNone);

            BoatPartOption bowspritNone = Util.CreatePartOption(container, "(empty bowsprit)", "(no bowsprit)");
            partsList[2].partOptions.Add(bowspritNone);
            //partsList[2].activeOption = partsList[2].partOptions.Count - 1;
            Plugin.modPartOptions.Add(bowspritNone);
            #endregion


            #region topmastStay
            BoatPartOption topmastStayNone = Util.CreatePartOption(container, "(no topmast stay)", "(no topmast forestay");

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

            Mast topmastStay4_mast = Util.CopyMast(container.Find("forestay_0_long_inner"), new Vector3(0.7f, 25.27f, 0f), new Vector3(318, 270, 90), new Vector3(1, 1, 0.79f), "midstay_f_upper", "topmast midstay", 22);
            topmastStay4_mast.reefWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.reefWinch[0] };
            topmastStay4_mast.leftAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.leftAngleWinch[0] };
            topmastStay4_mast.rightAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.rightAngleWinch[0] };
            topmastStay4_mast.mastReefAtt = new Transform[1] { topmastStay2_mast.mastReefAtt[0] };
            topmastStay4_mast.mastHeight = 11;

            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { topmastStayNone, topmastStay1.GetComponent<BoatPartOption>(), topmastStay2.GetComponent<BoatPartOption>(), topmastStay3.GetComponent<BoatPartOption>(), topmastStay4_mast.GetComponent<BoatPartOption>() });
            topmastStay1.GetComponent<BoatPartOption>().requiresDisabled.Add(container.Find("forestay_1_long").GetComponent<BoatPartOption>());

            partsList[3].partOptions.Remove(topmastStay1.GetComponent<BoatPartOption>());
            partsList[3].partOptions.Remove(topmastStay2.GetComponent<BoatPartOption>());
            partsList[3].partOptions.Remove(topmastStay3.GetComponent<BoatPartOption>());

            #endregion

            #region hammock
            BoatPartOption hammockNone = Util.CreatePartOption(container, "(empty hammock)", "(no hammock)");
            BoatPartOption hammock = Util.AddPartOption(container.Find("hammock").gameObject, "hammock");

            hammock.optionName = "hammock";
            hammock.childOptions = new GameObject[2] { container.Find("hammock_001").gameObject, boatRefs.walkCol.Find("hammock_001").gameObject };
            hammock.basePrice = 200;
            hammock.installCost = 100;
            hammock.mass = 5;
            hammock.requires = new List<BoatPartOption>();
            hammock.requiresDisabled = new List<BoatPartOption>();
            hammock.walkColObject = boatRefs.walkCol.Find("hammock").gameObject;

            BoatPart hammockPart = Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>(2) { hammock, hammockNone });
            #endregion

            #region foremast
            BoatPartOption foremastNone = Util.CreatePartOption(container, "(empty foremast)", "(no foremast)");
            /*var foremast = Util.CopyMast(mizzenMast1, new Vector3(8.877f, 0, 13.1f), Vector3.zero, mizzenMast1.localScale, "mast_004", "foremast");
            foremast.transform.Find("Cylinder_006").gameObject.SetActive(false);
            foremast.reefWinch = Util.CopyWinches(foremast.reefWinch, mizzenMast1.localPosition, foremast.transform.localPosition);*/
            var foremast = Util.CopyMast(mainMast1, new Vector3(8.87f, 0f, 15.865f), mainMast1.localEulerAngles, new Vector3(1f, 1f, 0.975f), "mast_04", "foremast", 29);
            foremast.reefWinch = Util.CopyWinches(foremast.reefWinch, mainMast1.localPosition, foremast.transform.localPosition);
            foremast.rightAngleWinch = Util.CopyWinches(foremast.rightAngleWinch, foremast.rightAngleWinch[0].transform.localPosition, new Vector3(3.6f, 1.98f, -2.27f));
            foremast.leftAngleWinch = Util.CopyWinches(foremast.leftAngleWinch, foremast.leftAngleWinch[0].transform.localPosition, new Vector3(3.6f, 1.98f, 2.27f));
            foremast.midAngleWinch = Util.CopyWinches(foremast.midAngleWinch, mainMast1.localPosition, foremast.transform.localPosition);
            foremast.midAngleWinch[0].transform.localPosition = new Vector3(-1.87f, 2.5f, -0.8f);
            foremast.midAngleWinch[1].transform.localPosition = new Vector3(-1.87f, 2.5f, 0.8f);
            foremast.midAngleWinch[2].transform.localPosition = new Vector3(-1.8f, 2.51f, -0.37f);
            foremast.midRopeAtt[0].parent.localPosition = new Vector3(-6.6f, -0.05f, -14.66f);

            Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { foremastNone, foremast.GetComponent<BoatPartOption>() });

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
            shrouds_back_opt.walkColObject.transform.localPosition = shrouds_back.localPosition;
            shrouds_back_opt.walkColObject.transform.localRotation = shrouds_back.localRotation;
            shrouds_back_opt.walkColObject.transform.localScale = shrouds_back.localScale;
            var second_shroud = UnityEngine.Object.Instantiate(shrouds_back, shrouds_back);
            second_shroud.localPosition = new Vector3(-0.04f, 0f, 0.15f);
            second_shroud.localScale = new Vector3(1f, 0.91f, 1f);
            second_shroud.localEulerAngles = new Vector3(0, 356, 0);
            second_shroud.GetComponent<BoatPartOption>().enabled = false;
            var second_walkCol = UnityEngine.Object.Instantiate(shrouds_back_opt.walkColObject, foremast.walkColMast);
            second_walkCol.transform.localPosition = second_shroud.localPosition;
            second_walkCol.transform.localRotation = second_shroud.localRotation;
            second_walkCol.transform.localScale = second_shroud.localScale;

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

            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { shrouds_side_opt, shrouds_back_opt });
            #endregion
            #region foremast2
            //var foremast = Util.CopyMast(mainMast1, new Vector3(10.87f, 0f, 15.565f), new Vector3(0, 20, 0), new Vector3(1f, 1f, 0.975f), "mast_04", "foremast", 29);
            #endregion


            #region mizzenMast
            Mast mizzen2 = Util.CopyMast(mainMast2, mainMast2.localPosition + new Vector3(-7, 0, 0), "mast_003", "mizzen mast 2", 25);
            mizzen2.reefWinch = Util.CopyWinches(mainMast2.GetComponent<Mast>().reefWinch, mainMast2.localPosition, mizzen2.transform.localPosition + new Vector3(0, 0.6f, 0));
            UnityEngine.Object.Instantiate(mizzenMast1.transform.Find("Cylinder_006"), mizzen2.transform, true);
            mizzen2.midAngleWinch = Util.CopyWinches(mizzenMast1.GetComponent<Mast>().midAngleWinch, mizzenMast1.localPosition, new Vector3(mizzen2.transform.localPosition.x - 1, mizzenMast1.localPosition.y, mizzenMast1.localPosition.z));
            mizzen2.midAngleWinch = mizzen2.midAngleWinch.AddToArray(UnityEngine.Object.Instantiate(mizzen2.midAngleWinch[1], container));
            mizzen2.midAngleWinch[2].transform.localPosition = mizzen2.midAngleWinch[1].transform.localPosition + new Vector3(0, 0, 0.376f);
            mizzen2.midAngleWinch[2].transform.localEulerAngles = new Vector3(1, 306, 358);
            mizzen2.midAngleWinch[1].transform.localEulerAngles = new Vector3(1, 246, 358);
            mizzen2.midAngleWinch[0].transform.localEulerAngles = mainMast1.GetComponent<Mast>().midAngleWinch[0].transform.localEulerAngles;
            mizzen2.midAngleWinch[0].transform.localPosition = mainMast1.GetComponent<Mast>().midAngleWinch[0].transform.localPosition + new Vector3(-0.36f, 0, 0);
            mizzen2.leftAngleWinch = Util.CopyWinches(mizzenMast1.GetComponent<Mast>().leftAngleWinch, mizzenMast1.GetComponent<Mast>().leftAngleWinch[0].transform.localPosition, mizzenMast1.GetComponent<Mast>().leftAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.06f, 0));
            mizzen2.rightAngleWinch = Util.CopyWinches(mizzenMast1.GetComponent<Mast>().rightAngleWinch, mizzenMast1.GetComponent<Mast>().rightAngleWinch[0].transform.localPosition, mizzenMast1.GetComponent<Mast>().rightAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.06f, 0));
            //mizzen2.orderIndex = 25;
            //mizzen2.Awake();
            partsList[1].partOptions.Add(mizzen2.GetComponent<BoatPartOption>());
            Plugin.modPartOptions.Add(mizzen2.GetComponent<BoatPartOption>());

            #region mizzenTopmast
            Mast mizzenTopmast = Util.CopyMast(topMast2, (topMast2.localPosition - mainMast2.localPosition) + mizzen2.transform.localPosition, "mast_003_extension", "mizzen topmast", 21);
            BoatPartOption mizzenTopmastNone = Util.CreatePartOption(structure, "(no mizzen topmast)", "(no mizzen topmast)");
            Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { mizzenTopmastNone, mizzenTopmast.GetComponent<BoatPartOption>() });
            mizzenTopmast.reefWinch = Util.CopyWinches(mizzenTopmast.reefWinch, mainMast2.localPosition, mizzen2.transform.localPosition + new Vector3(0, 1, 0));
            mizzenTopmast.leftAngleWinch = mizzenMast1.GetComponent<Mast>().leftAngleWinch;
            mizzenTopmast.rightAngleWinch = mizzenMast1.GetComponent<Mast>().rightAngleWinch;
            mizzenTopmast.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzen2.GetComponent<BoatPartOption>() };
            #endregion

            BoatPartOption mizShrBackOld = mizzenMast1.Find("part_shrouds_mizzen_back").GetComponent<BoatPartOption>();
            BoatPartOption mizzenShrouds = Util.CreatePartOption(structure, "part_shrouds_mizzen_back", "mizzen shrouds 2");
            mizzenShrouds.basePrice = mizShrBackOld.basePrice;
            mizzenShrouds.installCost = mizShrBackOld.installCost;
            mizzenShrouds.mass = mizShrBackOld.mass;
            var mizzenShrouds_1 = UnityEngine.Object.Instantiate(structure.Find("part_shrouds_main_back").Find("part_shrouds_1_back"), mizzenShrouds.transform);
            mizzenShrouds_1.localPosition = new Vector3(mizzen2.transform.localPosition.x, 0.01f, 17.1f);
            mizzenShrouds_1.localScale = new Vector3(1.1f, 1.1f, 1.04f);
            mizShrBackOld.transform.parent = mizzenShrouds.transform;

            var mizzenShrouds_walkCols = UnityEngine.Object.Instantiate(new GameObject(), mizzen2.walkColMast.parent);
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
            mizzenShrouds2_1.localPosition = new Vector3(mizzen2.transform.localPosition.x, 0.05f, 17.1f);
            mizzenShrouds2_1.localScale = new Vector3(1.1f, 1.1f, 1.05f);
            mizzenShrouds2_1.localEulerAngles = new Vector3(11.4f, 174.96f, 4f);
            mizShrSideOld.transform.parent = mizzenShrouds2.transform;

            var mizzenShrouds2_walkCols = UnityEngine.Object.Instantiate(new GameObject(), mizzen2.walkColMast.parent);
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
            mizzen2.GetComponent<BoatPartOption>().childOptions = new GameObject[4] {
                mizzenShrouds2_1.gameObject,
                mizzenShrouds2_1_walkCol.gameObject,
                mizzenShrouds_1.gameObject,
                mizzenShrouds_1_walkCol.gameObject,
            };
            partsList[6].partOptions = new List<BoatPartOption>() { mizzenShrouds2, mizzenShrouds };
            Plugin.modPartOptions.Add(mizzenShrouds2);

            UnityEngine.Object.Destroy(mizShrBackOld.GetComponent<BoatPartOption>());
            UnityEngine.Object.Destroy(mizShrSideOld.GetComponent<BoatPartOption>());
            #endregion

            #region forestayOuter
            var src3 = container.Find("forestay_0_short");
            Mast forestayOuter = Util.CopyMast(src3, src3.localPosition + new Vector3(4, 0, 0), new Vector3(311, 270, 90), new Vector3(1, 1, 0.94f), "foremast_stay", "foremast stay", 28);
            //Mast forestayOuter = Util.CopyMast(container.Find("forestay_0_long"), new Vector3(9.3f, 14.6f, 0f), new Vector3(319, 270, 90), new Vector3(1, 1, 0.76f), "foremast_stay", "foremast stay");
            forestayOuter.reefWinch = Util.CopyWinches(forestayOuter.reefWinch, src3.localPosition, forestayOuter.transform.localPosition);
            forestayOuter.mastHeight = 16;
            BoatPartOption forestayOuterOpt = forestayOuter.GetComponent<BoatPartOption>();
            forestayOuterOpt.requires = new List<BoatPartOption> {
                foremast.GetComponent<BoatPartOption>(),
                container.Find("part_bowsprit_long_gfx").GetComponent<BoatPartOption>() };
            forestayOuter.mastReefAtt[0] = foremast.transform.Find("rope_holder_jibs_front").Find("att");
            forestayOuter.mastReefAtt[1] = foremast.transform.Find("rope_holder_jibs_front").Find("att");
            partsList[3].partOptions.Add(forestayOuterOpt);
            #endregion

            #region forestayInner
            var src4 = container.Find("forestay_0_short_inner");
            Mast forestayInner = Util.CopyMast(src4, new Vector3(9.53f, 13.09f, 0), new Vector3(311, 270, 90), src4.transform.localScale, "foremast_stay_inner", "lower foremast stay", 27);
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
            partsList[4].partOptions.Add(forestayInnerOpt);
            #endregion

            #region middleStays
            var src5 = container.Find("part_stay_mid_0");
            var middlestay_2 = Util.CopyMast(src5, new Vector3(-6.55f, 17.7f, 0), new Vector3(315.8f, 270, 90), new Vector3(1, 1, 0.83f), "part_stay_mid_2", "middlestay 1-2", 26);
            middlestay_2.reefWinch = Util.CopyWinches(middlestay_2.reefWinch, src5.localPosition, new Vector3(-7.708f, src5.localPosition.y, 0));
            middlestay_2.reefWinch[0].transform.localEulerAngles = new Vector3(0, 90, 0);
            middlestay_2.transform.GetChild(0).localScale = new Vector3(3.6f, 3.24f, 3.8f);
            middlestay_2.transform.GetChild(1).localScale = new Vector3(2.4f, 2.2f, 2.4f);
            middlestay_2.mastHeight = 15;
            //middlestay_2.orderIndex = 26;
            //middlestay_2.Awake();
            middlestay_2.GetComponent<BoatPartOption>().requires[1] = mizzen2.GetComponent<BoatPartOption>();
            partsList[8].partOptions.Add(middlestay_2.GetComponent<BoatPartOption>());
            Plugin.modPartOptions.Add(middlestay_2.GetComponent<BoatPartOption>());

            var middlestay_3 = Util.CopyMast(src5, new Vector3(-9.86f, 14.57f, 0), new Vector3(312f, 270, 90), new Vector3(1, 1, 0.75f), "part_stay_mid_3", "middlestay 2-1", 24);
            middlestay_3.reefWinch = Util.CopyWinches(middlestay_3.reefWinch, src5.localPosition, new Vector3(-6.55f, src5.localPosition.y, 0));
            middlestay_3.transform.GetChild(0).localScale = new Vector3(4.5f, 3.96f, 4.5f);
            middlestay_3.transform.GetChild(1).localScale = new Vector3(2.28f, 1.99f, 2.33f);
            middlestay_3.mastHeight = 14;
            //middlestay_3.orderIndex = 24;
            //middlestay_3.Awake();
            middlestay_3.GetComponent<BoatPartOption>().requires[0] = structure.Find("mast_1").GetComponent<BoatPartOption>();
            partsList[8].partOptions.Add(middlestay_3.GetComponent<BoatPartOption>());
            Plugin.modPartOptions.Add(middlestay_3.GetComponent<BoatPartOption>());

            var middlestay_fore = Util.CopyMast(src5, new Vector3(0.3f, 17.7f, 0), new Vector3(308.7f, 270, 90), new Vector3(1, 1, 0.71f), "part_stay_mid_fore", "fore middlestay", 23);
            middlestay_fore.reefWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.reefWinch[1] };
            middlestay_fore.leftAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.leftAngleWinch[1] };
            middlestay_fore.rightAngleWinch = new GPButtonRopeWinch[1] { topmastStay2_mast.rightAngleWinch[1] };
            middlestay_fore.transform.GetChild(0).localScale = new Vector3(4.5f, 3.96f, 4.5f);
            middlestay_fore.transform.GetChild(1).localScale = new Vector3(2.28f, 1.99f, 2.33f);
            middlestay_fore.mastHeight = 13;
            //middlestay_fore.orderIndex = 23;
            //middlestay_fore.Awake();
            middlestay_fore.GetComponent<BoatPartOption>().requires = new List<BoatPartOption>() {  mainMast2.GetComponent<BoatPartOption>(), foremast.GetComponent<BoatPartOption>() };

            // topmast midstay
            var middlestayTopmast = Util.CopyMast(middlestay_2.transform, middlestay_2.transform.localPosition + new Vector3(0, 7.7f, 0), "part_stay_mid_topmast", "topmast middlestay", 20);
            middlestayTopmast.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzenTopmast.GetComponent<BoatPartOption>(), mainMast1.GetComponent<BoatPartOption>() };
            middlestayTopmast.reefWinch = Util.CopyWinches(middlestayTopmast.reefWinch, mizzen2.transform.localPosition, mizzen2.transform.localPosition + new Vector3(0f, 0.6f, 0f));
            middlestayTopmast.leftAngleWinch = Util.CopyWinches(middlestayTopmast.leftAngleWinch, middlestayTopmast.leftAngleWinch[0].transform.localPosition, middlestayTopmast.leftAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.05f, 0));
            middlestayTopmast.rightAngleWinch = Util.CopyWinches(middlestayTopmast.rightAngleWinch, middlestayTopmast.rightAngleWinch[0].transform.localPosition, middlestayTopmast.rightAngleWinch[0].transform.localPosition + new Vector3(0.6f, -0.05f, 0));

            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption>() { Util.CreatePartOption(container, "(empty fore midstay)", "(no fore middlestay)"), middlestay_fore.GetComponent<BoatPartOption>(), middlestayTopmast.GetComponent<BoatPartOption>() });
            #endregion

            // partsList[3].category = 2;
            foreach (BoatPartOption stay in partsList[3].partOptions)
            {
                if (stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    stay.requiresDisabled.Add(foremast.GetComponent<BoatPartOption>());
                    stay.requiresDisabled.Add(bowspritNone);

                }
            }
            // partsList[4].category = 2;
            foreach (BoatPartOption stay in partsList[4].partOptions)
            {
                if (stay.optionName.StartsWith("topmast") || stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    stay.requiresDisabled.Add(foremast.GetComponent<BoatPartOption>());

                }
            }
            topmastStay1.GetComponent<BoatPartOption>().requiresDisabled.Add(foremast.GetComponent<BoatPartOption>());
            topmastStay2.GetComponent<BoatPartOption>().requiresDisabled.Add(foremast.GetComponent<BoatPartOption>());
            topmastStay4_mast.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foremast.GetComponent<BoatPartOption>(), mainMast2.GetComponent<BoatPartOption>() };

            if (!Plugin.modCustomParts.Contains(partsList)) Plugin.modCustomParts.Add(partsList); //add boat to list of modified boats

        }
    }
}
