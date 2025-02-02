using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(ShipyardSailColChecker), "Awake")]
    internal class JunkGaffFixer
    {
        private static readonly int[] colfixSails = new int[] { 17, 18, 107 };

        public static void Postfix(Sail ___sail, ref Vector3 ___sailModelOffset)
        {
            if (colfixSails.Contains(___sail.prefabIndex))
            {
                ___sailModelOffset = new Vector3(0f, 0f, 1.5f);
            }
        }
    }
}
