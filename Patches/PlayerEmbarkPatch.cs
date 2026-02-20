/*using HarmonyLib;
using ShipyardExpansion.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(PlayerEmbarkDisembarkTrigger), "LateUpdate")]
    internal static class PlayerEmbarkPatch
    {
        public static bool Prefix(bool __runOriginal)
        {
            if (NANDLadder.animating || !__runOriginal) return false;
            return true;

        }

    }
}
*/