/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SE_Bridge;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Mast), "Awake")]
    internal static class TestPatch
    {
        public static void Postfix(Mast __instance)
        {
            if (__instance.onlyStaysails)
            {
                var col = __instance.GetComponent<CapsuleCollider>() ?? __instance.gameObject.AddComponent<CapsuleCollider>();
                col.direction = 2;
                col.center = new Vector3(0f, 0f, -(__instance.mastHeight / 2));
                col.radius = 0.1f;
                col.height = __instance.mastHeight;
                foreach (var req in __instance.GetComponent<BoatPartOption>()?.requires)
                {
                    if (req.GetComponent<Mast>() is Mast mast && !mast.mastCols.Contains(col))
                    {
                        mast.mastCols = mast.mastCols.AddToArray(col);
                    }
                }
                __instance.gameObject.AddComponent<SE_ClothColToggler>();
            }
        }
    }
}
*/