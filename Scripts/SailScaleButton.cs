using HarmonyLib;
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

        private static SailScaleButton clickHeldButton;

        private static float heldTimer;

        public static float step = 0.05f;

        public override void ExtraLateUpdate()
        {
            if ((clickHeldButton == this && Input.GetMouseButtonUp(0)) || GameInput.GetKeyUp(InputName.PickUp))
            {
                clickHeldButton = null;
            }

            if (clickHeldButton == this)
            {
                heldTimer -= Time.deltaTime;
                if (heldTimer <= 0f)
                {
                    heldTimer = 0.05f;
                    OnActivate();
                }
            }
        }
        public void SetText(string text)
        {
            base.transform.GetChild(0).GetComponent<TextMesh>().text = text;
        }
        public override void OnActivate()
        {
            base.OnActivate();
            UISoundPlayer.instance.PlayUISound(UISounds.buttonHover, 0.33f, 3f);
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
                Sail curSail = GameState.currentShipyard.sailInstaller.GetCurrentSail();
                float ratio = curSail.GetScaleZ() / curSail.GetScaleY();
                GameState.currentShipyard.ChangeSailScale(step, step * ratio);

            }
            else if (buttonType == ButtonType.scaleDown)
            {
                Sail curSail = GameState.currentShipyard.sailInstaller.GetCurrentSail();
                float ratio = curSail.GetScaleZ() / curSail.GetScaleY();
                GameState.currentShipyard.ChangeSailScale(-step, -step / ratio);

            }
            else if (buttonType == ButtonType.increaseHeight)
            {
                if (MastNotTallEnough(mast, sail))
                {
                    Debug.Log("mast cannot fit taller sail");
                    return;
                }
                GameState.currentShipyard.ChangeSailScale(step, 0f);
                //scaler.IncreaseHeight();
            }
            else if (buttonType == ButtonType.decreaseHeight)
            {
                GameState.currentShipyard.ChangeSailScale(-step, 0f);
            }
            else if (buttonType == ButtonType.increaseWidth)
            {
                GameState.currentShipyard.ChangeSailScale(0f, step);
            }
            else if (buttonType == ButtonType.decreaseWidth)
            {
                GameState.currentShipyard.ChangeSailScale(0f, -step);
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
            if (buttonType != ButtonType.flip)
            { HoldButton(); }

            ShipyardUI.instance.RefreshButtons();

        }
        private bool MastNotTallEnough(Mast mast, Sail sail)
        {
            float num = 0f;
            if (sail.UseExtendedMastHeight())
            {
                num = mast.extraBottomHeight;
            }

            if (sail.GetScaledHeight() > mast.mastHeight + num)
            {
                return true;
            }

            return false;
        }

        private void HoldButton()
        {
            if (clickHeldButton != this)
            {
                clickHeldButton = this;
                heldTimer = 0.16f;
            }
        }
    }
}
