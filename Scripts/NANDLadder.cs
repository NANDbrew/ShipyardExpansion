using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    public class NANDLadder : GoPointerButton
    {
        public static bool animating;
        public Transform[] targets;
        CharacterController player;
        Transform walkCol;

        public override void Start()
        {
            base.Start();
            player = Refs.charController;
            //GetComponentInParent<BoatRefs>().walkCol;
            //playerTrig = Refs.observerMirror.GetComponent<PlayerEmbarkDisembarkTrigger>();
            //lerpTime *= Vector3.Distance(target.localPosition, this.transform.localPosition);
            walkCol = GetComponentInParent<BoatRefs>().walkCol;
        }
        public override void OnActivate()
        {
            /*BoatEmbarkCollider embarkCol = GetComponentInParent<BoatRefs>().GetComponentInChildren<BoatEmbarkCollider>();
            playerTrig = Refs.observerMirror.GetComponentInChildren<PlayerEmbarkDisembarkTrigger>();
            if (PlayerEmbarkDisembarkTrigger.embarked)
            {
                AccessTools.Method(playerTrig.GetType(), "ExitBoat").Invoke(playerTrig, null);
            }
            AccessTools.Method(playerTrig.GetType(), "EnterBoat").Invoke(playerTrig, new object[] { embarkCol.transform.parent, embarkCol.walkCollider });
*/
            if (player.transform.parent == walkCol && targets.Length >= 1)
            {
                Transform target = targets.OrderBy(t => (t.transform.localPosition - player.transform.localPosition).sqrMagnitude).Last();
                this.StartCoroutine(HackPlayerPos(target));
            }
            else Debug.Log("player is not on the same boat");
        }
        private IEnumerator HackPlayerPos(Transform target)
        {
            animating = true;
            player.enabled = false;
            Vector3 start = player.transform.localPosition;
            float lerpTime = Vector3.Distance(start, target.localPosition) * 0.1f;
            if (start.y < target.localPosition.y || Plugin.climbSpeed.Value > 10) { lerpTime /= Plugin.climbSpeed.Value * 0.1f; }
            for (float t = 0f; t < 1f; t += Time.deltaTime / lerpTime)
            {
                player.transform.localPosition = Vector3.Lerp(start, target.localPosition, t);
                yield return new WaitForEndOfFrame();
                if (GameInput.GetKey(InputName.Activate)) { break; }
            }
            player.enabled = true;
            animating = false;
        }
    }
}
