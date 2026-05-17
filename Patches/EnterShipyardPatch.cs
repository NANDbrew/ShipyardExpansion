using HarmonyLib;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Shipyard), "AdmitShip")]
    internal static class EnterShipyardPatch
    {
        public static void Postfix()
        {
            var market = GameState.lastVisitedPort.island.GetComponent<IslandMarket>();
            int price = market.GetBuyPrice(23); // good index 23 = copper crate
            Plugin.UpdateCopperPrice(price);

        }
    }
}
