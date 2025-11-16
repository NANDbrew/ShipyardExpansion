using cakeslice;
using HarmonyLib;

namespace ShipyardExpansion
{
    [HarmonyPatch(typeof(GoPointerButton), "UpdateColor")]
    internal static class TablePatch
    {
        internal static bool Prefix(GoPointerButton __instance, Outline ___outline)
        {
            if (__instance is StaticTable table)
            {
                if (table.forceDisableOutline)
                {
                    ___outline.enabled = false; 
                }
                return false;
            }
            return true;
        }
    }
/*    [HarmonyPatch(typeof(GoPointerButton), "Look")]
    internal static class TablePatch2
    {
        internal static bool Prefix(GoPointerButton __instance)
        {
            if (__instance is StaticTable)
            {
                return false;
            }
            return true;
        }
    }*/

}
