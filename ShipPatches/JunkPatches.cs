using cakeslice;
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
    internal class JunkPatches
    {

        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            //Mast[] masts = __instance.GetComponent<BoatRefs>().masts;
            Transform container = boat.Find("junk medium (actual)");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast_mid_0");
            Transform mainMast2 = structure.Find("mast_mid_1");
            Transform mizzenMast1 = structure.Find("mast_mizzen_0");
            Transform mizzenMast2 = structure.Find("mast_mizzen_1");
            Transform foremast = structure.Find("mast_front_");
            Mast foremastM = foremast.GetComponent<Mast>();
            Transform bowsprit = structure.Find("mast_bowsprit");
            Transform walkColStruct = mainMast1.GetComponent<Mast>().walkColMast.parent;
            Transform walkCol = walkColStruct.parent;
            Mast mizzenMast1M = mizzenMast1.GetComponent<Mast>();
            Mast mizzenMast2M = mizzenMast2.GetComponent<Mast>();
            var midstaySource = container.Find("midstay_1-1");
            Transform forestay = container.Find("forestay_1");
            var forestaySource = container.Find("forestay_foremast");
            var forestay1Lower = container.Find("forestay_1_lower");
            var forestay0Lower = container.Find("forestay_0_lower");
            var forestay0 = container.Find("forestay_0");
            var forestayMid = container.Find("forestay_0_mid");


            Transform ropeExtMain1 = UnityEngine.Object.Instantiate(mainMast1.GetComponent<Mast>().mastReefAttExtension[0].parent, mainMast1);
            ropeExtMain1.localPosition = new Vector3(ropeExtMain1.localPosition.x, -ropeExtMain1.localPosition.y, ropeExtMain1.localPosition.z);
            ropeExtMain1.localEulerAngles = new Vector3(50, 270, 172);
            Transform ropeExtMain2 = UnityEngine.Object.Instantiate(mainMast2.GetComponent<Mast>().mastReefAttExtension[0].parent, mainMast2);
            ropeExtMain2.localPosition = new Vector3(ropeExtMain2.localPosition.x, -ropeExtMain2.localPosition.y, ropeExtMain2.localPosition.z);
            ropeExtMain2.localEulerAngles = new Vector3(50, 270, 172);



            PartRefs.junk = container;
            PartRefs.junkCol = walkCol;

            #region adjustments
            Transform ropeExtFore1 = UnityEngine.Object.Instantiate(ropeExtMain2, foremast, true);
            ropeExtFore1.localPosition = new Vector3(ropeExtMain2.localPosition.x, ropeExtMain2.localPosition.y, ropeExtFore1.localPosition.z);

            Transform[] main2ExtList = new Transform[2] { ropeExtMain2.GetChild(0), ropeExtMain2.GetChild(0), };
            Transform[] main1ExtList = new Transform[2] { ropeExtMain1.GetChild(0), ropeExtMain1.GetChild(0), };
            Transform[] fore1ExtList = new Transform[2] { ropeExtFore1.GetChild(0), ropeExtMain1.GetChild(0), };
            forestay.GetComponent<Mast>().mastReefAttExtension = main2ExtList;
            forestay1Lower.GetComponent<Mast>().mastReefAttExtension = main2ExtList;

            forestay0.GetComponent<Mast>().mastReefAttExtension = main1ExtList;
            forestayMid.GetComponent<Mast>().mastReefAttExtension = main1ExtList;
            forestay0Lower.GetComponent<Mast>().mastReefAttExtension = main1ExtList;
            container.Find("midstay_f-0").GetComponent<Mast>().mastReefAttExtension = main1ExtList;

            forestaySource.GetComponent<Mast>().mastReefAttExtension = fore1ExtList;
            
            mainMast1.GetComponent<Mast>().mastHeight = 18.8f;//= 17.5f;
            mainMast1.GetComponent<Mast>().extraBottomHeight = 0.5f;
            mainMast2.GetComponent<Mast>().mastHeight = 19.3f;//= 18.5f;
            mainMast2.GetComponent<Mast>().extraBottomHeight = 0f;//= 18.5f;
            mizzenMast1M.mastHeight = 12.5f;//= 12.6f;
            mizzenMast1M.extraBottomHeight = 0.6f;
            mizzenMast2M.mastHeight += 0.8f;//= 12.6f;
            foremastM.mastHeight += 2f;//= 11.6f;1
            foremast.GetComponent<BoatPartOption>().optionName = "foremast 1";

            forestay0Lower.localEulerAngles = new Vector3(305.2f, 270f, 90f);
            forestay0Lower.localScale = new Vector3(1, 1, 0.97f);
            forestay0Lower.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mainMast1.GetComponent<BoatPartOption>() };
            forestay0Lower.GetComponent<BoatPartOption>().requiresDisabled = new List<BoatPartOption> { foremast.GetComponent<BoatPartOption>() };
            forestay0Lower.GetComponent<Mast>().mastHeight = 18.3f;

            forestay.GetComponent<Mast>().mastHeight = 20;
            forestay1Lower.GetComponent<Mast>().mastHeight = 14;

            midstaySource.GetComponent<Mast>().reefWinch[0].transform.parent = container;

            partsList.availableParts[4].category = 2;
            partsList.availableParts[5].category = 2;
            partsList.availableParts[6].category = 2;
            partsList.availableParts[7].category = 2;

            midstaySource.GetComponent<BoatPartOption>().requires.Remove(mizzenMast2.GetComponent<BoatPartOption>());
            midstaySource.GetComponent<BoatPartOption>().requiresDisabled = new List<BoatPartOption> { mizzenMast1.GetComponent<BoatPartOption>(), structure.Find("mast_mizzen_(empty)").GetComponent<BoatPartOption>() };
            forestaySource.GetComponent<BoatPartOption>().requires.Add(bowsprit.GetComponent<BoatPartOption>());

            partsList.availableParts[2].partOptions.Add(Util.CreatePartOption(structure, "mast_mid_empty", "(no main mast)"));
            #endregion

            #region telltale
            var flagSource = structure.Find("mast_front_").Find("flag_cloth");
            BoatPartOption noFlag = Util.CreatePartOption(container, "(flag empty)", "(no telltale)");

            BoatPartOption flags_main = Util.CreatePartOption(container, "flag_main", "mainmast telltale");
            flags_main.basePrice = 50;
            flags_main.installCost = 10;

            Transform flags_main_0 = new GameObject() { name = "flags_main_0" }.transform;
            flags_main_0.transform.parent = flags_main.transform;
            var flag_main_0_side = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_side.name = "flag_main_0_side";
            flag_main_0_side.localPosition = new Vector3(1, 3, 2.9f);
            flag_main_0_side.localEulerAngles = new Vector3(355, 0, 357);
            //flag_main_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);

            Transform flags_main_1 = new GameObject() { name = "flags_main_1" }.transform;
            flags_main_1.transform.parent = flags_main.transform;
            var flag_main_1_side = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_side.name = "flag_main_1_side";
            flag_main_1_side.localPosition = new Vector3(4.76f, 3, 2.77f);
            flag_main_1_side.localEulerAngles = new Vector3(355, 0, 355);
            //flag_main_1_side.localScale = new Vector3(0.8f, 1f, 0.5f);

