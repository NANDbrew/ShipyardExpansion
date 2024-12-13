using cakeslice;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class JunkPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();

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

            midstaySource.GetComponent<Mast>().reefWinch[0].transform.SetParent(container);

            partsList.availableParts[4].category = 2;
            partsList.availableParts[5].category = 2;
            partsList.availableParts[6].category = 2;
            partsList.availableParts[7].category = 2;

            midstaySource.GetComponent<BoatPartOption>().requires.Remove(mizzenMast2.GetComponent<BoatPartOption>());
            midstaySource.GetComponent<BoatPartOption>().requiresDisabled = new List<BoatPartOption> { mizzenMast1.GetComponent<BoatPartOption>(), structure.Find("mast_mizzen_(empty)").GetComponent<BoatPartOption>() };
            forestaySource.GetComponent<BoatPartOption>().requires.Add(bowsprit.GetComponent<BoatPartOption>());

            #endregion

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_junk.prefab");
            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                mast.shipRigidbody = shipRigidbody;
            }

            Debug.Log("SE: instanting junk parts");
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);

            //mainMast2.GetComponent<Mast>().mastCols = mainMast2.GetComponent<Mast>().mastCols.AddToArray(modParts["crowsnest_empty"].partOptions[2].GetComponentInChildren<CapsuleCollider>());

            var modWalkCol = thing.transform.Find("SE_cols_junk");
            modWalkCol.SetParent(walkCol, false);

            try
            {
                var col = container.GetComponentInChildren<BoatEmbarkCollider>().GetComponent<MeshCollider>();
                Debug.Log(col);
                Mesh newMesh = thing.transform.Find("embark_col").GetComponent<MeshFilter>().sharedMesh;
                Debug.Log(newMesh);
                col.sharedMesh = newMesh;
                col.GetComponent<MeshFilter>().sharedMesh = newMesh;

            }
            catch { Debug.Log("couldn't patch junk embark"); }

            #region shrouds
            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] {
                thing.transform.Find("shrouds_main_side").Find("mast_mid_0").gameObject,
                modWalkCol.transform.Find("shrouds_main_side").Find("mast_mid_0").gameObject,
                thing.transform.Find("shrouds_main_back").Find("mast_mid_0").gameObject,
                modWalkCol.transform.Find("shrouds_main_back").Find("mast_mid_0").gameObject, });
            mainMast1.Find("static_rig_001").gameObject.SetActive(false);
            mainMast1.GetComponent<Mast>().walkColMast.Find("static_rig_001").gameObject.SetActive(false);
            mainMast1.GetComponent<MeshFilter>().sharedMesh = thing.transform.Find("mast_meshes").Find("mast_mid_0").GetComponent<MeshFilter>().sharedMesh;
            mainMast1.GetComponent<Mast>().walkColMast.GetComponent<MeshCollider>().sharedMesh = thing.transform.Find("mast_meshes").Find("mast_mid_0").GetComponent<MeshFilter>().sharedMesh;

            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] {
                thing.transform.Find("shrouds_main_side").Find("mast_mid_1").gameObject,
                modWalkCol.transform.Find("shrouds_main_side").Find("mast_mid_1").gameObject,
                thing.transform.Find("shrouds_main_back").Find("mast_mid_1").gameObject,
                modWalkCol.transform.Find("shrouds_main_back").Find("mast_mid_1").gameObject, });
            mainMast2.Find("static_rig_000").gameObject.SetActive(false);
            mainMast2.GetComponent<Mast>().walkColMast.Find("static_rig_000").gameObject.SetActive(false);
            mainMast2.GetComponent<MeshFilter>().sharedMesh = thing.transform.Find("mast_meshes").Find("mast_mid_1").GetComponent<MeshFilter>().sharedMesh;
            mainMast2.GetComponent<Mast>().walkColMast.GetComponent<MeshCollider>().sharedMesh = thing.transform.Find("mast_meshes").Find("mast_mid_1").GetComponent<MeshFilter>().sharedMesh;

            mizzenMast2.GetComponent<BoatPartOption>().childOptions = mizzenMast2.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] {
                thing.transform.Find("shrouds_miz_side").Find("mast_miz_1").gameObject,
                modWalkCol.transform.Find("shrouds_miz_side").Find("mast_miz_1").gameObject,
                thing.transform.Find("shrouds_miz_back").Find("mast_miz_1").gameObject,
                modWalkCol.transform.Find("shrouds_miz_back").Find("mast_miz_1").gameObject, });
            mizzenMast2.Find("static_rig_012").gameObject.SetActive(false);
            mizzenMast2.GetComponent<Mast>().walkColMast.Find("static_rig_012").gameObject.SetActive(false);
            mizzenMast2.GetComponent<MeshFilter>().sharedMesh = thing.transform.Find("mast_meshes").Find("mast_mizzen_1").GetComponent<MeshFilter>().sharedMesh;
            mizzenMast2.GetComponent<Mast>().walkColMast.GetComponent<MeshCollider>().sharedMesh = thing.transform.Find("mast_meshes").Find("mast_mizzen_1").GetComponent<MeshFilter>().sharedMesh;

            #endregion

            #region bed
            Transform bed = container.Find("bed");
            #endregion


            //BoatPart jibboomPart = Util.CreateAndAddPart(partsList, 1, new List<BoatPartOption> { Util.CreatePartOption(structure, "jibboom_none", "(no jibboom)") });

            #region late Adjustments
            //outerFstaySource.GetComponent<Mast>().mastReefAttExtension = main1ExtList;
            #endregion
        }
    }

}
