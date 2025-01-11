using HarmonyLib;
using Mono.Cecil;
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

        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            //Mast[] masts = boat.GetComponent<BoatRefs>().masts;
            //var boatRefs = boat.GetComponent<BoatRefs>();
            Transform container = boat.transform.Find("dhow medium new");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast");
            Transform mainMast2 = structure.Find("mast_1");
            Transform mizzenMast1 = structure.Find("mast_002");
            Transform topMast1 = structure.Find("mast_0_extension");
            Transform topMast2 = structure.Find("mast_1_extension");
            Transform topmastStay1 = container.Find("forestay_0_extension_long");
            Transform topmastStay2 = container.Find("forestay_1_extension_long");
            Transform topmastStay3 = container.Find("forestay_1_extension_short");
            Mast topmastStay1_mast = topmastStay1.GetComponent<Mast>();
            Mast topmastStay2_mast = topmastStay2.GetComponent<Mast>();
            Mast topmastStay3_mast = topmastStay3.GetComponent<Mast>();
            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;

           // Plugin.topmastRef = topMast1;
            PartRefs.sanbuq = container;
            PartRefs.sanbuqCol = walkCol;

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

            mainMast1.GetComponent<Mast>().mastHeight += 0.5f;
            mainMast1.GetComponent<Mast>().extraBottomHeight -= 0.5f;
            mainMast2.GetComponent<Mast>().mastHeight += 0.5f;
            mainMast2.GetComponent<Mast>().extraBottomHeight -= 0.5f;
            #endregion

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_sanbuq.prefab");

            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                mast.shipRigidbody = shipRigidbody;
            }
            Debug.Log("SE: instanting sanbuq parts");
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);
            var modWalkCol = thing.transform.Find("SE_cols_sanbuq");
            modWalkCol.SetParent(walkCol, false);


            #region topmastStay
            Debug.Log("topmast stay rejiggering");
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
            modParts["topmast_forestays_part"].partOptions.AddRange(new BoatPartOption[] { topmastStay1.GetComponent<BoatPartOption>(), topmastStay2.GetComponent<BoatPartOption>(), topmastStay3.GetComponent<BoatPartOption>() });
            //Util.CreateAndAddPart(partsList, 2, new List<BoatPartOption> { Util.CreatePartOption(container, "outer_forestay_empty", "(no outer forestay)"), topmastStay1.GetComponent<BoatPartOption>(), topmastStay2.GetComponent<BoatPartOption>(), topmastStay3.GetComponent<BoatPartOption>(), });

            partsList.availableParts[3].partOptions.Remove(topmastStay1.GetComponent<BoatPartOption>());
            partsList.availableParts[3].partOptions.Remove(topmastStay2.GetComponent<BoatPartOption>());
            partsList.availableParts[3].partOptions.Remove(topmastStay3.GetComponent<BoatPartOption>());
            var mainShrouds0Children = partsList.availableParts[5].partOptions[0];
            var mainShrouds1Children = partsList.availableParts[5].partOptions[1];
            mainShrouds0Children.childOptions = mainShrouds0Children.childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("crowsnest_0").Find("ladder_shr_0").gameObject, thing.transform.Find("crowsnest_0").Find("ladder_shr_0").gameObject, thing.transform.Find("flags_main").Find("mast_0").Find("flag_0").gameObject, thing.transform.Find("flags_main").Find("mast_1").Find("flag_0").gameObject });
            mainShrouds1Children.childOptions = mainShrouds1Children.childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("crowsnest_0").Find("ladder_shr_1").gameObject, thing.transform.Find("crowsnest_0").Find("ladder_shr_1").gameObject, thing.transform.Find("flags_main").Find("mast_0").Find("flag_1").gameObject, thing.transform.Find("flags_main").Find("mast_1").Find("flag_1").gameObject });

            partsList.availableParts[6].partOptions[0].childOptions = partsList.availableParts[6].partOptions[0].childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("mizzen_mast").Find("shrouds_side").gameObject, modWalkCol.transform.Find("mizzen_mast").Find("shrouds_mizzen_side").gameObject });
            partsList.availableParts[6].partOptions[1].childOptions = partsList.availableParts[6].partOptions[1].childOptions.AddRangeToArray(new GameObject[] { thing.transform.Find("mizzen_mast").Find("shrouds_back").gameObject, modWalkCol.transform.Find("mizzen_mast").Find("shrouds_mizzen_back").gameObject });

            mizzenMast1.GetComponent<BoatPartOption>().childOptions = mizzenMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(thing.transform.Find("flags_mizzen").GetChild(0).gameObject);

            mainMast1.GetComponent<BoatPartOption>().childOptions = mainMast1.GetComponent<BoatPartOption>().childOptions.AddToArray(thing.transform.Find("flags_main").Find("mast_0").gameObject);
            mainMast2.GetComponent<BoatPartOption>().childOptions = mainMast2.GetComponent<BoatPartOption>().childOptions.AddToArray(thing.transform.Find("flags_main").Find("mast_1").gameObject);
            topMast1.GetComponent<Mast>().mastCols = topMast1.GetComponent<Mast>().mastCols.AddToArray(thing.transform.Find("crowsnest_0").GetComponent<CapsuleCollider>());
            topMast2.GetComponent<Mast>().mastCols = topMast2.GetComponent<Mast>().mastCols.AddToArray(thing.transform.Find("crowsnest_1").GetComponent<CapsuleCollider>());
            #endregion

            #region hammock
            Debug.Log("sanbuq hammock");
            BoatPartOption hammock = modParts["hammock_part"].partOptions[0];//Util.AddPartOption(container.Find("hammock").gameObject, "hammock");

            hammock.childOptions = new GameObject[3] { container.Find("hammock").gameObject, container.Find("hammock_001").gameObject, walkCol.Find("hammock_001").gameObject };
            hammock.walkColObject = walkCol.Find("hammock").gameObject;

            #endregion

            #region late adjustments
            Debug.Log("Sanbuq late adjustments");
            partsList.availableParts[3].category = 2;
            foreach (BoatPartOption stay in partsList.availableParts[3].partOptions)
            {
                if (stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    stay.requiresDisabled.Add(partsList.availableParts[2].partOptions[2]);
                    stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[1]);
                    stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[2]);
                    //stay.requiresDisabled.Add(bowspritNone);

                }
            }
            partsList.availableParts[4].category = 2;
            foreach (BoatPartOption stay in partsList.availableParts[4].partOptions)
            {
                if (stay.optionName.StartsWith("topmast") || stay.optionName.StartsWith("(no") || stay.optionName.Contains("foremast")) continue;
                else
                {
                    stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[1]);
                    stay.requiresDisabled.Add(modParts["foremast_empty"].partOptions[2]);

                }
            }
            topmastStay1.GetComponent<BoatPartOption>().requires.Add(modParts["foremast_empty"].partOptions[0]);
            topmastStay2.GetComponent<BoatPartOption>().requires.Add(modParts["foremast_empty"].partOptions[0]);

            container.Find("(empty topmast)").GetComponent<BoatPartOption>().childOptions = topMast1.GetComponent<BoatPartOption>().childOptions.AddRangeToArray(new GameObject[] { mainMast1.Find("flag (2)").gameObject, mainMast2.Find("flag (1)").gameObject });

            #endregion

        }
    }
}
