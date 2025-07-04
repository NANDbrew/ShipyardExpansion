using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class KakamPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.Find("junk small");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast");
            Transform mainMast2 = structure.Find("mast_center");
            Transform mizzenMast = structure.Find("mast_001");
            Mast mizzenMastM = mizzenMast.GetComponent<Mast>();
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;

            boat.GetComponent<BoatRefs>().walkCol = walkCol; // fix vanilla missing ref

            // add references for save cleaner
            foreach (var part in partsList.availableParts)
            {
                Plugin.stockParts.Add(part, part.activeOption);
            }
            Plugin.moddedBoats.Add(partsList);

            #region adjustments
            // move stays to stays category
            partsList.availableParts[0].category = 2;
            partsList.availableParts[7].category = 2;

            mainMast1.GetComponent<Mast>().mastHeight = 9.7f;
            mainMast2.GetComponent<Mast>().mastHeight = 9.7f;
            //mainMast1.GetComponent<Mast>().startSailHeightOffset -= 0.4f;
            mizzenMastM.leftAngleWinch = new GPButtonRopeWinch[1] { mizzenMastM.leftAngleWinch[1] };
            mizzenMastM.rightAngleWinch = new GPButtonRopeWinch[1] { mizzenMastM.rightAngleWinch[1] };
            mizzenMastM.mastHeight = 8.4f;
            mizzenMastM.extraBottomHeight = 0.6f;
            var m1brace = mizzenMast.Find("mast_holder_001");
            m1brace.localPosition = new Vector3(-1.29f, 0, -10.12f);
            m1brace.localScale = new Vector3(0.87f, 0.87f, 0.94f);
            //mizzenMastM.GetComponent<Mast>().startSailHeightOffset -= 0.5f;
            var ropeHolder_003 = mainMast1.Find("rope_holder_003");
            mainMast1.GetComponent<Mast>().midRopeAtt[0].SetParent(ropeHolder_003);
            ropeHolder_003.localPosition = new Vector3(-3f, 0f, -11.3f);
            ropeHolder_003.localEulerAngles = new Vector3(270f, 0f, 0f);

            container.Find("Cube_002").SetParent(mizzenMast);
            container.Find("Cube_003").SetParent(mizzenMast);
            container.Find("Cube_004").SetParent(structure.Find("struct_var_1__long_roof_"));
            container.Find("hammock_001").GetComponent<BoatPartOption>().childOptions = new GameObject[] { structure.Find("mast_003").gameObject };
            #endregion

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_kakam.prefab");
            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                mast.shipRigidbody = shipRigidbody;
            }
#if DEBUG
            Debug.Log("SE: instanting kakam parts");
#endif
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);

            //mainMast2.GetComponent<Mast>().mastCols = mainMast2.GetComponent<Mast>().mastCols.AddToArray(modParts["crowsnest_empty"].partOptions[2].GetComponentInChildren<CapsuleCollider>());

            var modWalkCol = thing.transform.Find("SE_cols_kakam");
            modWalkCol.SetParent(walkCol, false);

            modParts["shrouds_main_side"].partOptions[0].childOptions = modParts["shrouds_main_side"].partOptions[0].childOptions.AddRangeToArray(new GameObject[] {
                mainMast1.Find("Cylinder_002").gameObject,
                mainMast1.Find("trim_001").gameObject,
                mainMast1.GetComponent<BoatPartOption>().walkColObject.transform.Find("Cylinder_002").gameObject,
                mainMast1.GetComponent<BoatPartOption>().walkColObject.transform.Find("trim_001").gameObject,
                mainMast2.Find("Cylinder_004").gameObject,
                mainMast2.Find("trim_002").gameObject,
                mainMast2.GetComponent<BoatPartOption>().walkColObject.transform.Find("Cylinder_004").gameObject,
                mainMast2.GetComponent<BoatPartOption>().walkColObject.transform.Find("trim_002").gameObject });

            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { 
                modParts["shrouds_main_side"].partOptions[1].transform.Find("shrouds_b_main0").gameObject, 
                modParts["shrouds_main_side"].partOptions[1].walkColObject.transform.Find("shrouds_b_main0").gameObject });
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { 
                modParts["shrouds_main_side"].partOptions[1].transform.Find("shrouds_b_main1").gameObject, 
                modParts["shrouds_main_side"].partOptions[1].walkColObject.transform.Find("shrouds_b_main1").gameObject });

            modParts["shrouds_mizzen_side"].partOptions[0].childOptions = new GameObject[] { 
                mizzenMast.Find("Cylinder_003").gameObject, 
                mizzenMast.Find("trim_000").gameObject, 
                mizzenMastM.walkColMast.Find("Cylinder_003").gameObject, 
                mizzenMastM.walkColMast.Find("trim_000").gameObject };
            mizzenMast.GetComponent<BoatPartOption>().childOptions = mizzenMast.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[]
            {
                modParts["shrouds_mizzen_side"].partOptions[1].transform.GetChild(0).gameObject, 
                modParts["shrouds_mizzen_side"].partOptions[1].walkColObject.transform.GetChild(0).gameObject,
            });
        }

    }
}
