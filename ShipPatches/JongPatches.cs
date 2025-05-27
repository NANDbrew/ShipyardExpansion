using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.ShipPatches
{
    internal class JongPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();

        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            //Mast[] masts = boat.GetComponent<BoatRefs>().masts;
            //var boatRefs = boat.GetComponent<BoatRefs>();
            Transform container = boat.transform.Find("junk large (3)");
            Transform subContainer = container.Find(container.name);
            Transform structure = subContainer.Find("structure");
            Transform staysContainer = subContainer.Find("masts_stays");

            Transform mainMast1 = structure.Find("masts_structure").Find("mast_main_1");
            Mast mainMast1M = mainMast1.GetComponent<Mast>();
            var mainMast2M = mainMast1.parent.Find("mast_main_2").GetComponent<Mast>();

            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;

            // Plugin.topmastRef = topMast1;
            //PartRefs.jong = container;
            //PartRefs.jongCol = walkCol;

            // add references for save cleaner
            foreach (var part in partsList.availableParts)
            {
                Plugin.stockParts.Add(part, part.activeOption);
            }
            Plugin.moddedBoats.Add(partsList);

            #region adjustments
            foreach (Mast stay in staysContainer.GetComponentsInChildren<Mast>())
            {
                stay.mastHeight += 1f;
                stay.GetComponent<BoatPartOption>().mass = Mathf.RoundToInt(stay.mastHeight);
            }
            mainMast1M.extraBottomHeight = 0.5f;
            mainMast1M.mastHeight = 23f;
            mainMast2M.mastHeight = 24f;

            int cabinMass = 600;
            foreach (var partOption in partsList.availableParts[16].partOptions)
            {
                partOption.mass += cabinMass;
                partOption.basePrice += cabinMass;
            }
            AccessTools.Field(typeof(BoatMass), "selfMass").SetValue(boat.GetComponent<BoatMass>(), (float)AccessTools.Field(typeof(BoatMass), "selfMass").GetValue(boat.GetComponent<BoatMass>()) - cabinMass);

            #endregion

            var prefab = AssetTools.bundle2.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_jong.prefab");

            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                mast.shipRigidbody = shipRigidbody;
            }
            Debug.Log("SE: instanting jong parts");
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);
            var modWalkCol = thing.transform.Find("SE_cols_jong");
            modWalkCol.SetParent(walkCol, false);

            /*var testMast = Util.CopyMast(mainMast1, mainMast1.transform.position + (Vector3.forward * 2), "testMast", "test mast", 67);
            partsList.availableParts[3].partOptions.Add(testMast.GetComponent<BoatPartOption>());*/


            #region late adjustments
            Debug.Log("jong late adjustments");
            Mast forestay1 = staysContainer.Find("front_stay_mainmast-bowsprit").GetComponent<Mast>();
            forestay1.reefWinch = forestay1.reefWinch.AddToArray(thing.transform.Find("winches").Find("winch_stay_main_reef (1)").GetComponent<GPButtonRopeWinch>());
            forestay1.leftAngleWinch = forestay1.leftAngleWinch.AddToArray(thing.transform.Find("winches").Find("winch_stay_front_left (3)").GetComponent<GPButtonRopeWinch>());
            forestay1.rightAngleWinch = forestay1.rightAngleWinch.AddToArray(thing.transform.Find("winches").Find("winch_stay_front_right (3)").GetComponent<GPButtonRopeWinch>());
            forestay1.maxSails = 2;
            forestay1.mastReefAtt = forestay1.mastReefAtt.AddToArray(forestay1.mastReefAtt[0]);

            Mast forestay2 = staysContainer.Find("front_stay_mainmast-hull").GetComponent<Mast>();
            forestay2.reefWinch = forestay1.reefWinch.AddToArray(thing.transform.Find("winches").Find("winch_stay_main_reef (1)").GetComponent<GPButtonRopeWinch>());
            forestay2.leftAngleWinch = forestay2.leftAngleWinch.AddToArray(thing.transform.Find("winches").Find("winch_stay_front_left (3)").GetComponent<GPButtonRopeWinch>());
            forestay2.rightAngleWinch = forestay2.rightAngleWinch.AddToArray(thing.transform.Find("winches").Find("winch_stay_front_right (3)").GetComponent<GPButtonRopeWinch>());
            forestay2.maxSails = 2;
            forestay2.mastReefAtt = forestay2.mastReefAtt.AddToArray(forestay2.mastReefAtt[0]);

            mainMast1M.midRopeAtt[0].gameObject.SetActive(false);
            mainMast1M.midRopeAtt = thing.transform.Find("mast_main_1_f").GetComponent<Mast>().midRopeAtt;
            mainMast2M.midRopeAtt[0].parent.gameObject.SetActive(false);
            mainMast2M.midRopeAtt = thing.transform.Find("mast_main_2_f").GetComponent<Mast>().midRopeAtt;

            mainMast1M.maxSails += 1;
            mainMast1M.reefWinch = mainMast1M.reefWinch.AddToArray(thing.transform.Find("winches/winch_front_reef_extra").GetComponent<GPButtonRopeWinch>());
            mainMast1M.midAngleWinch = mainMast1M.midAngleWinch.AddToArray(thing.transform.Find("winches/winch_angle_main_front (3)").GetComponent<GPButtonRopeWinch>());
            mainMast1M.mastReefAtt = mainMast1M.mastReefAtt.AddToArray(mainMast1M.mastReefAtt.Last());
            mainMast1M.mastReefAttExtension = mainMast1M.mastReefAttExtension.AddToArray(mainMast1M.mastReefAttExtension.Last());

            mainMast2M.maxSails += 1;
            mainMast2M.reefWinch = mainMast2M.reefWinch.AddToArray(thing.transform.Find("winches/winch_back_reef_extra").GetComponent<GPButtonRopeWinch>());
            mainMast2M.midAngleWinch = mainMast2M.midAngleWinch.AddToArray(thing.transform.Find("winches/winch_angle_main_back (3)").GetComponent<GPButtonRopeWinch>());
            mainMast2M.mastReefAtt = mainMast2M.mastReefAtt.AddToArray(mainMast2M.mastReefAtt.Last());
            mainMast2M.mastReefAttExtension = mainMast2M.mastReefAttExtension.AddToArray(mainMast2M.mastReefAttExtension.Last());


            #endregion
            #region cabinStuff
            container.Find("trim").gameObject.SetActive(false);
            walkCol.Find("trim").gameObject.SetActive(false);
            //structure.Find("junk_large_sliding_door_004").gameObject.SetActive(false);
            //structure.Find("junk_large_sliding_door_006").gameObject.SetActive(false);
            //subContainer.Find("interior_trigger_001").gameObject.SetActive(false);

            var cabin = thing.transform.Find("cabin");
            var cabinCol = modWalkCol.transform.Find("cabin");
            structure.Find("rope_hole").parent = cabin;
            structure.Find("rope_hole_001").gameObject.SetActive(false);
            structure.Find("rope_hole (1)").parent = cabin;
            structure.Find("rope_hole_001 (1)").parent = cabin;
            structure.Find("rope_hole_001 (2)").parent = cabin;
            container.Find("Cube").parent = cabin;
            container.Find("Cube.047").parent = cabin;
            walkCol.Find("Cube").parent = cabin;
            walkCol.Find("Cube.047").parent = cabin;

            var toggler = thing.transform.Find("railing").gameObject.AddComponent<Scripts.ObjectToggler>();
            toggler.offObjects = new GameObject[] { cabin.gameObject, cabinCol.gameObject, subContainer.Find("interior_trigger_001").gameObject, structure.Find("junk_large_sliding_door_004").gameObject, structure.Find("junk_large_sliding_door_006").gameObject };

            container.Find("walls").GetComponent<MeshFilter>().sharedMesh = thing.transform.Find("walls_end").GetComponent<MeshFilter>().sharedMesh;
            container.Find("walls mirrored").GetComponent<MeshFilter>().sharedMesh = thing.transform.Find("walls").GetComponent<MeshFilter>().sharedMesh;
            container.Find("trim mirrored").GetComponent<MeshFilter>().sharedMesh = thing.transform.Find("trim").GetComponent<MeshFilter>().sharedMesh;
            walkCol.Find("walls").GetComponent<MeshCollider>().sharedMesh = thing.transform.Find("walls_end").GetComponent<MeshFilter>().sharedMesh;
            walkCol.Find("walls mirrored").GetComponent<MeshCollider>().sharedMesh = thing.transform.Find("walls").GetComponent<MeshFilter>().sharedMesh;
            walkCol.Find("trim mirrored").GetComponent<MeshCollider>().sharedMesh = thing.transform.Find("trim").GetComponent<MeshFilter>().sharedMesh;

            #endregion

            #region telltales
            thing.transform.Find("shrouds_main1_side/wind_flag").parent = partsList.availableParts[13].partOptions[0].transform;
            thing.transform.Find("shrouds_main1_back/wind_flag").parent = partsList.availableParts[13].partOptions[1].transform;
            thing.transform.Find("shrouds_main2_side/wind_flag").parent = partsList.availableParts[14].partOptions[0].transform;
            thing.transform.Find("shrouds_main2_back/wind_flag").parent = partsList.availableParts[14].partOptions[1].transform;
            thing.transform.Find("telltale_mizzen/wind_flag").parent = mainMast1.parent.Find("mast_back");
            #endregion

            #region shrouds
            BoatPartOption mainMast1_opt = mainMast1.GetComponent<BoatPartOption>();
            mainMast1_opt.childOptions = mainMast1_opt.childOptions.AddRangeToArray(new GameObject[]{ partsList.availableParts[13].partOptions[0].walkColObject, partsList.availableParts[13].partOptions[1].walkColObject });
            partsList.availableParts[13].partOptions[1].transform.parent = thing.transform.Find("shrouds_main1_back");
            partsList.availableParts[13].partOptions[1].walkColObject.transform.parent = modWalkCol.transform.Find("shrouds_main1_back");
            partsList.availableParts[13].partOptions[0].transform.parent = thing.transform.Find("shrouds_main1_side");
            partsList.availableParts[13].partOptions[0].walkColObject.transform.parent = modWalkCol.transform.Find("shrouds_main1_side");
            partsList.availableParts[13].partOptions.RemoveRange(0, 3);

            BoatPartOption mainMast2_opt = mainMast2M.GetComponent<BoatPartOption>();
            mainMast2_opt.childOptions = mainMast2_opt.childOptions.AddRangeToArray(new GameObject[] { partsList.availableParts[14].partOptions[0].walkColObject, partsList.availableParts[14].partOptions[1].walkColObject });
            partsList.availableParts[14].partOptions[1].transform.parent = thing.transform.Find("shrouds_main2_back");
            partsList.availableParts[14].partOptions[1].walkColObject.transform.parent = modWalkCol.transform.Find("shrouds_main2_back");
            partsList.availableParts[14].partOptions[0].transform.parent = thing.transform.Find("shrouds_main2_side");
            partsList.availableParts[14].partOptions[0].walkColObject.transform.parent = modWalkCol.transform.Find("shrouds_main2_side");
            partsList.availableParts[14].partOptions.RemoveRange(0, 3);

            var mizzen1 = mainMast1.parent.Find("mast_back");
            BoatPartOption mizzen1_opt = mizzen1.GetComponent<BoatPartOption>();
            mizzen1_opt.childOptions = mizzen1_opt.childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("shrouds_mizzen_back").GetChild(0).gameObject, modWalkCol.transform.Find("shrouds_mizzen_back").GetChild(0).gameObject});
            var miz_side_opt = thing.transform.Find("shrouds_mizzen_side").GetComponent<BoatPartOption>();
            miz_side_opt.childOptions = miz_side_opt.childOptions.AddRangeToArray(new GameObject[] { mizzen1.Find("static_rig_002").gameObject, mizzen1.Find("static_rope_atts_001").gameObject, mizzen1_opt.walkColObject.transform.Find("static_rig_002").gameObject, mizzen1_opt.walkColObject.transform.Find("static_rope_atts_001").gameObject });

            var foremast1 = mainMast1.parent.Find("mast_front");
            BoatPartOption foremast1_opt = foremast1.GetComponent<BoatPartOption>();
            foremast1_opt.childOptions = foremast1_opt.childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("shrouds_fore_back").GetChild(0).gameObject, modWalkCol.transform.Find("shrouds_fore_back").GetChild(0).gameObject });
            var fore_side_opt = thing.transform.Find("shrouds_fore_side").GetComponent<BoatPartOption>();
            fore_side_opt.childOptions = fore_side_opt.childOptions.AddRangeToArray(new GameObject[] { foremast1.Find("static_rig_007").gameObject, foremast1.Find("static_rope_atts").gameObject, foremast1_opt.walkColObject.transform.Find("static_rig_007").gameObject, foremast1_opt.walkColObject.transform.Find("static_rope_atts").gameObject });
            #endregion
        }
    }
}
