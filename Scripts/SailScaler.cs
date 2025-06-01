using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    public class SailScaler : MonoBehaviour
    {
        public static float scaleStep = 0.05f;
        public static float angleStep = 1;
        Sail sail;
        Transform shadowCol;
        Transform windCenter;
        Transform colChecker;
        public Transform rotatablePart;
        public Transform scaleablePart;
        public float[] ratioLimits = { 0.3f, 2f };
        public float[] scaleLimits = { 0.5f, 2.5f };
        public float[] angleLimits = { 340f, 15f };
        public Vector3 Scale { get; private set; }
        public float Angle { get; private set; }
        public bool Flipped { get; private set; }
        Vector3 startScale;
        float baseHeight;
        Vector3 basePos;
        float ratio = 1f;
        public ScaleType scaleType = ScaleType.Uniform;
        string baseName;
        public bool flippable = false;
        float jibStartAngle;

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
            if (scaleablePart == null)
            {
                Debug.LogError("SailScaler: scaleable part is null!");
                //UnityEngine.GameObject.Destroy(this);
                this.enabled = false;
            }
            startScale = scaleablePart.localScale;
            Scale = startScale;
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
            if (SailLimits.ratioLimits.ContainsKey(sail.prefabIndex)) ratioLimits = SailLimits.ratioLimits[sail.prefabIndex];

            else if (sail.category == SailCategory.gaff || sail.category == SailCategory.junk) scaleLimits = SailLimits.sizeLimits[-1];
            if (startScale.y < scaleLimits[0]) scaleLimits[0] = startScale.y * 0.8f;
            if (startScale.y > scaleLimits[1]) scaleLimits[1] = startScale.y * 1.2f;
            flippable = sail.category == SailCategory.staysail || SailLimits.flippableSquares.Contains(sail.prefabIndex);

            jibStartAngle = flippable ? scaleablePart.localEulerAngles.x : jibStartAngle;
        }
        #region rotation
        public void SetAngle(float newAngle)
        {
            if (rotatablePart == null) return;

            newAngle = (newAngle + 360) % 360;
            if (newAngle > angleLimits[1] && newAngle < 180) newAngle = angleLimits[1];
            else if (newAngle < angleLimits[0] && newAngle > 180) newAngle = angleLimits[0];
            flippable = SailLimits.flippableSquares.Contains(sail.prefabIndex) || sail.category == SailCategory.staysail;


            shadowCol.SetParent(scaleablePart);
            windCenter.SetParent(scaleablePart);
            rotatablePart.gameObject.SetActive(false);
            rotatablePart.localEulerAngles = new Vector3(rotatablePart.localEulerAngles.x, newAngle, rotatablePart.localEulerAngles.z);
            Angle = rotatablePart.localEulerAngles.y;
            rotatablePart.gameObject.SetActive(true);
            shadowCol.SetParent(transform);
            windCenter.SetParent(transform);
            if (GameState.currentShipyard != null && GameState.currentShipyard.sailInstaller.GetCurrentSail() == sail)
            {
                colChecker.localEulerAngles = new Vector3(colChecker.localEulerAngles.x, rotatablePart.localEulerAngles.y, colChecker.localEulerAngles.z);
                GameState.currentShipyard.sailInstaller.MoveHeldSail(0);
                //GameState.currentShipyard.sailInstaller.RecheckAllSailsCols();
                //sail.UpdateInstallPosition();
            }
        }
        public void FlipJib()
        {
            FlipJib(!Flipped);
        }
        public void FlipJib(bool inv)
        {
            if (scaleablePart == null) return;
            shadowCol.SetParent(scaleablePart, true);
            windCenter.SetParent(scaleablePart, true);
            scaleablePart.gameObject.SetActive(false);
            scaleablePart.localEulerAngles = new Vector3(jibStartAngle + 180, scaleablePart.localEulerAngles.y, scaleablePart.localEulerAngles.z);
            Debug.Log("jib flip rotation = " + scaleablePart.localEulerAngles.ToString());
            if (inv)
            {
                scaleablePart.localPosition = new Vector3(scaleablePart.localPosition.x, scaleablePart.localPosition.y, -sail.installHeight);
                scaleablePart.localEulerAngles = new Vector3(jibStartAngle + 180, scaleablePart.localEulerAngles.y, scaleablePart.localEulerAngles.z);
            }
            else
            {
                scaleablePart.localPosition = new Vector3(scaleablePart.localPosition.x, scaleablePart.localPosition.y, 0f);
                scaleablePart.localEulerAngles = new Vector3(jibStartAngle, scaleablePart.localEulerAngles.y, scaleablePart.localEulerAngles.z);
            }
            shadowCol.SetParent(transform, true);
            windCenter.SetParent(transform, true);
            scaleablePart.gameObject.SetActive(true);

            if (inv && sail.category == SailCategory.staysail && scaleablePart.gameObject.GetComponentInChildren<RopeEffect>() is RopeEffect childRope)
            {
                var rope = /*sail.GetComponent<SailConnections>().mastReefAttExtension ??*/ sail.GetComponent<SailConnections>().mastReefAttachment;
                sail.GetComponent<SailConnections>().mastReefAttExtension.gameObject.SetActive(false);
                if (childRope.GetComponent<MeshFilter>() != null)
                {
                    rope.GetComponent<RopeEffect>().attachment = childRope.transform;
                    childRope.attachment.gameObject.SetActive(false);
                    childRope.GetComponent<LineRenderer>().enabled = false;
                    childRope.enabled = false;
                }
                else
                {
                    rope.GetComponent<RopeEffect>().attachment = childRope.attachment;
                    childRope.gameObject.SetActive(false);
                }
            }
            if (SailLimits.flippableSquares.Contains(sail.prefabIndex) && sail.GetComponent<SailConnections>().reefController is RopeControllerSailReef controller)
            {
                controller.reverseReefing = inv;
            }

            if (GameState.currentShipyard != null && GameState.currentShipyard.sailInstaller.GetCurrentSail() == sail)
            {
                colChecker.localEulerAngles = new Vector3(colChecker.localEulerAngles.x, scaleablePart.localEulerAngles.y, colChecker.localEulerAngles.z);
            }

            Flipped = inv;
        }

        public void RotateFwd()
        {
            SetAngle(Angle + angleStep);
        }
        public void RotateBkwd()
        {
            SetAngle(Angle - angleStep);
        }
        #endregion

        #region scaling
        public void SetScaleAbs(float width, float height)
        {
            width = Mathf.Clamp(width, scaleLimits[0], scaleLimits[1]);
            height = Mathf.Clamp(height, scaleLimits[0], scaleLimits[1]);
            float newRatio = height / width;
            Vector3 colScale;
            if (scaleType == ScaleType.Jib)
            {
                Scale = new Vector3(width, height, height);
                colScale = Scale;
            }
            else
            {
                Scale = new Vector3(width, height, width);
                float colRot = colChecker.localEulerAngles.x - scaleablePart.localEulerAngles.x;
                if (colRot < 0) colRot = -colRot;
                if (colRot == 90)
                {
                    colScale = new Vector3(width, width, height);
                }
                else colScale = Scale;
            }
            ratio = newRatio;
            shadowCol.SetParent(scaleablePart, true);
            windCenter.SetParent(scaleablePart, true);
            scaleablePart.gameObject.SetActive(false);
            scaleablePart.localScale = Scale;
            if (basePos.magnitude > 0.1f)
            {
                scaleablePart.localPosition = basePos * height;
                //Debug.Log(sail.sailName + "basePos > 0");
            }

            if (Flipped)
            {
                scaleablePart.localPosition = new Vector3(scaleablePart.localPosition.x, scaleablePart.localPosition.y, -sail.installHeight);
            }
            if (scaleablePart.GetComponent<SailPartLocations>() is SailPartLocations locs) // special treatment for hacked lug sail
            {
                scaleablePart.localPosition = new Vector3(locs.forwardOffset * width, scaleablePart.localPosition.y, scaleablePart.localPosition.z);
            }
            if (!sail.IsInstalled())
            {
                colChecker.localScale = colScale;
            }
            shadowCol.SetParent(transform, true);
            windCenter.SetParent(transform, true);
            scaleablePart.gameObject.SetActive(true);
            sail.SetSailArea();
            if (Plugin.percentSailNames.Value)
            {
                sail.sailName = baseName + " " + "(" + Mathf.RoundToInt((height / startScale.y) * 100) + "%)";
            }
            else sail.sailName = baseName;
            UpdateInstallHeight();
        }
        public void SetScaleRel(float newScale)
        {
            if (newScale < scaleLimits[0] || newScale > scaleLimits[1]) return;
            if (newScale * ratio < scaleLimits[0] || newScale * ratio > scaleLimits[1]) return;
            SetScaleAbs(newScale, newScale * ratio);
        }
        public void SetScaleRel(float newScale, float newRatio)
        {
            if (newRatio < ratioLimits[0] || newRatio > ratioLimits[1]) return;
            if (newScale < scaleLimits[0] || newScale > scaleLimits[1]) return;
            if (newScale * newRatio < scaleLimits[0] || newScale * newRatio > scaleLimits[1]) return;
            SetScaleAbs(newScale, newScale * newRatio);
        }

        public void IncreaseHeight()
        {
            SetScaleRel(Scale.x, (Scale.y + scaleStep) / Scale.x);
        }
        public void IncreaseWidth()
        {
            SetScaleRel(Scale.x + scaleStep, Scale.y / (Scale.x + scaleStep));
        }
        public void DecreaseHeight()
        {
            SetScaleRel(Scale.x, (Scale.y - scaleStep) / Scale.x);
        }
        public void DecreaseWidth()
        {
            SetScaleRel(Scale.x - scaleStep, Scale.y / (Scale.x - scaleStep));
        }
        public void ScaleUp()
        {
            SetScaleRel(Scale.x + scaleStep, ratio);
        }
        public void ScaleDown()
        {
            SetScaleRel(Scale.x - scaleStep, ratio);
        }
        public void UpdateInstallHeight()
        {
            UpdateInstallHeight(transform.parent);
        }
        public void UpdateInstallHeight(Transform mast)
        {
            sail.installHeight = Scale.y * baseHeight;
            if (mast && mast.localScale.z != 1)
            {
                sail.installHeight /= mast.localScale.z;
    #if DEBUG
                Debug.Log("adjusting sail \"" + sail.sailName + "\" scale for resized mast \"" + mast.name + "\"");
    #endif

            }
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

            foreach (Renderer rend in mastCol.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = true;
            }
            return mastCol;
        }
        public void ReturnMastCol()
        {
            mastCol.position = mastColPos;
            mastCol.rotation = mastColRot;
            foreach (Renderer rend in mastCol.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = false;
            }
        }
#endif

    }

}
