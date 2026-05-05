using HarmonyLib;
using SE_Bridge;
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
        public static void Patch(Transform boat, BoatCustomParts partsList, BoatRefs boatRefs)
        {
            Transform container = boat.transform.Find("dhow");
            Transform mainMast = container.Find("mast");
            Mast mainMastM = mainMast.GetComponent<Mast>();
            Transform walkCol = mainMastM.walkColMast.parent;

            Transform mainMastTall = container.Find("mast_taller");
            Mast mainMastTallM = mainMastTall.GetComponent<Mast>();
            Transform shortForestay = container.Find("forestay_low");
            Transform lowForestay = container.Find("forestay");

            Plugin.moddedBoats.Add(partsList);

#if DEBUG
            Debug.Log("Dhow adjustments");
#endif
            #region adjustments

            partsList.availableParts[1].category = 2;
            partsList.availableParts[4].category = 2;
            shortForestay.localScale = new Vector3(1, 1, 1.02f);
            shortForestay.GetComponent<BoatPartOption>().requires.Remove(container.Find("bowsprit").GetComponent<BoatPartOption>());
            container.Find("empty low forestay").GetComponent<BoatPartOption>().optionName = "(no lower forestay)";
            container.Find("mooring_attachment_000").parent = mainMast;
            var mainFlag = container.Find("flag");
            mainFlag.SetParent(mainMast);
            var tallFlag = UnityEngine.Object.Instantiate(mainFlag, mainMastTall, false);
            tallFlag.localPosition += new Vector3(0f, 0f, -0.7f);

            var ropeHolderMain = container.Find("rope_holder_001");
            ropeHolderMain.SetParent(mainMast);
            var tallMastReefAtt = UnityEngine.Object.Instantiate(ropeHolderMain, mainMastTall, false).GetChild(0);
            tallMastReefAtt.parent.localPosition += new Vector3(0f, 0f, 0.3f);
            mainMastTallM.mastReefAtt[0] = tallMastReefAtt;
            mainMastTallM.mastReefAtt[1] = tallMastReefAtt;
            //mainMastTall.GetComponent<CapsuleCollider>().radius = 0.2f;

            //mainMast.GetComponent<CapsuleCollider>().radius = 0.2f;

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

            shortForestay.GetComponent<Mast>().mastReefAtt = lowForestay.GetComponent<Mast>().mastReefAtt;
            #endregion

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_dhow.prefab");
            AssetTools.PreparePrefab(prefab, boatRefs);
#if DEBUG
            Debug.Log("SE: instanting dhow parts");
#endif
            var rudder = container.Find("rudder").GetComponent<HingeJoint>();
            foreach (var tiller in prefab.GetComponent<SE_BoatCustomData>().tillers)
            {
                tiller.attachedRudder = rudder;
            }

            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);

            var modWalkCol = thing.GetComponent<SE_BoatCustomData>().walkCol;
            modWalkCol.SetParent(walkCol, false);

            Util.AddChildOptions(mainMast.GetComponent<BoatPartOption>(), new GameObject[] { thing.transform.Find("main_shrouds_side").Find("short_s").gameObject, modWalkCol.transform.Find("main_shrouds_side").Find("short_s").gameObject, thing.transform.Find("main_shrouds_back").Find("short_b").gameObject, modWalkCol.transform.Find("main_shrouds_back").Find("short_b").gameObject });
            Util.AddChildOptions(mainMastTall.GetComponent<BoatPartOption>(), new GameObject[] { thing.transform.Find("main_shrouds_side").Find("tall_s").gameObject, modWalkCol.transform.Find("main_shrouds_side").Find("tall_s").gameObject, thing.transform.Find("main_shrouds_back").Find("tall_b").gameObject, modWalkCol.transform.Find("main_shrouds_back").Find("tall_b").gameObject });

            var lowFlag = container.Find("flag_low").gameObject;

            modParts["flag_empty"].partOptions[0].childOptions = modParts["flag_empty"].partOptions[0].childOptions.AddToArray(lowFlag);
            lowFlag.transform.SetParent(thing.transform.Find("main_shrouds_side").Find("short_s"));

            //mainMastTallM.mastCols = mainMastTallM.mastCols.AddToArray(thing.transform.Find("crowsnest_low").GetComponent<CapsuleCollider>());
            //Util.AddChildOptions(modParts["main_shrouds_side"].partOptions[0], new GameObject[] { staticRig, rigCol, TallMastChildren[0], TallMastChildren[1] });

            modParts["net_0"].partOptions[0].childOptions = new GameObject[]{ container.Find("hammock_001").gameObject };
            //var table = modParts["net_0"].partOptions[1].transform.GetChild(0).gameObject.AddComponent<StaticTable>();
            //table.allowPlacingItems = true;

            BoatPartOption wheel = Util.AddPartOption(container.Find("steering_wheel").gameObject, "steering wheel");

            wheel.basePrice = 600;
            wheel.installCost = 350;
            wheel.walkColObject = walkCol.Find("Cube_004").gameObject;
            wheel.childOptions = new GameObject[] { container.Find("Cube_004").gameObject, container.Find("rope_holder").gameObject };
            modParts["tiller_container"].partOptions.Insert(0, wheel);

            var switcher = modParts["tiller_container"].partOptions[1].GetComponentInChildren<RopeAttAutoSwitcher>();
            Transform switchingAtt = (Transform)AccessTools.Field(switcher.GetType(), "att").GetValue(switcher);
            switcher.otherParent = mainMastM.midRopeAtt[0];
            for (int i = 0; i < mainMastM.midRopeAtt.Length; i++)
            {
                mainMastM.midRopeAtt[i] = switchingAtt;
            }
            for (int i = 0; i < mainMastTallM.midRopeAtt.Length; i++)
            {
                mainMastTallM.midRopeAtt[i] = switchingAtt;
            }

            #region late adjustments
            //highForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
            lowForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[2]);
            lowForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[3]);
            shortForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[2]);
            shortForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[3]);

            try
            {
                walkCol.Find("flag_low").GetComponent<MeshCollider>().enabled = false;
            } 
            catch 
            {
                Debug.Log("didn't patch dhow flag collider"); 
            }
            #endregion
        }
    }

}
