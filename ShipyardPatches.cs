using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class ShipyardPatches
    {
        [HarmonyPatch(typeof(Shipyard), "AdmitShip")]
        private static class UnrollSails
        {
            public static void Postfix(GameObject ship)
            {
                if (!Plugin.unrollSails.Value) return;
                foreach (var mast in ship.GetComponent<BoatRefs>().masts)
                {
                    if (mast == null) continue;
                    foreach (var sail in mast.sails)
                    {
                        sail.GetComponent<Sail>().currentUnroll = 1;
                        sail.GetComponent<Sail>().enabled = false;
                        sail.transform.localEulerAngles = Vector3.zero;
                    }
                }
            }
        }
    }
}
