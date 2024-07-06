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
            Transform foreMast = structure.Find("mast_front_");
            Transform bowsprit = structure.Find("mast_bowsprit");
            Transform walkColStruct = mainMast1.GetComponent<Mast>().walkColMast.parent;
            Transform walkCol = walkColStruct.parent;
            Mast mizzenMast2M = mizzenMast2.GetComponent<Mast>();

            PartRefs.junk = container;
            PartRefs.junkCol = walkCol;

            #region adjustments
            mainMast1.GetComponent<Mast>().mastHeight += 1f;//= 17.5f;
            mainMast2.GetComponent<Mast>().mastHeight += 1.8f;//= 18.5f;
            mainMast2.GetComponent<Mast>().extraBottomHeight = 3f;//= 18.5f;
            mizzenMast1.GetComponent<Mast>().mastHeight += 2f;//= 12.6f;
            mizzenMast2M.mastHeight += 0.8f;//= 12.6f;
            foreMast.GetComponent<Mast>().mastHeight += 2f;//= 11.6f;

/*            mainMast1.GetComponent<Mast>().startSailHeightOffset += 1f;//= 17.5f;
            mainMast2.GetComponent<Mast>().startSailHeightOffset += 1.8f; //= 18.5f;
            mizzenMast1.GetComponent<Mast>().startSailHeightOffset = -1.5f;//= 12.6f;
            mizzenMast2M.startSailHeightOffset += 0.8f;//= 12.6f;
            foreMast.GetComponent<Mast>().startSailHeightOffset = -1.5f;//= 11.6f;*/

            container.Find("forestay_1").GetComponent<Mast>().mastHeight = 20;
            container.Find("forestay_1_lower").GetComponent<Mast>().mastHeight = 14;


            partsList.availableParts[4].category = 2;
            partsList.availableParts[5].category = 2;
            partsList.availableParts[6].category = 2;
            partsList.availableParts[7].category = 2;

            #endregion

            #region telltale
            var flagSource = structure.Find("mast_front_").Find("flag_cloth");
            BoatPartOption noFlag = Util.CreatePartOption(container, "(flag empty)", "(no telltale)");

            BoatPartOption flags_main = Util.CreatePartOption(container, "flag_main", "mainmast telltale");
            flags_main.basePrice = 50;
            flags_main.installCost = 10;

            Transform flags_main_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_0" }.transform, flags_main.transform);
            var flag_main_0_side = UnityEngine.Object.Instantiate(flagSource, flags_main_0);
            flag_main_0_side.name = "flag_main_0_side";
            flag_main_0_side.localPosition = new Vector3(1, 3, 2.9f);
            flag_main_0_side.localEulerAngles = new Vector3(355, 0, 357);
            //flag_main_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);

            Transform flags_main_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_main_1" }.transform, flags_main.transform);
            var flag_main_1_side = UnityEngine.Object.Instantiate(flagSource, flags_main_1);
            flag_main_1_side.name = "flag_main_1_side";
            flag_main_1_side.localPosition = new Vector3(4.75f, 3, 2.8f);
            flag_main_1_side.localEulerAngles = new Vector3(355, 0, 375);
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

            Transform flags_mizzen_0 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_0" }.transform, flags_mizzen.transform);
            var flag_mizzen_0_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_0);
            flag_mizzen_0_side.name = "flag_mizzen_0_side";
            flag_mizzen_0_side.localPosition = new Vector3(-10.2f, 3.1f, 1.6f);
            flag_mizzen_0_side.localEulerAngles = new Vector3(356, 0, 357);
            //flag_mizzen_0_side.localScale = new Vector3(0.8f, 1f, 0.5f);

            Transform flags_mizzen_1 = UnityEngine.Object.Instantiate(new GameObject() { name = "flags_mizzen_1" }.transform, flags_mizzen.transform);
            var flag_mizzen_1_side = UnityEngine.Object.Instantiate(flagSource, flags_mizzen_1);
            flag_mizzen_1_side.name = "flag_mizzen_1_side";
            flag_mizzen_1_side.localPosition = new Vector3(-5.17f, 3, 2.61f);
            flag_mizzen_1_side.localEulerAngles = new Vector3(354, 0, 357);
            //flag_mizzen_1_side.localScale = new Vector3(0.8f, 1f, 0.5f);


            //flags_fore.requires.Add(foreMast.GetComponent<BoatPartOption>());
            //flags_main.requiresDisabled.Add(container.Find("(no mainmast)").GetComponent<BoatPartOption>());
            flags_mizzen.requiresDisabled.Add(structure.Find("mast_mizzen_(empty)").GetComponent<BoatPartOption>());
            Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption>() { noFlag, flags_main, flags_mizzen });


            mizzenMast1.GetComponent<BoatPartOption>().childOptions = mizzenMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_0.gameObject);
            mizzenMast2.GetComponent<BoatPartOption>().childOptions = mizzenMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_mizzen_1.gameObject);

            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_0.gameObject);
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_main_1.gameObject);

            //foreMast.GetComponent<BoatPartOption>().childOptions = foreMast.GetComponent<BoatPartOption>().childOptions.AddToArray(flags_fore_0.gameObject);

            #endregion

            #region flatsprit
            Mast flatsprit = Util.CopyMast(bowsprit, new Vector3(20.75f, 0f, 5.6f), new Vector3(0, 67, 0), Vector3.one, "bowsprit_2", "bowsprit 2", 63);
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
            var forestaySource = container.Find("forestay_foremast");
            Mast forestayF = Util.CopyMast(forestaySource, forestaySource.localPosition, new Vector3(309.873f, 270f, 90f), new Vector3(1, 1, 1.24f), "forestay_foremast_flat", "foremast forestay 2", 34);
            forestayF.mastHeight = 13.5f;
            forestayF.GetComponent<BoatPartOption>().requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF.GetComponent<BoatPartOption>().requires.Add(flatsprit.GetComponent<BoatPartOption>());
            partsList.availableParts[4].partOptions.Add(forestayF.GetComponent<BoatPartOption>());

            var forestaySource2 = container.Find("forestay_1");
            Mast forestayF2 = Util.CopyMast(forestaySource2, forestaySource2.localPosition, new Vector3(310.2f, 270f, 90f), new Vector3(1, 1, 1.14f), "forestay_1_flat", "forestay 3", 35);
            forestayF2.mastHeight = 21f;
            forestayF2.GetComponent<BoatPartOption>().requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF2.GetComponent<BoatPartOption>().requires.Add(flatsprit.GetComponent<BoatPartOption>());
            partsList.availableParts[4].partOptions.Add(forestayF2.GetComponent<BoatPartOption>());

            var forestaySource3 = container.Find("forestay_1_lower");
            Mast forestayF3 = Util.CopyMast(forestaySource3, forestaySource3.localPosition, forestaySource3.localEulerAngles, new Vector3(1, 1, 1.043f), "forestay_1_lower_flat", "lower forestay 3", 36);
            forestayF3.mastHeight = 14.5f;
            forestayF3.GetComponent<BoatPartOption>().requires.Remove(bowsprit.GetComponent<BoatPartOption>());
            forestayF3.GetComponent<BoatPartOption>().requires.Add(flatsprit.GetComponent<BoatPartOption>());
            partsList.availableParts[6].partOptions.Add(forestayF3.GetComponent<BoatPartOption>());
            #endregion

            #region foreward midstay
            var midstaySource = container.Find("midstay_1-1");
            Mast midstay = Util.CopyMast(midstaySource, new Vector3(-3.65f, 23.5f, 0f), midstaySource.localEulerAngles, new Vector3(1, 1, 0.98f), "midstay_topmast", "top middlestay", 32);
            midstay.reefWinch = Util.CopyWinches(midstay.reefWinch, Vector3.zero, new Vector3(0f, -0.28f, -13.274f));
            midstay.leftAngleWinch = Util.CopyWinches(midstay.leftAngleWinch, Vector3.zero, new Vector3(-0.4f, 0, 0));
            midstay.rightAngleWinch = Util.CopyWinches(midstay.rightAngleWinch, Vector3.zero, new Vector3(-0.4f, 0, 0));
            BoatPartOption midstayOpt = midstay.GetComponent<BoatPartOption>();
            BoatPartOption midstayNone = Util.CreatePartOption(container, "(no top middlestay)", "(no top middlestay)");
            Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { midstayNone, midstayOpt });
            midstayOpt.requires = new List<BoatPartOption> { mizzenMast2.GetComponent<BoatPartOption>(), mainMast2.GetComponent<BoatPartOption>() };
            #endregion

            #region topmast
            partsList.StartCoroutine(AddTopmast(mizzenMast2M, structure, walkColStruct, partsList));
            #endregion


        }
        private static IEnumerator AddTopmast(Mast mizzenMast2M, Transform structure, Transform walkColStruct, BoatCustomParts partsList)
        {
            Debug.Log("waiting for topmast...");
            yield return new WaitUntil(() => PartRefs.sanbuq != null);
            Debug.Log("found topmast");
            Mast topMast1 = Util.CopyMast(PartRefs.sanbuq.Find("structure").Find("mast_0_extension"), structure, walkColStruct, new Vector3(-4.03f, 0f, 23.45f), Vector3.zero, new Vector3(0.75f, 0.75f, 0.75f), "mizzen_topmast", "mizzen topmast", 33);
            topMast1.reefWinch = Util.CopyWinches(mizzenMast2M.reefWinch, Vector3.zero, Vector3.zero);
            topMast1.reefWinch[0].transform.localPosition = new Vector3(-0.3f, 0, -13.793f);
            topMast1.reefWinch[0].transform.localEulerAngles = new Vector3(0, 270, 90);
            topMast1.midAngleWinch = new GPButtonRopeWinch[1] { Util.CopyWinch(mizzenMast2M.midAngleWinch[1], new Vector3(-9f, 2.52f, 1.46f)) };
            topMast1.leftAngleWinch = new GPButtonRopeWinch[1] { mizzenMast2M.leftAngleWinch[1] };
            topMast1.rightAngleWinch = new GPButtonRopeWinch[1] { mizzenMast2M.rightAngleWinch[1] };
            topMast1.midRopeAtt = mizzenMast2M.midRopeAtt;

            topMast1.GetComponent<MeshRenderer>().material.color = new Color(0.65f, 0.7f, 0.7f);

            //Debug.Log("copied topmast");
            BoatPartOption topMast1None = Util.CreatePartOption(structure, "empty_topmast", "(no topmast)");
            BoatPart topmastPart = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { topMast1None, topMast1.GetComponent<BoatPartOption>() });
            topMast1.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { mizzenMast2M.GetComponent<BoatPartOption>() };
            topmastPart.SetOptionEnabled(1, false);
            mizzenMast2M.leftAngleWinch = new GPButtonRopeWinch[1] { mizzenMast2M.leftAngleWinch[0] };
            mizzenMast2M.rightAngleWinch = new GPButtonRopeWinch[1] { mizzenMast2M.rightAngleWinch[0] };
            //Debug.Log("hacked winches");
        }
    }

}
