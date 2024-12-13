using HarmonyLib;
using ShipyardExpansion.Scripts;
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
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {

            Transform container = boat.transform.Find("medi small");
            Transform structure = container.Find("structure");
            var mainMast1 = structure.Find("mast").GetComponent<BoatPartOption>();
            var mainMast2 = structure.Find("mast_front").GetComponent<BoatPartOption>();
            var walkCol = mainMast1.walkColObject.transform.parent.parent;
            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_cog.prefab");
            prefab.transform.GetComponentInChildren<GPButtonSteeringWheel>().attachedRudder = container.Find("rudder").GetComponent<HingeJoint>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                //mast.gameObject.SetActive(false);
                mast.shipRigidbody = shipRigidbody;
            }

            Debug.Log("SE: instanting cog parts");
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);
            //Debug.Log("modParts.Count = " + modParts.Count);

            BoatPartOption shrouds_back = modParts["shrouds"].partOptions[0];
            BoatPartOption shrouds_side = modParts["shrouds"].partOptions[1];

            mainMast1.transform.Find("trim_014").gameObject.SetActive(false);
            mainMast1.transform.Find("mast_002").gameObject.SetActive(false);
            mainMast1.transform.Find("Cylinder_002").gameObject.SetActive(false);
            mainMast1.childOptions = mainMast1.childOptions.AddRangeToArray(new GameObject[] { shrouds_back.transform.GetChild(0).gameObject, shrouds_side.transform.GetChild(0).gameObject, shrouds_back.walkColObject.transform.GetChild(0).gameObject, shrouds_side.walkColObject.transform.GetChild(0).gameObject });
            mainMast1.GetComponent<Mast>().mastCols = mainMast1.GetComponent<Mast>().mastCols.AddRangeToArray(shrouds_back.transform.GetChild(0).GetComponentsInChildren<CapsuleCollider>());
            mainMast1.GetComponent<Mast>().mastCols = mainMast1.GetComponent<Mast>().mastCols.AddRangeToArray(shrouds_side.transform.GetChild(0).GetComponentsInChildren<CapsuleCollider>());

            var mainMast1Col = mainMast1.walkColObject;
            mainMast1Col.transform.Find("trim_014").gameObject.SetActive(false);
            mainMast1Col.transform.Find("mast_002").gameObject.SetActive(false);
            mainMast1Col.transform.Find("Cylinder_002").gameObject.SetActive(false);

            mainMast2.transform.Find("trim_016").gameObject.SetActive(false);
            mainMast2.transform.Find("mast_004").gameObject.SetActive(false);
            mainMast2.transform.Find("Cylinder_004").gameObject.SetActive(false);
            mainMast2.childOptions = mainMast2.childOptions.AddRangeToArray(new GameObject[] { shrouds_back.transform.GetChild(1).gameObject, shrouds_side.transform.GetChild(1).gameObject, shrouds_back.walkColObject.transform.GetChild(1).gameObject, shrouds_side.walkColObject.transform.GetChild(1).gameObject });
            mainMast2.GetComponent<Mast>().mastCols = mainMast2.GetComponent<Mast>().mastCols.AddRangeToArray(shrouds_back.transform.GetChild(1).GetComponentsInChildren<CapsuleCollider>());
            mainMast2.GetComponent<Mast>().mastCols = mainMast2.GetComponent<Mast>().mastCols.AddRangeToArray(shrouds_side.transform.GetChild(1).GetComponentsInChildren<CapsuleCollider>());

            var mainMast2Col = mainMast2.walkColObject;
            mainMast2Col.transform.Find("trim_016").gameObject.SetActive(false);
            mainMast2Col.transform.Find("mast_004").gameObject.SetActive(false);
            mainMast2Col.transform.Find("Cylinder_004").gameObject.SetActive(false);

            //partsList.availableParts.Add(new BoatPart { category = 1, activeOption = 0, partOptions = new List<BoatPartOption> { shrouds_back, shrouds_side } });

            Transform mastWalkCol = mainMast1.GetComponent<Mast>().walkColMast;
            Transform walkCols = mastWalkCol.parent.parent;
            Transform bowspritM = structure.Find("mast (bowsprit)");
            Transform bowsprit = structure.Find("mast_001");
            Transform forestay = container.Find("mast forestay");
            Mast mainMast1M = mainMast1.GetComponent<Mast>();
            Transform mizzenMast = structure.Find("mast_mizzen");
            Mast mizzenMastM = mizzenMast.GetComponent<Mast>();
            PartRefs.cog = container;
            PartRefs.cogCol = walkCols;


            #region adjustments
            mainMast1M.mastHeight += 1.2f;//= 11.5f;
            mainMast1M.extraBottomHeight = 1f;
            mainMast2.GetComponent<Mast>().mastHeight = 12f;//= 11.5f;
            mainMast2.GetComponent<Mast>().extraBottomHeight = 0f;
            mizzenMastM.mastHeight = 9.5f;
            mizzenMastM.extraBottomHeight = 0f;
            forestay.GetComponent<Mast>().mastHeight = 12.3f;
            #endregion



            //Debug.Log("Cog: bowsprit");
            #region sprit
            BoatPartOption bowspritOpt = Util.AddPartOption(bowspritM.gameObject, "bowsprit");
            bowspritOpt.basePrice = 500;
            bowspritOpt.installCost = 200;
            bowspritOpt.mass = 20;
            bowspritOpt.childOptions = new GameObject[1] { bowsprit.gameObject };
            bowspritOpt.walkColObject = walkCol.Find("structure").Find("mast_001").gameObject;
            modParts["bowsprit_empty"].partOptions.Insert(0, bowspritOpt);

            #endregion
         
            Debug.Log("Cog: late adjustments");
            #region late adjustments
            var ropeHolderAft = container.Find("struct_var_1__low_roof_").Find("mast_003");
            ropeHolderAft.SetParent(mizzenMast);
            mizzenMast.GetChild(2).position = ropeHolderAft.GetChild(0).position;
            mizzenMast.GetChild(2).rotation = ropeHolderAft.GetChild(0).rotation;
            ropeHolderAft.GetChild(0).gameObject.SetActive(false);

            partsList.availableParts[1].category = 2;
            //partsList.availableParts[1].partOptions.Add(Util.CreatePartOption(container, "(no forestay)", "(no forestay)"));

            #endregion
            BoatPartOption bottomHelm = Util.AddPartOption(container.GetComponentInChildren<GPButtonSteeringWheel>().gameObject, "helm 1");

            bottomHelm.basePrice = 800;
            bottomHelm.installCost = 450;
            bottomHelm.walkColObject = walkCol.Find("structure").Find("Cube_004").gameObject;
            bottomHelm.childOptions = new GameObject[] { structure.Find("Cube_004").gameObject };
            modParts["helm_base_top_2"].partOptions.Insert(0, bottomHelm);

            var modWalkCol = thing.transform.Find("SE_cols_cog");
            modWalkCol.SetParent(walkCol, false);

            try
            {
                var col = container.Find("embark_col").GetComponent<MeshCollider>();
                Debug.Log(col);
                var newMesh = thing.transform.Find("embark_col").GetComponent<MeshFilter>();
                Debug.Log(newMesh);
                col.sharedMesh = newMesh.sharedMesh;
                //col.mesh = newMesh.mesh;

            }
            catch { Debug.Log("couldn't patch cog embark"); }

            /*            #region bermuda mast
                        Mast bermudaMast = Util.CopyMast(mainMast1, new Vector3(-1, 0, 13.5f), new Vector3(0, 340, 0), new Vector3(1, 1, 1.04f), "mast_bermuda", "bermuda mast", 39);
                        //BoatPartOption bermudaMastOpt = bermudaMast.GetComponent<BoatPartOption>();
                        for (int i = 0; i < bermudaMast.transform.childCount; i++)
                        {
                            Transform child = bermudaMast.transform.GetChild(i);
                            if (child.name == "wind_vane") child.gameObject.SetActive(false);
                            if (child.name == "wind_vane_arrow") child.gameObject.SetActive(false);
                            if (child.name == "rope_holder_000") child.gameObject.SetActive(false);
                            if (child.name == "rope_holder_003") child.gameObject.SetActive(false);
                            //if (child.name == "rope_holder_002 (1)") child.gameObject.SetActive(false);
                            if (child.name == "rope_holder_004") child.gameObject.SetActive(false);
                        }
                        Transform rope_holder_low = bermudaMast.transform.Find("rope_holder_004 (1)");
                        rope_holder_low.localPosition = new Vector3(-4, 0, -12.52f);
                        rope_holder_low.localEulerAngles = new Vector3(270, 22, 0);
                        bermudaMast.leftAngleWinch = bermudaMast.leftAngleWinch.AddRangeToArray(midstay.leftAngleWinch);
                        bermudaMast.rightAngleWinch = bermudaMast.rightAngleWinch.AddRangeToArray(midstay.rightAngleWinch);
                        bermudaMast.mastReefAtt[0].parent.localEulerAngles = new Vector3(270, 279, 0);
                        bermudaMast.mastReefAtt[0].parent.localPosition = new Vector3(-0.24f, 0f, 2f);
                        bermudaMast.mastReefAtt = new Transform[] { bermudaMast.mastReefAtt[0], bermudaMast.transform.Find("rope_holder_002 (1)").GetChild(0) };
                        bermudaMast.onlyStaysails = true;
                        bermudaMast.onlySquareSails = true;
                        bermudaMast.mastHeight = 13f;
                        bermudaMast.extraBottomHeight = 1.5f;
                        partsList.availableParts[0].partOptions.Add(bermudaMast.GetComponent<BoatPartOption>());
                        #endregion

                        #region bermuda forestay1
                        Mast forestay_bermuda_0 = Util.CopyMast(forestay, new Vector3(0.75f, 12.98f, 0f), new Vector3(311, 270, 90), new Vector3(1, 1, 1.06f), "forestay_bermuda_0", "forestay 3", 40);
                        forestay_bermuda_0.reefWinch[0].transform.parent = bermudaMast.transform;
                        forestay_bermuda_0.reefWinch[0].transform.localPosition = new Vector3(0.31f, 0, -12.7f);
                        forestay_bermuda_0.reefWinch[0].transform.localEulerAngles = new Vector3(0, 90, 0);
                        forestay_bermuda_0.reefWinch[0].rope = null;
                        forestay_bermuda_0.mastReefAtt = new Transform[] { bermudaMast.mastReefAtt.Last() };
                        //forestay_bermuda_0.mastHeight = 12.7f;
                        forestay_bermuda_0.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { bermudaMast.GetComponent<BoatPartOption>() };
                        forestay_bermuda_0.GetComponent<BoatPartOption>().requiresDisabled = new List<BoatPartOption> { bowspritNone };
                        partsList.availableParts[1].partOptions.Add(forestay_bermuda_0.GetComponent<BoatPartOption>());

                        #endregion
                        #region bermuda forestay2
                        Mast forestay_bermuda_1 = Util.CopyMast(forestay_bermuda_0.transform, new Vector3(1.2f, 12.98f, 0f), new Vector3(319.5f, 270, 90), new Vector3(1, 1, 1.09f), "forestay_bermuda_1", "forestay 3 long", 41);
                        forestay_bermuda_1.reefWinch = forestay_bermuda_0.reefWinch;
                        forestay_bermuda_1.mastReefAtt = new Transform[] { bermudaMast.mastReefAtt.Last() };
                        //forestay_bermuda_1.mastHeight = 12.9f;
                        forestay_bermuda_1.GetComponent<BoatPartOption>().requires = new List<BoatPartOption> { bowspritLongOpt, bermudaMast.GetComponent<BoatPartOption>() };
                        partsList.availableParts[1].partOptions.Add(forestay_bermuda_1.GetComponent<BoatPartOption>());
                        *//*foreach (var partOption in partsList.availableParts[0].partOptions)
                        {
                            if (partOption.requires.Contains(mainMast1.GetComponent<BoatPartOption>()))
                            {
                                partOption.requires.Remove(mainMast1.GetComponent<BoatPartOption>());
                                partOption.requiresDisabled.Add(mainMast2.GetComponent<BoatPartOption>());
                            }
                        }*//*
                        #endregion
            */
        }

    }

}
