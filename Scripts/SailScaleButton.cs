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
            rotateBackward,
            flip
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
            SailScaler scaler = sail.GetComponent<SailScaler>();
            if (buttonType == ButtonType.scaleUp)
            {
                if (MastNotTallEnough(mast, sail))
                {
                    Debug.Log("mast cannot fit bigger sail");
                    return;
                }
                scaler.ScaleUp();
            }
            else if (buttonType == ButtonType.scaleDown)
            {
                scaler.ScaleDown();
            }
            else if (buttonType == ButtonType.increaseHeight)
            {
                if (MastNotTallEnough(mast, sail))
                {
                    Debug.Log("mast cannot fit taller sail");
                    return;
                }
                scaler.IncreaseHeight();
            }
            else if (buttonType == ButtonType.decreaseHeight)
            {
                scaler.DecreaseHeight();
            }
            else if (buttonType == ButtonType.increaseWidth)
            {
                scaler.IncreaseWidth();
            }
            else if (buttonType == ButtonType.decreaseWidth)
            {
                scaler.DecreaseWidth();
            }
            else if (buttonType == ButtonType.rotateForward)
            {
                scaler.RotateFwd();
            }
            else if (buttonType == ButtonType.rotateBackward)
            {
                scaler.RotateBkwd();
            }
            else if (buttonType == ButtonType.flip)
            {
                scaler.FlipJib();
            }
            float move = 0f;
            float extra = sail.UseExtendedMastHeight()? mast.extraBottomHeight : 0f;
            if (sail.GetCurrentInstallHeight() < sail.installHeight - extra)
            {
                move = sail.installHeight - sail.GetCurrentInstallHeight();
            }
            GameState.currentShipyard.sailInstaller.MoveHeldSail(move);

            ShipyardUI.instance.RefreshButtons();

        }
        private bool MastNotTallEnough(Mast mast, Sail sail)
        {
            ShipyardSailInstaller sailInstaller = GameState.currentShipyard.sailInstaller;
            return (bool)AccessTools.Method(sailInstaller.GetType(), "MastNotTallEnough").Invoke(sailInstaller, new object[2] { mast, sail });
        }
    }
}
