using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class BrigPatches
    {
        [HarmonyPatch(typeof(SaveableBoatCustomization))]
        internal static class PartsPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Adder(SaveableBoatCustomization __instance, BoatCustomParts ___parts)
            {
                if (__instance.name != "BOAT medi medium (50)") return;
                Transform container = __instance.transform.Find("medi medium new");
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

                #region adjustments
                Util.MoveMast(mizzenMast1, new Vector3(-12.43f, mizzenMast1.localPosition.y, mizzenMast1.localPosition.z), true);
                Util.MoveMast(backstay1_0top, new Vector3(-11.4f, backstay1_0top.localPosition.y, backstay1_0top.localPosition.z), true);
                Util.MoveMast(backstay1_0bottom, new Vector3(-11.41f, backstay1_0bottom.localPosition.y, backstay1_0bottom.localPosition.z), true);

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

                BoatPart shrouds = new BoatPart() {
                    partOptions = new List<BoatPartOption>() { backOption, sideOption },
                    category = 1,
                    activeOption = 0 };
                ___parts.availableParts.Add(shrouds);
                Plugin.modParts.Add(shrouds);
                #endregion

                #region longstays
                Vector3 newPos = new Vector3(mainMast2.localPosition.x, foreMast1.localPosition.y, foreMast1.localPosition.z);
                Transform stayAtt = mainMast2.Find("rope_holder_005").Find("stay att");
                Transform stayAtt2 = UnityEngine.Object.Instantiate(stayAtt.parent, mainMast2).GetChild(0);
                stayAtt2.parent.localPosition = stayAtt.parent.localPosition + new Vector3(0f, 0f, -6.6f);

                var mainstayOuter = Util.CopyMast(forestay_top_src, new Vector3(0f, 25.78f, 0f), forestay_top_src.localEulerAngles, new Vector3(1f, 1f, 1.18f), "mainstay_top", "forestay 3", 28);
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
                ___parts.availableParts[4].partOptions.Add(mainstayOuterOpt);
                Plugin.modPartOptions.Add(mainstayOuterOpt);

                var mainstayInner = Util.CopyMast(forestay_0_mid, new Vector3(0f, 19.44f, 0f), forestay_0_mid.localEulerAngles, new Vector3(1f, 1f, 1.18f), "mainstay_bottom", "lower forestay 3", 29);
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
                ___parts.availableParts[5].partOptions.Add(mainstayInner.GetComponent<BoatPartOption>());
                Plugin.modPartOptions.Add(mainstayInnerOpt);
                #endregion

                #region telltale
                var flagSource = structure.Find("mast_Front_0").Find("wind_flag");
                //flagSource.localScale = new Vector3(0.8f, 1f, 0.5f);
                //flagSource.GetComponent<MeshRenderer>().material.color = Color.green;
                BoatPartOption noFlag = Util.CreatePartOption(container, "(flag empty)", "(no telltale)");

                BoatPartOption flags_main = Util.CreatePartOption(container, "flag_main", "main mast telltale");

                Transform flags_main_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_0" }.transform, flags_main.transform);
                var flag_main_0_side = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
                flag_main_0_side.name = "flag_main_0_side";
                flag_main_0_side.localPosition = new Vector3(-5.24f, 8f, 3.19f);
                flag_main_0_side.localEulerAngles = new Vector3(87, 0, 0);
                var flag_main_0_back = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
                flag_main_0_back.name = "flag_main_0_back";
                flag_main_0_back.localPosition = new Vector3(-6.53f, 8f, 2.9f);
                flag_main_0_back.localEulerAngles = new Vector3(79, 340, 0);

                Transform flags_main_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_1" }.transform, flags_main.transform);
                var flag_main_1_side = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
                flag_main_1_side.name = "flag_main_1_side";
                flag_main_1_side.localPosition = new Vector3(-0.53f, 8f, 3.22f);
                flag_main_1_side.localEulerAngles = new Vector3(86, 0, 0);
                var flag_main_1_back = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
                flag_main_1_back.name = "flag_main_1_back";
                flag_main_1_back.localPosition = new Vector3(-1.62f, 8f, 2.76f);
                flag_main_1_back.localEulerAngles = new Vector3(80, 340, 0);


                BoatPartOption flags_fore = Util.CreatePartOption(container, "flag_fore", "fore mast telltale");

                Transform flags_fore_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_fore_0" }.transform, flags_fore.transform);
                var flag_fore_0_side = UnityEngine.Object.Instantiate(flagSource, flags_fore_0);
                flag_fore_0_side.name = "flag_fore_0_side";
                flag_fore_0_side.localPosition = new Vector3(6.62f, 8f, 2.76f);
                flag_fore_0_side.localEulerAngles = new Vector3(87, 0, 0);
                var flag_fore_0_back = UnityEngine.Object.Instantiate(flagSource, flags_fore_0);
                flag_fore_0_back.name = "flag_fore_0_back";
                flag_fore_0_back.localPosition = new Vector3(5.53f, 8f, 2.9f);
                flag_fore_0_back.localEulerAngles = new Vector3(79, 340, 0);

                Transform flags_fore_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_fore_1" }.transform, flags_fore.transform);
                var flag_fore_1_side = UnityEngine.Object.Instantiate(flagSource, flags_fore_1);
                flag_fore_1_side.name = "flag_fore_1_side";
                flag_fore_1_side.localPosition = new Vector3(11.02f, 8f, 2.27f);
                flag_fore_1_side.localEulerAngles = new Vector3(89, 0, 0);
                var flag_fore_1_back = UnityEngine.Object.Instantiate(flagSource, flags_fore_1);
                flag_fore_1_back.name = "flag_fore_1_back";
                flag_fore_1_back.localPosition = new Vector3(9.91f, 8f, 2.16f);
                flag_fore_1_back.localEulerAngles = new Vector3(81, 326, 0);


                BoatPartOption flags_mizzen = Util.CreatePartOption(container, "flag_mizzen", "mizzen mast telltale");

                Transform flags_mizzen_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_0" }.transform, flags_mizzen.transform);
                var flag_mizzen_0_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
                flag_mizzen_0_side.name = "flag_mizzen_0_side";
                flag_mizzen_0_side.localPosition = new Vector3(-11.8f, 8f, 2.7f);
                flag_mizzen_0_side.localEulerAngles = new Vector3(87, 0, 0);
                var flag_mizzen_0_back = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
                flag_mizzen_0_back.name = "flag_mizzen_0_back";
                flag_mizzen_0_back.localPosition = new Vector3(-12.24f, 8f, 2.45f);
                flag_mizzen_0_back.localEulerAngles = new Vector3(80, 340, 0);

                Transform flags_mizzen_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_1" }.transform, flags_mizzen.transform);
                var flag_mizzen_1_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
                flag_mizzen_1_side.name = "flag_mizzen_1_side";
                flag_mizzen_1_side.localPosition = new Vector3(-7.65f, 8f, 3.15f);
                flag_mizzen_1_side.localEulerAngles = new Vector3(87, 0, 0);
                var flag_mizzen_1_back = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
                flag_mizzen_1_back.name = "flag_mizzen_1_back";
                flag_mizzen_1_back.localPosition = new Vector3(-8.15f, 8f, 2.76f);
                flag_mizzen_1_back.localEulerAngles = new Vector3(80, 350, 0);

                //UnityEngine.Object.Destroy(flagSource);

                flags_fore.requiresDisabled.Add(noForemast);
                flags_main.requiresDisabled.Add(container.Find("(no mainmast)").GetComponent<BoatPartOption>());
                flags_mizzen.requiresDisabled.Add(container.Find("(no mizzen mast)").GetComponent<BoatPartOption>());
                BoatPart flag = new BoatPart()
                {
                    partOptions = new List<BoatPartOption>() { noFlag, flags_fore, flags_main, flags_mizzen },
                    category = 1,
                    activeOption = 0
                };
                ___parts.availableParts.Add(flag);
                Plugin.modParts.Add(flag);

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
                if (!Plugin.modCustomParts.Contains(___parts)) Plugin.modCustomParts.Add(___parts); //add boat to list of modified boats
            }
        }
    }
}
