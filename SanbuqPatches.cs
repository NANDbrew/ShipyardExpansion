﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class SanbuqPatches
    {
        [HarmonyPatch(typeof(SaveableBoatCustomization))]
        internal static class PartsPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Adder(SaveableBoatCustomization __instance, BoatCustomParts ___parts)
            {

                if (__instance.name != "BOAT dhow medium (20)") return;

                Transform container = __instance.transform.Find("dhow medium new");
                Transform structure = container.Find("structure");
                Transform mainMast1 = structure.Find("mast");
                Transform mainMast2 = structure.Find("mast_1");
                Transform mizzenMast1 = structure.Find("mast_002");


                // move main mast aft to make room
                Util.MoveMast(mainMast2, mainMast2.localPosition + new Vector3(-0.2f, 0, 0), true);
                Util.MoveMast(structure.Find("mast_1_extension"), structure.Find("mast_1_extension").localPosition + new Vector3(-0.2f, 0, 0), true);

                BoatPartOption mainMastNone = Util.CreatePartOption(container, "(empty mainmast)", "(no mainmast)");
                ___parts.availableParts[0].partOptions.Add(mainMastNone);

                BoatPartOption foremastNone = Util.CreatePartOption(container, "(empty foremast)", "(no foremast");

                BoatPartOption forestayNone = Util.CreatePartOption(container, "(empty forestay)", "(no forestay)");
                ___parts.availableParts[3].partOptions.Add(forestayNone);
                ___parts.availableParts[3].activeOption = ___parts.availableParts[3].partOptions.Count - 1;

                BoatPartOption bowspritNone = Util.CreatePartOption(container, "(empty bowsprit)", "(no bowsprit)");
                ___parts.availableParts[2].partOptions.Add(bowspritNone);
                ___parts.availableParts[2].activeOption = ___parts.availableParts[2].partOptions.Count - 1;


                /*var foremast = Util.CopyMast(mizzenMast1, new Vector3(8.877f, 0, 13.1f), Vector3.zero, mizzenMast1.localScale, "mast_004", "foremast");
                foremast.transform.Find("Cylinder_006").gameObject.SetActive(false);
                foremast.reefWinch = Util.CopyWinches(foremast.reefWinch, mizzenMast1.localPosition, foremast.transform.localPosition);*/
                var foremast = Util.CopyMast(mainMast1, new Vector3(mainMast1.localPosition.x + 4, mainMast1.localPosition.y, mainMast1.localPosition.z), "mast_04", "foremast", 29);
                foremast.reefWinch = Util.CopyWinches(foremast.reefWinch, mainMast1.localPosition, foremast.transform.localPosition);
                foremast.rightAngleWinch = Util.CopyWinches(foremast.rightAngleWinch, foremast.rightAngleWinch[0].transform.localPosition, new Vector3(3.6f, 1.98f, -2.27f));
                foremast.leftAngleWinch = Util.CopyWinches(foremast.leftAngleWinch, foremast.leftAngleWinch[0].transform.localPosition, new Vector3(3.6f, 1.98f, 2.27f));
                foremast.midAngleWinch = Util.CopyWinches(foremast.midAngleWinch, mainMast1.localPosition, foremast.transform.localPosition);
                foremast.midAngleWinch[0].transform.localPosition = new Vector3(-1.87f, 2.5f, -0.8f);
                foremast.midAngleWinch[1].transform.localPosition = new Vector3(-1.87f, 2.5f, 0.8f);
                foremast.midAngleWinch[2].transform.localPosition = new Vector3(-1.8f, 2.51f, -0.37f);
                foremast.midRopeAtt[0].parent.localPosition = new Vector3(-6.6f, -0.05f, -14.66f);
                //foremast.orderIndex = 29;
                //foremast.Awake();

                BoatPart foremastPart = new BoatPart {
                    partOptions = new List<BoatPartOption>(2) { foremastNone, foremast.GetComponent<BoatPartOption>() },
                    category = 0,
                    activeOption = 0 };
                ___parts.availableParts.Add(foremastPart);


                var source1 = structure.Find("part_shrouds_main_back").GetChild(0).gameObject;
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
                shrouds_back_opt.childOptions = new GameObject[0];

                var source2 = structure.Find("part_shrouds_main_side").GetChild(0).gameObject;
                GameObject shrouds_side = UnityEngine.GameObject.Instantiate(source2, foremast.transform);
                shrouds_side.transform.localPosition = new Vector3(0f, 0f, 1.48f);
                shrouds_side.transform.localEulerAngles = new Vector3(6.2f, 174.6f, 348f);
                shrouds_side.transform.localScale = new Vector3(1.1f, 1.1f, 1.09f);
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
                BoatPart shrouds = new BoatPart()
                {
                    partOptions = new List<BoatPartOption>() { shrouds_side_opt, shrouds_back_opt },
                    category = 1,
                    activeOption = 0
                };

                /*var shrouds_side = foremast.transform.Find("part_shrouds_mizzen_side");
                shrouds_side.GetComponent<BoatPartOption>().optionName = "fore shrouds 1";
                shrouds_side.localPosition = new Vector3(-0.98f, -0.6f, 3.66f);
                shrouds_side.localEulerAngles = new Vector3(4.6f, 359.2f, 14f);
                shrouds_side.localScale = new Vector3(-1f, 0.9f, 1f);
                shrouds_side.GetChild(0).localPosition = new Vector3(5.7288f, 0f, -16.8f);
                shrouds_side.GetChild(0).localScale = new Vector3(1.1f, 1.1f, 1f);
                var shrouds_back = foremast.transform.Find("part_shrouds_mizzen_back");
                shrouds_side.GetComponent<BoatPartOption>().optionName = "fore shrouds 2";
                shrouds_back.localPosition = new Vector3(1.38f, 0f, 3.61f);
                shrouds_back.localScale = new Vector3(1.1f, 0.86f, 1.135f);
                shrouds_back.GetChild(0).localPosition = new Vector3(5.2f, 0, -15.1f);
                shrouds_back.GetChild(0).localScale = new Vector3(1f, 1f, 0.85f);

                BoatPart shrouds = new BoatPart() {
                    partOptions = new List<BoatPartOption>() { 
                        foremast.transform.Find("part_shrouds_mizzen_side").GetComponent<BoatPartOption>(), 
                        foremast.transform.Find("part_shrouds_mizzen_back").GetComponent<BoatPartOption>() },
                    category = 1,
                    activeOption = 0 };*/
                ___parts.availableParts.Add(shrouds);

                Mast mizzen2 = Util.CopyMast(mainMast2, mainMast2.localPosition + new Vector3(-7, 0, 0), "mast_003", "mizzen mast 2", 25);
                mizzen2.reefWinch = Util.CopyWinches(mizzen2.reefWinch, mainMast2.localPosition, mizzen2.transform.localPosition + new Vector3(0, 0.6f, 0));
                UnityEngine.Object.Instantiate(mizzenMast1.transform.Find("Cylinder_006"), mizzen2.transform, true);
                mizzen2.leftAngleWinch = mizzenMast1.GetComponent<Mast>().leftAngleWinch;
                mizzen2.rightAngleWinch = mizzenMast1.GetComponent<Mast>().rightAngleWinch;
                //mizzen2.orderIndex = 25;
                //mizzen2.Awake();
                ___parts.availableParts[1].partOptions.Add(mizzen2.GetComponent<BoatPartOption>());

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
                ___parts.availableParts[6].partOptions = new List<BoatPartOption>() { mizzenShrouds2, mizzenShrouds };

                UnityEngine.Object.Destroy(mizShrBackOld.GetComponent<BoatPartOption>());
                UnityEngine.Object.Destroy(mizShrSideOld.GetComponent<BoatPartOption>());




                var src3 = container.Find("forestay_0_short");
                Mast forestayOuter = Util.CopyMast(src3, src3.localPosition + new Vector3(4, 0, 0), new Vector3(311, 270, 90), new Vector3(1, 1, 0.94f), "foremast_stay", "foremast stay", 28);
                //Mast forestayOuter = Util.CopyMast(container.Find("forestay_0_long"), new Vector3(9.3f, 14.6f, 0f), new Vector3(319, 270, 90), new Vector3(1, 1, 0.76f), "foremast_stay", "foremast stay");

                forestayOuter.reefWinch = Util.CopyWinches(forestayOuter.reefWinch, src3.localPosition, forestayOuter.transform.localPosition);
                BoatPartOption forestayOuterOpt = forestayOuter.GetComponent<BoatPartOption>();
                forestayOuterOpt.requires = new List<BoatPartOption> {
                    foremast.GetComponent<BoatPartOption>(),
                    container.Find("part_bowsprit_long_gfx").GetComponent<BoatPartOption>() };
                forestayOuter.mastReefAtt[0] = foremast.transform.Find("rope_holder_jibs_front").Find("att");
                forestayOuter.mastReefAtt[1] = foremast.transform.Find("rope_holder_jibs_front").Find("att");
                /*var ropeHolder = UnityEngine.Object.Instantiate(mainMast1.transform.Find("rope_holder_jibs_front"), foremast.transform);
                forestayOuter.mastReefAtt[0] = ropeHolder.GetChild(0);
                forestayOuter.mastReefAtt[1] = ropeHolder.GetChild(0);*/
                //forestayOuter.GetComponent<Mast>().orderIndex = 28;
                //forestayOuter.GetComponent<Mast>().Awake();
                ___parts.availableParts[3].partOptions.Add(forestayOuterOpt);

                var src4 = container.Find("forestay_0_short_inner");
                Mast forestayInner = Util.CopyMast(src4, new Vector3(9.53f, 13f, 0), new Vector3(311, 270, 90), src4.transform.localScale, "foremast_stay_inner", "lower foremast stay", 27);
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
                ___parts.availableParts[4].partOptions.Add(forestayInnerOpt);


                var src5 = container.Find("part_stay_mid_0");
                var middlestay_2 = Util.CopyMast(src5, new Vector3(-6.55f, 17.7f, 0), new Vector3(315.8f, 270, 90), new Vector3(1, 1, 0.83f), "part_stay_mid_2", "middlestay 1-2", 26);
                middlestay_2.reefWinch = Util.CopyWinches(middlestay_2.reefWinch, src5.localPosition, new Vector3(-6.55f, src5.localPosition.y, 0));
                middlestay_2.transform.GetChild(0).localScale = new Vector3(3.6f, 3.24f, 3.8f);
                middlestay_2.transform.GetChild(1).localScale = new Vector3(2.4f, 2.2f, 2.4f);
                middlestay_2.mastHeight = 15;
                //middlestay_2.orderIndex = 26;
                //middlestay_2.Awake();
                middlestay_2.GetComponent<BoatPartOption>().requires[1] = mizzen2.GetComponent<BoatPartOption>();
                ___parts.availableParts[8].partOptions.Add(middlestay_2.GetComponent<BoatPartOption>());

                var middlestay_3 = Util.CopyMast(src5, new Vector3(-9.86f, 14.57f, 0), new Vector3(312f, 270, 90), new Vector3(1, 1, 0.75f), "part_stay_mid_3", "middlestay 2-1", 24);
                middlestay_3.reefWinch = Util.CopyWinches(middlestay_3.reefWinch, src5.localPosition, new Vector3(-6.55f, src5.localPosition.y, 0));
                middlestay_3.transform.GetChild(0).localScale = new Vector3(4.5f, 3.96f, 4.5f);
                middlestay_3.transform.GetChild(1).localScale = new Vector3(2.28f, 1.99f, 2.33f);
                middlestay_3.mastHeight = 14;
                //middlestay_3.orderIndex = 24;
                //middlestay_3.Awake();
                middlestay_3.GetComponent<BoatPartOption>().requires[0] = structure.Find("mast_1").GetComponent<BoatPartOption>();
                ___parts.availableParts[8].partOptions.Add(middlestay_3.GetComponent<BoatPartOption>());

                var middlestay_fore = Util.CopyMast(src5, new Vector3(0.3f, 17.7f, 0), new Vector3(308.7f, 270, 90), new Vector3(1, 1, 0.71f), "part_stay_mid_fore", "foreward middlestay", 23);
                middlestay_fore.reefWinch = Util.CopyWinches(middlestay_fore.reefWinch, src5.localPosition, new Vector3(mainMast2.localPosition.x, src5.localPosition.y, 0));
                middlestay_fore.leftAngleWinch = Util.CopyWinches(middlestay_fore.leftAngleWinch, src5.localPosition, new Vector3(src5.localPosition.x + 6, src5.localPosition.y, 0));
                middlestay_fore.rightAngleWinch = Util.CopyWinches(middlestay_fore.rightAngleWinch, src5.localPosition, new Vector3(src5.localPosition.x + 6, src5.localPosition.y, 0));
                middlestay_fore.transform.GetChild(0).localScale = new Vector3(4.5f, 3.96f, 4.5f);
                middlestay_fore.transform.GetChild(1).localScale = new Vector3(2.28f, 1.99f, 2.33f);
                middlestay_fore.mastHeight = 13;
                //middlestay_fore.orderIndex = 23;
                //middlestay_fore.Awake();
                middlestay_fore.GetComponent<BoatPartOption>().requires = new List<BoatPartOption>() {
                structure.Find("mast_1").GetComponent<BoatPartOption>(),
                foremast.GetComponent<BoatPartOption>() };
                BoatPart middlestayF = new BoatPart() {
                    partOptions = new List<BoatPartOption>() { Util.CreatePartOption(container, "(empty fore midstay)", "(no foreward middlestay"), middlestay_fore.GetComponent<BoatPartOption>() },
                    category = 2,
                    activeOption = 0 };
                ___parts.availableParts.Add(middlestayF);


                foreach (BoatPartOption stay in ___parts.availableParts[3].partOptions)
                {
                    if (stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                    else
                    {
                        stay.requiresDisabled.Add(foremast.GetComponent<BoatPartOption>());
                        stay.requiresDisabled.Add(bowspritNone);

                    }

                }
                foreach (BoatPartOption stay in ___parts.availableParts[4].partOptions)
                {
                    if (stay.optionName.StartsWith("topmast") || stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                    else
                    {
                        stay.requiresDisabled.Add(foremast.GetComponent<BoatPartOption>());

                    }

                }
            }
        }
    }
}
