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
        Transform scaleablePart;
        public Vector3 scale;
        Vector3 startScale;
        float baseHeight;
        float ratio = 1f;
        float maxRatio = 2f;
        float minRatio = 0.5f;
        float maxScale = 2.5f;
        float minScale = 0.5f;
        float stepSize = 0.05f;
        float maxAngle = 15;
        float minAngle = -15;
        public float angle = 0;
        float angleStep = 5;
        Transform shadowCol;
        Transform windCenter;
        Transform colChecker;
        public Transform rotatablePart;
        ScaleType scaleType = ScaleType.Uniform;

        public ScaleType GetScaleType()
        {
            return scaleType;
        }

        private void Awake()
        {
            scaleablePart = GetComponentInChildren<Animator>().transform;
            sail = GetComponent<Sail>();
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
            else if (sail.category == SailCategory.other)
            {
                rotatablePart = scaleablePart;
            }

            if (ScaleTypes.stretchableSquares.Contains(sail.prefabIndex)) 
            {
                scaleType = ScaleType.Square;
            }
            else if (ScaleTypes.stretchableJibs.Contains(sail.prefabIndex))
            {
                scaleType = ScaleType.Jib;
            }
        }

        public void SetAngle(float newAngle)
        {
            if (rotatablePart == null) return;
            shadowCol.parent = scaleablePart;
            windCenter.parent = scaleablePart;
            
            rotatablePart.localEulerAngles = new Vector3(rotatablePart.localEulerAngles.x, newAngle, rotatablePart.localEulerAngles.z);
            angle = newAngle;

            shadowCol.parent = transform;
            windCenter.parent = transform;
            if (GameState.currentShipyard != null && GameState.currentShipyard.sailInstaller.GetCurrentSail() == sail)
            {
                colChecker.localEulerAngles = rotatablePart.localEulerAngles;
                float dist = 0;
                //GameState.currentShipyard.sailInstaller.MoveHeldSail(dist);
                //GameState.currentShipyard.sailInstaller.RecheckAllSailsCols();
                //sail.UpdateInstallPosition();
            }
        }

        public void RotateFwd()
        {
            if (angle >= maxAngle) return;
            angle += angleStep;
            SetAngle(angle);
        }
        public void RotateBkwd()
        {
            if (angle <= minAngle) return;
            angle -= angleStep;
            SetAngle(angle);
        }

        public Vector3 SetScaleRel(float newScale, float newRatio)
        {
            return SetScaleAbs(newScale, newScale * newRatio);

        }

        public Vector3 SetScaleAbs(float width, float height)
        {

            if (scaleType == ScaleType.Uniform)
            {
                scale = new Vector3(width, width, width);
                sail.installHeight = baseHeight * width;
            }
            else if (scaleType == ScaleType.Square)
            {
                scale = new Vector3(width, height, width);
                sail.installHeight = baseHeight * height;
            }
            else if (scaleType == ScaleType.Jib)
            {
                scale = new Vector3(height, width, width);
                sail.installHeight = height;
            }
            ratio = height / width;
            shadowCol.parent = scaleablePart;
            windCenter.parent = scaleablePart;
            scaleablePart.gameObject.SetActive(false);
            scaleablePart.localScale = scale;
            colChecker.localScale = scale;
            scaleablePart.gameObject.SetActive(true);
            sail.SetSailArea();
            shadowCol.parent = transform;
            windCenter.parent = transform;

            if (GameState.currentShipyard != null && GameState.currentShipyard.sailInstaller.GetCurrentSail() == sail)
            {
                float dist = 0;
                if (sail.GetCurrentInstallHeight() < sail.installHeight)
                {
                    dist = sail.installHeight - sail.GetCurrentInstallHeight();
                }
                GameState.currentShipyard.sailInstaller.MoveHeldSail(dist);
                //sail.UpdateInstallPosition();
            }

            return scale;
        }

        public void IncreaseHeight()
        {
            if (ratio >= maxRatio) 
            {
                Debug.Log("cannot make sail taller");
                return;
            }
            SetScaleAbs(scale.x, scale.y + stepSize);
        }
        public void IncreaseWidth()
        {
            if (ratio <= minRatio) 
            {
                Debug.Log("cannot make sail wider");
                return;
            }
            SetScaleAbs(scale.x + stepSize, scale.y);
        }
        public void DecreaseHeight()
        {
            if (ratio <= minRatio)
            {
                Debug.Log("cannot make sail shorter");
                return;
            }
            SetScaleAbs(scale.x, scale.y - stepSize);
        }
        public void DecreaseWidth()
        {
            if (ratio >= maxRatio)
            {
                Debug.Log("cannot make sail narrower");
                return;
            }
            SetScaleAbs(scale.x - stepSize, scale.y);
        }
        public void ScaleUp()
        {
            if (scale.x >= maxScale)
            {
                Debug.Log("cannot make sail bigger");
                return;
            }
            SetScaleRel(scale.x + stepSize, ratio);
        }
        public void ScaleDown()
        {
            if (scale.x <= minScale)
            {
                Debug.Log("cannot make sail smaller");
                return;
            }
            SetScaleRel(scale.x - stepSize, ratio);
        }

    }
    public enum ScaleType
    {
        Uniform,
        Square,
        Jib
    }
    public static class ScaleTypes
    {
        public static List<int> stretchableSquares = new List<int> { 4, 32, 48, 49, 50, 51, 52, 59, 80, 113, 114, 115, 116,  };
        public static List<int> stretchableJibs = new List<int> { 31, 34, 110, 111,  };


    }


    [HarmonyPatch(typeof(SailConnections), "Awake")]
    internal static class CompAdder
    {
        public static void Postfix(SailConnections __instance)
        {
            __instance.gameObject.AddComponent<SailScaler>();
        }
    }
}
