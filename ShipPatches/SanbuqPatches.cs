using HarmonyLib;
using Mono.Cecil;
using SE_Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class SanbuqPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();

        public static void Patch(Transform boat, BoatCustomParts partsList, BoatRefs boatRefs)
        {
            //Mast[] masts = boat.GetComponent<BoatRefs>().masts;
            //var boatRefs = boat.GetComponent<BoatRefs>();
            Transform container = boat.transform.Find("dhow medium new");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast");
            Transform mainMast2 = structure.Find("mast_1");
            Transform mizzenMast1 = structure.Find("mast_002");
            Transform topMast1 = structure.Find("mast_0_extension");
            var topmast1M = topMast1.GetComponent<Mast>();
            Transform topMast2 = structure.Find("mast_1_extension");
            var topmast2M = topMast2.GetComponent<Mast>();
            Transform topmastStay1 = container.Find("forestay_0_extension_long");
            Transform topmastStay2 = container.Find("forestay_1_extension_long");
            Transform topmastStay3 = container.Find("forestay_1_extension_short");
            Mast topmastStay1_mast = topmastStay1.GetComponent<Mast>();
            Mast topmastStay2_mast = topmastStay2.GetComponent<Mast>();
            Mast topmastStay3_mast = topmastStay3.GetComponent<Mast>();
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;

            // add references for save cleaner
            Plugin.moddedBoats.Add(partsList);

            #region adjustments
            topmastStay3.GetComponent<BoatPartOption>().requiresDisabled.Add(container.Find("forestay_1_long").GetComponent<BoatPartOption>());

            mainMast1.gameObject.GetComponent<BoatPartOption>().optionName = "main mast 1";
            mainMast2.gameObject.GetComponent<BoatPartOption>().optionName = "main mast 2";
            BoatPartOption mainMastNone = Util.CreatePartOption(container, "(empty mainmast)", "(no main mast)");
            partsList.availableParts[0].partOptions.Add(mainMastNone);

            BoatPartOption forestayNone = Util.CreatePartOption(container, "(empty forestay)", "(no forestay)");
            partsList.availableParts[3].partOptions.Add(forestayNone);

            /*BoatPartOption bowspritNone = Util.CreatePartOption(container, "(empty bowsprit)", "(no bowsprit)");
            partsList.availableParts[2].partOptions.Add(bowspritNone);*/
            topmast1M.extraBottomHeight += 1f;
            topmast2M.extraBottomHeight += 1f;
            mainMast1.GetComponent<Mast>().mastHeight += 0.5f;
            mainMast1.GetComponent<Mast>().extraBottomHeight -= 0.5f;
            mainMast2.GetComponent<Mast>().mastHeight += 0.5f;
            mainMast2.GetComponent<Mast>().extraBottomHeight -= 0.5f;
            #endregion


            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_sanbuq.prefab");

            AssetTools.PreparePrefab(prefab, boatRefs);

            var helm1 = container.Find("steering_wheel");
            var helmB = container.Find("steering_wheel (big)");

            var newHelms = prefab.GetComponent<SE_BoatCustomData>().tillers;
            newHelms[1].attachedRudder = helm1.GetComponent<GPButtonSteeringWheel>().attachedRudder;
            newHelms[0].attachedRudder = helmB.GetComponent<GPButtonSteeringWheel>().attachedRudder;


#if DEBUG
            Debug.Log("SE: instanting sanbuq parts");
#endif
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);
            var modWalkCol = thing.GetComponent<SE_BoatCustomData>().walkCol;
            modWalkCol.SetParent(walkCol, false);

            var crowsnests = thing.transform.Find("crowsnest");
            var crowsnestCols = modWalkCol.Find("crowsnest");


            #region topmastStay
#if DEBUG
            Debug.Log("topmast stay rejiggering");
