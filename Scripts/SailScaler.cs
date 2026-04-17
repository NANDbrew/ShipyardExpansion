using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace ShipyardExpansion
{
    public class SailScaler : MonoBehaviour
    {
        public static float scaleStep = 0.05f;
        public static float angleStep = 1;
        Sail sail;
        //Transform shadowCol;
        //Transform windCenter;
        Transform colChecker;
        public Transform rotatablePart;
        public Transform scaleablePart;
        public float[] ratioLimits = { 0.3f, 2.5f };
        public float[] scaleLimits = { 0.2f, 3f };
        public float[] angleLimits = { 340f, 15f };
        //public Vector3 Scale { get; private set; }
        public float Angle { get; private set; }
        public bool Flipped { get; private set; }
        //Vector3 startScale;
        //float baseHeight;
        //Vector3 basePos;
        //float ratio = 1f;
        public ScaleType scaleType = ScaleType.Uniform;
        string baseName;
        public bool flippable = false;
        float jibStartAngle;

/*        public float GetBaseHeight()
        {
            return baseHeight;
        }*/
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
                
                this.enabled = false;
            }
            //startScale = scaleablePart.localScale;
            //Scale = startScale;
            //baseHeight = sail.installHeight / startScale.y;
            //basePos = scaleablePart.localPosition / startScale.y;

            //shadowCol = GetComponentInChildren<SailShadowCol>().transform;
            //windCenter = sail.windcenter;
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
            else if (SailClothRotations.clothRotations.ContainsKey(sail.prefabIndex))
            {
                scaleablePart.GetComponentInChildren<WindCloth>().transform.localEulerAngles = SailClothRotations.clothRotations[sail.prefabIndex];
                scaleType = ScaleType.Square;
                Debug.Log("SailSaler: Rotated sailcloth");
            }

            if (SailLimits.angleLimits.ContainsKey(sail.prefabIndex)) angleLimits = SailLimits.angleLimits[sail.prefabIndex];
            //if (SailLimits.sizeLimits.ContainsKey(sail.prefabIndex)) scaleLimits = SailLimits.sizeLimits[sail.prefabIndex];
            //if (SailLimits.ratioLimits.ContainsKey(sail.prefabIndex)) ratioLimits = SailLimits.ratioLimits[sail.prefabIndex];

            else if (sail.category == SailCategory.gaff || sail.category == SailCategory.junk) scaleLimits = SailLimits.sizeLimits[-1];

            //if (startScale.y < scaleLimits[0]) scaleLimits[0] = startScale.y * 0.8f;
            //if (startScale.y > scaleLimits[1]) scaleLimits[1] = startScale.y * 1.2f;
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

            rotatablePart.gameObject.SetActive(false);
            rotatablePart.localEulerAngles = new Vector3(rotatablePart.localEulerAngles.x, newAngle, rotatablePart.localEulerAngles.z);
            Angle = rotatablePart.localEulerAngles.y;
            rotatablePart.gameObject.SetActive(true);

            if (GameState.currentShipyard != null && GameState.currentShipyard.sailInstaller.GetCurrentSail() == sail)
            {
                colChecker.localEulerAngles = new Vector3(colChecker.localEulerAngles.x, rotatablePart.localEulerAngles.y, colChecker.localEulerAngles.z);
                GameState.currentShipyard.sailInstaller.MoveHeldSail(0);

            }
        }
        public void FlipJib()
        {
            FlipJib(!Flipped);
        }
        public void FlipJib(bool inv)
        {
            if (scaleablePart == null) return;

            scaleablePart.gameObject.SetActive(false);
            scaleablePart.localEulerAngles = new Vector3(jibStartAngle + 180, scaleablePart.localEulerAngles.y, scaleablePart.localEulerAngles.z);
            Debug.Log("jib flip rotation = " + scaleablePart.localEulerAngles.ToString());
            if (inv)
            {
                scaleablePart.localPosition = new Vector3(scaleablePart.localPosition.x, scaleablePart.localPosition.y, -sail.GetScaledHeight());
                scaleablePart.localEulerAngles = new Vector3(jibStartAngle + 180, scaleablePart.localEulerAngles.y, scaleablePart.localEulerAngles.z);
            }
            else
            {
                scaleablePart.localPosition = new Vector3(scaleablePart.localPosition.x, scaleablePart.localPosition.y, 0f);
                scaleablePart.localEulerAngles = new Vector3(jibStartAngle, scaleablePart.localEulerAngles.y, scaleablePart.localEulerAngles.z);
            }

            scaleablePart.gameObject.SetActive(true);

            if (inv && sail.category == SailCategory.staysail && scaleablePart.gameObject.GetComponentInChildren<RopeEffect>() is RopeEffect childRope)
            {
                var rope = sail.GetComponent<SailConnections>().mastReefAttachment;
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

            Vector3 newScale = new Vector3(width, height, width);

            if (scaleType == ScaleType.Square)
            {
                newScale = new Vector3(width, height, width);
            }
            else if (scaleType == ScaleType.Jib)
            {
                newScale = new Vector3(width, height, height);
                //colScale = Scale;
            }
            else
            {
                newScale = new Vector3(height, height, height);
            }

            float num = scaleablePart.localPosition.z / scaleablePart.localScale.y;
            float num2 = scaleablePart.localPosition.x / scaleablePart.localScale.x;
            scaleablePart.localScale = newScale;
            scaleablePart.localPosition = new Vector3(num2 * scaleablePart.localScale.x, 0f, num * scaleablePart.localScale.y);
            AccessTools.Method(sail.GetType(), "UpdateScaledInstallHeight").Invoke(sail, null);
            sail.SetSailArea();
            Transform transform = sail.GetComponent<SailConnections>().colChecker.transform;
            if (transform.parent.gameObject.layer == 8)
            {
                transform.localScale = new Vector3(scaleablePart.localScale.x, scaleablePart.localScale.z, scaleablePart.localScale.y);
            }

            // add scale to name so player knows which sail is which
            if (Plugin.percentSailNames.Value && (!Mathf.Approximately(width, 1) || !Mathf.Approximately(height, 1)))
            {
                string size2 = "";
                if (width != height) size2 = "x" + Mathf.RoundToInt((width / 1) * 100) + "%";
                sail.sailName = $"{baseName} ({Mathf.RoundToInt((height / 1) * 100)}%{size2})";
            }
            else sail.sailName = baseName;
            //UpdateInstallHeight();
        }

        // ratio-aware scaling
        public void SetScaleRel(float newScale)
        {
            if (newScale < scaleLimits[0] || newScale > scaleLimits[1]) return;
            float ratio = sail.GetScaleY() / sail.GetScaleZ();
            if (newScale * ratio < scaleLimits[0] || newScale * ratio > scaleLimits[1]) return;
            SetScaleAbs(newScale, newScale * ratio);
        }
        public void SetScaleRel(float newScale, float newRatio)
        {
            if (newRatio < ratioLimits[0] || newRatio > ratioLimits[1]) return;
            if (newScale < scaleLimits[0] || newScale > scaleLimits[1]) return;
            if (newScale * newRatio < scaleLimits[0] || newScale * newRatio > scaleLimits[1]) return;
            //float ratio = sail.GetScaleY() / sail.GetScaleZ();

            SetScaleAbs(newScale, newScale * newRatio);
            //sail.ChangeScale(newScale, newScale * newRatio);
        }

        public void IncreaseHeight()
        {
            SetScaleRel(sail.GetScaleZ(), (sail.GetScaleY() + scaleStep) / sail.GetScaleZ());
        }
        public void IncreaseWidth()
        {
            SetScaleRel(sail.GetScaleZ() + scaleStep, sail.GetScaleY() / (sail.GetScaleZ() + scaleStep));
        }
        public void DecreaseHeight()
        {
            SetScaleRel(sail.GetScaleZ(), (sail.GetScaleY() - scaleStep) / sail.GetScaleZ());
        }
        public void DecreaseWidth()
        {
            SetScaleRel(sail.GetScaleZ() - scaleStep, sail.GetScaleY() / (sail.GetScaleZ() - scaleStep));
        }
        public void ScaleUp()
        {
            float ratio = sail.GetScaleY() / sail.GetScaleZ();
            SetScaleRel(sail.GetScaleZ() + scaleStep, ratio);
        }
        public void ScaleDown()
        {
            float ratio = sail.GetScaleY() / sail.GetScaleZ();
            SetScaleRel(sail.GetScaleY() - scaleStep, ratio);
        }
        public void UpdateInstallHeight()
        {
            UpdateInstallHeight(transform.parent);
        }

        // this is just to adjust for masts with non-standard scaling
        public void UpdateInstallHeight(Transform mast)
        {

            //sail.installHeight = Scale.y * baseHeight;
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
