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
    internal class DhowPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.transform.Find("dhow");
            Transform mainMast = container.Find("mast");
            Mast mainMastM = mainMast.GetComponent<Mast>();
            Transform walkCol = mainMastM.walkColMast.parent;

            Transform mainMastTall = container.Find("mast_taller");
            Transform shortForestay = container.Find("forestay_low");
            Transform lowForestay = container.Find("forestay");

            PartRefs.dhow = container;
            PartRefs.dhowCol = walkCol;

            Debug.Log("Dhow adjustments");
            #region adjustments
            partsList.availableParts[1].category = 2;
            partsList.availableParts[4].category = 2;
            shortForestay.localScale = new Vector3(1, 1, 1.02f);
            shortForestay.GetComponent<BoatPartOption>().requires.Remove(container.Find("bowsprit").GetComponent<BoatPartOption>());
            container.Find("empty low forestay").GetComponent<BoatPartOption>().optionName = "(no lower forestay)";


            var mainFlag = container.Find("flag");
            mainFlag.SetParent(mainMast);
            var tallFlag = UnityEngine.Object.Instantiate(mainFlag, mainMastTall, false);
            tallFlag.localPosition += new Vector3(0f, 0f, -0.7f);

            var ropeHolderMain = container.Find("rope_holder_001");
            ropeHolderMain.SetParent(mainMast);
            var tallMastReefAtt = UnityEngine.Object.Instantiate(ropeHolderMain, mainMastTall, false).GetChild(0);
            tallMastReefAtt.parent.localPosition += new Vector3(0f, 0f, 0.3f);
            mainMastTall.GetComponent<Mast>().mastReefAtt[0] = tallMastReefAtt;
            mainMastTall.GetComponent<Mast>().mastReefAtt[1] = tallMastReefAtt;

            /*GameObject winchMount = container.Find("Cylinder").gameObject;
            GameObject staticRig = container.Find("Cylinder_002").gameObject;
            GameObject rigCol = container.Find("rig_col").gameObject;*/
            container.Find("Cylinder").gameObject.SetActive(false);
            container.Find("Cylinder_002").gameObject.SetActive(false);
            container.Find("rig_col").gameObject.SetActive(false);
            container.Find("static_rig").gameObject.SetActive(false);
            container.Find("static_rig_001").gameObject.SetActive(false);

            walkCol.Find("Cylinder").gameObject.SetActive(false);
            walkCol.Find("Cylinder_002").gameObject.SetActive(false);
            walkCol.Find("rig_col").gameObject.SetActive(false);
            walkCol.Find("static_rig").gameObject.SetActive(false);
            walkCol.Find("static_rig_001").gameObject.SetActive(false);


            //mainMast.GetComponent<BoatPartOption>().childOptions = new GameObject[] { staticRig, rigCol, winchMount };
            //GameObject[] TallMastChildren = new GameObject[] { UnityEngine.Object.Instantiate(staticRig, container, true), UnityEngine.Object.Instantiate(rigCol, container, true), UnityEngine.Object.Instantiate(winchMount, container, true) };
            //mainMastTall.GetComponent<BoatPartOption>().childOptions = TallMastChildren;


            //mainMastTall.GetComponent<Mast>().mastCols = mainMastTall.GetComponent<Mast>().mastCols.AddToArray(mainMastTall.GetComponent<BoatPartOption>().childOptions[1].GetComponent<CapsuleCollider>());
            shortForestay.GetComponent<Mast>().mastReefAtt = lowForestay.GetComponent<Mast>().mastReefAtt;
            #endregion

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_dhow.prefab");
            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                mast.shipRigidbody = shipRigidbody;
            }

            Debug.Log("SE: instanting dhow parts");
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);

            var modWalkCol = thing.transform.Find("SE_cols_dhow");
            modWalkCol.SetParent(walkCol, false);

            mainMast.GetComponent<BoatPartOption>().childOptions = new GameObject[] { thing.transform.Find("main_shrouds_side").Find("short_s").gameObject, modWalkCol.transform.Find("main_shrouds_side").Find("short_s").gameObject, thing.transform.Find("main_shrouds_back").Find("short_b").gameObject, modWalkCol.transform.Find("main_shrouds_back").Find("short_b").gameObject };
            mainMastTall.GetComponent<BoatPartOption>().childOptions = new GameObject[] { thing.transform.Find("main_shrouds_side").Find("tall_s").gameObject, modWalkCol.transform.Find("main_shrouds_side").Find("tall_s").gameObject, thing.transform.Find("main_shrouds_back").Find("tall_b").gameObject, modWalkCol.transform.Find("main_shrouds_back").Find("tall_b").gameObject };

            var lowFlag = container.Find("flag_low").gameObject;

            modParts["flag_empty"].partOptions[0].childOptions = modParts["flag_empty"].partOptions[0].childOptions.AddToArray(lowFlag);
            lowFlag.transform.SetParent(thing.transform.Find("main_shrouds_side").Find("short_s"));

            //modParts["main_shrouds_side"].partOptions[0].childOptions = modParts["main_shrouds_side"].partOptions[0].childOptions.AddRangeToArray(new GameObject[] { staticRig, rigCol, TallMastChildren[0], TallMastChildren[1] });


            #region late adjustments
            //highForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
            lowForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[2]);
            shortForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[2]);
            #endregion
        }
    }
}
