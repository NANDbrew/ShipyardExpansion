using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class SailScaleButton : GoPointerButton
    {
        public enum ButtonType
        {
            scaleUp,
            scaleDown,
            increaseHeight,
            decreaseHeight,
            rotateForward,
            rotateBackward
        }
        public ButtonType buttonType;

        public void SetText(string text)
        {
            base.transform.GetChild(0).GetComponent<TextMesh>().text = text;
        }
        public override void OnActivate()
        {
            base.OnActivate();
            UISoundPlayer.instance.PlayUISound(UISounds.buttonClick, 1f, 1f);
            if (buttonType == ButtonType.scaleUp)
            {
                GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>().ScaleUp();
            }
            else if (buttonType == ButtonType.scaleDown)
            {
                GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>().ScaleDown();
            }
            else if (buttonType == ButtonType.increaseHeight)
            {
                GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>().IncreaseHeight();
            }
            else if (buttonType == ButtonType.decreaseHeight)
            {
                GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>().DecreaseHeight();
            }
            else if (buttonType == ButtonType.rotateForward)
            {
                GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>().RotateFwd();
            }
            else if (buttonType == ButtonType.rotateBackward)
            {
                GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailScaler>().RotateBkwd();
            }
            ShipyardUI.instance.RefreshButtons();

        }
    }
}
