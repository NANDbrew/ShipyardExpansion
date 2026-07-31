/*using HarmonyLib;
using SE_Bridge;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Shipyard), "AdmitShip")]
    internal static class EnterShipyardPatch
    {
        public static void Postfix(Shipyard __instance)
        {
            var market = GameState.lastVisitedPort.island.GetComponent<IslandMarket>();
            float dist = 99999;
            foreach (var port in Port.ports)
            {
                if (port == null) continue;
                var d = Vector3.Distance(__instance.transform.position, port.transform.position);
                if (d < dist)
                {
                    market = port.island.GetComponent<IslandMarket>();
                    dist = d;
                }
            }
            int price = market.GetSellPrice(23); // good index 23 = copper crate
            foreach (var clad in __instance.GetCurrentBoat().GetComponentsInChildren<SE_Cladding>())
            { 
                clad.SetPriceModifier(price);
            }
        }
    }
}
*/