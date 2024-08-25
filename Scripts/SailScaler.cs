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

        float scaleFactor = 1f;
        
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
            scaleablePart = GetComponentInChildren<Animator>().transform;
            if (scaleablePart == null) UnityEngine.GameObject.Destroy(this);
            startScale = scaleablePart.localScale;
            scale = startScale;
            baseHeight = sail.installHeight / startScale.y;

            shadowCol = GetComponentInChildren<SailShadowCol>().transform;
            windCenter = sail.windcenter;
            colChecker = GetComponent<SailConnections>().colChecker.transform;
            if (sail.category == SailCategory.lateen)
            {
                rotatablePart = transform;
            }
            else if (sail.category == SailCategory.other && !sail.name.Contains("lug"))
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
        }
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
           /* if (angle + angleStep < maxAngle || angle > 270)
            {

            }*/
            SetAngle(angle + angleStep);
        }
        public void RotateBkwd()
        {
            /*if (angle - angleStep > minAngle || angle < 90)
            {
            }*/
            SetAngle(angle - angleStep);
        }
        public Vector3 SetScaleRel(float newScale)
        {
            return SetScaleAbs(newScale, newScale * ratio);

        }

        public Vector3 SetScaleRel(float newScale, float newRatio)
        {
            return SetScaleAbs(newScale, newScale * newRatio);

        }

        public Vector3 SetScaleAbs(float width, float height)
        {
            if (scaleType == ScaleType.Jib)
            {
                scale = new Vector3(width, height, height);
            }
            else
            {
                scale = new Vector3(width, height, width);
            }
            if (transform.parent.localScale.y != 1)
            {
                sail.installHeight = height * baseHeight * transform.localScale.y - (baseHeight * transform.localScale.y - baseHeight);
            }
            else
            {
                sail.installHeight = height * baseHeight;
            }

            ratio = height / width;
            shadowCol.parent = scaleablePart;
            windCenter.parent = scaleablePart;
            scaleablePart.gameObject.SetActive(false);
            scaleablePart.localScale = scale;
            colChecker.localScale = scale;
            if (scaleablePart.GetComponent<SailPartLocations>() is SailPartLocations locs)
            {
                scaleablePart.localPosition = new Vector3(locs.forwardOffset * width, scaleablePart.localPosition.y, scaleablePart.localPosition.z);
            }
            scaleablePart.gameObject.SetActive(true);
            sail.SetSailArea();
            shadowCol.parent = transform;
            windCenter.parent = transform;

            return scale;
        }

        public void IncreaseHeight()
        {
            if (ratio >= ratioLimits[1]) 
            {
                Debug.Log("cannot make sail taller");
                return;
            }
            SetScaleAbs(scale.x, scale.y + scaleStep);
        }
        public void IncreaseWidth()
        {
            if (ratio <= ratioLimits[0]) 
            {
                Debug.Log("cannot make sail wider");
                return;
            }
            SetScaleAbs(scale.x + scaleStep, scale.y);
        }
        public void DecreaseHeight()
        {
            if (ratio <= ratioLimits[0])
            {
                Debug.Log("cannot make sail shorter");
                return;
            }
            SetScaleAbs(scale.x, scale.y - scaleStep);
        }
        public void DecreaseWidth()
        {
            if (ratio >= ratioLimits[1])
            {
                Debug.Log("cannot make sail narrower");
                return;
            }
            SetScaleAbs(scale.x - scaleStep, scale.y);
        }
        public void ScaleUp()
        {
            if (scale.x + scaleStep > scaleLimits[1])
            {
                Debug.Log("cannot make sail bigger");
                return;
            }
            SetScaleRel(scale.x + scaleStep, ratio);
        }
        public void ScaleDown()
        {
            if (scale.x - scaleStep < scaleLimits[0])
            {
                Debug.Log("cannot make sail smaller");
                return;
            }
            SetScaleRel(scale.x - scaleStep, ratio);
        }

    }
    public enum ScaleType
    {
        Uniform,
        Square,
        Jib
    }

}
