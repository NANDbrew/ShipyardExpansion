using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace ShipyardExpansion
{
    public class SailScaler : MonoBehaviour
    {
        Sail sail;
        public Transform scaleablePart;
        public Vector3 scale;
        Vector3 startScale;
        float baseHeight;
        Vector3 basePos;
        float ratio = 1f;
        float[] ratioLimits = new float[2] { 0.3f, 2f };
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
        public bool flipped = false;
        public bool flippable = false;
        //float scaleFactor = 1f;
        //Dictionary<BoxCollider, Vector3> colStartCenters;
        //Dictionary<BoxCollider, Vector3> colBaseCenters;
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
            flippable = sail.category == SailCategory.staysail || SailLimits.flippableSquares.Contains(sail.prefabIndex);
            /*colStartCenters = new Dictionary<BoxCollider, Vector3>();
            colBaseCenters = new Dictionary<BoxCollider, Vector3>();
            foreach (var col in colChecker.GetComponentsInChildren<BoxCollider>())
            {
                colStartCenters.Add(col, col.center);
                Vector3 cent = new Vector3(col.center.x / startScale.x, col.center.y / startScale.y, col.center.z / startScale.z);
                colBaseCenters.Add(col, cent);
            }*/
            jibStartAngle = scaleablePart.localEulerAngles.x;
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
            angle = rotatablePart.localEulerAngles.y;
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
            FlipJib(!flipped);
        }
        public void FlipJib(bool inv)
        {
            if (scaleablePart == null) return;
            shadowCol.SetParent(scaleablePart);
            windCenter.SetParent(scaleablePart);
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
            scaleablePart.gameObject.SetActive(true);
            shadowCol.SetParent(transform);
            windCenter.SetParent(transform);

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
                //float height = inv? -sail.installHeight: sail.installHeight;
                //GameState.currentShipyard.sailInstaller.MoveHeldSail(height);
                //GameState.currentShipyard.sailInstaller.RecheckAllSailsCols();
                //sail.UpdateInstallPosition();
            }

            flipped = inv;
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
            /*if (newRatio < ratioLimits[0] || newRatio > ratioLimits[1]) return;
            if (height < scaleLimits[0] && scaleType == ScaleType.Jib) return;
            if (width < scaleLimits[0] && scaleType != ScaleType.Jib) return;
            if (width > scaleLimits[1] || height > scaleLimits[1]) return;*/
            width = Mathf.Clamp(width, scaleLimits[0], scaleLimits[1]);
            height = Mathf.Clamp(height, scaleLimits[0], scaleLimits[1]);
            float newRatio = height / width;
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
                    colScale = new Vector3(width, width, height);
                }
                else colScale = scale;
            }
            ratio = newRatio;
            shadowCol.SetParent(scaleablePart);
            windCenter.SetParent(scaleablePart);
            scaleablePart.gameObject.SetActive(false);
            scaleablePart.localScale = scale;
            scaleablePart.localPosition = basePos * height;

            if (scaleablePart.GetComponent<SailPartLocations>() is SailPartLocations locs)
            {
                scaleablePart.localPosition = new Vector3(locs.forwardOffset * width, scaleablePart.localPosition.y, scaleablePart.localPosition.z);
            }
            scaleablePart.gameObject.SetActive(true);
            if (!sail.IsInstalled())
            {
                colChecker.localScale = colScale;
            }
            /*foreach (var col in colStartCenters.Keys)
            {
                if (col.gameObject == colChecker.gameObject)
                {
                    col.center = Vector3.Scale(colBaseCenters[col], colScale);
                }
                else col.center = Vector3.Scale(colBaseCenters[col], scale);
            }*/
            sail.SetSailArea();
            shadowCol.SetParent(transform);
            windCenter.SetParent(transform);
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
            SetScaleRel(scale.x, (scale.y + scaleStep) / scale.x);
        }
        public void IncreaseWidth()
        {
            SetScaleRel(scale.x + scaleStep, scale.y / (scale.x + scaleStep));
        }
        public void DecreaseHeight()
        {
            SetScaleRel(scale.x, (scale.y - scaleStep) / scale.x);
        }
        public void DecreaseWidth()
        {
            SetScaleRel(scale.x - scaleStep, scale.y / (scale.x - scaleStep));
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
            sail.installHeight = scale.y * baseHeight;
            if (flipped) scaleablePart.localPosition = new Vector3(scaleablePart.localPosition.x, scaleablePart.localPosition.y, -sail.installHeight);
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
    public enum ScaleType
    {
        Uniform,
        Square,
        Jib
    }

}
