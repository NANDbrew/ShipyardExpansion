
using System.Collections;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    public class ShipyardUnfurlButton : GoPointerButton
    {
        private static TextMesh text;
        private static bool furled = Plugin.unrollSails.Value;

        private void Awake()
        {
            text = transform.GetChild(0).GetComponent<TextMesh>();
        }

        public override void OnActivate()
        {
            float amount = furled ? 1f : 0f;
            StartCoroutine(UnrollSails(GameState.currentShipyard.GetCurrentBoat(), 0f, amount));
            //furled = !furled;
        }
        public static void SetState(bool state)
        {
            furled = state;
            text.text = furled ? "Unfurl\nSails" : "Furl\nSails";
        }
        public static IEnumerator UnrollSails(GameObject ship, float wait, float amount)
        {
            yield return new WaitForSeconds(wait);
            foreach (var mast in ship.GetComponent<BoatRefs>().masts)
            {
                if (mast == null) continue;
                foreach (var sail in mast.sails)
                {
                    sail.GetComponent<Sail>().currentUnroll = amount;
                    //sail.GetComponent<Sail>().enabled = false;
                    sail.transform.localEulerAngles = new Vector3(sail.transform.localEulerAngles.x, sail.transform.localEulerAngles.y, 0f);
                    sail.GetComponent<ReefEffectAnimUniversal>().RefreshCloth();
                }
            }
            SetState(Mathf.Approximately(amount, 0f));
            Debug.Log("unfurled sails on " + ship.name);
        }
    }
}
