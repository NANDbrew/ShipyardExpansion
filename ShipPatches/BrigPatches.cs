using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class BrigPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.Find("medi medium new");
            Transform structure = container.Find("structure_container");
            Transform mizzenMast1 = structure.Find("mast_mizzen_0");
            Transform mizzenMast2 = structure.Find("mast_mizzen_1");
            Transform mainMast1 = structure.Find("mast_Back_0");
            Transform mainMast2 = structure.Find("mast_Back_1");
            Transform foreMast1 = structure.Find("mast_Front_0");
            Transform foreMast2 = structure.Find("mast_Front_1");
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;
            

            // add references for save cleaner
            foreach (var part in partsList.availableParts)
            {
                Plugin.stockParts.Add(part, part.activeOption);
            }
            Plugin.moddedBoats.Add(partsList);


            #region adjustments
            mainMast1.GetComponent<Mast>().mastHeight = 18.8f;
            mizzenMast1.GetComponent<Mast>().mastHeight = 19f;
            mizzenMast1.GetComponent<Mast>().extraBottomHeight = 1f;
            mizzenMast2.GetComponent<Mast>().mastHeight = 19f;
            mizzenMast2.GetComponent<Mast>().extraBottomHeight = 1f;
            foreMast1.GetComponent<Mast>().mastHeight = 20f;
            foreMast1.GetComponent<Mast>().extraBottomHeight = 0.7f;

            #endregion
            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_brig.prefab");

            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                mast.shipRigidbody = shipRigidbody;
                //mast.gameObject.SetActive(false);
            }

            Debug.Log("SE: instanting brig parts");
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);
            var modWalkCol = thing.transform.Find("SE_cols_brig");
            modWalkCol.SetParent(walkCol, false);

            modParts["fore_topmast_empty"].partOptions[0].childOptions = new GameObject[] { foreMast1.GetComponentInChildren<WindClothSimple>().gameObject, foreMast2.GetComponentInChildren<WindClothSimple>().gameObject };
            modParts["main_topmast_empty"].partOptions[0].childOptions = new GameObject[] { mainMast1.GetComponentInChildren<WindClothSimple>().gameObject, mainMast2.GetComponentInChildren<WindClothSimple>().gameObject };

            mizzenMast1.GetComponent<BoatPartOption>().childOptions = mizzenMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { modParts["shrouds_mizzen_back"].partOptions[1].transform.GetChild(0).gameObject, modParts["shrouds_mizzen_back"].partOptions[1].walkColObject.transform.GetChild(0).gameObject, thing.transform.Find("shrouds_mizzen_back").transform.GetChild(0).GetChild(0).gameObject });
            mizzenMast2.GetComponent<BoatPartOption>().childOptions = mizzenMast2.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { modParts["shrouds_mizzen_back"].partOptions[1].transform.GetChild(1).gameObject, modParts["shrouds_mizzen_back"].partOptions[1].walkColObject.transform.GetChild(1).gameObject, thing.transform.Find("shrouds_mizzen_back").transform.GetChild(0).GetChild(1).gameObject });

            modParts["shrouds_mizzen_back"].partOptions[0].childOptions = new GameObject[] { mizzenMast1.Find("static_rig_011").gameObject, mizzenMast1.Find("trim_007").gameObject, mizzenMast1.GetComponent<BoatPartOption>().walkColObject.transform.Find("static_rig_011").gameObject, mizzenMast1.GetComponent<BoatPartOption>().walkColObject.transform.Find("trim_007").gameObject, mizzenMast2.Find("static_rig_012").gameObject, mizzenMast2.Find("trim_009").gameObject, mizzenMast2.GetComponent<BoatPartOption>().walkColObject.transform.Find("static_rig_012").gameObject, mizzenMast2.GetComponent<BoatPartOption>().walkColObject.transform.Find("trim_009").gameObject };
            var tops_main = thing.transform.Find("shrouds_main");
            var tops_fore = thing.transform.Find("shrouds_fore");
            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { tops_main.GetChild(0).gameObject, tops_main.GetComponent<BoatPartOption>().walkColObject.transform.GetChild(0).gameObject, thing.transform.Find("telltale_main/telltale_main_cont").GetChild(0).gameObject, thing.transform.Find("telltale_main/telltale_main_cont2").GetChild(0).gameObject });
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { tops_main.GetChild(1).gameObject, tops_main.GetComponent<BoatPartOption>().walkColObject.transform.GetChild(1).gameObject, thing.transform.Find("telltale_main/telltale_main_cont").GetChild(1).gameObject, thing.transform.Find("telltale_main/telltale_main_cont2").GetChild(1).gameObject });
            foreMast1.GetComponent<BoatPartOption>().childOptions = foreMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { tops_fore.GetChild(0).gameObject, tops_fore.GetComponent<BoatPartOption>().walkColObject.transform.GetChild(0).gameObject, thing.transform.Find("telltale_fore/telltale_fore_cont").GetChild(0).gameObject, thing.transform.Find("telltale_fore/telltale_fore_cont2").GetChild(0).gameObject });
            foreMast2.GetComponent<BoatPartOption>().childOptions = foreMast2.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { tops_fore.GetChild(1).gameObject, tops_fore.GetComponent<BoatPartOption>().walkColObject.transform.GetChild(1).gameObject, thing.transform.Find("telltale_fore/telltale_fore_cont").GetChild(1).gameObject, thing.transform.Find("telltale_fore/telltale_fore_cont2").GetChild(1).gameObject });
            partsList.availableParts[12].partOptions[0].childOptions = partsList.availableParts[12].partOptions[0].childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("crowsnest_main_0/ladder_shr_0").gameObject, thing.transform.Find("crowsnest_main_1/ladder_shr_0").gameObject, thing.transform.Find("telltale_main").GetChild(1).gameObject });
            partsList.availableParts[12].partOptions[1].childOptions = partsList.availableParts[12].partOptions[1].childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("crowsnest_main_0/ladder_shr_1").gameObject, thing.transform.Find("crowsnest_main_1/ladder_shr_1").gameObject, thing.transform.Find("telltale_main").GetChild(0).gameObject });
            partsList.availableParts[11].partOptions[0].childOptions = partsList.availableParts[11].partOptions[0].childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("crowsnest_fore_0/ladder_shr_0").gameObject, thing.transform.Find("crowsnest_fore_1/ladder_shr_0").gameObject, thing.transform.Find("telltale_fore").GetChild(1).gameObject, thing.transform.Find("mast_fore/static_rig").gameObject, modWalkCol.Find("mast_fore/static_rig").gameObject, thing.transform.Find("mast_fore (1)/static_rig").gameObject, modWalkCol.Find("mast_fore (1)/static_rig").gameObject });
            partsList.availableParts[11].partOptions[1].childOptions = partsList.availableParts[11].partOptions[1].childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("crowsnest_fore_0/ladder_shr_1").gameObject, thing.transform.Find("crowsnest_fore_1/ladder_shr_1").gameObject, thing.transform.Find("telltale_fore").GetChild(0).gameObject, thing.transform.Find("mast_fore/shrouds_alt_F1").gameObject, modWalkCol.Find("mast_fore/shrouds_alt_F1").gameObject, thing.transform.Find("mast_fore (1)/shrouds_alt_F0").gameObject, modWalkCol.Find("mast_fore (1)/shrouds_alt_F0").gameObject });


            #region late adjustments
            var mizzenMast1M = mizzenMast1.GetComponent<Mast>();
            mizzenMast1M.maxSails += 1;
            mizzenMast1M.reefWinch = mizzenMast1M.reefWinch.AddToArray(thing.transform.Find("winches/rope_winch_mizzen0_reef (2)").GetComponent<GPButtonRopeWinch>());
            mizzenMast1M.mastReefAtt = mizzenMast1M.mastReefAtt.AddToArray(mizzenMast1M.mastReefAtt.Last());
            mizzenMast1M.midAngleWinch = mizzenMast1M.midAngleWinch.AddToArray(thing.transform.Find("winches/rope_winch_mizzen_angle (4)").GetComponent<GPButtonRopeWinch>());
            
            var mizzenMast2M = mizzenMast2.GetComponent<Mast>();
            mizzenMast2M.maxSails += 1;
            mizzenMast2M.reefWinch = mizzenMast2M.reefWinch.AddToArray(thing.transform.Find("winches/rope_winch_mizzen1_reef (2)").GetComponent<GPButtonRopeWinch>());
            mizzenMast2M.mastReefAtt = mizzenMast2M.mastReefAtt.AddToArray(mizzenMast2M.mastReefAtt.Last());
            mizzenMast2M.midAngleWinch = mizzenMast1M.midAngleWinch;

            #endregion
        }
    }
}
