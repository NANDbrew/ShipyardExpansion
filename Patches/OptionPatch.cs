/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(BoatPart), "SetOptionEnabled")]
    internal static class OptionPatch
    {
        public static void Postfix(int i, bool state, List<BoatPartOption> ___partOptions)
        {
            //___partOptions[i].gameObject.SetActive(state);
            GameObject[] childOptions = ___partOptions[i].childOptions;
            for (int j = 0; j < childOptions.Length; j++)
            {
                childOptions[j].SetActive(state);
                if (childOptions[j].GetComponent<Mast>() is Mast mast)
                {
                    mast.Awake();
                }

            }
        }
    }
}
*/