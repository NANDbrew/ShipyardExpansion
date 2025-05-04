using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    public class ObjectToggler : MonoBehaviour
    {
        public GameObject[] offObjects = new GameObject[0];
        public GameObject[] onObjects = new GameObject[0];

        public void OnEnable()
        {
            ToggleArray(offObjects, false);
            ToggleArray(onObjects, true);

        }
        public void OnDisable()
        {
            ToggleArray(offObjects, true);
            ToggleArray(onObjects, false);
        }

        public static void ToggleArray(GameObject[] parts, bool state)
        {
            foreach (GameObject obj in parts)
            {
                if (obj.GetComponent<GPButtonTrapdoor>() is GPButtonTrapdoor door)
                {
                    Transform col = (Transform)AccessTools.Field(typeof(GPButtonTrapdoor), "walkCol").GetValue(door);
                    if (col) col.gameObject.SetActive(state);
                }
                obj.SetActive(state);
            }
        }
    }
}
