using HarmonyLib;
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
            increaseWidth,
            decreaseWidth,
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
            Mast mast = GameState.currentShipyard.sailInstaller.GetCurrentMast();
            Sail sail = GameState.currentShipyard.sailInstaller.GetCurrentSail();
            
            if (buttonType == ButtonType.scaleUp)
            {
                if (MastNotTallEnough(mast, sail))
                {
                    Debug.Log("mast cannot fit bigger sail");
                    return;
                }
                sail.GetComponent<SailScaler>().ScaleUp();

                if (sail.GetCurrentInstallHeight() < sail.installHeight)
                {
                    GameState.currentShipyard.sailInstaller.MoveHeldSail(sail.installHeight - sail.GetCurrentInstallHeight());
                }
                //else (sail.GetCurrentInstallHeight() < sail.installHeight)
                //sail.UpdateInstallPosition();

            }
            else if (buttonType == ButtonType.scaleDown)
            {
                float oldHeight = sail.installHeight;
                sail.GetComponent<SailScaler>().ScaleDown();
                if (!sail.UseExtendedMastHeight())
                {
                    GameState.currentShipyard.sailInstaller.MoveHeldSail(sail.installHeight - oldHeight);
                }
            }
            else if (buttonType == ButtonType.increaseHeight)
            {
                if (MastNotTallEnough(mast, sail))
                {
                    Debug.Log("mast cannot fit taller sail");
                    return;
                }

                sail.GetComponent<SailScaler>().IncreaseHeight();
                if (sail.GetCurrentInstallHeight() < sail.installHeight)
                {
                    GameState.currentShipyard.sailInstaller.MoveHeldSail(sail.installHeight - sail.GetCurrentInstallHeight());
                }

            }
            else if (buttonType == ButtonType.decreaseHeight)
            {
                sail.GetComponent<SailScaler>().DecreaseHeight();
            }
            else if (buttonType == ButtonType.increaseWidth)
            {
                sail.GetComponent<SailScaler>().IncreaseWidth();
            }
            else if (buttonType == ButtonType.decreaseWidth)
            {
                sail.GetComponent<SailScaler>().DecreaseWidth();
            }
            else if (buttonType == ButtonType.rotateForward)
            {
                sail.GetComponent<SailScaler>().RotateFwd();
            }
            else if (buttonType == ButtonType.rotateBackward)
            {
                sail.GetComponent<SailScaler>().RotateBkwd();
            }
            GameState.currentShipyard.sailInstaller.MoveHeldSail(0);

            ShipyardUI.instance.RefreshButtons();

        }
        private bool MastNotTallEnough(Mast mast, Sail sail)
        {
            ShipyardSailInstaller sailInstaller = GameState.currentShipyard.sailInstaller;
            return (bool)AccessTools.Method(sailInstaller.GetType(), "MastNotTallEnough").Invoke(sailInstaller, new object[2] { mast, sail });
        }
    }
}