/*
            BoatPartOption flags_fore = Util.CreatePartOption(container, "flag_fore", "fore mast telltale");

            Transform flags_fore_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_fore_0" }.transform, flags_fore.transform);
            var flag_fore_0_side = UnityEngine.Object.Instantiate(flagSource, flags_fore_0);
            flag_fore_0_side.name = "flag_fore_0_side";
            flag_fore_0_side.localPosition = new Vector3(6.62f, 8f, 2.76f);
            flag_fore_0_side.localEulerAngles = new Vector3(87, 0, 0);*/
            //flag_fore_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);

            BoatPartOption flags_mizzen = Util.CreatePartOption(container, "flag_mizzen", "mizzen mast telltale");
            flags_mizzen.basePrice = 50;
            flags_mizzen.installCost = 10;

            Transform flags_mizzen_0 = new GameObject() { name = "flags_mizzen_0" }.transform;
            flags_mizzen_0.parent = flags_mizzen.transform;
            var flag_mizzen_0_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
            flag_mizzen_0_side.name = "flag_mizzen_0_side";
            flag_mizzen_0_side.localPosition = new Vector3(-10.2f, 3.1f, 1.6f);
            flag_mizzen_0_side.localEulerAngles = new Vector3(356, 0, 357);
            //flag_mizzen_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);

            Transform flags_mizzen_1 = new GameObject() { name = "flags_mizzen_1" }.transform;
            flags_mizzen_1.parent = flags_mizzen.transform;
            var flag_mizzen_1_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
            flag_mizzen_1_side.name = "flag_mizzen_1_side";
            flag_mizzen_1_side.localPosition = new Vector3(-5.17f, 3, 2.61f);
            flag_mizzen_1_side.localEulerAngles = new Vector3(354, 0, 357);

            Transform flags_mizzen_2 = new GameObject() { name = "flags_mizzen_2" }.transform;
            flags_mizzen_2.parent = flags_mizzen.transform;
            var flag_mizzen_2_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_2);
            flag_mizzen_2_side.name = "flag_mizzen_2_side";
            flag_mizzen_2_side.localPosition = new Vector3(-5.46f, 3, 2.62f);
            flag_mizzen_2_side.localEulerAngles = new Vector3(354, 0, 357);
            //flag_mizzen_2_side.localScale = new Vector3(0.8f, 1f, 0.5f);


            //flags_fore.requires.Add(foremast.GetComponent<BoatPartOption>());
            //flags_main.requiresDisabled.Add(container.Find("(no mainmast)").GetComponent<BoatPartOption>());
            flags_mizzen.requiresDisabled.Add(structure.Find("mast_mizzen_(empty)").GetComponent<BoatPartOption>());
            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { noFlag, flags_main, flags_mizzen });


            mizzenMast1.GetComponent<BoatPartOption>().childOptions = mizzenMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_0.gameObject);
            mizzenMast2.GetComponent<BoatPartOption>().childOptions = mizzenMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_1.gameObject);

            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_0.gameObject);
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_1.gameObject);

            //foremast.GetComponent<BoatPartOption>().childOptions = foremast.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_fore_0.gameObject);

            #endregion

            #region flatsprit
            Mast flatsprit = Util.CopyMast(bowsprit, new Vector3(20.9f, 0f, 5.65f), new Vector3(0, 67, 0), Vector3.one, "bowsprit_2", "bowsprit 2", 63);
            var mastCont = UnityEngine.Object.Instantiate(new GameObject(), flatsprit.transform);
            flatsprit.enabled = false;
            mastCont.gameObject.SetActive(false);
            Mast newMast = mastCont.AddComponent<Mast>();
            newMast.gameObject.name = "bowsprit_2_mast";
            newMast.mastCols = flatsprit.mastCols;
            newMast.walkColMast = flatsprit.walkColMast;
            newMast.mastHeight = flatsprit.mastHeight;
            newMast.maxSails = flatsprit.maxSails;
            newMast.reefWinch = flatsprit.reefWinch;
            newMast.midAngleWinch = flatsprit.midAngleWinch;
            newMast.leftAngleWinch = flatsprit.leftAngleWinch;
            newMast.rightAngleWinch = flatsprit.rightAngleWinch;
            newMast.midRopeAtt = flatsprit.midRopeAtt;
            newMast.mastReefAtt = flatsprit.mastReefAtt;
            newMast.mastReefAttExtension = flatsprit.mastReefAttExtension;
            newMast.shipRigidbody = flatsprit.shipRigidbody;
            newMast.startSailPrefabs = flatsprit.startSailPrefabs;
            newMast.onlySquareSails = true;
            newMast.transform.localEulerAngles = new Vector3(0, 328, 0);
            UnityEngine.Object.Destroy(flatsprit);
            newMast.orderIndex = 31;
            mastCont.gameObject.SetActive(true);
            partsList.availableParts[0].partOptions.Add(flatsprit.GetComponent<BoatPartOption>());
            #endregion

            #region forestay flatsprit
            Mast forestayF = Util.CopyMast(forestaySource, forestaySource.localPosition, new Vector3(309.873f, 270f, 90f), new Vector3(1, 1, 1.24f), "forestay_foremast_flat", "foremast forestay 2", 34);
            //forestayF.mastHeight = 13.5f;
            forestayF.leftAngleWinch = new GPButtonRopeWinch[] { foremastM.leftAngleWinch[1] };
            forestayF.rightAngleWinch = new GPButtonRopeWinch[] { foremastM.rightAngleWinch[1] };
            //forestayF.GetComponent<BoatPartOption>().requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { flatsprit.GetComponent<BoatPartOption>(), foremast.GetComponent<BoatPartOption>() };
            partsList.availableParts[4].partOptions.Add(forestayF.GetComponent<BoatPartOption>());

            Mast forestayF2 = Util.CopyMast(forestay, forestay.localPosition, new Vector3(310.2f, 270f, 90f), new Vector3(1, 1, 1.14f), "forestay_1_flat", "forestay 2 long", 35);
            //forestayF2.mastHeight = 22.4f;
            forestayF2.GetComponent<BoatPartOption>().requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF2.GetComponent<BoatPartOption>().requires.Add(flatsprit.GetComponent<BoatPartOption>());
            partsList.availableParts[4].partOptions.Add(forestayF2.GetComponent<BoatPartOption>());

            Mast forestayF3 = Util.CopyMast(forestay1Lower, forestay1Lower.localPosition, forestay1Lower.localEulerAngles, new Vector3(1, 1, 1.043f), "forestay_1_lower_flat", "lower forestay 2 long", 36);
            //forestayF3.mastHeight = 14.5f;
            forestayF3.GetComponent<BoatPartOption>().requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF3.GetComponent<BoatPartOption>().requires.Add(flatsprit.GetComponent<BoatPartOption>());
            partsList.availableParts[6].partOptions.Add(forestayF3.GetComponent<BoatPartOption>());

            Mast forestayF4 = Util.CopyMast(forestay0, forestay0.localPosition, new Vector3(316.8f, 270f, 90f), new Vector3(1, 1, 1.12f), "forestay_0_long", "forestay 1 long", 43);
            BoatPartOption forestayF4Opt = forestayF4.GetComponent<BoatPartOption>();
            forestayF4Opt.requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF4Opt.requires.Add(flatsprit.GetComponent<BoatPartOption>());
            partsList.availableParts[4].partOptions.Add(forestayF4Opt);

            Mast forestayF5 = Util.CopyMast(forestayMid, forestayMid.localPosition, forestayMid.localEulerAngles, new Vector3(1, 1, 1.067f), "forestay_0_mid_long", "middle forestay 1 long", 44);
            BoatPartOption forestayF5Opt = forestayF5.GetComponent<BoatPartOption>();
            forestayF5Opt.requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF5Opt.requires.Add(flatsprit.GetComponent<BoatPartOption>());
            partsList.availableParts[5].partOptions.Add(forestayF5Opt);
            #endregion

            #region tall mizzen
            Mast mizzen3 = Util.CopyMast(mainMast1, new Vector3(mizzenMast2.localPosition.x, 0, 20.8f), "mast_mizzen_2", "mizzen mast 2 tall", 37);
            mizzen3.reefWinch = Util.CopyWinches(mizzenMast2M.reefWinch, Vector3.zero, Vector3.zero);
            mizzen3.reefWinch = mizzen3.reefWinch.AddToArray(Util.CopyWinch(mizzenMast2M.reefWinch[1], new Vector3(-0.29f, 0f, -18.26f)));
            foreach (var winch in mizzen3.reefWinch)
            {
                winch.transform.parent = mizzen3.transform;
            }
            mizzen3.reefWinch.Last().transform.localPosition = new Vector3(-0.29f, 0f, -18.26f);
            mizzen3.reefWinch.Last().transform.localEulerAngles = new Vector3(0f, 270f, 0f);
            mizzen3.mastHeight = 17.3f;
            mizzen3.extraBottomHeight = 0;
            mizzen3.midAngleWinch = mizzenMast2M.midAngleWinch.AddToArray(Util.CopyWinch(mizzenMast2M.midAngleWinch[1], new Vector3(-9f, 2.52f, 1.46f)));
            mizzen3.leftAngleWinch = mizzenMast2M.leftAngleWinch;
            mizzen3.rightAngleWinch = mizzenMast2M.rightAngleWinch;
            var m3shrouds = mizzen3.transform.Find("static_rig_001");
            m3shrouds.localPosition = new Vector3(m3shrouds.localPosition.x, m3shrouds.localPosition.y, -19.9f);
            m3shrouds.localScale = new Vector3(1.2f, 1.05f, 1.14f);
            var m3shroudsCol = mizzen3.walkColMast.Find(m3shrouds.name);
            m3shroudsCol.localPosition = m3shrouds.localPosition;
            m3shroudsCol.localScale = m3shrouds.localScale;
            var m3brace = mizzen3.transform.Find("trim_003");
            m3brace.localScale = new Vector3(0.92f, 0.92f, 1f);
            m3brace.localPosition = new Vector3(0, 0, -17.55f);

            UnityEngine.Object.Instantiate(mizzenMast2.Find("mast_front_000"), mizzen3.transform, true);
            UnityEngine.Object.Instantiate(mizzenMast2M.walkColMast.Find("mast_front_000"), mizzen3.walkColMast, true);
            mizzen3.midRopeAtt[0] = UnityEngine.Object.Instantiate(mizzenMast2M.midRopeAtt[0].parent, mizzen3.transform, true).GetChild(0);
            for (int i = 0; i < mizzen3.midRopeAtt.Length; i++)
            {
                mizzen3.midRopeAtt[i] = mizzen3.midRopeAtt[0];
            }
            mizzen3.mastReefAttExtension = new Transform[0];
            mizzen3.GetComponent<BoatPartOption>().childOptions = mizzen3.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_2.gameObject);

            partsList.availableParts[3].partOptions.Add(mizzen3.GetComponent<BoatPartOption>());
            #endregion

            #region mizzen midstay
            Mast midstay = Util.CopyMast(midstaySource, new Vector3(midstaySource.localPosition.x, 23.4f, 0f), "midstay_topmast", "top middlestay", 32);
            midstay.reefWinch = Util.CopyWinches(midstay.reefWinch, midstay.reefWinch[0].transform.localPosition, new Vector3(midstay.reefWinch[0].transform.localPosition.x, midstay.reefWinch[0].transform.localPosition.y, -midstay.reefWinch[0].transform.localPosition.z));
            midstay.reefWinch[0].transform.localScale = new Vector3(midstay.reefWinch[0].transform.localScale.x, midstay.reefWinch[0].transform.localScale.y, -midstay.reefWinch[0].transform.localScale.z);
            //midstay.reefWinch[0].transform.parent = mizzen3.transform;
            midstay.leftAngleWinch = Util.CopyWinches(midstay.leftAngleWinch, Vector3.zero, new Vector3(-0.4f, 0, 0));
            midstay.rightAngleWinch = Util.CopyWinches(midstay.rightAngleWinch, Vector3.zero, new Vector3(-0.4f, 0, 0));
            midstay.mastReefAtt = mizzen3.mastReefAtt;
            BoatPartOption midstayOpt = midstay.GetComponent<BoatPartOption>();
            BoatPartOption midstayNone = Util.CreatePartOption(container, "(no top middlestay)", "(no top middlestay)");
            BoatPart topMidstayPart = Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { midstayNone, midstayOpt });
            midstayOpt.requires = new List<BoatPartOption> { mizzen3.GetComponent<BoatPartOption>(), mainMast1.GetComponent<BoatPartOption>() };
            #endregion

            #region topmast
            var topmastReefWinch = new GPButtonRopeWinch[1] { Util.CopyWinch(foremastM.reefWinch[1], new Vector3(foremastM.reefWinch[1].transform.localPosition.x, foremastM.reefWinch[1].transform.localPosition.y, -foremastM.reefWinch[1].transform.localPosition.z)) };
            topmastReefWinch[0].transform.localEulerAngles = new Vector3(0, 0, 90);
            var topmastMidAngleWinch = new GPButtonRopeWinch[] { Util.CopyWinch(foremastM.midAngleWinch[1], new Vector3(-3.255f, 2.26f, 1.745f)) };
            var topmastLeftAngleWinch = forestaySource.GetComponent<Mast>().leftAngleWinch;
            var topmastRightAngleWinch = forestaySource.GetComponent<Mast>().rightAngleWinch;

            BoatPartOption topMast2None = Util.CreatePartOption(structure, "empty_mizzen_topmast", "(no mizzen topmast)");
            BoatPart topmast2Part = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { topMast2None, });

            BoatPartOption topMast1None = Util.CreatePartOption(structure, "empty_fore_topmast", "(no fore topmast)");
            BoatPart topmastPart = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { topMast1None, });

            #endregion

            #region topmast midstay 2
            Mast midstay2WinchSource = container.Find("midstay_0-0").GetComponent<Mast>();
            Transform midstay2Source = container.Find("midstay_f-0_top");
            Mast midstay2 = Util.CopyMast(midstay2Source, new Vector3(-8.7f, midstay2Source.localPosition.y, 0f), midstay2Source.localEulerAngles, new Vector3(1f, 1f, 1.16f), "midstay_1-m1_top", "mizzen topstay", 45);
            BoatPartOption midstay2Opt = midstay2.GetComponent<BoatPartOption>();
            midstay2.reefWinch = Util.CopyWinches(midstay2WinchSource.reefWinch, midstay2WinchSource.reefWinch[0].transform.localPosition, new Vector3(midstay2WinchSource.reefWinch[0].transform.localPosition.x, -midstay2WinchSource.reefWinch[0].transform.localPosition.y, midstay2WinchSource.reefWinch[0].transform.localPosition.z));
            midstay2.reefWinch[0].transform.localScale = new Vector3(midstay2.reefWinch[0].transform.localScale.x, midstay2.reefWinch[0].transform.localScale.y, -midstay2.reefWinch[0].transform.localScale.z);
            midstay2.leftAngleWinch = Util.CopyWinches(midstay2WinchSource.leftAngleWinch, Vector3.zero, new Vector3(0.7f, 0f, 0.3f));
            midstay2.rightAngleWinch = Util.CopyWinches(midstay2WinchSource.rightAngleWinch, Vector3.zero, new Vector3(0.7f, 0f, -0.3f));
            midstay2.mastReefAtt = mizzenMast1M.mastReefAtt;
            //BoatPartOption midstay2None = Util.CreatePartOption(container, "(no mizzen topstay)", "(no mizzen topstay)");
            //Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { midstay2None, midstay2Opt });
            topMidstayPart.partOptions.Add(midstay2Opt);
            //midstay2Opt.requires = new List<BoatPartOption> { mizzen3.GetComponent<BoatPartOption>() };
            #endregion            
            #region topmast midstay 3
            Transform midstay3Source = container.Find("midstay_1-0");
            Mast midstay3 = Util.CopyMast(midstay3Source, new Vector3(-8.6f, 23.25f, 0f), midstay3Source.localEulerAngles, new Vector3(1f, 1f, 0.97f), "midstay_0-m1_top", "mizzen topstay 2", 46);
            BoatPartOption midstay3Opt = midstay3.GetComponent<BoatPartOption>();
            midstay3.reefWinch = midstay2.reefWinch;
            midstay3.leftAngleWinch = midstay2.leftAngleWinch;
            midstay3.rightAngleWinch = midstay2.rightAngleWinch;
            midstay3.mastReefAtt = mizzenMast1M.mastReefAtt;
            //BoatPartOption midstay3None = Util.CreatePartOption(container, "(no mizzen topstay)", "(no mizzen topstay)");
            //Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { midstay3None, midstay3Opt });
            topMidstayPart.partOptions.Add(midstay3Opt);
            //midstay3Opt.requires = new List<BoatPartOption> { mizzen3.GetComponent<BoatPartOption>() };
            #endregion

            #region topmast2
            var topmast2ReefWinch = new GPButtonRopeWinch[1] { Util.CopyWinch(mizzenMast1M.reefWinch[0], new Vector3(0.27f, 0f, mizzenMast1M.reefWinch[0].transform.localPosition.z)) };
            topmast2ReefWinch[0].transform.localEulerAngles = new Vector3(0, 90, 0);
            var topmast2MidAngleWinch = new GPButtonRopeWinch[] { mizzen3.midAngleWinch.Last() };
            var topmast2LeftAngleWinch = new GPButtonRopeWinch[] { mizzen3.leftAngleWinch.Last() };
            var topmast2RightAngleWinch = new GPButtonRopeWinch[] { mizzen3.rightAngleWinch.Last() };

            #endregion
            #region raked foremast
            Mast foremast2 = Util.CopyMast(foremast, new Vector3(14.95f, 0f, 14.97f), new Vector3(0, 12.5f, 0), foremast.localScale, "mast_front_1", "raked foremast", 38);
            foremast2.reefWinch = Util.CopyWinches(foremastM.reefWinch, Vector3.zero, Vector3.zero);
            foremast2.reefWinch[0].transform.localPosition = new Vector3(11.78f, 2.082f, 0f);
            foremast2.reefWinch[0].transform.localEulerAngles = new Vector3(348, foremast2.reefWinch[0].transform.localEulerAngles.y, foremast2.reefWinch[0].transform.localEulerAngles.z);
            foremast2.reefWinch[1].transform.localPosition = new Vector3(12.1f, foremast2.reefWinch[1].transform.localPosition.y, foremast2.reefWinch[1].transform.localPosition.z);
            Transform f2Shrouds = foremast2.transform.Find("static_rope_atts");
            f2Shrouds.localPosition = new Vector3(-14.57f, 0f, -17f);
            f2Shrouds.localScale = new Vector3(1.1f, 1.1f, 1.132f);
            Transform f2ShroudsCol = foremast2.walkColMast.Find(f2Shrouds.name);
            f2ShroudsCol.localPosition = f2Shrouds.localPosition;
            f2ShroudsCol.localScale = f2Shrouds.localScale;

            partsList.availableParts[1].partOptions.Add(foremast2.GetComponent<BoatPartOption>());
            #endregion

            #region flying forestay
      /*      Mast outerForestay = Util.CopyMast(forestay, new Vector3(12.7f, 22.2f, 0f), new Vector3(314.5f, 270f, 90f), new Vector3(1, 1, 0.95f), "forestay_outer", "outer forestay", 39);
            outerForestay.reefWinch = Util.CopyWinches(outerForestay.reefWinch, outerForestay.reefWinch[0].transform.localPosition, new Vector3(outerForestay.reefWinch[0].transform.localPosition.x, outerForestay.reefWinch[0].transform.localPosition.y, -outerForestay.reefWinch[0].transform.localPosition.z));
            outerForestay.mastReefAttExtension = fore1ExtList;
            BoatPartOption outerForestayOpt = outerForestay.GetComponent<BoatPartOption>();
            BoatPartOption outerForestayNone = Util.CreatePartOption(outerForestay.transform.parent, "forestay_outer_empty", "(no outer forestay)");
            BoatPart outerForestayPart = Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { outerForestayNone, outerForestayOpt });

            Mast outerForestay2 = Util.CopyMast(forestaySource, new Vector3(15.9f, 16.3f, 0), new Vector3(322.5f, 270f, 90f), new Vector3(1, 1, 1.2f), "forestay_outer_2", "outer forestay 2", 40);
            outerForestay2.mastHeight = 12f;
            outerForestay2.mastReefAtt[0] = foremast2.transform.GetChild(4).GetChild(0);
            outerForestay2.mastReefAttExtension = new Transform[0];
            outerForestayPart.partOptions.Add(outerForestay2.GetComponent<BoatPartOption>());
            outerForestay2.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foremast2.GetComponent<BoatPartOption>() };*/
            #endregion

            Transform outerFstaySource = container.Find("midstay_f-0_top");
            Mast upperForemastStay = Util.CopyMast(outerFstaySource, new Vector3(6.3f, outerFstaySource.localPosition.y, 0), "midstay_f1-0_top", "foremast stay 2", 41);
            BoatPartOption upperFmastStayOpt = upperForemastStay.GetComponent<BoatPartOption>();
            upperFmastStayOpt.requires = new List<BoatPartOption> { foremast2.GetComponent<BoatPartOption>(), mainMast2.GetComponent<BoatPartOption>() };
            partsList.availableParts[5].partOptions.Add(upperFmastStayOpt);

            Mast foremastForestayR = Util.CopyMast(forestaySource, new Vector3(15.8f, 16f, 0), new Vector3(299f, 270f, 90f), new Vector3(1, 1, 1.05f), "forestay_foremast_3", "foremast forestay 3", 40);
            //foremastForestayR.mastHeight = 12f;
            foremastForestayR.mastReefAtt[0] = foremast2.transform.GetChild(4).GetChild(0);
            foremastForestayR.mastReefAttExtension = new Transform[] { foremast2.transform.GetChild(5).GetChild(0) };
            foremastForestayR.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { foremast2.GetComponent<BoatPartOption>(), flatsprit.GetComponent<BoatPartOption>() };
            partsList.availableParts[4].partOptions.Add(foremastForestayR.GetComponent<BoatPartOption>());

            #region bed
            BoatPartOption bedOpt = Util.AddPartOption(container.Find("bed").gameObject, "bed");
            bedOpt.basePrice = 500;
            bedOpt.installCost = 100;
            BoatPartOption noBed = Util.CreatePartOption(container, "bed_empty", "(no bed)");
            var bedPart = Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption> { bedOpt, noBed });
            #endregion


            //BoatPart jibboomPart = Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption> { Util.CreatePartOption(structure, "jibboom_none", "(no jibboom)") });

            #region late Adjustments
            outerFstaySource.GetComponent<Mast>().mastReefAttExtension = main1ExtList;
            #endregion

            partsList.StartCoroutine(AddTopmast(foremastM,
                structure,
                walkColStruct,
                partsList,
                topmastReefWinch,
                topmastMidAngleWinch,
                topmastLeftAngleWinch,
                topmastRightAngleWinch,
                topmast2ReefWinch,
                topmast2MidAngleWinch,
                topmast2LeftAngleWinch,
                topmast2RightAngleWinch,
                mizzenMast1M,
                //outerForestayOpt,
                flatsprit.GetComponent<BoatPartOption>(),
                //outerForestay2.GetComponent<BoatPartOption>(),
                midstay2Opt,
                topmastPart,
                topmast2Part,
                bedPart
                ));

        }

        private static IEnumerator AddTopmast(Mast parentMast,
            Transform structure,
            Transform walkColStruct,
            BoatCustomParts partsList,
            GPButtonRopeWinch[] topmastReefWinch,
            GPButtonRopeWinch[] topmastMidAngleWinch,
            GPButtonRopeWinch[] topmastLeftAngleWinch,
            GPButtonRopeWinch[] topmastRightAngleWinch,
            GPButtonRopeWinch[] topmast2ReefWinch,
            GPButtonRopeWinch[] topmast2MidAngleWinch,
            GPButtonRopeWinch[] topmast2LeftAngleWinch,
            GPButtonRopeWinch[] topmast2RightAngleWinch,
            Mast parentMast2,
            //BoatPartOption extra1,
            BoatPartOption extra2,
            //BoatPartOption extra3,
            BoatPartOption extra4,
            BoatPart topmastPart,
            BoatPart topmast2Part,
            BoatPart bedPart)

            {
            Debug.Log("waiting for topmast...");
            yield return new WaitUntil(() => PartRefs.sanbuq != null);
            Debug.Log("found topmast");
            Mast topMast1 = Util.CopyMast(PartRefs.sanbuq.Find("structure").Find("mast_0_extension"), structure, walkColStruct, new Vector3(12.25f, 0f, 22.2f), Vector3.zero, new Vector3(0.75f, 0.75f, 0.75f), "fore_topmast", "fore topmast", 42);
            topMast1.midRopeAtt = parentMast.midRopeAtt;

            topMast1.reefWinch = topmastReefWinch;
            topMast1.midAngleWinch = topmastMidAngleWinch;
            topMast1.leftAngleWinch = topmastLeftAngleWinch;
            topMast1.rightAngleWinch = topmastRightAngleWinch;
/*            midstay.mastReefAtt[0] = topMast1.transform.GetChild(0).GetChild(1);
            midstay.mastReefAttExtension[0] = topMast1.transform.GetChild(0).GetChild(2);
*/
            topMast1.GetComponent<MeshRenderer>().material.color = new Color(0.65f, 0.7f, 0.7f);
            topMast1.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { parentMast.GetComponent<BoatPartOption>() };
            topmastPart.partOptions.Add(topMast1.GetComponent<BoatPartOption>());

            topMast1.transform.Find("flagT").gameObject.SetActive(false);
            var wind_flag = UnityEngine.Object.Instantiate(structure.Find("mast_mid_0").Find("flag_cloth (2)"), topMast1.transform, false);
            wind_flag.localPosition = new Vector3(0, 0, 0.8f);
            //mizzenMast2M.leftAngleWinch = new GPButtonRopeWinch[1] { mizzenMast2M.leftAngleWinch[0] };
            //mizzenMast2M.rightAngleWinch = new GPButtonRopeWinch[1] { mizzenMast2M.rightAngleWinch[0] };
            //Debug.Log("hacked winches");

            Mast topMast2 = Util.CopyMast(topMast1.transform, new Vector3(-9.04f, 0f, 23.2f), "mizzen_topmast", "mizzen topmast", 33);
            topMast2.reefWinch = topmast2ReefWinch;
            topMast2.midAngleWinch = topmast2MidAngleWinch;
            topMast2.leftAngleWinch = topmast2LeftAngleWinch;
            topMast2.rightAngleWinch = topmast2RightAngleWinch;

            topMast2.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { parentMast2.GetComponent<BoatPartOption>() };
            topmast2Part.partOptions.Add(topMast2.GetComponent<BoatPartOption>());

            //midstay.GetComponent<BoatPartOption>().requires.Add(topMast1.GetComponent<BoatPartOption>());
            //midstay.mastReefAtt = topMast1.mastReefAtt;
            #region jibboom
            /*Mast jibboomM = Util.CopyMast(topMast1.transform, new Vector3(26f, 0f, 8.1f), new Vector3(0, 67, 180), topMast1.transform.localScale, "jibboom", "jibboom", 63);
            Transform jibboom = jibboomM.transform;
            GameObject.Destroy(jibboom.GetComponent<Mast>());
            BoatPartOption jibboomOpt = jibboom.GetComponent<BoatPartOption>();
            jibboomOpt.requires = new List<BoatPartOption> { extra2 };
            jibboom.GetChild(0).gameObject.SetActive(false);
            jibboom.GetChild(1).gameObject.SetActive(false);
            jibboomPart.partOptions.Add(jibboomOpt);*/
            #endregion

            /*extra1.requires = new List<BoatPartOption> { jibboomOpt, topMast1.GetComponent<BoatPartOption>() };
            extra1.GetComponent<Mast>().mastReefAtt = topMast1.mastReefAtt;
            //extra1.GetComponent<Mast>().mastReefAttExtension = topMast1.mastReefAttExtension;
            extra3.requires.Add(jibboomOpt);*/
            extra4.requires = new List<BoatPartOption> { topMast2.GetComponent<BoatPartOption>() };
            extra4.GetComponent<Mast>().mastReefAtt = topMast2.mastReefAtt;

            // hammock
            yield return new WaitUntil(() => PartRefs.kakam != null);
            var hammock = UnityEngine.Object.Instantiate(PartRefs.kakam.Find("hammock_001"), structure, false);
            hammock.gameObject.SetActive(false);
            hammock.localPosition = new Vector3(-5.65f, 1.3f, 1.2f);
            hammock.localEulerAngles = new Vector3(0, 0, 275);
            var spar = hammock.Find("mast_003");
            spar.localPosition = new Vector3(-1.39f, -0.25f, 1.58f);
            spar.localEulerAngles = new Vector3(0, 90.1f, 240.7f);
            spar.localScale = new Vector3(0.61f, 0.61f, 1.4f);
            hammock.GetComponent<HingeJoint>().connectedBody = partsList.gameObject.GetComponent<Rigidbody>();
            hammock.gameObject.SetActive(true);
            foreach (var outline in hammock.GetComponents<Outline>())
            {
                if (!ReferenceEquals(Traverse.Create(hammock.GetComponent<GPButtonBed>()).Field("outline").GetValue(), outline))
                {
                    UnityEngine.Object.Destroy(outline);
                    break;
                }
            }
            hammock.parent = structure.parent;
            bedPart.partOptions.Add(hammock.GetComponent<BoatPartOption>());

        }
    }

}
