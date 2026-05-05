/*using HarmonyLib;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Shipyard), "AdmitShip")]
    internal static class EnterShipyardPatch
    {
        public static void Postfix(Shipyard __instance)
        {
            var market = GameState.lastVisitedPort.island.GetComponent<IslandMarket>();
            int price = market.GetBuyPrice(23);
            Plugin.UpdateCopperPrice(price);
        }

    }
}*/
