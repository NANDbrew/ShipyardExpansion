using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class BrigPatches
    {
        static Transform mast_001;
        static Transform mast_001_col;
        static Mast spritTopmast1;
        static Mast spritTopmast2;

        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.Find("medi medium new");
            Transform structure = container.Find("structure_container");
            Transform mizzenMast1 = structure.Find("mast_mizzen_0");
            Transform mizzenMast2 = structure.Find("mast_mizzen_1");
            Transform mainMast1 = structure.Find("mast_Back_0");
            Transform mainMast2 = structure.Find("mast_Back_1");
            Transform foreMast1 = structure.Find("mast_Front_0");
            Transform foreMast2 = structure.Find("mast_Front_1");
            Transform backstay1_0top = container.Find("backstay_1-0_top");
            Transform backstay1_0bottom = container.Find("backstay_1-0_bottom");
            Transform forestay_top_src = container.Find("forestay_0_top_longsprit");
            Transform forestay_0_mid = container.Find("forestay_0_mid");
            Transform midstayTop1 = container.Find("midstay_0-0_top");
            BoatPartOption noForemast = container.Find("(no foremast)").GetComponent<BoatPartOption>();
            BoatPartOption noBowsprit = container.Find("(no bowsprit)").GetComponent<BoatPartOption>();
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent;

            PartRefs.brig = container;
            PartRefs.brigCol = walkCol;

            #region adjustments
            Util.MoveMast(mizzenMast1, new Vector3(-12.43f, mizzenMast1.localPosition.y, mizzenMast1.localPosition.z), true);
            Util.MoveMast(backstay1_0top, new Vector3(-11.4f, backstay1_0top.localPosition.y, backstay1_0top.localPosition.z), true);
            Util.MoveMast(backstay1_0bottom, new Vector3(-11.41f, 14.6f, backstay1_0bottom.localPosition.z), true);

            backstay1_0top.localScale = new Vector3(1, 1, 1.17f);
            backstay1_0bottom.localScale = new Vector3(1, 1, 1.17f);
            backstay1_0top.GetComponent<Mast>().mastHeight = 14;
            backstay1_0bottom.GetComponent<Mast>().mastHeight = 14;
            mainMast1.GetComponent<Mast>().mastHeight = 18.8f;
 
            #endregion

            #region mizzenShrouds
            Transform mizzenShroudsBack = UnityEngine.Object.Instantiate(new GameObject() { name = "parts_shrouds_M_default" }.transform, container);
            mizzenShroudsBack.transform.localEulerAngles = new Vector3 (270f, 0f, 0f);
            Transform mizzenShroudsSide = UnityEngine.Object.Instantiate(container.Find("parts_shrouds_F_spread"), container);
            BoatPartOption backOption = Util.CopyPartOption(container.Find("parts_shrouds_F_default").GetComponent<BoatPartOption>(), mizzenShroudsBack.gameObject, "mizzen shrouds 1");
            BoatPartOption sideOption = mizzenShroudsSide.GetComponent<BoatPartOption>();

            var newCont1 = UnityEngine.Object.Instantiate(new GameObject() { name = "shrouds_default_1" }, mizzenShroudsBack);
            //newCont1.transform.localPosition = mizzenMast1.localPosition;
            mizzenMast1.Find("static_rig_011").parent = newCont1.transform;
            mizzenMast1.Find("trim_007").parent = newCont1.transform;
            newCont1.transform.localScale = new Vector3(1, 0.91f, 1);
            var newCont2 = UnityEngine.Object.Instantiate(new GameObject() { name = "shrouds_default_2" }, mizzenShroudsBack);
            //newCont2.transform.localPosition = mizzenMast2.localPosition;
            mizzenMast2.Find("static_rig_012").parent = newCont2.transform;
            mizzenMast2.Find("trim_009").parent = newCont2.transform;
            backOption.walkColObject = UnityEngine.Object.Instantiate(new GameObject() { name = mizzenShroudsBack.name }, sideOption.walkColObject.transform.parent);
            var walkCol1 = UnityEngine.Object.Instantiate(new GameObject() { name = newCont1.name }, backOption.walkColObject.transform);
            var walkCol2 = UnityEngine.Object.Instantiate(new GameObject() { name = newCont2.name }, backOption.walkColObject.transform);

            mizzenShroudsSide.name = "parts_shrouds_M_spread";
            sideOption.optionName = "mizzen shrouds 2";
            sideOption.walkColObject = UnityEngine.Object.Instantiate(sideOption.walkColObject, sideOption.walkColObject.transform.parent);
            var sideShrouds1 = mizzenShroudsSide.Find("shrouds_alt_F0");
            sideShrouds1.localPosition = new Vector3(-12.06f, 0, 17.4f); // new (-12.8f, 0f, 21.8f)
            sideShrouds1.localScale = new Vector3(0.934f, 0.9f, 0.795f); // new (0.93f, -1f, 1f)
            var trim1 = sideShrouds1.Find("trim_013");
            trim1.localScale = new Vector3(-trim1.localScale.x, trim1.localScale.y, trim1.localScale.z);
            trim1.localPosition = new Vector3(trim1.localPosition.x, -trim1.localPosition.y, trim1.localPosition.z);
            var sideCol1 = sideOption.walkColObject.transform.Find("shrouds_alt_F0");
            sideCol1.localPosition = sideShrouds1.localPosition;
            sideCol1.localRotation = sideShrouds1.localRotation;
            var sideShrouds2 = UnityEngine.Object.Instantiate(sideShrouds1, mizzenShroudsSide);
            var sideCol2 = UnityEngine.Object.Instantiate(sideOption.walkColObject.transform.GetChild(0), sideOption.walkColObject.transform);
            sideShrouds2.localPosition = new Vector3(-7.9f, 0, 17.35f);
            sideShrouds2.localScale = new Vector3(1.075f, -1f, 0.798f);
            sideCol2.localPosition = sideShrouds2.localPosition;
            sideCol2.localRotation = sideShrouds2.localRotation;
            UnityEngine.Object.Destroy(mizzenShroudsSide.Find("shrouds_alt_F1").gameObject);
            UnityEngine.Object.Destroy(sideOption.walkColObject.transform.Find("shrouds_alt_F1").gameObject);

            var mizMastComp = mizzenMast1.GetComponent<Mast>();
            mizMastComp.walkColMast.transform.Find("static_rig_011").parent = walkCol1.transform;
            mizMastComp.walkColMast.transform.Find("trim_007").parent = walkCol1.transform;
            var mizMastComp2 = mizzenMast2.GetComponent<Mast>();
            mizMastComp2.walkColMast.transform.Find("static_rig_012").parent = walkCol2.transform;
            mizMastComp2.walkColMast.transform.Find("trim_009").parent = walkCol2.transform;

            mizzenMast1.GetComponent<BoatPartOption>().childOptions = new GameObject[4] { 
                newCont1, 
                walkCol1,
                sideShrouds1.gameObject,
                sideOption.walkColObject.transform.GetChild(0).gameObject,
                };
            mizzenMast2.GetComponent<BoatPartOption>().childOptions = new GameObject[4] {
                newCont2,
                walkCol2,
                sideShrouds2.gameObject,
                sideCol2.gameObject,
                };

            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption> { backOption, sideOption });
            #endregion

            #region longstays
            Vector3 newPos = new Vector3(mainMast2.localPosition.x, foreMast1.localPosition.y, foreMast1.localPosition.z);
            Transform stayAtt = mainMast2.Find("rope_holder_005").Find("stay att");
            Transform stayAtt2 = UnityEngine.Object.Instantiate(stayAtt.parent, mainMast2).GetChild(0);
            stayAtt2.parent.localPosition = stayAtt.parent.localPosition + new Vector3(0f, 0f, -6.6f);

            var mainstayOuter = Util.CopyMast(forestay_top_src, new Vector3(0f, 25.78f, 0f), forestay_top_src.localEulerAngles, new Vector3(1f, 1f, 1.18f), "mainstay_top", "forestay 3", 32);
            mainstayOuter.reefWinch = Util.CopyWinches(mainstayOuter.reefWinch, foreMast1.localPosition, newPos);

            var outerLeft1 = UnityEngine.Object.Instantiate(container.Find("rope_winch_jib_angle_left"), container).GetComponent<GPButtonRopeWinch>();
            outerLeft1.transform.localPosition = new Vector3(-0.9f, 2.66f, 3.15f);
            outerLeft1.name = "rope_winch_jib_angle_left1_mod";
            var outerLeft2 = foreMast1.GetComponent<Mast>().leftAngleWinch[0];

            mainstayOuter.leftAngleWinch = new GPButtonRopeWinch[2] { outerLeft1, outerLeft2 };
            //mainstayOuter.leftAngleWinch[1] = foreMast1.GetComponent<Mast>().leftAngleWinch[0];

            var outerRight1 = UnityEngine.Object.Instantiate(container.Find("rope_winch_jib_angle_right"), container).GetComponent<GPButtonRopeWinch>();
            outerRight1.transform.localPosition = new Vector3(-0.9f, 2.66f, -3.15f);
            outerRight1.name = "rope_winch_jib_angle_right1_mod";
            var outerRight2 = foreMast1.GetComponent<Mast>().rightAngleWinch[0];

            mainstayOuter.rightAngleWinch = new GPButtonRopeWinch[2] { outerRight1, outerRight2 };
            //mainstayOuter.rightAngleWinch[1] = foreMast1.GetComponent<Mast>().rightAngleWinch[0];

            mainstayOuter.mastReefAtt = new Transform[2] { stayAtt, stayAtt };
            mainstayOuter.mastHeight = 25;
            var mainstayOuterOpt = mainstayOuter.GetComponent<BoatPartOption>();
            mainstayOuterOpt.requires = new List<BoatPartOption> {
                mainMast2.GetComponent<BoatPartOption>(), 
                noForemast,
                };
            mainstayOuterOpt.requiresDisabled = new List<BoatPartOption> { noBowsprit };
            partsList.availableParts[4].partOptions.Add(mainstayOuterOpt);
            
            var mainstayInner = Util.CopyMast(forestay_0_mid, new Vector3(0f, 19.44f, 0f), forestay_0_mid.localEulerAngles, new Vector3(1f, 1f, 1.18f), "mainstay_bottom", "lower forestay 3", 31);
            mainstayInner.reefWinch = Util.CopyWinches(mainstayInner.reefWinch, foreMast1.localPosition, newPos);
            mainstayInner.reefWinch = new GPButtonRopeWinch[] { mainstayInner.reefWinch[0], Util.CopyWinch(mainstayInner.reefWinch[0], new Vector3(mainstayInner.reefWinch[0].transform.localPosition.x, 3.108f, mainstayInner.reefWinch[0].transform.localPosition.z)) };

            mainstayInner.leftAngleWinch = Util.CopyWinches(mainstayOuter.leftAngleWinch, mainstayOuter.leftAngleWinch[0].transform.localPosition, new Vector3(-1.57f, 2.69f, 3.15f));
            mainstayInner.rightAngleWinch = Util.CopyWinches(mainstayOuter.rightAngleWinch, mainstayOuter.rightAngleWinch[0].transform.localPosition, new Vector3(-1.57f, 2.69f, -3.15f));


            /*                var innerLeft1 = UnityEngine.Object.Instantiate(foreMast1.GetComponent<Mast>().leftAngleWinch[0], container);
                            innerLeft1.transform.localPosition += new Vector3(-0.5f, 0, 0);
                            innerLeft1.name = "rope_winch_jib_angle_left1_mod";
                            var innerLeft2 = UnityEngine.Object.Instantiate(outerLeft1, container);
                            innerLeft2.transform.localPosition = new Vector3(-1.6f, 2.7f, 3.06f);
                            innerLeft2.name = "rope_winch_jib_angle_left2_mod";
                            mainstayInner.leftAngleWinch = new GPButtonRopeWinch[2] { innerLeft1, innerLeft2.GetComponent<GPButtonRopeWinch>() };
                            Debug.Log("leftAngleWinch= " + mainstayInner.leftAngleWinch[0]);
                            var innerRight1 = UnityEngine.Object.Instantiate(foreMast1.GetComponent<Mast>().rightAngleWinch[0], container);
                            innerRight1.transform.localPosition += new Vector3(-0.5f, 0, 0);
                            innerRight1.name = "rope_winch_jib_angle_right1_mod";
                            var innerRight2 = UnityEngine.Object.Instantiate(outerRight1, container);
                            innerRight2.transform.localPosition = new Vector3(-1.6f, 2.7f, -3.06f);
                            innerRight2.name = "rope_winch_jib_angle_right2_mod";
                            mainstayInner.rightAngleWinch = new GPButtonRopeWinch[2] { innerRight1, innerRight2.GetComponent<GPButtonRopeWinch>() };
                            Debug.Log("rightAngleWinch= " + mainstayInner.rightAngleWinch[0]);*/

            stayAtt2.parent.parent = mainstayInner.transform;
            mainstayInner.mastReefAtt = new Transform[2] { stayAtt2, stayAtt2 };
            mainstayInner.mastHeight = 20;
            mainstayInner.maxSails = 2;
            var mainstayInnerOpt = mainstayInner.GetComponent<BoatPartOption>();
            mainstayInnerOpt.requires = new List<BoatPartOption> {
                mainMast2.GetComponent<BoatPartOption>(), 
                noForemast,
                };
            mainstayInnerOpt.requiresDisabled = new List<BoatPartOption> { noBowsprit };
            partsList.availableParts[5].partOptions.Add(mainstayInner.GetComponent<BoatPartOption>());
                        #endregion

            #region telltale
            var flagSource = structure.Find("mast_Front_0").Find("wind_flag");
            //flagSource.localScale = new Vector3(0.8f, 1f, 0.5f);
            //flagSource.GetComponent<MeshRenderer>().material.color = Color.green;
            BoatPartOption noFlag = Util.CreatePartOption(container, "(flag empty)", "(no telltale)");

            BoatPartOption flags_main = Util.CreatePartOption(container, "flag_main", "mainmast telltale");
            flags_main.basePrice = 50;
            flags_main.installCost = 10;

            Transform flags_main_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_0" }.transform, flags_main.transform);
            var flag_main_0_side = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_side.name = "flag_main_0_side";
            flag_main_0_side.localPosition = new Vector3(-5.25f, 6f, 3.28f);
            flag_main_0_side.localEulerAngles = new Vector3(87, 0, 0);
            flag_main_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);
            var flag_main_0_back = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_back.name = "flag_main_0_back";
            flag_main_0_back.localPosition = new Vector3(-6.78f, 6f, 3.26f);
            flag_main_0_back.localEulerAngles = new Vector3(79, 340, 0);
            flag_main_0_back.localScale = new Vector3(0.8f, 1f, 0.5f);

            Transform flags_main_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_1" }.transform, flags_main.transform);
            var flag_main_1_side = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_side.name = "flag_main_1_side";
            flag_main_1_side.localPosition = new Vector3(-0.55f, 6f, 3.33f);
            flag_main_1_side.localEulerAngles = new Vector3(86, 0, 0);
            flag_main_1_side.localScale = new Vector3(0.8f, 1f, 0.5f);
            var flag_main_1_back = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_back.name = "flag_main_1_back";
            flag_main_1_back.localPosition = new Vector3(-1.84f, 6f, 3.07f);
            flag_main_1_back.localEulerAngles = new Vector3(80, 340, 0);
            flag_main_1_back.localScale = new Vector3(0.8f, 1f, 0.5f);


            BoatPartOption flags_fore = Util.CreatePartOption(container, "flag_fore", "foremast telltale");
            flags_fore.basePrice = 50;
            flags_fore.installCost = 10;

            Transform flags_fore_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_fore_0" }.transform, flags_fore.transform);
            var flag_fore_0_side = UnityEngine.Object.Instantiate(flagSource, flags_fore_0);
            flag_fore_0_side.name = "flag_fore_0_side";
            flag_fore_0_side.localPosition = new Vector3(6.76f, 5f, 3.05f);
            flag_fore_0_side.localEulerAngles = new Vector3(87, 0, 0);
            flag_fore_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);
            var flag_fore_0_back = UnityEngine.Object.Instantiate(flagSource, flags_fore_0);
            flag_fore_0_back.name = "flag_fore_0_back";
            flag_fore_0_back.localPosition = new Vector3(5.55f, 5f, 3.05f);
            flag_fore_0_back.localEulerAngles = new Vector3(79, 340, 0);
            flag_fore_0_back.localScale = new Vector3(0.8f, 1f, 0.5f);

            Transform flags_fore_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_fore_1" }.transform, flags_fore.transform);
            var flag_fore_1_side = UnityEngine.Object.Instantiate(flagSource, flags_fore_1);
            flag_fore_1_side.name = "flag_fore_1_side";
            flag_fore_1_side.localPosition = new Vector3(10.95f, 5f, 2.4f);
            flag_fore_1_side.localEulerAngles = new Vector3(89, 0, 0);
            flag_fore_1_side.localScale = new Vector3(0.8f, 1f, 0.5f);
            var flag_fore_1_back = UnityEngine.Object.Instantiate(flagSource, flags_fore_1);
            flag_fore_1_back.name = "flag_fore_1_back";
            flag_fore_1_back.localPosition = new Vector3(9.55f, 5f, 2.6f);
            flag_fore_1_back.localEulerAngles = new Vector3(81, 326, 0);
            flag_fore_1_back.localScale = new Vector3(0.8f, 1f, 0.5f);

            BoatPartOption flags_mizzen = Util.CreatePartOption(container, "flag_mizzen", "mizzen telltale");
            flags_mizzen.basePrice = 50;
            flags_mizzen.installCost = 10;

            Transform flags_mizzen_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_0" }.transform, flags_mizzen.transform);
            var flag_mizzen_0_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
            flag_mizzen_0_side.name = "flag_mizzen_0_side";
            flag_mizzen_0_side.localPosition = new Vector3(-11.8f, 8f, 2.7f);
            flag_mizzen_0_side.localEulerAngles = new Vector3(87, 0, 0);
            flag_mizzen_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);
            var flag_mizzen_0_back = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
            flag_mizzen_0_back.name = "flag_mizzen_0_back";
            flag_mizzen_0_back.localPosition = new Vector3(-12.24f, 8f, 2.45f);
            flag_mizzen_0_back.localEulerAngles = new Vector3(80, 340, 0);
            flag_mizzen_0_back.localScale = new Vector3(0.8f, 1f, 0.5f);

            Transform flags_mizzen_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_1" }.transform, flags_mizzen.transform);
            var flag_mizzen_1_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
            flag_mizzen_1_side.name = "flag_mizzen_1_side";
            flag_mizzen_1_side.localPosition = new Vector3(-7.65f, 8f, 3.15f);
            flag_mizzen_1_side.localEulerAngles = new Vector3(87, 0, 0);
            flag_mizzen_1_side.localScale = new Vector3(0.8f, 1f, 0.5f);
            var flag_mizzen_1_back = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
            flag_mizzen_1_back.name = "flag_mizzen_1_back";
            flag_mizzen_1_back.localPosition = new Vector3(-8.15f, 8f, 2.76f);
            flag_mizzen_1_back.localEulerAngles = new Vector3(80, 350, 0);
            flag_mizzen_1_back.localScale = new Vector3(0.8f, 1f, 0.5f);

            //UnityEngine.Object.Destroy(flagSource);

            flags_fore.requiresDisabled.Add(noForemast);
            flags_main.requiresDisabled.Add(container.Find("(no mainmast)").GetComponent<BoatPartOption>());
            flags_mizzen.requiresDisabled.Add(container.Find("(no mizzen mast)").GetComponent<BoatPartOption>());
            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { noFlag, flags_fore, flags_main, flags_mizzen });


            sideOption.childOptions = sideOption.childOptions.AddRangeToArray(new GameObject[2] { flag_mizzen_1_side.gameObject, flag_mizzen_0_side.gameObject });
            backOption.childOptions = backOption.childOptions.AddRangeToArray(new GameObject[2] { flag_mizzen_1_back.gameObject, flag_mizzen_0_back.gameObject });
            mizzenMast1.GetComponent<BoatPartOption>().childOptions = mizzenMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_0.gameObject);
            mizzenMast2.GetComponent<BoatPartOption>().childOptions = mizzenMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_1.gameObject);

            var mainSideOption = container.Find("parts_shrouds_B_spread").gameObject.GetComponent<BoatPartOption>();
            var mainBackOption = container.Find("parts_shrouds_B_default").gameObject.GetComponent<BoatPartOption>();
            mainSideOption.childOptions = mainSideOption.childOptions.AddRangeToArray(new GameObject[2] { flag_main_1_side.gameObject, flag_main_0_side.gameObject });
            mainBackOption.childOptions = mainBackOption.childOptions.AddRangeToArray(new GameObject[2] { flag_main_1_back.gameObject, flag_main_0_back.gameObject });
            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_0.gameObject);
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_1.gameObject);

            var foreSideOption = container.Find("parts_shrouds_F_spread").gameObject.GetComponent<BoatPartOption>();
            var foreBackOption = container.Find("parts_shrouds_F_default").gameObject.GetComponent<BoatPartOption>();
            foreSideOption.childOptions = foreSideOption.childOptions.AddRangeToArray(new GameObject[2] { flag_fore_1_side.gameObject, flag_fore_0_side.gameObject });
            foreBackOption.childOptions = foreBackOption.childOptions.AddRangeToArray(new GameObject[2] { flag_fore_1_back.gameObject, flag_fore_0_back.gameObject });
            foreMast1.GetComponent<BoatPartOption>().childOptions = foreMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_fore_0.gameObject);
            foreMast2.GetComponent<BoatPartOption>().childOptions = foreMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_fore_1.gameObject);

            #endregion

            /*Mast bowsprit = structure.Find("bowsprit_standard").GetChild(0).GetComponent<Mast>();
            //var spritTopmast = Util.CreatePartOption(structure, "sprit_topmast", "sprit topmast");
            var spritTopmast = UnityEngine.Object.Instantiate(Plugin.spritRef, structure);
            var partOption = Util.CopyPartOption(bowsprit.GetComponent<BoatPartOption>(), spritTopmast.gameObject, "sprit topmast");
            //Mast spritTopmastM = Util.CopyMast(bowsprit, )
            spritTopmast.transform.localPosition = new Vector3(20.6f, 9.3f, 0);
            Mast stmM = spritTopmast.gameObject.AddComponent<Mast>();
            stmM.orderIndex = 33;
            stmM.mastHeight = 5;
            stmM.maxSails = 1;
            stmM.startingSailColor = 0;
            stmM.shipRigidbody = boat.GetComponent<Rigidbody>();
            stmM.reefWinch = Util.CopyWinches(bowsprit.reefWinch, bowsprit.reefWinch[0].transform.localPosition, bowsprit.reefWinch[0].transform.localPosition + new Vector3(1, 0, 0));
            stmM.leftAngleWinch = Util.CopyWinches(bowsprit.leftAngleWinch, bowsprit.leftAngleWinch[0].transform.localPosition, bowsprit.leftAngleWinch[0].transform.localPosition + new Vector3(1, 0, 0));
            stmM.rightAngleWinch = Util.CopyWinches(bowsprit.rightAngleWinch, bowsprit.rightAngleWinch[0].transform.localPosition, bowsprit.rightAngleWinch[0].transform.localPosition + new Vector3(1, 0, 0));
            stmM.Awake();
            BoatPartOption stmNone = Util.CreatePartOption(container, "(no-sprit_topmast)", "(no sprit topmast)");
            BoatPart part = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { stmNone, partOption });*/

            #region sprit topmast
            Debug.Log("trying to add part");
            BoatPartOption bowsprit = structure.Find("bowsprit_standard").GetComponent<BoatPartOption>();
            BoatPartOption bowspritLong = structure.Find("bowsprit_long").GetComponent<BoatPartOption>();

            spritTopmast1 = Util.CopyMast(bowsprit.transform.GetChild(0), structure, walkCol, new Vector3(20.1f, 0f, 9.3f), Vector3.zero, Vector3.one, "sprit_topmast", "sprit topmast", 33);
            spritTopmast1.mastHeight = 4;
            spritTopmast1.reefWinch = Util.CopyWinches(spritTopmast1.reefWinch, spritTopmast1.reefWinch[0].transform.localPosition, new Vector3(16.14f, 4.57f, -0.27f)); // alt pos = 20.37f, 7.6f, 0
            spritTopmast1.reefWinch[0].transform.localEulerAngles = new Vector3(355, 183, 160); // alt rotation = 0, 270, 0
            spritTopmast1.leftAngleWinch = Util.CopyWinches(spritTopmast1.leftAngleWinch, spritTopmast1.leftAngleWinch[0].transform.localPosition, new Vector3(12.4f, 3.12f, 1.5f));
            spritTopmast1.rightAngleWinch = Util.CopyWinches(spritTopmast1.rightAngleWinch, spritTopmast1.rightAngleWinch[0].transform.localPosition, new Vector3(12.4f, 3.12f, -1.5f));

            var stmFlag = UnityEngine.Object.Instantiate(flagSource, spritTopmast1.transform);
            stmFlag.transform.localPosition = new Vector3(0.3f, 0, 1);

           /* var spritBrace0 = Util.CopyMast(container.Find("backstay_1-1_top"), new Vector3(20, 6.7f, -0.15f), new Vector3(334.82f, 78.3f, 0), new Vector3(0.95f, 0.95f, 1), "sprit_brace_R", "sprit brace R", 63);
            spritBrace0.GetComponent<Mast>().enabled = false;
            spritBrace0.GetComponent<BoatPartOption>().enabled = false;
            //spritBrace0.transform.parent = spritTopmast1.transform;
            var spritBrace1 = Util.CopyMast(container.Find("backstay_1-1_top"), new Vector3(20, 6.7f, 0.15f), new Vector3(334.82f, 101.7f, 0), new Vector3(0.95f, 0.95f, 1), "sprit_brace_L", "sprit brace L", 63);
            spritBrace1.GetComponent<Mast>().enabled = false;
            spritBrace1.GetComponent<BoatPartOption>().enabled = false;*/
            //spritBrace1.transform.parent = spritTopmast1.transform;

            BoatPartOption partOption = Util.CopyPartOption(bowsprit, spritTopmast1.gameObject, "sprit topmast 1");
            partOption.mass = 20;
            partOption.basePrice /= 2;
            partOption.installCost /= 2;

            spritTopmast2 = Util.CopyMast(spritTopmast1.transform, new Vector3(23.5f, 0, 10.9f), "sprit_topmast_2", "sprit topmast 2", 34);
            //spritTopmast2.reefWinch = Util.CopyWinches(spritTopmast2.reefWinch, spritTopmast1.reefWinch[0].transform.localPosition, new Vector3(23.76f, 9.2f, 0));
            BoatPartOption spritTopmast2_opt = spritTopmast2.GetComponent<BoatPartOption>();
            partOption.requires = new List<BoatPartOption> { bowsprit };
            spritTopmast2_opt.requires = new List<BoatPartOption> { bowspritLong };

            /*var spritBrace20 = spritTopmast2.transform.Find("sprit_brace_R");
            spritBrace20.localPosition = new Vector3(-1.75f, 0.15f, -3.94f);
            spritBrace20.localEulerAngles = new Vector3(7, 64.35f, 84.96f);
            spritBrace20.localScale = new Vector3(0.95f, 0.95f, 1.2f);
            var spritBrace21 = spritTopmast2.transform.Find("sprit_brace_L");
            spritBrace21.localPosition = new Vector3(-1.75f, -0.15f, -3.94f);
            spritBrace21.localEulerAngles = new Vector3(353.3f, 64.35f, 95.04f);
            spritBrace21.localScale = new Vector3(0.95f, 0.95f, 1.2f);

            var spritBrace20Col = spritBrace20.GetComponent<Mast>().walkColMast;
            spritBrace20Col.parent = spritTopmast2.walkColMast;
            spritBrace20Col.localPosition = new Vector3(-1.75f, 0.15f, -3.94f);
            spritBrace20Col.localEulerAngles = new Vector3(7, 64.35f, 84.96f);
            spritBrace20Col.localScale = new Vector3(0.95f, 0.95f, 1.2f);
            var spritBrace21Col = spritBrace21.GetComponent<Mast>().walkColMast;
            spritBrace21Col.parent = spritTopmast2.walkColMast;
            spritBrace21Col.localPosition = new Vector3(-1.75f, -0.15f, -3.94f);
            spritBrace21Col.localEulerAngles = new Vector3(353.3f, 64.35f, 95.04f);
            spritBrace21Col.localScale = new Vector3(0.95f, 0.95f, 1.2f);*/


            BoatPartOption stmNone = Util.CreatePartOption(structure, "(no-sprit_topmast)", "(no sprit topmast)");
            Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { stmNone, partOption, spritTopmast2_opt });
            bowspritLong.transform.GetChild(0).localPosition += new Vector3(0, 0, -1f);
            bowsprit.transform.GetChild(0).localPosition += new Vector3(0, 0, -1f);
            #endregion

            #region topmasts
            GPButtonRopeWinch[] topmast0Reef = Util.CopyWinches(new GPButtonRopeWinch[] { foreMast1.GetComponent<Mast>().reefWinch.Last() }, Vector3.zero, new Vector3(0, 0.66f, 0));
            GPButtonRopeWinch[] topmast01Reef = Util.CopyWinches(new GPButtonRopeWinch[] { foreMast2.GetComponent<Mast>().reefWinch.Last() }, Vector3.zero, new Vector3(0, 0.66f, 0));
            GPButtonRopeWinch[] topmast0Angle = new GPButtonRopeWinch[1] { Util.CopyWinch(foreMast2.GetComponent<Mast>().midAngleWinch.Last(), new Vector3(-1.22f, 3.15f, -0.27f)) };

            GPButtonRopeWinch[] topmast1Reef = new GPButtonRopeWinch[1] { Util.CopyWinch(container.Find("rope_winch_mastB0_reef (3)").GetComponent<GPButtonRopeWinch>(), new Vector3(-5.1f, 5.93f, 0)) };
            topmast1Reef[0].transform.localScale = new Vector3(1, 1, -1);
            GPButtonRopeWinch[] topmast11Reef = Util.CopyWinches(new GPButtonRopeWinch[] { mainMast2.GetComponent<Mast>().reefWinch.Last() }, Vector3.zero, new Vector3(0, 0.66f, 0));
            GPButtonRopeWinch[] topmast1Angle = new GPButtonRopeWinch[1] { Util.CopyWinch(mainMast2.GetComponent<Mast>().midAngleWinch.Last(), new Vector3(-4.03f, 5.21f, -1.2f)) };
            GPButtonRopeWinch[] topmast1LeftAngle = new GPButtonRopeWinch[1] { Util.CopyWinch(mainMast1.GetComponent<Mast>().leftAngleWinch[0], new Vector3(-13.7f, 5.6f, 2.48f)) };
            GPButtonRopeWinch[] topmast1RightAngle = new GPButtonRopeWinch[1] { Util.CopyWinch(mainMast1.GetComponent<Mast>().rightAngleWinch[0], new Vector3(-13.7f, 5.6f, -2.48f)) };

            GPButtonRopeWinch[] topmast2Reef = Util.CopyWinches(new GPButtonRopeWinch[] { mizzenMast1.GetComponent<Mast>().reefWinch.Last() }, Vector3.zero, new Vector3(0.1f, -0.66f, -0.62f));
            topmast2Reef[0].transform.localScale = new Vector3(0.9f, 0.9f, -0.9f);
            GPButtonRopeWinch[] topmast21Reef = Util.CopyWinches(new GPButtonRopeWinch[] { mizzenMast2.GetComponent<Mast>().reefWinch.Last() }, Vector3.zero, new Vector3(0, 0, -0.642f));
            topmast21Reef[0].transform.localScale = new Vector3(0.9f, 0.9f, -0.9f);
            GPButtonRopeWinch[] topmast2Angle = new GPButtonRopeWinch[1] { Util.CopyWinch(mizzenMast1.GetComponent<Mast>().leftAngleWinch.Last(), new Vector3(-15.2f, 5.53f, 0f)) };
            //topmast2Angle[0].transform.localEulerAngles = new Vector3(270, 0, 0);
            GPButtonRopeWinch[] topmast2LeftAngle = new GPButtonRopeWinch[1] { Util.CopyWinch(mizzenMast1.GetComponent<Mast>().leftAngleWinch[0], new Vector3(-15.3f, 5.726f, 2.27f)) };
            GPButtonRopeWinch[] topmast2RightAngle = new GPButtonRopeWinch[1] { Util.CopyWinch(mizzenMast1.GetComponent<Mast>().rightAngleWinch[0], new Vector3(-15.3f, 5.726f, -2.27f)) };


            var topmast0None = Util.CreatePartOption(structure, "fore_topmast_empty", "(no fore topmast)");
            var topmast0Part = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { topmast0None, });
            var topmast1None = Util.CreatePartOption(structure, "main_topmast_empty", "(no main topmast)");
            var topmast1Part = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { topmast1None, });
            var topmast2None = Util.CreatePartOption(structure, "mizzen_topmast_empty", "(no mizzen topmast)");
            var topmast2Part = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { topmast2None, });


            #endregion

            #region topmast stays
            Mast mainTopstay1 = Util.CopyMast(midstayTop1, new Vector3(-4.7f, 32.7f, 0), midstayTop1.localEulerAngles, new Vector3(1, 1, 0.99f), "mainstay_topmast_1-1", "main topmast stay 1", 41);
            mainTopstay1.reefWinch = Util.CopyWinches(mainTopstay1.reefWinch, Vector3.zero, new Vector3(0f, 0, 0.66f));
            mainTopstay1.leftAngleWinch = Util.CopyWinches(mainTopstay1.leftAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            mainTopstay1.rightAngleWinch = Util.CopyWinches(mainTopstay1.rightAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            mainTopstay1.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foreMast1.GetComponent<BoatPartOption>() };
            
            var midstayTop01 = container.Find("midstay_0-1_top");
            Mast mainTopstay2 = Util.CopyMast(midstayTop01, new Vector3(0.1f, 32.5f, 0), midstayTop01.localEulerAngles, new Vector3(1, 1, 0.97f), "mainstay_topmast_1-2", "main topmast stay 2", 42);
            mainTopstay2.reefWinch = Util.CopyWinches(mainTopstay2.reefWinch, Vector3.zero, new Vector3(0f, 0.66f, 0f));
            mainTopstay2.leftAngleWinch = mainTopstay1.leftAngleWinch;
            mainTopstay2.rightAngleWinch = mainTopstay1.rightAngleWinch;
            mainTopstay2.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foreMast1.GetComponent<BoatPartOption>() };
            
            var midstayTop10 = container.Find("midstay_1-0_top");
            Mast mainTopstay3 = Util.CopyMast(midstayTop10, new Vector3(-4.62f, 32.7f, 0), midstayTop10.localEulerAngles, new Vector3(1, 1, 0.988f), "mainstay_topmast_2-1", "main topmast stay 3", 43);
            mainTopstay3.reefWinch = mainTopstay1.reefWinch;
            mainTopstay3.leftAngleWinch = mainTopstay1.leftAngleWinch;
            mainTopstay3.rightAngleWinch = mainTopstay1.rightAngleWinch;
            mainTopstay3.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foreMast2.GetComponent<BoatPartOption>() };
            var midstayTop11 = container.Find("midstay_1-1_top");
            Mast mainTopstay4 = Util.CopyMast(midstayTop11, new Vector3(0.1f, 32.7f, 0), midstayTop11.localEulerAngles, new Vector3(1, 1, 0.99f), "mainstay_topmast_2-2", "main topmast stay 4", 44);
            mainTopstay4.reefWinch = mainTopstay2.reefWinch;
            mainTopstay4.leftAngleWinch = mainTopstay1.leftAngleWinch;
            mainTopstay4.rightAngleWinch = mainTopstay1.rightAngleWinch;
            mainTopstay4.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foreMast2.GetComponent<BoatPartOption>() };

            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption>() { Util.CreatePartOption(container, "main_topmast_stay_empty", "(no main topmast stay)"), mainTopstay1.GetComponent<BoatPartOption>(), mainTopstay2.GetComponent<BoatPartOption>(), mainTopstay3.GetComponent<BoatPartOption>(), mainTopstay4.GetComponent<BoatPartOption>() });
            
            var mizzenTSource1 = container.Find("backstay_1-0_top");
            Mast mizzenTopstay1 = Util.CopyMast(mizzenTSource1, new Vector3(-11, 27.5f, 0), mizzenTSource1.localEulerAngles, new Vector3(1, 1, 1.126f), "mizzen_topmast_stay_1-1", "mizzen topmast stay 1", 45);
            mizzenTopstay1.reefWinch = new GPButtonRopeWinch[] { Util.CopyWinch(mizzenTopstay1.reefWinch[0], new Vector3(-11.74f, 5.9f, 0)) };
            mizzenTopstay1.reefWinch[0].transform.localEulerAngles = new Vector3(0, 90, 0);
            mizzenTopstay1.leftAngleWinch = Util.CopyWinches(mizzenTopstay1.leftAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            mizzenTopstay1.rightAngleWinch = Util.CopyWinches(mizzenTopstay1.rightAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            mizzenTopstay1.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mainMast1.GetComponent<BoatPartOption>() };

            var mizzenTSource2 = container.Find("backstay_1-1_top");
            Mast mizzenTopstay2 = Util.CopyMast(mizzenTSource2, new Vector3(-7f, 27.65f, 0), "mizzen_topmast_stay_1-2", "mizzen topmast stay 2", 46);
            mizzenTopstay2.reefWinch = Util.CopyWinches(mizzenTopstay2.reefWinch, Vector3.zero, new Vector3(0f, 0.66f, 0));
            mizzenTopstay2.leftAngleWinch = mizzenTopstay1.leftAngleWinch;
            mizzenTopstay2.rightAngleWinch = mizzenTopstay1.rightAngleWinch;
            mizzenTopstay2.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mainMast1.GetComponent<BoatPartOption>() };

            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption>() { Util.CreatePartOption(container, "mizzen_topmast_stay_empty", "(no mizzen topmast stay)"), mizzenTopstay1.GetComponent<BoatPartOption>(), mizzenTopstay2.GetComponent<BoatPartOption>() });

            Transform topMForestaySrc = container.Find("forestay_0_top_shortsprit");
            Mast topmastForestay1 = Util.CopyMast(topMForestaySrc, new Vector3(7.5f, 31.4f, 0), new Vector3(305, 270, 90), new Vector3(1, 1, 1.276f), "forestay_topmast_0", "topmast forestay 1", 48);
            topmastForestay1.reefWinch = Util.CopyWinches(topmastForestay1.reefWinch, Vector3.zero, new Vector3(0f, 0, 0.57f));
            topmastForestay1.reefWinch[0].transform.localEulerAngles = new Vector3(0, 45, 90);
            topmastForestay1.reefWinch[1].transform.localEulerAngles = new Vector3(0, 45, 90);
            topmastForestay1.leftAngleWinch = Util.CopyWinches(topmastForestay1.leftAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            topmastForestay1.rightAngleWinch = Util.CopyWinches(topmastForestay1.rightAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            topmastForestay1.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { bowspritLong.GetComponent<BoatPartOption>() };
            topmastForestay1.mastHeight = 24;

            Transform topMForestaySrc2 = container.Find("forestay_1_top_shortsprit");
            Mast topmastForestay2 = Util.CopyMast(topMForestaySrc2, new Vector3(11.67f, 30.363f, 0), new Vector3(298.7f, 270, 90), new Vector3(1, 1, 1.33f), "forestay_topmast_1", "topmast forestay 2", 49);
            topmastForestay2.reefWinch = Util.CopyWinches(topmastForestay2.reefWinch, Vector3.zero, new Vector3(0f, 0, 0.57f));
            topmastForestay2.reefWinch[0].transform.localEulerAngles = new Vector3(0, 45, 90);
            topmastForestay2.reefWinch[1].transform.localEulerAngles = new Vector3(0, 45, 90);
            topmastForestay2.leftAngleWinch = Util.CopyWinches(topmastForestay2.leftAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            topmastForestay2.rightAngleWinch = Util.CopyWinches(topmastForestay2.rightAngleWinch, Vector3.zero, new Vector3(0.5f, 0, 0));
            topmastForestay2.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { bowspritLong.GetComponent<BoatPartOption>() };
            topmastForestay2.mastHeight = 20.7f;

            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { Util.CreatePartOption(container, "topmast_forestay_none", "(no outer forestay)"), topmastForestay1.GetComponent<BoatPartOption>(), topmastForestay2.GetComponent<BoatPartOption>() });

            #endregion

            #region raked foremast
            Mast foreMast2M = foreMast2.GetComponent<Mast>();
            Mast foremastRaked = Util.CopyMast(mizzenMast1, new Vector3(16.4f, 0, 18.7f), new Vector3(0, 20, 0), mizzenMast1.localScale, "mast_F_raked", "raked foremast", 47);
            foremastRaked.reefWinch = foreMast2M.reefWinch;
            foremastRaked.leftAngleWinch = foreMast2M.leftAngleWinch;
            foremastRaked.rightAngleWinch = foreMast2M.rightAngleWinch;
            foremastRaked.midAngleWinch = foreMast2M.midAngleWinch;
            foremastRaked.transform.GetChild(0).localPosition = new Vector3(-1.47f, 0, -24.9f);
            foremastRaked.transform.GetChild(0).localScale = new Vector3(1, 1, 0.65f);
            #region shrouds
            Transform foreMast3Shrouds = UnityEngine.Object.Instantiate(newCont1.transform, foremastRaked.transform, false);
            foreMast3Shrouds.localPosition = new Vector3(-15.2f, 0, -25.6f);
            foreMast3Shrouds.localEulerAngles = new Vector3(0, 9, 0);
            foreMast3Shrouds.localScale = new Vector3(-1, 1.09f, 1);
            foreMast3Shrouds.GetChild(0).localPosition = new Vector3(-11.83f, 0, 19.9f);
            foreMast3Shrouds.GetChild(0).localEulerAngles = new Vector3(0, 5, 90);
            foreMast3Shrouds.GetChild(0).localScale = new Vector3(0.8f, 0.8f, 1f);
            foreMast3Shrouds.GetChild(1).localPosition = new Vector3(-6.4f, 0, 0.11f);
            foreMast3Shrouds.GetChild(1).localEulerAngles = new Vector3(0, 10, 0);
            Transform foreMast3ShroudsCol = UnityEngine.Object.Instantiate(walkCol1.transform, foremastRaked.walkColMast, false);
            partsList.availableParts[1].partOptions.Add(foremastRaked.GetComponent<BoatPartOption>());
            foreMast3ShroudsCol.localPosition = foreMast3Shrouds.localPosition;
            foreMast3ShroudsCol.localEulerAngles = foreMast3Shrouds.localEulerAngles;
            foreMast3ShroudsCol.localScale = foreMast3Shrouds.localScale;
            foreMast3ShroudsCol.GetChild(0).localPosition = foreMast3Shrouds.GetChild(0).localPosition;
            foreMast3ShroudsCol.GetChild(0).localEulerAngles = foreMast3Shrouds.GetChild(0).localEulerAngles;
            foreMast3ShroudsCol.GetChild(0).localScale = foreMast3Shrouds.GetChild(0).localScale;
            foreMast3ShroudsCol.GetChild(1).localPosition = foreMast3Shrouds.GetChild(1).localPosition;
            foreMast3ShroudsCol.GetChild(1).localEulerAngles = foreMast3Shrouds.GetChild(1).localEulerAngles;
            /*var testCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            testCapsule.transform.parent = foreMast3Shrouds;
            testCapsule.transform.localPosition = new Vector3(-12.5f, 1.8f, 14);
            testCapsule.transform.localEulerAngles = new Vector3(80.3f, 185, 180);
            testCapsule.GetComponent<CapsuleCollider>().radius = 0.25f;
            testCapsule.GetComponent<CapsuleCollider>().height = 16f;
            var testCapsule2 = UnityEngine.Object.Instantiate(testCapsule, foreMast3Shrouds, false);
            testCapsule2.transform.localPosition = new Vector3(-12.5f, -1.8f, 14);
            testCapsule2.transform.localEulerAngles = new Vector3(279, 185, 180);
            foremastRaked.mastCols = foremastRaked.mastCols.AddRangeToArray(new CapsuleCollider[] { testCapsule.GetComponent<CapsuleCollider>(), testCapsule2.GetComponent<CapsuleCollider>() });
            */
            #endregion
            #endregion

            partsList.StartCoroutine(AddCopiedPart(structure,
                walkCol, 
                partsList, 
                topmast1Reef, 
                topmast1LeftAngle, 
                topmast1RightAngle, 
                topmast0Reef, 
                topmast0Part, 
                topmast1Part, 
                topmast01Reef, 
                topmast11Reef,
                topmast0Angle,
                topmast1Angle,
                topmast2Reef,
                topmast2Angle,
                topmast2LeftAngle,
                topmast2RightAngle,
                topmast21Reef,
                topmast2Part));
        }

        private static IEnumerator AddCopiedPart(Transform parent,
            Transform walkCol,
            BoatCustomParts partsList,
            GPButtonRopeWinch[] extra0,
            GPButtonRopeWinch[] extra1,
            GPButtonRopeWinch[] extra2,
            GPButtonRopeWinch[] extra3,
            BoatPart topmast0Part,
            BoatPart topmast1Part,
            GPButtonRopeWinch[] extra4,
            GPButtonRopeWinch[] extra5,
            GPButtonRopeWinch[] extra6,
            GPButtonRopeWinch[] extra7,            
            GPButtonRopeWinch[] extra8,
            GPButtonRopeWinch[] extra9,           
            GPButtonRopeWinch[] extra10,
            GPButtonRopeWinch[] extra11,
            GPButtonRopeWinch[] extra12,
            BoatPart topmast2Part)
        {
            Debug.Log("trying to add part");
            var mainMast1 = parent.Find("mast_Back_0");
            var mainMast2 = parent.Find("mast_Back_1");
            var foreMast1 = parent.Find("mast_Front_0");
            var foreMast2 = parent.Find("mast_Front_1");
            var mizzenMast1 = parent.Find("mast_mizzen_0");
            var mizzenMast2 = parent.Find("mast_mizzen_1");


            yield return new WaitUntil(() => PartRefs.cog != null && PartRefs.sanbuq != null);
            mast_001 = UnityEngine.Object.Instantiate(PartRefs.cog.Find("structure").Find("mast_001"), parent.Find("sprit_topmast"), false);
            mast_001.localPosition = new Vector3(0, 0, 1.6f);
            mast_001.localScale = new Vector3(0.4f, 0.4f, 0.5f);
            mast_001.localEulerAngles = Vector3.zero;
            mast_001.GetComponent<Renderer>().material.color = new Color(0.875f, 1, 1);

            var mast2 = UnityEngine.Object.Instantiate(mast_001, spritTopmast2.transform, false);

            Mast topmast0 = Util.CopyMast(PartRefs.sanbuq.Find("structure").Find("mast_0_extension"), parent, walkCol, new Vector3(6.7f, 0, 30.8f), Vector3.zero, new Vector3(1.2f, 1.2f, 0.75f), "mast_f0_extension_0", "fore topmast 1", 35);
            topmast0.leftAngleWinch = new GPButtonRopeWinch[] { parent.parent.Find("rope_winch_sqfront_angle_left_mod").GetComponent<GPButtonRopeWinch>()};
            topmast0.rightAngleWinch = new GPButtonRopeWinch[] { parent.parent.Find("rope_winch_sqfront_angle_right_mod").GetComponent<GPButtonRopeWinch>() };
            topmast0.reefWinch = extra3;
            topmast0.midAngleWinch = extra6;
            topmast0.midRopeAtt = foreMast1.GetComponent<Mast>().midRopeAtt;
            var foreFlag0 = foreMast1.Find("wind_flag");
            var topmast0Flag = UnityEngine.Object.Instantiate(foreFlag0, topmast0.transform, false);
            topmast0Flag.transform.localPosition = new Vector3(0.2f, 0, -0.2f);
            topmast0Part.partOptions.Add(topmast0.GetComponent<BoatPartOption>());
            topmast0.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foreMast1.GetComponent<BoatPartOption>() };
            parent.parent.Find("forestay_topmast_0").GetComponent<BoatPartOption>().requires.Add(topmast0.GetComponent<BoatPartOption>());
            topmast0.transform.Find("flagT").gameObject.SetActive(false);

            Mast topmast01 = Util.CopyMast(topmast0.transform, new Vector3(10.85f, 0, 29.9f), "mast_f1_extension_1", "fore topmast 2", 37);
            topmast01.reefWinch = extra4;
            topmast01.midRopeAtt = foreMast2.GetComponent<Mast>().midRopeAtt;
            var foreFlag1 = parent.Find("mast_Front_1").Find("wind_flag_002");
            topmast0Part.partOptions[0].childOptions = new GameObject[] { foreFlag0.gameObject, foreFlag1.gameObject };
            topmast0Part.partOptions.Add(topmast01.GetComponent<BoatPartOption>());
            topmast01.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foreMast2.GetComponent<BoatPartOption>() };
            parent.parent.Find("forestay_topmast_1").GetComponent<BoatPartOption>().requires.Add(topmast01.GetComponent<BoatPartOption>());

            Mast topmast1 = Util.CopyMast(topmast0.transform, new Vector3(-5.4f, 0, 31.9f), "mast_m0_extension_1", "main topmast 1", 36);
            topmast1.reefWinch = extra0;
            topmast1.leftAngleWinch = extra1;
            topmast1.rightAngleWinch = extra2;
            topmast1.midAngleWinch = extra7;
            topmast1.midRopeAtt = mainMast1.GetComponent<Mast>().midRopeAtt;
            var mainFlag0 = mainMast1.Find("wind_flag_001");
            topmast1Part.partOptions.Add(topmast1.GetComponent<BoatPartOption>());
            topmast1.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mainMast1.GetComponent<BoatPartOption>() };

            parent.parent.Find("mainstay_topmast_1-1").GetComponent<BoatPartOption>().requires.Add(topmast1.GetComponent<BoatPartOption>());
            parent.parent.Find("mainstay_topmast_2-1").GetComponent<BoatPartOption>().requires.Add(topmast1.GetComponent<BoatPartOption>());

            Mast topmast11 = Util.CopyMast(topmast1.transform, new Vector3(-0.6f, 0, 31.9f), "mast_m1_extension_1", "main topmast 2", 38);
            topmast11.reefWinch = extra5;
            topmast11.midRopeAtt = mainMast2.GetComponent<Mast>().midRopeAtt;
            var mainFlag1 = mainMast2.Find("wind_flag_003");
            topmast1Part.partOptions[0].childOptions = new GameObject[] { mainFlag0.gameObject, mainFlag1.gameObject };
            topmast1Part.partOptions.Add(topmast11.GetComponent<BoatPartOption>());
            topmast11.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mainMast2.GetComponent<BoatPartOption>() };

            parent.parent.Find("mainstay_topmast_1-2").GetComponent<BoatPartOption>().requires.Add(topmast11.GetComponent<BoatPartOption>());
            parent.parent.Find("mainstay_topmast_2-2").GetComponent<BoatPartOption>().requires.Add(topmast11.GetComponent<BoatPartOption>());

            Mast topmast2 = Util.CopyMast(topmast1.transform, new Vector3(-11.97f, 0, 27f), Vector3.zero, new Vector3(1.1f, 1.1f, 0.75f), "mast_miz0_extension_1", "mizzen topmast 1", 39);
            topmast2.reefWinch = extra8;
            topmast2.midAngleWinch = extra9;
            topmast2.leftAngleWinch = extra10;
            topmast2.rightAngleWinch = extra11;
            topmast2.midRopeAtt = mizzenMast1.GetComponent<Mast>().midRopeAtt;
            topmast2Part.partOptions.Add(topmast2.GetComponent<BoatPartOption>());
            topmast2.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzenMast1.GetComponent<BoatPartOption>() };

            parent.parent.Find("mizzen_topmast_stay_1-1").GetComponent<BoatPartOption>().requires.Add(topmast2.GetComponent<BoatPartOption>());

            Mast topmast21 = Util.CopyMast(topmast2.transform, new Vector3(-7.95f, 0, 27f), "mast_miz1_extension_1", "mizzen topmast 2", 40);
            topmast21.reefWinch = extra12;
            topmast21.midRopeAtt = mizzenMast2.GetComponent<Mast>().midRopeAtt;
            topmast2Part.partOptions.Add(topmast21.GetComponent<BoatPartOption>());
            topmast21.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzenMast2.GetComponent<BoatPartOption>() };

            parent.parent.Find("mizzen_topmast_stay_1-2").GetComponent<BoatPartOption>().requires.Add(topmast21.GetComponent<BoatPartOption>());

            //yield return new WaitUntil(() => PartRefs.cogCol != null && PartRefs.sanbuqCol != null);
            var mast_001_col_parent = UnityEngine.Object.Instantiate(new GameObject(), walkCol).transform;
            mast_001_col = UnityEngine.Object.Instantiate(PartRefs.cogCol.Find("structure").Find("mast_001"), mast_001_col_parent, false);
            mast_001_col.localPosition = mast_001.localPosition;
            mast_001_col.localScale = mast_001.localScale;
            mast_001_col.localEulerAngles = mast_001.localEulerAngles;
            mast_001_col_parent.localPosition = spritTopmast1.transform.localPosition;

            var mastCol2 = UnityEngine.Object.Instantiate(mast_001_col_parent, walkCol, false);
            mastCol2.localPosition = spritTopmast2.transform.localPosition;

            spritTopmast1.walkColMast = mast_001_col_parent;
            spritTopmast1.GetComponent<BoatPartOption>().walkColObject.SetActive(false);
            spritTopmast1.GetComponent<BoatPartOption>().walkColObject = mast_001_col_parent.gameObject;

            spritTopmast2.walkColMast = mastCol2;
            spritTopmast2.GetComponent<BoatPartOption>().walkColObject.SetActive(false);
            spritTopmast2.GetComponent<BoatPartOption>().walkColObject = mastCol2.gameObject;

            
            partsList.RefreshParts();

        }
    }
}
