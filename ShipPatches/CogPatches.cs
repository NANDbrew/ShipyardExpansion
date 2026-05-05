using HarmonyLib;
using SE_Bridge;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class CogPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();
        public static void Patch(Transform boat, BoatCustomParts partsList, BoatRefs boatRefs)
        {
            var walkCol = boatRefs.walkCol;
            Transform container = boatRefs.boatModel;
            Transform structure = container.Find("structure");
            var mainMast1 = structure.Find("mast").GetComponent<BoatPartOption>();
            var mainMast2 = structure.Find("mast_front").GetComponent<BoatPartOption>();
            BoatEmbarkCollider embarkCol = boat.GetComponentInChildren<BoatEmbarkCollider>();

            Plugin.moddedBoats.Add(partsList);

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_cog.prefab");
            AssetTools.PreparePrefab(prefab, boatRefs);
            var rudder = container.Find("rudder").GetComponent<HingeJoint>();
            foreach (var tiller in prefab.GetComponent<SE_BoatCustomData>().tillers)
            {
                tiller.attachedRudder = rudder;
            }
            foreach (var door in prefab.GetComponent<SE_BoatCustomData>().doors)
            {
                door.importedActualBoat = container;
                door.embarkCol = embarkCol;
            }
#if DEBUG
            Debug.Log("SE: instanting cog parts");
#endif
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);
            //Debug.Log("modParts.Count = " + modParts.Count);

            BoatPartOption shrouds_back = modParts["shrouds"].partOptions[0];
            BoatPartOption shrouds_side = modParts["shrouds"].partOptions[1];

            mainMast1.transform.Find("trim_014").gameObject.SetActive(false);
            mainMast1.transform.Find("mast_002").gameObject.SetActive(false);
            mainMast1.transform.Find("Cylinder_002").gameObject.SetActive(false);
            Util.AddChildOptions(mainMast1, new GameObject[] { shrouds_back.transform.GetChild(0).gameObject, shrouds_side.transform.GetChild(0).gameObject, shrouds_back.walkColObject.transform.GetChild(0).gameObject, shrouds_side.walkColObject.transform.GetChild(0).gameObject });
            mainMast1.GetComponent<Mast>().mastCols = mainMast1.GetComponent<Mast>().mastCols.AddRangeToArray(shrouds_back.transform.GetChild(0).GetComponentsInChildren<CapsuleCollider>());
            mainMast1.GetComponent<Mast>().mastCols = mainMast1.GetComponent<Mast>().mastCols.AddRangeToArray(shrouds_side.transform.GetChild(0).GetComponentsInChildren<CapsuleCollider>());

            var mainMast1Col = mainMast1.walkColObject;
            mainMast1Col.transform.Find("trim_014").gameObject.SetActive(false);
            mainMast1Col.transform.Find("mast_002").gameObject.SetActive(false);
            mainMast1Col.transform.Find("Cylinder_002").gameObject.SetActive(false);

            mainMast2.transform.Find("trim_016").gameObject.SetActive(false);
            mainMast2.transform.Find("mast_004").gameObject.SetActive(false);
            mainMast2.transform.Find("Cylinder_004").gameObject.SetActive(false);
            Util.AddChildOptions(mainMast2, new GameObject[] { shrouds_back.transform.GetChild(1).gameObject, shrouds_side.transform.GetChild(1).gameObject, shrouds_back.walkColObject.transform.GetChild(1).gameObject, shrouds_side.walkColObject.transform.GetChild(1).gameObject });
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

#if DEBUG
            Debug.Log("Cog: late adjustments");
#endif
            #region late adjustments
            var ropeHolderAft = container.Find("struct_var_1__low_roof_").Find("mast_003");
            ropeHolderAft.SetParent(mizzenMast);
            mizzenMast.GetChild(2).position = ropeHolderAft.GetChild(0).position;
            mizzenMast.GetChild(2).rotation = ropeHolderAft.GetChild(0).rotation;
            ropeHolderAft.GetChild(0).gameObject.SetActive(false);

            partsList.availableParts[1].category = 2;

            var hammock = thing.GetComponentInChildren<GPButtonBed>(true);
            hammock.damage = boat.GetComponent<BoatDamage>();
            hammock.GetComponent<HingeJoint>().connectedBody = boat.GetComponent<Rigidbody>();


            #endregion
            BoatPartOption bottomHelm = Util.AddPartOption(container.Find("steering_wheel").gameObject, "helm 1");

            bottomHelm.basePrice = 800;
            bottomHelm.installCost = 450;
            bottomHelm.walkColObject = walkCol.Find("structure").Find("Cube_004").gameObject;
            bottomHelm.childOptions = new GameObject[] { structure.Find("Cube_004").gameObject };
            modParts["helm_base_top_2"].partOptions.Insert(0, bottomHelm);

            var modWalkCol = thing.GetComponent<SE_BoatCustomData>().walkCol;
            modWalkCol.SetParent(walkCol, false);

            var deck1 = thing.transform.Find("deck_1").GetComponent<BoatPartOption>();
            deck1.transform.Find("Cylinder").SetParent(mainMast1.transform, true);
            deck1.walkColObject.transform.Find("Cylinder").SetParent(mainMast1Col.transform, true);

            var deck0 = thing.transform.Find("deck_0").GetComponent<BoatPartOption>();
            deck0.childOptions = new GameObject[] { structure.Find("trim_015").gameObject, walkCol.Find("structure/trim_015").gameObject, walkCol.Find("hull").gameObject };

        }

    }

}
