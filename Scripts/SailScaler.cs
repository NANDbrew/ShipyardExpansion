using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace ShipyardExpansion
{
    internal class SailScaler : MonoBehaviour
    {
        Sail sail;
        public Transform scaleablePart;
        public Vector3 scale;
        Vector3 startScale;
        float baseHeight;
        Vector3 basePos;
        float ratio = 1f;
        float[] ratioLimits = new float[2] { 0.5f, 2f };
        float[] scaleLimits = new float[2] { 0.5f, 2.5f };
        float[] angleLimits = new float[2] { 340f, 15f };
        public float scaleStep = 0.05f;
        public float angle = 0;
        float angleStep = 1;
        Transform shadowCol;
        Transform windCenter;
        Transform colChecker;
        public Transform rotatablePart;
        ScaleType scaleType = ScaleType.Uniform;
        string baseName;
        //float scaleFactor = 1f;
        
        public float GetBaseHeight()
        {
            return baseHeight;
        }
        public ScaleType GetScaleType()
        {
            return scaleType;
        }

        private void Awake()
        {
            sail = GetComponent<Sail>();
            baseName = sail.sailName;
            scaleablePart = GetComponentInChildren<Animator>().transform;
            if (scaleablePart == null) UnityEngine.GameObject.Destroy(this);
            startScale = scaleablePart.localScale;
            scale = startScale;
            baseHeight = sail.installHeight / startScale.y;
            basePos = scaleablePart.localPosition / startScale.y;

            shadowCol = GetComponentInChildren<SailShadowCol>().transform;
            windCenter = sail.windcenter;
            colChecker = GetComponent<SailConnections>().colChecker.transform;
            if (sail.category == SailCategory.lateen)
            {
                rotatablePart = transform;
            }
            else if (sail.category == SailCategory.other)
            {
                rotatablePart = scaleablePart;
            }

            if (SailLimits.stretchableSquares.Contains(sail.prefabIndex)) 
            {
                scaleType = ScaleType.Square;
            }
            else if (SailLimits.stretchableJibs.Contains(sail.prefabIndex))
            {
                scaleType = ScaleType.Jib;
            }
            if (SailLimits.angleLimits.ContainsKey(sail.prefabIndex)) angleLimits = SailLimits.angleLimits[sail.prefabIndex];
            if (SailLimits.sizeLimits.ContainsKey(sail.prefabIndex)) scaleLimits = SailLimits.sizeLimits[sail.prefabIndex];
            else if (sail.category == SailCategory.gaff || sail.category == SailCategory.junk) scaleLimits = SailLimits.sizeLimits[-1];
            if (startScale.y < scaleLimits[0]) scaleLimits[0] = startScale.y * 0.8f;
            if (startScale.y > scaleLimits[1]) scaleLimits[1] = startScale.y * 1.2f;
        }
        #region rotation
        public void SetAngle(float newAngle)
        {
            if (rotatablePart == null) return;

            newAngle = (newAngle + 360) % 360;
            if (newAngle > angleLimits[1] && newAngle < 180) newAngle = angleLimits[1];
            else if (newAngle < angleLimits[0] && newAngle > 180) newAngle = angleLimits[0];

            shadowCol.parent = scaleablePart;
            windCenter.parent = scaleablePart;
            rotatablePart.gameObject.SetActive(false);
            rotatablePart.localEulerAngles = new Vector3(rotatablePart.localEulerAngles.x, newAngle, rotatablePart.localEulerAngles.z);
            angle = rotatablePart.localEulerAngles.y;
            rotatablePart.gameObject.SetActive(true);
            shadowCol.parent = transform;
            windCenter.parent = transform;
            if (GameState.currentShipyard != null && GameState.currentShipyard.sailInstaller.GetCurrentSail() == sail)
            {
                colChecker.localEulerAngles = new Vector3(colChecker.localEulerAngles.x, rotatablePart.localEulerAngles.y, colChecker.localEulerAngles.z);
                GameState.currentShipyard.sailInstaller.MoveHeldSail(0);
                //GameState.currentShipyard.sailInstaller.RecheckAllSailsCols();
                //sail.UpdateInstallPosition();
            }
        }

        public void RotateFwd()
        {
            SetAngle(angle + angleStep);
        }
        public void RotateBkwd()
        {
            SetAngle(angle - angleStep);
        }
        #endregion

        #region scaling
        public void SetScaleAbs(float width, float height)
        {
            float newRatio = height / width;
            if (newRatio < ratioLimits[0] || newRatio > ratioLimits[1]) return;
            if (height < scaleLimits[0] && scaleType == ScaleType.Jib) return;
            if (width < scaleLimits[0] && scaleType != ScaleType.Jib) return;
            if (width > scaleLimits[1] || height > scaleLimits[1]) return;
            Vector3 colScale;
            if (scaleType == ScaleType.Jib)
            {
                scale = new Vector3(width, height, height);
                colScale = scale;
            }
            else
            {
                scale = new Vector3(width, height, width);
                float colRot = colChecker.localEulerAngles.x - scaleablePart.localEulerAngles.x;
                if (colRot < 0) colRot = -colRot;
                if (colRot == 90)
                {
                    colScale = new Vector3(height, width, height);
                }
                else colScale = scale;
            }
            ratio = newRatio;
            UpdateInstallHeight();
            shadowCol.parent = scaleablePart;
            windCenter.parent = scaleablePart;
            scaleablePart.gameObject.SetActive(false);
            scaleablePart.localScale = scale;
            scaleablePart.localPosition = basePos * height;
            if (!sail.IsInstalled())
            {
                colChecker.localScale = colScale;
            }
            if (scaleablePart.GetComponent<SailPartLocations>() is SailPartLocations locs)
            {
                scaleablePart.localPosition = new Vector3(locs.forwardOffset * width, scaleablePart.localPosition.y, scaleablePart.localPosition.z);
            }
            scaleablePart.gameObject.SetActive(true);
            sail.SetSailArea();
            shadowCol.parent = transform;
            windCenter.parent = transform;
            if (Plugin.percentSailNames.Value)
            {
                sail.sailName = baseName + " " + "(" + Mathf.RoundToInt((height / startScale.y) * 100) + "%)";
            }
            else sail.sailName = baseName;
        }
        public void SetScaleRel(float newScale)
        {
            SetScaleAbs(newScale, newScale * ratio);
        }
        public void SetScaleRel(float newScale, float newRatio)
        {
            SetScaleAbs(newScale, newScale * newRatio);
        }

        public void IncreaseHeight()
        {
            SetScaleAbs(scale.x, scale.y + scaleStep);
        }
        public void IncreaseWidth()
        {
            SetScaleAbs(scale.x + scaleStep, scale.y);
        }
        public void DecreaseHeight()
        {
            SetScaleAbs(scale.x, scale.y - scaleStep);
        }
        public void DecreaseWidth()
        {
            SetScaleAbs(scale.x - scaleStep, scale.y);
        }
        public void ScaleUp()
        {
            SetScaleRel(scale.x + scaleStep, ratio);
        }
        public void ScaleDown()
        {
            SetScaleRel(scale.x - scaleStep, ratio);
        }
        public void UpdateInstallHeight()
        {
            UpdateInstallHeight(transform.parent);
        }
        public void UpdateInstallHeight(Transform mast)
        {
            sail.installHeight = scale.y * baseHeight / mast.localScale.z;
            Debug.Log("adjusting sail scale for resized mast");

        }

        #endregion
#if DEBUG
        // !!FOR TESTING ONLY!! remove before publishing!
        public Transform mastCol;
        public Vector3 mastColPos;
        public Quaternion mastColRot;
        public Transform AlignMastCol()
        {
            Transform mast = transform.parent;
            mastCol = mast.GetComponent<Mast>().walkColMast;
            mastColPos = mastCol.position;
            mastColRot = mastCol.rotation;
            mastCol.transform.position = mast.transform.position;
            mastCol.transform.rotation = mast.transform.rotation;
            return mastCol;
        }
        public void ReturnMastCol()
        {
            mastCol.position = mastColPos;
            mastCol.rotation = mastColRot;
        }

#endif
    }
    public enum ScaleType
    {
        Uniform,
        Square,
        Jib
    }

}
