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
    internal class CogPatches
    {
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.transform.Find("medi small");
            Transform structure = container.Find("structure");
            var mainMast1 = structure.Find("mast");
            var mainMast2 = structure.Find("mast_front");
            var mizzenMast = structure.Find("mast_mizzen");
            Transform mastWalkCol = mainMast1.GetComponent<Mast>().walkColMast;
            Transform mastWalkCol2 = mainMast2.GetComponent<Mast>().walkColMast;
            Transform walkCols = mastWalkCol.parent.parent;
            var bowspritM = structure.Find("mast (bowsprit)");
            Transform bowsprit = structure.Find("mast_001");
            /*Plugin.spritRef = bowsprit;
            Plugin.spritColRef = mainMast1.GetComponent<Mast>().walkColMast.parent.Find(bowsprit.name);*/
            //Debug.Log("Cog: adjustments");

            PartRefs.cog = container;
            PartRefs.cogCol = walkCols;

            #region adjustments
            mainMast1.GetComponent<Mast>().mastHeight += 1.2f;//= 11.5f;
            mainMast1.GetComponent<Mast>().extraBottomHeight = 0.5f;
            mainMast2.GetComponent<Mast>().mastHeight += 1.2f;//= 11.5f;
            mainMast2.GetComponent<Mast>().extraBottomHeight = 0.1f;
            /*            mainMast1.GetComponent<Mast>().startSailHeightOffset += 1.2f;//= 11.5f;
                        mainMast2.GetComponent<Mast>().startSailHeightOffset += 1.2f;//= 11.5f;*/



            #endregion


            //Debug.Log("Cog: shrouds");
            #region shrouds
            BoatPartOption backOption = Util.CreatePartOption(container, "parts_shrouds_back", "shrouds 1");
            backOption.transform.localEulerAngles = new Vector3(270, 0, 0);
            var mainShrouds1 = mainMast1.Find("Cylinder_002");
            mainShrouds1.parent = backOption.transform;
            mainMast1.Find("trim_014").parent = mainShrouds1;
            mainMast1.Find("mast_002").parent = mainShrouds1;

            var colContainer1 = UnityEngine.Object.Instantiate(new GameObject() { name = backOption.name }, walkCols);
            colContainer1.transform.localEulerAngles = backOption.transform.localEulerAngles;
            var mainShrouds1Col = mastWalkCol.Find("Cylinder_002");
            mastWalkCol.Find("trim_014").parent = mainShrouds1Col;
            mastWalkCol.Find("mast_002").gameObject.SetActive(false);//.parent = mainShrouds1Col;
            mainShrouds1Col.parent = colContainer1.transform;
            backOption.walkColObject = colContainer1;

            BoatPartOption sideOption = Util.CopyPartOptionObj(backOption, "part_shrouds_side", "shrouds 2");
            var mainShrouds2 = sideOption.transform.GetChild(0);
            var mainShroudAnchors2 = mainShrouds2.transform.GetChild(0);
            var mainShroudSpreader2 = mainShrouds2.transform.GetChild(1);
            mainShrouds2.localPosition = new Vector3(0.4f, 0f, 0.4f);
            mainShrouds2.localEulerAngles = new Vector3(0f, 7.5f, 0f);
            mainShrouds2.localScale = new Vector3(-mainShrouds2.localScale.x, mainShrouds2.localScale.y, mainShrouds2.localScale.z);
            mainShroudAnchors2.localPosition = new Vector3(2.15f, 0f, -0.14f);
            mainShroudAnchors2.localEulerAngles = new Vector3(0f, 7.5f, 0f);
            mainShroudSpreader2.localPosition = new Vector3(1.1f, 0f, mainShroudSpreader2.localPosition.z);
            mainShroudSpreader2.localEulerAngles = new Vector3(90f, 0f, 0f);
            mainShroudSpreader2.localScale = new Vector3(0.5f, mainShroudSpreader2.localScale.y, mainShroudSpreader2.localScale.z);

            var colContainer2 = sideOption.walkColObject;
            Transform mainShrouds2Col = colContainer2.transform.GetChild(0);
            mainShrouds2Col.localPosition = mainShrouds2.localPosition;
            mainShrouds2Col.localEulerAngles = mainShrouds2.localEulerAngles;
            mainShrouds2Col.localScale = mainShrouds2.localScale;
            mainShrouds2Col.GetChild(0).localPosition = mainShroudAnchors2.localPosition;
            mainShrouds2Col.GetChild(0).localEulerAngles = mainShroudAnchors2.localEulerAngles;
            mainShrouds2Col.GetChild(0).localScale = mainShroudAnchors2.localScale;
/*            mainShrouds2Col.GetChild(1).localPosition = mainShroudSpreader2.localPosition;
            mainShrouds2Col.GetChild(1).localEulerAngles = mainShroudSpreader2.localEulerAngles;
            mainShrouds2Col.GetChild(1).localScale = mainShroudSpreader2.localScale;*/

            var frontShrouds1 = mainMast2.Find("Cylinder_004");
            mainMast2.Find("trim_016").parent = frontShrouds1;
            mainMast2.Find("mast_004").parent = frontShrouds1;
            frontShrouds1.parent = backOption.transform;

            var frontShrouds1Col = mastWalkCol2.Find("Cylinder_004");
            mastWalkCol2.Find("trim_016").parent = frontShrouds1Col;
            mastWalkCol2.Find("mast_004").gameObject.SetActive(false); //mastWalkCol2.Find("mast_004").parent = frontShrouds1Col;
            frontShrouds1Col.parent = colContainer1.transform;

            var frontShrouds2 = UnityEngine.Object.Instantiate(frontShrouds1, sideOption.transform);
            var frontShroudAnchors2 = frontShrouds2.transform.GetChild(0);
            var frontShroudSpreader2 = frontShrouds2.transform.GetChild(1);
            frontShrouds2.localPosition = new Vector3(4.2930f, 0f, 1.0511f);
            frontShrouds2.localEulerAngles = new Vector3(0f, 8.3f, 0f);
            frontShrouds2.localScale = new Vector3(-1.05f, 0.86f, 0.96f);
            frontShroudAnchors2.localPosition = new Vector3(-1.53f, 0f, 0.42f);
            frontShroudAnchors2.localEulerAngles = new Vector3(0f, 15f, 0f);
            frontShroudAnchors2.localScale = new Vector3(-1f, frontShroudAnchors2.localScale.y, frontShroudAnchors2.localScale.z);
            frontShroudSpreader2.localPosition = new Vector3(1.48f, 0f, frontShroudSpreader2.localPosition.z);
            frontShroudSpreader2.localEulerAngles = new Vector3(90f, 8.3f, 0f);
            frontShroudSpreader2.localScale = new Vector3(0.4f, frontShroudSpreader2.localScale.y, 1f);

            Transform frontShrouds2Col = UnityEngine.Object.Instantiate(frontShrouds1Col, colContainer2.transform);
            frontShrouds2Col.localPosition = frontShrouds2.localPosition;
            frontShrouds2Col.localEulerAngles = frontShrouds2.localEulerAngles;
            frontShrouds2Col.localScale = frontShrouds2.localScale;
            frontShrouds2Col.GetChild(0).localPosition = frontShroudAnchors2.localPosition;
            frontShrouds2Col.GetChild(0).localEulerAngles = frontShroudAnchors2.localEulerAngles;
            frontShrouds2Col.GetChild(0).localScale = frontShroudAnchors2.localScale;


            // if we want these we have to re-parent mastWalkCol2's mast_004 instead of just disabling it
/*            frontShrouds2Col.GetChild(1).localPosition = frontShroudSpreader2.localPosition;
            frontShrouds2Col.GetChild(1).localEulerAngles = frontShroudSpreader2.localEulerAngles;
            frontShrouds2Col.GetChild(1).localScale = frontShroudSpreader2.localScale;*/


            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption> { backOption, sideOption });

            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[4] { mainShrouds1.gameObject, mainShrouds1Col.gameObject, mainShrouds2.gameObject, mainShrouds2Col.gameObject });
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[4] { frontShrouds1.gameObject, frontShrouds1Col.gameObject, frontShrouds2.gameObject, frontShrouds2Col.gameObject });
            #endregion

            //Debug.Log("Cog: midstay");
            #region midstay
            Transform forestay = container.Find("mast forestay");
            Mast midstay = Util.CopyMast(forestay, new Vector3(-3.8f, 11f, 0f), new Vector3(312, 270, 90), new Vector3(1f, 1f, 0.9f), "mast midstay", "middlestay", 31);
            midstay.mastHeight = 10.7f;
            BoatPartOption midstayOpt = midstay.GetComponent<BoatPartOption>();
            midstayOpt.requires = new List<BoatPartOption> { mainMast2.GetComponent<BoatPartOption>(), mizzenMast.GetComponent<BoatPartOption>() };
            BoatPartOption noMidstay = Util.CreatePartOption(container, "(no_midstay)", "(no middlestay)");
            midstay.reefWinch[0].transform.localPosition = new Vector3(-7.78f, 0.22f, -6.6f);
            midstay.reefWinch[0].transform.localEulerAngles = new Vector3(270f, 312f, 0f);
            midstay.reefWinch[0].transform.parent = container;
            midstay.reefWinch[0].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            midstay.reefWinch[0].rope = null;
            midstay.leftAngleWinch = Util.CopyWinches(mainMast1.GetComponent<Mast>().leftAngleWinch, Vector3.zero, new Vector3(-0.4f, 0f, 0f));
            midstay.rightAngleWinch = Util.CopyWinches(mainMast1.GetComponent<Mast>().rightAngleWinch, Vector3.zero, new Vector3(-0.4f, 0f, 0f));
            //midstay.leftAngleWinch[0].transform.localPosition = new Vector3(-2.16f, 1.3f, 1.978f);
            //midstay.rightAngleWinch[0].transform.localPosition = new Vector3(-2.16f, 1.3f, -1.978f);

            Transform ropeHolder = UnityEngine.Object.Instantiate(midstay.mastReefAtt[0].parent, midstay.transform, true);
            ropeHolder.localPosition = new Vector3(-0.55f, 0.12f, 2f);
            midstay.mastReefAtt[0] = ropeHolder.GetChild(0);


            Mast innerForestay = Util.CopyMast(forestay, new Vector3(1.513f, 12f, 0f), new Vector3(302f, 270f, 90f), new Vector3(1, 1, 0.94f), "mast_inner_forestay", "bottom forestay", 34);
            innerForestay.reefWinch[0].transform.localPosition = new Vector3(-6.58f, 0f, -8.55f);
            innerForestay.reefWinch[0].transform.localEulerAngles = new Vector3(0, 120, 180);
            innerForestay.leftAngleWinch = Util.CopyWinches(innerForestay.leftAngleWinch, innerForestay.leftAngleWinch[0].transform.localPosition, innerForestay.leftAngleWinch[0].transform.localPosition + new Vector3(0.5f, 0, 0));
            innerForestay.rightAngleWinch = Util.CopyWinches(innerForestay.rightAngleWinch, innerForestay.rightAngleWinch[0].transform.localPosition, innerForestay.rightAngleWinch[0].transform.localPosition + new Vector3(0.5f, 0, 0));
            innerForestay.mastHeight = 11.3f;
            BoatPartOption innerForestayOpt = innerForestay.GetComponent<BoatPartOption>();
            innerForestayOpt.requiresDisabled = new List<BoatPartOption> { mainMast2.GetComponent<BoatPartOption>(), forestay.GetComponent<BoatPartOption>() };

            BoatPart newStays = Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption>() { noMidstay, midstayOpt, innerForestayOpt });
            #endregion

            //Debug.Log("Cog: bowsprit");
            #region longsprit
            BoatPartOption bowspritOpt = Util.AddPartOption(bowspritM.gameObject, "bowsprit");
            bowspritOpt.basePrice = 500;
            bowspritOpt.installCost = 200;
            bowspritOpt.mass = 20;
            bowspritOpt.childOptions = new GameObject[1] { bowsprit.gameObject };
            bowspritOpt.walkColObject = bowspritM.GetComponent<Mast>().walkColMast.transform.parent.Find("mast_001").gameObject;

            BoatPartOption bowspritNone = Util.CreatePartOption(bowsprit.parent, "(no_bowsprit)", "(no bowsprit)");
            Mast bowspritLongM = Util.CopyMast(bowspritM, bowspritM.localPosition + new Vector3(2f, 0f, 1.1f), "bowsprit_long", "long bowsprit", 32);
            Transform bowspritLong = UnityEngine.Object.Instantiate(bowsprit, bowsprit.parent);
            bowspritLong.localPosition = bowsprit.localPosition + new Vector3(2f, 0f, 1.1f);
            bowspritLong.localScale = new Vector3(bowspritLong.localScale.x, bowspritLong.localScale.y, 0.8f);

            bowspritLongM.walkColMast.transform.localScale = bowspritLong.localScale;
            BoatPartOption bowspritLongOpt = bowspritLongM.GetComponent<BoatPartOption>();
            bowspritLongOpt.basePrice = 750;
            bowspritLongOpt.installCost = 200;
            bowspritLongOpt.mass = 30;
            bowspritLongOpt.childOptions = new GameObject[1] { bowspritLong.gameObject };
            bowspritLongOpt.walkColObject = UnityEngine.Object.Instantiate(bowspritOpt.walkColObject, bowspritOpt.walkColObject.transform.parent);
            bowspritLongOpt.walkColObject.transform.localPosition = bowspritLong.localPosition;
            bowspritLongOpt.walkColObject.transform.localScale = bowspritLong.localScale;
            BoatPart bowspritPart = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption>() { bowspritOpt, bowspritLongOpt, bowspritNone });
            #endregion

            //Debug.Log("Cog: forestay3");
            #region forestay3
            var forestay2 = container.Find("forestay_front_mast");
            var forestay3 = Util.CopyMast(forestay2, forestay2.localPosition, "forestay_mast_front_long", "forestay 2 long", 33);
            forestay3.transform.localEulerAngles += new Vector3(10, 0, 0);
            forestay3.mastHeight = 11.8f;
            forestay3.reefWinch[0].transform.position = forestay2.GetComponent<Mast>().reefWinch[0].transform.position;
            forestay3.reefWinch[0].transform.rotation = forestay2.GetComponent<Mast>().reefWinch[0].transform.rotation;
            var forestay3_opt = forestay3.GetComponent<BoatPartOption>();
            forestay3_opt.requires.Add(bowspritLongOpt);
            partsList.availableParts[1].partOptions.Add(forestay3_opt);
            

            #endregion

            //Debug.Log("Cog: forestay4");
            #region forestay4
            var forestay4 = Util.CopyMast(forestay, new Vector3(2.213f, 13.3f, 0f), new Vector3(315, 270, 90), new Vector3(1, 1, 1.04f), "forestay_mast_long", "forestay 1 long", 35);
            //forestay4.transform.localEulerAngles += new Vector3(10, 0, 0);
            forestay4.mastHeight = 12.8f;
            forestay4.reefWinch[0].transform.position = forestay.GetComponent<Mast>().reefWinch[0].transform.position;
            forestay4.reefWinch[0].transform.rotation = forestay.GetComponent<Mast>().reefWinch[0].transform.rotation;
            var forestay4_opt = forestay4.GetComponent<BoatPartOption>();
            forestay4_opt.requires.Add(bowspritLongOpt);
            partsList.availableParts[1].partOptions.Add(forestay4_opt);

            #endregion
            //Debug.Log("Cog: foremast");
            #region foremast
            var mizzenMast_mast = mizzenMast.GetComponent<Mast>();
            var foremast = Util.CopyMast(mizzenMast, new Vector3(8.8f, 0f, 9.8f), new Vector3(0f, 17f, 0f), new Vector3(1f, 1f, 0.91f), "foremast", "foremast", 36);
            foremast.mastHeight = 8;
            foremast.reefWinch[0] = foremast.transform.Find("winch_reef_mizzen").GetComponent<GPButtonRopeWinch>();
            foremast.reefWinch[0].transform.localPosition = new Vector3(-0.22f, 0f, -9.9f);
            foremast.reefWinch[0].transform.localEulerAngles = new Vector3(0f, 270f, 2.625f);
            foremast.midAngleWinch = Util.CopyWinches(mizzenMast_mast.midAngleWinch, mizzenMast_mast.midAngleWinch[0].transform.localPosition, new Vector3(mainMast1.GetComponent<Mast>().midAngleWinch[0].transform.localPosition.x, mainMast1.GetComponent<Mast>().midAngleWinch[0].transform.localPosition.y, -mainMast1.GetComponent<Mast>().midAngleWinch[0].transform.localPosition.z));
            foremast.midAngleWinch[0].transform.localEulerAngles = new Vector3(0f, 3.4f, 0f);
            foremast.leftAngleWinch = forestay.GetComponent<Mast>().leftAngleWinch;
            foremast.rightAngleWinch = forestay.GetComponent<Mast>().rightAngleWinch;
            foremast.midRopeAtt[0].transform.parent.localPosition = new Vector3(-2.78f, 0f, -11.8f);
            foremast.midRopeAtt[0].transform.parent.localEulerAngles = new Vector3(270f, 340f, 0f);
            var foremast_opt = foremast.GetComponent<BoatPartOption>();
            BoatPartOption foremast_none = Util.CreatePartOption(foremast.transform.parent, "(no foremast)", "(no foremast)");
            Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { foremast_none, foremast_opt });
            #endregion

            #region telltale
            var flagSource = mainMast1.Find("wind_cloth");
            BoatPartOption noFlag = Util.CreatePartOption(container, "(flag empty)", "(no telltale)");

            BoatPartOption flags_main = Util.CreatePartOption(container, "flag_main", "mainmast telltale");
            flags_main.basePrice = 30;
            flags_main.installCost = 6;

            Transform flags_main_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_0" }.transform, flags_main.transform);
            var flag_main_0_side = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_side.name = "flag_main_0_side";
            flag_main_0_side.localPosition = new Vector3(0, 2, 2.1f);
            flag_main_0_side.localEulerAngles = new Vector3(84, 0, 0);
            flag_main_0_side.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            flag_main_0_side.GetComponent<Renderer>().material.color = new Color(0, 1, 0);

            var flag_main_0_back = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_back.name = "flag_main_0_back";
            flag_main_0_back.localPosition = new Vector3(-1.6f, 2, 2.15f);
            flag_main_0_back.localEulerAngles = new Vector3(81, 330, 0);
            flag_main_0_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            flag_main_0_back.GetComponent<Renderer>().material.color = new Color(0, 1, 0);


            Transform flags_main_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_1" }.transform, flags_main.transform);
            var flag_main_1_side = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_side.name = "flag_main_1_side";
            flag_main_1_side.localPosition = new Vector3(4.06f, 2.5f, 1.6f);
            flag_main_1_side.localEulerAngles = new Vector3(84, 0, 0);
            flag_main_1_side.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            flag_main_1_side.GetComponent<Renderer>().material.color = new Color(0, 1, 0);

            var flag_main_1_back = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_back.name = "flag_main_1_back";
            flag_main_1_back.localPosition = new Vector3(2.29f, 2.5f, 1.88f);
            flag_main_1_back.localEulerAngles = new Vector3(83, 0, 330);
            flag_main_1_back.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            flag_main_1_back.GetComponent<Renderer>().material.color = new Color(0, 1, 0);


            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { noFlag, flags_main });

            sideOption.childOptions = sideOption.childOptions.AddRangeToArray(new GameObject[2] { flag_main_1_side.gameObject, flag_main_0_side.gameObject });
            backOption.childOptions = backOption.childOptions.AddRangeToArray(new GameObject[2] { flag_main_1_back.gameObject, flag_main_0_back.gameObject });
            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_0.gameObject);
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_1.gameObject);

            //foreMast.GetComponent<BoatPartOption>().childOptions = foreMast.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_fore_0.gameObject);

            #endregion


            Debug.Log("Cog: late adjustments");
            #region late adjustments
            var ropeHolderAft = container.Find("struct_var_1__low_roof_").Find("mast_003");
            ropeHolderAft.parent = mizzenMast;
            mizzenMast.GetChild(2).position = ropeHolderAft.GetChild(0).position;
            mizzenMast.GetChild(2).rotation = ropeHolderAft.GetChild(0).rotation;
            ropeHolderAft.GetChild(0).gameObject.SetActive(false);

            partsList.availableParts[1].category = 2;
            foreach (var option in partsList.availableParts[1].partOptions)
            {
                option.requiresDisabled.Add(foremast_opt);
            }
            partsList.availableParts[1].partOptions.Add(Util.CreatePartOption(container, "(no forestay)", "(no forestay)"));

            #endregion

            #region mizzen2
            var mizzen2 = Util.CopyMast(mizzenMast, mizzenMast.localPosition + new Vector3(1.5f, 0, 0), "mizzen_mast_2", "mizzen mast 2", 37);
            mizzen2.transform.Find("mast_003").localPosition += new Vector3(-1.5f, 0, 0);
            mizzen2.transform.GetChild(2).localPosition += new Vector3(-1.5f, 0, 0);
            var mizzen2_opt = mizzen2.GetComponent<BoatPartOption>();
            partsList.availableParts[2].partOptions.Add(mizzen2_opt);
            #endregion

           #region midstay 2
            Mast midstay2 = Util.CopyMast(midstay.transform, new Vector3(-2.55f, 11, 0), new Vector3(307, 270, 90), new Vector3(1, 1, 0.85f), "midstay_2", "midstay 2", 38);
            midstay2.mastHeight = 10.2f;
            midstay2.reefWinch = Util.CopyWinches(midstay2.reefWinch, Vector3.zero, new Vector3(1.4f, 0, 0));
            //midstay2.reefWinch[0].transform.localEulerAngles = new Vector3(270f, 307f, 0f);
            BoatPartOption midstay2_opt = midstay2.GetComponent<BoatPartOption>();
            midstay2_opt.requires = new List<BoatPartOption> { mainMast2.GetComponent<BoatPartOption>(), mizzen2_opt };
            newStays.partOptions.Add(midstay2_opt);
            #endregion 
            

            #region helm
            var wheel = container.Find("steering_wheel");
            var wheelHolder = structure.Find("Cube_004");
            var wheelHolderCol = walkCols.Find("structure").Find(wheelHolder.name);
            //wheelHolderCol.parent = wheelHolderCol.parent.parent;
            //wheel.parent = wheelHolder;
            BoatPartOption bottomHelm = Util.AddPartOption(wheel.gameObject, "helm 1");
            bottomHelm.basePrice = 800;
            bottomHelm.installCost = 450;

            Transform wheel2 = UnityEngine.Object.Instantiate(wheel, container, true);
            wheel2.localPosition = new Vector3(-3.11f, 3.8f, 0f);// new Vector3(1.1f, 0.91f, 0.05f);
            BoatPartOption topHelm = wheel2.GetComponent<BoatPartOption>();
            topHelm.optionName = "helm 2";
            topHelm.requires = new List<BoatPartOption> { container.Find("struct_var_2__balcony_").GetComponent<BoatPartOption>() };
            bottomHelm.walkColObject = wheelHolderCol.gameObject;
            bottomHelm.childOptions = new GameObject[1] { wheelHolder.gameObject };
            partsList.StartCoroutine(AddCopiedPart(structure, walkCols.Find("structure"), topHelm));
            Shipyard
            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption> { bottomHelm, topHelm });
            #endregion

        }
        private static IEnumerator AddCopiedPart(Transform parent, Transform walkCol, BoatPartOption option)
        {
            Debug.Log("trying to add part");
            yield return new WaitUntil(() => PartRefs.sanbuq != null);
            var holes = UnityEngine.Object.Instantiate(PartRefs.sanbuq.Find("structure").Find("Cube_005"), parent, false);
            holes.localPosition = new Vector3(-1.46f, 2.3224f, -1.81f);
            holes.localScale = new Vector3(1.155f, 1.155f, 1.1f);
            var ropes = UnityEngine.Object.Instantiate(PartRefs.sanbuq.Find("steering_ropes"), holes, false);
            ropes.localPosition = new Vector3(-1.37f, -2.01f, 1.9f);
            ropes.localEulerAngles = Vector3.zero;
            //ropes.localScale = new Vector3(0.4f, 0.4f, 0.5f);
            //ropes.GetComponent<MeshRenderer>().material.color = new Color(0.875f, 1, 1);

            yield return new WaitUntil(() => PartRefs.sanbuqCol != null);
            var holesCol = UnityEngine.Object.Instantiate(PartRefs.sanbuqCol.Find("structure").Find("Cube_005"), walkCol, false);
            holesCol.localPosition = holes.localPosition;
            holesCol.localScale = holes.localScale;
            holesCol.localEulerAngles = holes.localEulerAngles;
            option.walkColObject = holesCol.gameObject;
            option.childOptions = new GameObject[1] { holes.gameObject };
            

        }
    }

}
