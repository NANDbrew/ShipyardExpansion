using HarmonyLib;


namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Mast), "OnDisable")]
    internal static class MastPatch
    {
        internal static void Prefix(Mast __instance)
        {
            if (Plugin.autoRemoveSails.Value && GameState.currentShipyard != null && GameState.currentShipyard.GetCurrentBoat() == __instance.shipRigidbody.gameObject)
            {
                var inst = GameState.currentShipyard.sailInstaller;
                for (int i = __instance.sails.Count - 1; i >= 0; i--)
                {
                    inst.DestroySail(__instance, __instance.sails[i].GetComponent<Sail>());
                    //UnityEngine.Debug.Log("SE: removed sail from mast: " + __instance.name);
                }
            }
        }
    }
}
