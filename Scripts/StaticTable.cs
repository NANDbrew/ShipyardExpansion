namespace ShipyardExpansion
{
    internal class StaticTable : GoPointerButton
    {
        public bool forceDisableOutline = true;
        public override bool OnItemClick(PickupableItem heldItem)
        {
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
