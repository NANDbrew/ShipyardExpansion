using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            BoatPartOption noForemast = container.Find("(no foremast)").GetComponent<BoatPartOption>();
            BoatPartOption noBowsprit = container.Find("(no bowsprit)").GetComponent<BoatPartOption>();
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent;
            #region adjustments
            Util.MoveMast(mizzenMast1, new Vector3(-12.43f, mizzenMast1.localPosition.y, mizzenMast1.localPosition.z), true);
            Util.MoveMast(backstay1_0top, new Vector3(-11.4f, backstay1_0top.localPosition.y, backstay1_0top.localPosition.z), true);
            Util.MoveMast(backstay1_0bottom, new Vector3(-11.41f, 14.6f, backstay1_0bottom.localPosition.z), true);

            backstay1_0top.localScale = new Vector3(1, 1, 1.17f);
            backstay1_0bottom.localScale = new Vector3(1, 1, 1.17f);
            backstay1_0top.GetComponent<Mast>().mastHeight = 14;
            backstay1_0bottom.GetComponent<Mast>().mastHeight = 14;
            //var mizShroudsBack = UnityEngine.Object.Instantiate(new GameObject() { name = "shrouds_M_default" }, mizzenMast1);
            //BoatPartOption backOption = Util.CopyPartOption(container.Find("parts_shrouds_F_default").GetComponent<BoatPartOption>(), mizShroudsBack, "mizzen mast shrouds 1");
            //backOption.childOptions = new GameObject[0];
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

            mainstayInner.leftAngleWinch = Util.CopyWinches(mainstayOuter.leftAngleWinch, Vector3.zero, new Vector3(-0.67f, 0, 0.1f));
            mainstayInner.rightAngleWinch = Util.CopyWinches(mainstayOuter.rightAngleWinch, Vector3.zero, new Vector3(-0.67f, 0, -0.1f));


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
            flags_main.basePrice = 10;

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
            flags_fore.basePrice = 10;

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
            flags_mizzen.basePrice = 10;

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
            spritTopmast1.reefWinch = Util.CopyWinches(spritTopmast1.reefWinch, spritTopmast1.reefWinch[0].transform.localPosition, new Vector3(20.37f, 7.6f, 0));
            spritTopmast1.reefWinch[0].transform.localEulerAngles = new Vector3(0, 270, 0);
            spritTopmast1.leftAngleWinch = Util.CopyWinches(spritTopmast1.leftAngleWinch, spritTopmast1.leftAngleWinch[0].transform.localPosition, new Vector3(12.4f, 3.12f, 1.5f));
            spritTopmast1.rightAngleWinch = Util.CopyWinches(spritTopmast1.rightAngleWinch, spritTopmast1.rightAngleWinch[0].transform.localPosition, new Vector3(12.4f, 3.12f, -1.5f));

            var stmFlag = UnityEngine.Object.Instantiate(flagSource, spritTopmast1.transform);
            stmFlag.transform.localPosition = new Vector3(0.3f, 0, 1);

            BoatPartOption partOption = Util.CopyPartOption(bowsprit, spritTopmast1.gameObject, "sprit topmast 1");
            partOption.mass = 20;
            partOption.basePrice /= 2;
            partOption.installCost /= 2;


            spritTopmast2 = Util.CopyMast(spritTopmast1.transform, new Vector3(23.5f, 0, 10.9f), "sprit_topmast_2", "sprit topmast 2", 34);
            spritTopmast2.reefWinch = Util.CopyWinches(spritTopmast2.reefWinch, spritTopmast1.reefWinch[0].transform.localPosition, new Vector3(23.76f, 9.2f, 0));
            BoatPartOption spritTopmast2_opt = spritTopmast2.GetComponent<BoatPartOption>();
            partOption.requires = new List<BoatPartOption> { bowsprit };
            spritTopmast2_opt.requires = new List<BoatPartOption> { bowspritLong };

            BoatPartOption stmNone = Util.CreatePartOption(structure, "(no-sprit_topmast)", "(no sprit topmast)");
            Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { stmNone, partOption, spritTopmast2_opt });
            bowspritLong.transform.GetChild(0).localPosition += new Vector3(0, 0, -1f);
            bowsprit.transform.GetChild(0).localPosition += new Vector3(0, 0, -1f);
            partsList.StartCoroutine(AddCopiedPart(spritTopmast1.transform, walkCol));
            #endregion
        }

        private static IEnumerator AddCopiedPart(Transform parent, Transform walkCol)
        {
            Debug.Log("trying to add part");
            yield return new WaitUntil(() => Plugin.spritRef != null);
            mast_001 = UnityEngine.Object.Instantiate(Plugin.spritRef, parent, false);
            mast_001.localPosition = new Vector3(0, 0, 1.6f);
            mast_001.localScale = new Vector3(0.4f, 0.4f, 0.5f);
            mast_001.localEulerAngles = Vector3.zero;
            mast_001.GetComponent<MeshRenderer>().material.color = new Color(0.875f, 1, 1);

            var mast2 = UnityEngine.Object.Instantiate(mast_001, spritTopmast2.transform, false);
            //mast2.localPosition = spritTopmast2.transform.localPosition;

            yield return new WaitUntil(() => Plugin.spritColRef != null);
            var mast_001_col_parent = UnityEngine.Object.Instantiate(new GameObject(), walkCol).transform;
            mast_001_col = UnityEngine.Object.Instantiate(Plugin.spritColRef, mast_001_col_parent, false);
            mast_001_col.localPosition = mast_001.localPosition;
            mast_001_col.localScale = mast_001.localScale;
            mast_001_col.localEulerAngles = mast_001.localEulerAngles;
            mast_001_col_parent.localPosition = spritTopmast1.transform.localPosition;

            var mastCol2 = UnityEngine.Object.Instantiate(mast_001_col_parent, walkCol, false);
            mastCol2.localPosition = spritTopmast2.transform.localPosition;

            spritTopmast1.walkColMast = mast_001_col_parent;
            spritTopmast1.GetComponent<BoatPartOption>().walkColObject = mast_001_col_parent.gameObject;
            spritTopmast2.walkColMast = mastCol2;
            spritTopmast2.GetComponent<BoatPartOption>().walkColObject = mastCol2.gameObject;

        }
    }
}
