using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class DhowPatches
    {
        public static void Patch(Transform boat, BoatCustomParts partsList)
        {
            Transform container = boat.transform.Find("dhow");
            Transform mainMast = container.Find("mast");
            Transform mizzen_old = container.Find("mast_mizzen");
            Transform shrouds = container.Find("Cylinder_002");
            var shroudAnchor = container.Find("static_rig_001");

            #region adjustments
            partsList.availableParts[1].category = 2;
            partsList.availableParts[4].category = 2;
            #endregion

            #region mizzenMast
            Mast mizzen_new = Util.CopyMast(mainMast, mizzen_old.localPosition, mizzen_old.localEulerAngles, mainMast.localScale, "mast_mizzen_1", "mizzen mast", 29);
            mizzen_new.reefWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().reefWinch, mainMast.localPosition, mizzen_new.transform.localPosition + new Vector3(0.47f, -0.2f, 0));
            mizzen_new.reefWinch[0].name = "";
            mizzen_new.reefWinch[0].rope = null;
            mizzen_new.reefWinch[1].name = "";
            mizzen_new.midAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().midAngleWinch, mainMast.localPosition, mainMast.localPosition + new Vector3(-1.45f, 0, 0));
            mizzen_new.leftAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().leftAngleWinch, Vector3.zero, Vector3.zero);
            mizzen_new.leftAngleWinch[0].transform.localPosition = new Vector3(-6.4f, 1.26f, 1.5f);
            mizzen_new.leftAngleWinch[0].transform.localEulerAngles = new Vector3(288, 99, 75);
            mizzen_new.rightAngleWinch = Util.CopyWinches(mainMast.GetComponent<Mast>().rightAngleWinch, Vector3.zero, Vector3.zero);
            mizzen_new.rightAngleWinch[0].transform.localPosition = new Vector3(-6.4f, 1.26f, -1.5f);
            mizzen_new.rightAngleWinch[0].transform.localEulerAngles = new Vector3(288, 59, 120);
            var rope_holder_aft = UnityEngine.Object.Instantiate(container.Find("rope_holder"), mizzen_new.transform);
            rope_holder_aft.transform.localPosition = mizzen_old.Find("rope_holder_002").localPosition;
            rope_holder_aft.transform.localRotation = mizzen_old.Find("rope_holder_002").localRotation;
            mizzen_new.midRopeAtt[0] = UnityEngine.Object.Instantiate(container.Find("rope_att_angle_extension"), rope_holder_aft.transform);
            mizzen_new.midRopeAtt[0].transform.localPosition = Vector3.zero;
            mizzen_new.midRopeAtt[1] = mizzen_new.midRopeAtt[0];
            var mizzenReefAtt = UnityEngine.Object.Instantiate(container.Find("rope_holder_001"), mizzen_new.transform);
            mizzenReefAtt.transform.localPosition = new Vector3(-0.2f, 0.03f, 0.6f);
            mizzenReefAtt.transform.localEulerAngles = new Vector3(90, 97, 0);
            mizzen_new.mastReefAtt[0] = mizzenReefAtt.Find("att");
            mizzen_new.mastReefAtt[1] = mizzenReefAtt.Find("att");
            //mizzen_new.midRopeAtt = 
            //mizzen_new.orderIndex = 29;
            //mizzen_new.Awake();

            BoatPartOption emptyMizzen = Util.CreatePartOption(container, "(empty mizzen)", "(no mizzen mast)");
            BoatPart mizzenPart = Util.CreateAndAddPart(partsList, 0, new List<BoatPartOption> { emptyMizzen, mizzen_new.GetComponent<BoatPartOption>() });

            var mizzenShrouds = UnityEngine.Object.Instantiate(shrouds, mizzen_new.transform);
            mizzenShrouds.localPosition = new Vector3(-2.3f, -0.01f, -6.3f);
            mizzenShrouds.localEulerAngles = Vector3.zero;
            var mizzenAnchor0 = UnityEngine.Object.Instantiate(shroudAnchor, mizzenShrouds);
            mizzenAnchor0.localPosition = new Vector3(1.42f, -1.34f, 0.35f);
            mizzenAnchor0.localEulerAngles = new Vector3(0, 15, 86);
            //var mizzenAnchor1 = UnityEngine.Object.Instantiate(shroudAnchor, mizzenShrouds);
            #endregion
        }
        
    }
}
