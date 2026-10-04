using HarmonyLib;
using SE_Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class BigBuqPatches
    {
        static Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();
        public static void Patch(Transform boat, BoatCustomParts partsList, BoatRefs boatRefs)
        {
            Transform container = boat.Find("dhow large");
            Transform structure = container.Find("structure");
            Transform mainMast1 = structure.Find("mast_main_0");

            Transform walkCol = mainMast1.GetComponent<Mast>().walkColMast.parent.parent;

            boat.GetComponent<BoatRefs>().walkCol = walkCol; // fix vanilla missing ref

            Plugin.moddedBoats.Add(partsList);

            #region adjustments

            #endregion

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_sanbig.prefab");
            AssetTools.PreparePrefab(prefab, boatRefs);
            var rudder = container.Find("rudder").GetComponent<HingeJoint>();
            foreach (var tiller in prefab.GetComponent<SE_BoatCustomData>().tillers)
            {
                tiller.attachedRudder = rudder;
            }
#if DEBUG
            Debug.Log("SE: instanting kakam parts");
#endif
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);

            //mainMast2.GetComponent<Mast>().mastCols = mainMast2.GetComponent<Mast>().mastCols.AddToArray(modParts["crowsnest_empty"].partOptions[2].GetComponentInChildren<CapsuleCollider>());

            var modWalkCol = thing.GetComponent<SE_BoatCustomData>().walkCol;
            modWalkCol.SetParent(walkCol, false);


        }

    }
}
