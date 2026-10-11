
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    public class SailUnlockButton : GoPointerButton
    {
        public override void OnActivate()
        {
            var selectedSail = GameState.currentShipyard.sailInstaller.GetCurrentSail();
            if (selectedSail == null || !selectedSail.IsInstalled())
            {
                return;
            }
            selectedSail.SetInstalled(false);
            selectedSail.GetComponent<SailScaler>().scaleLimits[1] = Mathf.Max(selectedSail.GetScaleY(), selectedSail.GetScaleZ());

            GameState.currentShipyard.AddRefund(new ShipyardRefund(selectedSail.sailName, Mathf.RoundToInt(selectedSail.GetSailPrice() * GameState.currentShipyard.GetCurrencyRate() * 0.5f)));
            //GameState.currentShipyard.AddRefund(new ShipyardRefund(selectedSail.sailName, Mathf.RoundToInt(selectedSail.GetSailPrice())));
            GameState.currentShipyard.UpdateOrder();
            ShipyardUI.instance.UpdateMoveButtons();
        }
    }
}
