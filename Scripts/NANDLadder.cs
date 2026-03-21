using System.Collections;
using System.Linq;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    public class NANDLadder : GoPointerButton
    {
        public static bool animating;
        public Transform[] targets;
        CharacterController player;
        public Transform walkCol;

        public override void Start()
        {
            base.Start();
            player = Refs.charController;
        }
        public override void OnActivate()
        {
            if (player.transform.parent == walkCol && targets.Length >= 1)
            {
                Transform target = targets.OrderBy(t => (t.transform.localPosition - player.transform.localPosition).sqrMagnitude).Last();
                this.StartCoroutine(HackPlayerPos(target));
            }
            else Debug.Log("NANDLadder: player is not on the same boat");
        }
        private IEnumerator HackPlayerPos(Transform target)
        {
            animating = true;
            //player.enabled = false;
            Refs.SetPlayerControl(false);
            Vector3 start = player.transform.localPosition;
            float lerpTime = Vector3.Distance(start, target.localPosition) * 0.1f;
            if (start.y < target.localPosition.y || Plugin.climbSpeed.Value > 10) { lerpTime /= Plugin.climbSpeed.Value * 0.1f; }
            for (float t = 0f; t < 1f; t += Time.deltaTime / lerpTime)
            {
                if (player.transform.parent != walkCol) 
                {
                    Debug.LogWarning("NANDLadder: player disembarked mid-climb");
                    break; 
                }
                if (GameInput.GetKey(InputName.Activate))
                {
                    Debug.Log("NANDLadder: player cancelled climb");
                    break;
                }
                player.transform.localPosition = Vector3.Lerp(start, target.localPosition, t);
                yield return new WaitForEndOfFrame();
            }
            Refs.SetPlayerControl(true);
            //player.enabled = true;
            animating = false;
        }
    }
}