#endif
            topmastStay1_mast.reefWinch = Util.CopyWinches(topmastStay1_mast.reefWinch, Vector3.zero, Vector3.up);
            topmastStay1_mast.leftAngleWinch = Util.CopyWinches(topmastStay1_mast.leftAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, -0.17f));
            topmastStay1_mast.rightAngleWinch = Util.CopyWinches(topmastStay1_mast.rightAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, 0.13f));
            foreach (GPButtonRopeWinch winch in topmastStay1_mast.leftAngleWinch)
            {
                winch.transform.localEulerAngles = new Vector3(0, 186, 0);
            }
            foreach (GPButtonRopeWinch winch in topmastStay1_mast.rightAngleWinch)
            {
                winch.transform.localEulerAngles = new Vector3(0, 356, 0);
            }
            topmastStay2_mast.reefWinch = Util.CopyWinches(topmastStay2_mast.reefWinch, Vector3.zero, Vector3.up);
            topmastStay2_mast.leftAngleWinch = Util.CopyWinches(topmastStay2_mast.leftAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, -0.15f));
            topmastStay2_mast.rightAngleWinch = Util.CopyWinches(topmastStay2_mast.rightAngleWinch, Vector3.zero, new Vector3(-0.2f, -0.2f, 0.14f));
            foreach (GPButtonRopeWinch winch in topmastStay2_mast.leftAngleWinch)
            {
                winch.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            foreach (GPButtonRopeWinch winch in topmastStay2_mast.rightAngleWinch)
            {
                winch.transform.localEulerAngles = Vector3.zero;
            }

            topmastStay3_mast.reefWinch = topmastStay2_mast.reefWinch;
            topmastStay3_mast.leftAngleWinch = topmastStay2_mast.leftAngleWinch;
            topmastStay3_mast.rightAngleWinch = topmastStay2_mast.rightAngleWinch;
            modParts["topmast_forestays_part"].partOptions.InsertRange(1, new BoatPartOption[] { topmastStay1.GetComponent<BoatPartOption>(), topmastStay2.GetComponent<BoatPartOption>(), topmastStay3.GetComponent<BoatPartOption>() });
            //Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { Util.CreatePartOption(container, "outer_forestay_empty", "(no outer forestay)"), topmastStay1.GetComponent<BoatPartOption>(), topmastStay2.GetComponent<BoatPartOption>(), topmastStay3.GetComponent<BoatPartOption>(), });

            partsList.availableParts[3].partOptions.Remove(topmastStay1.GetComponent<BoatPartOption>());
            partsList.availableParts[3].partOptions.Remove(topmastStay2.GetComponent<BoatPartOption>());
            partsList.availableParts[3].partOptions.Remove(topmastStay3.GetComponent<BoatPartOption>());
            var mainShrouds0Children = partsList.availableParts[5].partOptions[0];
            var mainShrouds1Children = partsList.availableParts[5].partOptions[1];
            Util.AddChildOptions(mainShrouds0Children, new GameObject[] { crowsnests.Find("crowsnest_0/ladder_shr_0").gameObject, crowsnests.Find("crowsnest_1/ladder_shr_0").gameObject, thing.transform.Find("flags_main").Find("mast_0").Find("flag_0").gameObject, thing.transform.Find("flags_main").Find("mast_1").Find("flag_0").gameObject });
            Util.AddChildOptions(mainShrouds1Children, new GameObject[] { crowsnests.Find("crowsnest_0/ladder_shr_1").gameObject, crowsnests.Find("crowsnest_1/ladder_shr_1").gameObject, thing.transform.Find("flags_main").Find("mast_0").Find("flag_1").gameObject, thing.transform.Find("flags_main").Find("mast_1").Find("flag_1").gameObject });

            // mizzen shrouds
            Util.AddChildOptions(partsList.availableParts[6].partOptions[0], new GameObject[] { thing.transform.Find("mizzen_mast").Find("shrouds_side").gameObject, modWalkCol.transform.Find("mizzen_mast").Find("shrouds_mizzen_side").gameObject, thing.transform.Find("mizzen_mast2").Find("shrouds_side").gameObject, modWalkCol.transform.Find("mizzen_mast2").Find("shrouds_mizzen_side").gameObject });
            Util.AddChildOptions(partsList.availableParts[6].partOptions[1], new GameObject[] { thing.transform.Find("mizzen_mast").Find("shrouds_back").gameObject, modWalkCol.transform.Find("mizzen_mast").Find("shrouds_mizzen_back").gameObject, thing.transform.Find("mizzen_mast2").Find("shrouds_back").gameObject, modWalkCol.transform.Find("mizzen_mast2").Find("shrouds_mizzen_back").gameObject });

            mizzenMast1.GetComponent<BoatPartOption>().childOptions = mizzenMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(thing.transform.Find("flags_mizzen").GetChild(0).gameObject);

            Util.AddChildOptions(mainMast1.GetComponent<BoatPartOption>(), new GameObject[] {thing.transform.Find("flags_main").Find("mast_0").gameObject, crowsnests.GetChild(0).gameObject, crowsnestCols.GetChild(0).gameObject});
            Util.AddChildOptions(mainMast2.GetComponent<BoatPartOption>(), new GameObject[] {thing.transform.Find("flags_main").Find("mast_1").gameObject, crowsnests.GetChild(1).gameObject, crowsnestCols.GetChild(1).gameObject});

            // crow's nest cloth colliders
            topMast1.GetComponent<Mast>().mastCols = topMast1.GetComponent<Mast>().mastCols.AddToArray(crowsnests.Find("crowsnest_0/cloth_col").GetComponent<CapsuleCollider>());
            topMast2.GetComponent<Mast>().mastCols = topMast2.GetComponent<Mast>().mastCols.AddToArray(crowsnests.Find("crowsnest_1/cloth_col").GetComponent<CapsuleCollider>());
#endregion

#region hammock
#if DEBUG
            Debug.Log("sanbuq hammock");
#endif
            BoatPartOption hammock = modParts["hammock_part"].partOptions[0];//Util.AddPartOption(container.Find("hammock").gameObject, "hammock");

            hammock.childOptions = new GameObject[3] { container.Find("hammock").gameObject, container.Find("hammock_001").gameObject, walkCol.Find("hammock_001").gameObject };
            hammock.walkColObject = walkCol.Find("hammock").gameObject;

            #endregion

            BoatPartOption frontHelm = Util.CreatePartOption(container, "front_helm_parent", "helm 1");
            frontHelm.childOptions = new GameObject[] { structure.Find("Cube_005").gameObject, container.Find("steering_ropes").gameObject };
            helm1.parent = frontHelm.transform;
            helmB.parent = frontHelm.transform;

            frontHelm.basePrice = 900;
            frontHelm.installCost = 500;
            frontHelm.walkColObject = walkCol.Find("structure").Find("Cube_005").gameObject;
            //frontHelm.childOptions = new GameObject[] { structure.Find("Cube_004").gameObject };
            modParts["wheel_holder"].partOptions.Insert(0, frontHelm);
            /*
                        partsList.availableParts[9].partOptions[0] = Util.CopyPartOption(partsList.availableParts[9].partOptions[0], new GameObject("wheel_opt"), partsList.availableParts[9].partOptions[0].optionName);
                        partsList.availableParts[9].partOptions[1] = Util.CopyPartOption(partsList.availableParts[9].partOptions[1], new GameObject("wheel_opt_b"), partsList.availableParts[9].partOptions[1].optionName);
            */
            Util.AddChildOption(partsList.availableParts[9].partOptions[0], thing.transform.Find("wheel_holder/orienter/steering_wheel").gameObject);
            Util.AddChildOption(partsList.availableParts[9].partOptions[1], thing.transform.Find("wheel_holder/orienter/steering_wheel (big)").gameObject);

            #region late adjustments
#if DEBUG
            Debug.Log("Sanbuq late adjustments");
#endif
            topmast1M.midAngleWinch = topmast2M.midAngleWinch.Concat(topmast2M.midAngleWinch).ToArray();
            topmast1M.midRopeAtt = topmast1M.midRopeAtt.Concat(topmast1M.midRopeAtt).ToArray();
            topmast1M.mastReefAtt = topmast1M.mastReefAtt.AddRangeToArray(topmast1M.mastReefAtt);
            topmast1M.reefWinch = topmast1M.reefWinch.AddToArray(thing.transform.Find("winches/rope_winch_extension0_reef (1)").gameObject.GetComponent<GPButtonRopeWinch>());
            topmast1M.maxSails = 2;

            topmast2M.midAngleWinch = topmast2M.midAngleWinch.Concat(topmast2M.midAngleWinch).ToArray();
            topmast2M.mastReefAtt = topmast2M.mastReefAtt.AddRangeToArray(topmast2M.mastReefAtt);
            topmast2M.reefWinch = topmast2M.reefWinch.AddToArray(thing.transform.Find("winches/rope_winch_extension1_reef (1)").gameObject.GetComponent<GPButtonRopeWinch>());
            topmast2M.maxSails = 2;

            partsList.availableParts[3].category = 2;
            foreach (BoatPartOption stay in partsList.availableParts[3].partOptions)
            {
                if (stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    stay.requiresDisabled.Add(partsList.availableParts[2].partOptions[2]);
                    //stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[1]);
                    //stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[2]);
                    //stay.requiresDisabled.Add(bowspritNone);
                    stay.requires.Add(modParts["foremast_empty"].partOptions[0]);

                }
            }
            partsList.availableParts[4].category = 2;
            foreach (BoatPartOption stay in partsList.availableParts[4].partOptions)
            {
                if (stay.optionName.StartsWith("topmast") || stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    //stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[1]);
                    //stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[2]);
                    stay.requires.Add(modParts["foremast_empty"].partOptions[0]);
                }
            }
            topmastStay1.GetComponent<BoatPartOption>().requires.Add(modParts["foremast_empty"].partOptions[0]);
            topmastStay2.GetComponent<BoatPartOption>().requires.Add(modParts["foremast_empty"].partOptions[0]);

            var topmastNone = container.Find("(empty topmast)").GetComponent<BoatPartOption>();
            Util.AddChildOptions(topmastNone, new GameObject[] { mainMast1.Find("flag (2)").gameObject, mainMast2.Find("flag (1)").gameObject });
            UnityEngine.Object.Instantiate(mainMast1.Find("flag (2)"), topMast1, false).localPosition = Vector3.zero;
            UnityEngine.Object.Instantiate(mainMast1.Find("flag (2)"), topMast2, false).localPosition = Vector3.zero;

            var miz_topmastNone = modParts["mizzen_topmast_empty"].partOptions[0];
            miz_topmastNone.childOptions = miz_topmastNone.childOptions.AddToArray(mizzenMast1.Find("flag").gameObject);
#endregion

        }
    }
}
