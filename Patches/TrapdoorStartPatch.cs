using HarmonyLib;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(GPButtonTrapdoor), "Awake")]
    internal static class TrapdoorStartPatch
    {
        public static void Prefix(GPButtonTrapdoor __instance)
        {
            if (__instance.importedActualBoat == null)
            {
                __instance.importedActualBoat = __instance.transform.root.GetComponent<BoatRefs>().boatModel;
            }
        }
    }
}
