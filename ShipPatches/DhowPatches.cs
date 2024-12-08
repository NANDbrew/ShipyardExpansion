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
            Transform walkCols = mainMastM.walkColMast.parent;

            Transform mainMastTall = container.Find("mast_taller");
            Transform shortForestay = container.Find("forestay_low");
            Transform lowForestay = container.Find("forestay");

            PartRefs.dhow = container;
            PartRefs.dhowCol = walkCols;

            Debug.Log("Dhow adjustments");
            #region adjustments
            partsList.availableParts[1].category = 2;
            partsList.availableParts[4].category = 2;
            shortForestay.localScale = new Vector3(1, 1, 1.02f);
            shortForestay.GetComponent<BoatPartOption>().requires.Remove(container.Find("bowsprit").GetComponent<BoatPartOption>());
            container.Find("empty low forestay").GetComponent<BoatPartOption>().optionName = "(no lower forestay)";

            var prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/ShipyardExpansion/SE_parts_dhow.prefab");
            Rigidbody shipRigidbody = boat.GetComponent<Rigidbody>();
            foreach (Mast mast in prefab.GetComponentsInChildren<Mast>(true))
            {
                mast.gameObject.SetActive(false);
                mast.shipRigidbody = shipRigidbody;
            }

            Debug.Log("SE: instanting dhow parts");
            var thing = UnityEngine.Object.Instantiate(prefab, container, false);
            Debug.Log("SE: instantiated " + thing);

            modParts = AssetTools.HandleImports(thing, partsList);

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

            shortForestay.GetComponent<Mast>().mastReefAtt = lowForestay.GetComponent<Mast>().mastReefAtt;
            #endregion

            var modWalkCol = thing.transform.Find("SE_cols_dhow");
            modWalkCol.SetParent(walkCols, false);

            try
            {
                var col = container.Find("embark_col").GetComponent<MeshCollider>();
                Debug.Log(col);
                var newMesh = thing.transform.Find("embark_col").GetComponent<MeshFilter>();
                Debug.Log(newMesh);
                col.sharedMesh = newMesh.sharedMesh;
                //col.mesh = newMesh.mesh;

            }
            catch { Debug.Log("couldn't patch dhow embark"); }

            #region late adjustments
            //highForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(rakedMain.GetComponent<BoatPartOption>());
            lowForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[2]);
            shortForestay.GetComponent<BoatPartOption>().requiresDisabled.Add(partsList.availableParts[0].partOptions[2]);
            #endregion
        }
    }
}
