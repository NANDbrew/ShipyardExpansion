using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{


    public class TopsailAngleMirror : MonoBehaviour
    {
        private HingeJoint sail;

        private SailConnections connections;

        private RopeEffect midRope;

        public HingeJoint sailBelow;

        private Transform midConnection;

        private bool ready;

        private void Awake()
        {
            sail = GetComponent<HingeJoint>();
        }

        private void Initialize()
        {
            connections = GetComponent<SailConnections>();
            midRope = connections.angleControllerMid.GetComponent<RopeEffect>();
            SailConnections component = sailBelow.GetComponent<SailConnections>();
            midConnection = component.angleControllerMid.GetComponent<RopeEffect>().attachment.GetComponent<RopeEffect>().attachment.transform;
/*            component.midRopeAttachment.position = __instance.midAngleWinch[mastOrder].transform.position;
            component.midRopeAttachment.parent = __instance.midAngleWinch[mastOrder].transform;
*/
            ready = true;
        }

        private void LateUpdate()
        {
            if (!ready)
            {
                Initialize();
            }

            sail.limits = sailBelow.limits;
            connections.angleControllerMid.transform.position = midConnection.position;
            //connections.midRopeAttachment.transform.position = midConnection.position;
            midRope.currentRopeLength = 0f;
        }
    }

}
