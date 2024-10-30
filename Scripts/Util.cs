using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    public static class Util
    {
        public static void MoveMast(Transform mast, Vector3 position, bool moveWinches)
        {
            if (moveWinches) MoveWinches(mast.GetComponent<Mast>().reefWinch, mast.localPosition, position);
            foreach (var child in mast.GetComponent<BoatPartOption>().childOptions)
            {
                child.transform.localPosition += (position - mast.localPosition);
            }
            mast.localPosition = position;
            mast.GetComponent<Mast>().walkColMast.transform.localPosition = position;
        }
        public static void MoveWinches(GPButtonRopeWinch[] source, Vector3 sourcePosition, Vector3 targetPosition)
        {
            for (int i = 0; i < source.Length; i++)
            {

                Vector3 vector = source[i].transform.localPosition - sourcePosition;
                source[i].transform.localPosition = targetPosition + vector;
            }
        }
        public static Mast CopyMast(Transform source, Vector3 position, string name, string prettyName, int index)
        {
            return CopyMast(source, source.parent, source.GetComponent<Mast>().walkColMast.parent, position, source.localEulerAngles, source.localScale, name, prettyName, index);
        }
        public static Mast CopyMast(Transform source, Vector3 position, Vector3 eulerAngles, Vector3 scale, string name, string prettyName, int index)
        {
            return CopyMast(source, source.parent, source.GetComponent<Mast>().walkColMast.parent, position, eulerAngles, scale, name, prettyName, index);
        }
        public static Mast CopyMast(Transform source, Transform parent, Transform walkColParent, Vector3 position, Vector3 eulerAngles, Vector3 scale, string name, string prettyName, int index)
        {
            source.gameObject.SetActive(false);
            Transform mast = UnityEngine.Object.Instantiate(source, parent);
            float scaleFactor = scale.z / source.localScale.z;
            Mast mastComp = mast.GetComponent<Mast>();
            mastComp.orderIndex = index;
            mast.name = "SE_" + name;
            mast.localPosition = position;
            mast.localEulerAngles = eulerAngles;
            mast.localScale = scale;
            mastComp.walkColMast = UnityEngine.Object.Instantiate(source.GetComponent<Mast>().walkColMast, walkColParent);
            mastComp.walkColMast.name = name;
            mastComp.walkColMast.transform.localPosition = position;
            mastComp.walkColMast.transform.localEulerAngles = eulerAngles;
            mastComp.walkColMast.transform.localScale = scale;
            mastComp.startSailPrefab = null;
            mastComp.shipRigidbody = parent.GetComponentInParent<Rigidbody>();
            //mastComp.mastHeight = (float)Math.Round(mastComp.mastHeight * scaleFactor, 1);
            if (mast.GetComponent<BoatPartOption>() is BoatPartOption mastOption)
            {
                //BoatPartOption mastOption = mast.GetComponent<BoatPartOption>();
                mastOption.mass = Mathf.RoundToInt(mastOption.mass * scaleFactor);
                mastOption.basePrice = Mathf.RoundToInt(mastOption.mass * scaleFactor);
                mastOption.optionName = prettyName;
                mastOption.childOptions = new GameObject[0];
                mastOption.walkColObject = mastComp.walkColMast.gameObject;
            }
            source.gameObject.SetActive(true);
            mast.gameObject.SetActive(true);
            //mastComp.Awake();
            return mastComp;
        }
        public static GPButtonRopeWinch[] CopyWinches(GPButtonRopeWinch[] source, Vector3 sourcePosition, Vector3 targetPosition)
        {
            GPButtonRopeWinch[] winches = new GPButtonRopeWinch[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                source[i].gameObject.SetActive(false);
                var winch = UnityEngine.Object.Instantiate(source[i], source[i].transform.parent);
                winch.name = source[i].name + "_mod";
                Vector3 vector = source[i].transform.localPosition - sourcePosition;
                winch.transform.localPosition = targetPosition + vector;
                winch.rope = null;
                winches[i] = winch;
                source[i].gameObject.SetActive(true);
                winch.gameObject.SetActive(true);
            }
            return winches;
        }
        public static GPButtonRopeWinch CopyWinch(GPButtonRopeWinch source, Vector3 targetPosition)
        {

            source.gameObject.SetActive(false);
            var winch = UnityEngine.Object.Instantiate(source, source.transform.parent);
            winch.name = source.name + "_mod";
            winch.transform.localPosition = targetPosition;
            winch.rope = null;
            source.gameObject.SetActive(true);
            winch.gameObject.SetActive(true);
            
            return winch;
        }
        public static BoatPartOption CreatePartOption(Transform parent, string name, string prettyName)
        {
            GameObject part = UnityEngine.Object.Instantiate(new GameObject(), parent);
            BoatPartOption partOption = AddPartOption(part, prettyName);
            part.name = name;

            return partOption;
        }
        public static BoatPartOption AddPartOption(GameObject target, string prettyName)
        {
            //GameObject part = UnityEngine.Object.Instantiate(new GameObject(), parent);
            BoatPartOption partOption = target.AddComponent<BoatPartOption>();
            partOption.optionName = prettyName;
            partOption.childOptions = new GameObject[0];
            partOption.requires = new List<BoatPartOption>();
            partOption.requiresDisabled = new List<BoatPartOption>();
            partOption.walkColObject = target;
            
            return partOption;
        }

        public static BoatPartOption CopyPartOption(BoatPartOption source, GameObject target, string prettyName)
        {
            //GameObject part = UnityEngine.Object.Instantiate(source.gameObject, source.transform.parent);
            //GameObject walkCol = UnityEngine.Object.Instantiate(source.walkColObject, source.walkColObject.transform.parent);
            //BoatPartOption partOption = part.GetComponent<BoatPartOption>();
            //part.transform.localPosition = position;
            //walkCol.transform.localPosition = position;
            BoatPartOption partOption = target.AddComponent<BoatPartOption>();
            partOption.optionName = prettyName;
            partOption.childOptions = source.childOptions;
            partOption.requires = source.requires;
            partOption.requiresDisabled = source.requiresDisabled;
            partOption.walkColObject = source.walkColObject;
            partOption.basePrice = source.basePrice;
            partOption.installCost = source.installCost;
            partOption.mass = source.mass;
            partOption.childMast = source.childMast;
            
            return partOption;
        }
        public static BoatPartOption CopyPartOptionObj(BoatPartOption source, string name, string prettyName)
        {
            return CopyPartOptionObj(source, source.transform.localPosition, source.transform.localEulerAngles, source.transform.localScale, name, prettyName);
        }

        public static BoatPartOption CopyPartOptionObj(BoatPartOption source, Vector3 position, Vector3 eulerAngles, Vector3 scale, string name, string prettyName)
        {
            GameObject part = UnityEngine.Object.Instantiate(source.gameObject, source.transform.parent);
            part.name = name;
            GameObject walkCol = UnityEngine.Object.Instantiate(source.walkColObject, source.walkColObject.transform.parent);
            walkCol.name = name;
            BoatPartOption partOption = part.GetComponent<BoatPartOption>();
            partOption.walkColObject = walkCol;
            part.transform.localPosition = position;
            part.transform.localEulerAngles = eulerAngles;
            part.transform.localScale = scale;
            walkCol.transform.localPosition = position;
            walkCol.transform.localEulerAngles = eulerAngles;
            walkCol.transform.localScale = scale;
            partOption.optionName = prettyName;
            

            return partOption;
        }
        public static BoatPart CreateAndAddPart(BoatCustomParts partsList, int category, List<BoatPartOption> partOptions)
        {
            BoatPart newPart = new BoatPart
            {
                partOptions = partOptions,
                category = category,
                activeOption = 0
            };
            partsList.availableParts.Add(newPart);


            return newPart;
        }
        public static BoatPart CreateAndInsertPart(BoatCustomParts partsList, int category, int index, List<BoatPartOption> partOptions)
        {
            BoatPart newPart = new BoatPart
            {
                partOptions = partOptions,
                category = category,
                activeOption = 0
            };
            partsList.availableParts.Insert(index, newPart);


            return newPart;
        }


        public static GameObject CopySail(GameObject[] sailPrefabs, int prefabIndex, Vector3 position, Vector3 eulerAngles, string name, string prettyName, int newIndex)
        {
            Transform sailObject = sailPrefabs[prefabIndex].GetComponentInChildren<Animator>().transform;
            return CopySail(sailPrefabs, prefabIndex, position, eulerAngles, sailObject.localScale.x, name, prettyName, newIndex);
        }
        public static GameObject CopySail(GameObject[] sailPrefabs, int prefabIndex, Vector3 position, Vector3 eulerAngles, float scale, string name, string prettyName, int newIndex)
        {
            //Debug.Log("thing");
            Transform sailBase = UnityEngine.Object.Instantiate(sailPrefabs[prefabIndex], Plugin.prefabContainer).transform;
            var sail = sailBase.GetComponent<Sail>();
            sail.prefabIndex = newIndex;
            sail.sailName = prettyName;
            sailBase.name = newIndex + " SAIL " + name;
            var windShadow = sailBase.Find("wind shadow col");

            Transform sailObject = sailBase.GetComponentInChildren<Animator>().transform;
            sail.windcenter.parent = sailObject;
            windShadow.parent = sailObject;
            sailObject.localPosition = position;
            sailObject.localEulerAngles = eulerAngles;
            sail.windcenter.parent = sailBase;
            windShadow.parent = sailBase;
            sailObject.SetAsFirstSibling();
            SailPartLocations offsetStore = sailObject.gameObject.AddComponent<SailPartLocations>();
            offsetStore.forwardOffset = position.x / scale;
            var col_parent = sailBase.GetComponent<SailConnections>().colChecker.transform;
            UnityEngine.Object.Destroy(col_parent.GetComponent<Rigidbody>());
            var locs = col_parent.gameObject.AddComponent<SailPartLocations>();

            foreach (Transform child in col_parent.gameObject.GetComponentsInChildren<Transform>(true).Where(go => go.gameObject != col_parent.gameObject))
            {
                child.transform.localPosition += position;
                /*                UnityEngine.Object.Destroy(child.GetComponent<Rigidbody>());
                                UnityEngine.Object.Destroy(child.GetComponent<ShipyardSailColCheckerSub>());
                */
                locs.locations.Add(child.transform.localPosition);
            }
            sail.installHeight = (float)Math.Round(sail.installHeight * (scale / sailObject.localScale.y), 2);
            sailObject.localScale = new Vector3(scale, scale, scale);
            //sailBase.gameObject.SetActive(false);
            sailPrefabs[newIndex] = sailBase.gameObject;
            return sailBase.gameObject;
        }
    }
}
