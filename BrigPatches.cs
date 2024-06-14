using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class BrigPatches
    {
        [HarmonyPatch(typeof(SaveableBoatCustomization))]
        internal static class PartsPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Adder(SaveableBoatCustomization __instance, BoatCustomParts ___parts)
            {
                if (__instance.name != "BOAT medi medium (50)") return;
                Transform container = __instance.transform.Find("medi medium new");
                Transform structure = container.Find("structure_container");
                Transform mizzenMast1 = structure.Find("mast_mizzen_0");
                Transform mizzenMast2 = structure.Find("mast_mizzen_1");

                #region adjustments
                Util.MoveMast(mizzenMast1, new Vector3(-12.43f, mizzenMast1.localPosition.y, mizzenMast1.localPosition.z), true);
                //var mizShroudsBack = UnityEngine.Object.Instantiate(new GameObject() { name = "shrouds_M_default" }, mizzenMast1);
                //BoatPartOption backOption = Util.CopyPartOption(container.Find("parts_shrouds_F_default").GetComponent<BoatPartOption>(), mizShroudsBack, "mizzen mast shrouds 1");
                //backOption.childOptions = new GameObject[0];


                Transform mizzenShroudsBack = UnityEngine.Object.Instantiate(new GameObject() { name = "parts_shrouds_M_default" }.transform, container);
                mizzenShroudsBack.transform.localEulerAngles = new Vector3 (270f, 0f, 0f);
                Transform mizzenShroudsSide = UnityEngine.Object.Instantiate(container.Find("parts_shrouds_F_spread"), container);
                BoatPartOption backOption = Util.CopyPartOption(container.Find("parts_shrouds_F_default").GetComponent<BoatPartOption>(), mizzenShroudsBack.gameObject, "mizzen shrouds 1");
                BoatPartOption sideOption = mizzenShroudsSide.GetComponent<BoatPartOption>();

                var newCont1 = UnityEngine.Object.Instantiate(new GameObject() { name = "shrouds_default_1" }, mizzenShroudsBack);
                //newCont1.transform.localPosition = mizzenMast1.localPosition;
                mizzenMast1.Find("static_rig_011").parent = newCont1.transform;
                mizzenMast1.Find("trim_007").parent = newCont1.transform;
                newCont1.transform.localScale = new Vector3(1, 0.91f, 1);
                var newCont2 = UnityEngine.Object.Instantiate(new GameObject() { name = "shrouds_default_2" }, mizzenShroudsBack);
                //newCont2.transform.localPosition = mizzenMast2.localPosition;
                mizzenMast2.Find("static_rig_012").parent = newCont2.transform;
                mizzenMast2.Find("trim_009").parent = newCont2.transform;
                backOption.walkColObject = UnityEngine.Object.Instantiate(new GameObject() { name = mizzenShroudsBack.name }, sideOption.walkColObject.transform.parent);
                var walkCol1 = UnityEngine.Object.Instantiate(new GameObject() { name = newCont1.name }, backOption.walkColObject.transform);
                var walkCol2 = UnityEngine.Object.Instantiate(new GameObject() { name = newCont2.name }, backOption.walkColObject.transform);

                //mizzenShroudsSide.transform.localScale = new Vector3(0.935f, 1, 0.8f);
                mizzenShroudsSide.name = "parts_shrouds_M_spread";
                sideOption.optionName = "mizzen shrouds 2";
                sideOption.walkColObject = UnityEngine.Object.Instantiate(sideOption.walkColObject, sideOption.walkColObject.transform.parent);
                var sideShrouds1 = mizzenShroudsSide.Find("shrouds_alt_F0");
                sideShrouds1.localPosition = new Vector3(-12.1f, 0, 17.4f); // new (-12.8f, 0f, 21.8f)
                sideShrouds1.localScale = new Vector3(0.934f, 0.9f, 0.795f); // new (0.93f, -1f, 1f)
                var trim1 = sideShrouds1.Find("trim_013");
                trim1.localScale = new Vector3(-trim1.localScale.x, trim1.localScale.y, trim1.localScale.z);
                trim1.localPosition = new Vector3(trim1.localPosition.x, -trim1.localPosition.y, trim1.localPosition.z);
                var sideCol1 = sideOption.walkColObject.transform.Find("shrouds_alt_F0");
                sideCol1.localPosition = sideShrouds1.localPosition;
                sideCol1.localRotation = sideShrouds1.localRotation;
                var sideShrouds2 = UnityEngine.Object.Instantiate(sideShrouds1, mizzenShroudsSide);
                var sideCol2 = UnityEngine.Object.Instantiate(sideOption.walkColObject.transform.GetChild(0));
                sideShrouds2.localPosition = new Vector3(-7.9f, 0, 17.35f);
                sideShrouds2.localScale = new Vector3(1.075f, -1f, 0.798f);
                sideCol2.localPosition = sideShrouds2.localPosition;
                sideCol2.localRotation = sideShrouds2.localRotation;
                UnityEngine.Object.Destroy(mizzenShroudsSide.Find("shrouds_alt_F1").gameObject);
                UnityEngine.Object.Destroy(sideOption.walkColObject.transform.Find("shrouds_alt_F1").gameObject);



                var mizMastComp = mizzenMast1.GetComponent<Mast>();
                mizMastComp.walkColMast.transform.Find("static_rig_011").parent = walkCol1.transform;
                mizMastComp.walkColMast.transform.Find("trim_007").parent = walkCol1.transform;
                var mizMastComp2 = mizzenMast2.GetComponent<Mast>();
                mizMastComp2.walkColMast.transform.Find("static_rig_012").parent = walkCol2.transform;
                mizMastComp2.walkColMast.transform.Find("trim_009").parent = walkCol2.transform;


                mizzenMast1.GetComponent<BoatPartOption>().childOptions = new GameObject[4] { 
                    newCont1, 
                    walkCol1,
                    sideShrouds1.gameObject,
                    sideOption.walkColObject.transform.GetChild(0).gameObject,
                    };
                mizzenMast2.GetComponent<BoatPartOption>().childOptions = new GameObject[4] {
                    newCont2,
                    walkCol2,
                    sideShrouds2.gameObject,
                    sideCol2.gameObject
                    };
                //var mizShroudsSide = UnityEngine.Object.Instantiate(new GameObject() { name = "shrouds_M_spread" }, mizzenMast1);
                //var mizShroudsSide = UnityEngine.Object.Instantiate(container.Find("parts_shrouds_F_spread").GetChild(0), mizzenMast1);
                //BoatPartOption sideOption = Util.CopyPartOption(container.Find("parts_shrouds_F_spread").GetComponent<BoatPartOption>(), mizShroudsSide.gameObject, "mizzen mast shrouds 2");
                //sideOption.walkColObject = UnityEngine.Object.Instantiate(sideOption.walkColObject.transform.GetChild(0).gameObject, mizzenMast1.GetComponent<>.transform);
                //mizShroudsSide.transform.localPosition = new Vector3()
                //sideOption.childOptions = new GameObject[0];
                BoatPart shrouds = new BoatPart() {
                    partOptions = new List<BoatPartOption>() { backOption, sideOption },
                    category = 1,
                    activeOption = 0 };
                ___parts.availableParts.Add(shrouds);
                #endregion
            }
        }
    }
}
