using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal static class Util
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
            return CopyMast(source, position, source.localEulerAngles, source.localScale, name, prettyName, index);
        }
        public static Mast CopyMast(Transform source, Vector3 position, Vector3 eulerAngles, Vector3 scale, string name, string prettyName, int index)
        {
            source.gameObject.SetActive(false);
            Transform mast = UnityEngine.Object.Instantiate(source, source.parent);
            Mast mastComp = mast.GetComponent<Mast>();
            mastComp.orderIndex = index;
            mast.name = name;
            mast.localPosition = position;
            mast.localEulerAngles = eulerAngles;
            mast.localScale = scale;
            BoatPartOption mastOption = mast.GetComponent<BoatPartOption>();
            mastOption.optionName = prettyName;
            mastOption.childOptions = new GameObject[0];
            mastComp.walkColMast = UnityEngine.Object.Instantiate(source.GetComponent<Mast>().walkColMast, source.GetComponent<Mast>().walkColMast.parent);
            mastComp.walkColMast.name = name;
            mastComp.walkColMast.transform.localPosition = position;
            mastComp.walkColMast.transform.localEulerAngles = eulerAngles;
            mastComp.walkColMast.transform.localScale = scale;
            mastOption.walkColObject = mastComp.walkColMast.gameObject;
            //mastComp.orderIndex = 29;
            mastComp.startSailPrefab = null;
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

                Vector3 vector = source[i].transform.localPosition - sourcePosition;
                winch.transform.localPosition = targetPosition + vector;
                winches[i] = winch;
                source[i].gameObject.SetActive(true);
                winch.gameObject.SetActive(true);
            }
            return winches;
        }

        public static BoatPartOption CreatePartOption(Transform parent, string name, string prettyName)
        {
            GameObject part = UnityEngine.Object.Instantiate(new GameObject(), parent);
            BoatPartOption partOption = part.AddComponent<BoatPartOption>();
            partOption.optionName = prettyName;
            partOption.name = name;
            partOption.childOptions = new GameObject[0];
            partOption.requires = new List<BoatPartOption>();
            partOption.requiresDisabled = new List<BoatPartOption>();
            partOption.walkColObject = part;

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
    }
}
