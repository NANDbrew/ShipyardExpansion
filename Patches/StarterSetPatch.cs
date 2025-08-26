using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(StarterSet), "InitiateStarterSet")]
    internal static class CogStarterSetPatch
    {
        /*public static void Prefix(StarterSet __instance)
        {
            if (__instance.region == PortRegion.medi)
            {
                __instance.transform.Find("10 barrel water").localPosition = new UnityEngine.Vector3(3.2f, 1.344f, 0.848f);
            }
        }*/
        [HarmonyPostfix]
        public static IEnumerator InitiateStarterSet(IEnumerator original, StarterSet __instance, Transform ___starterBoat)
        {
            if (!Plugin.starterSetFix.Value)
            {
                yield return original;
            }
            
            GameState.lastBoat = ___starterBoat;
            GameState.lastOwnedBoat = ___starterBoat;
            List<Transform> items = new List<Transform>();
            foreach (Transform item in __instance.transform)
            {
                if (item == null) continue;
                item.gameObject.SetActive(value: true);
                items.Add(item);
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            foreach (Transform item2 in items)
            {
                if (item2 == null) continue;
                item2.transform.Translate(Vector3.up * -15f, Space.World);
                item2.GetComponent<ShipItem>().GetItemRigidbody().transform.Translate(Vector3.up * -15f, Space.World);
                //item2.GetComponent<ShipItem>().itemRigidbodyC.transform.Translate(Vector3.up * -15f, Space.World);
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            foreach (Transform item3 in items)
            {
                if (item3 == null) continue;
                item3.GetComponent<ShipItem>().sold = true;
                item3.GetComponent<SaveablePrefab>().RegisterToSave();
            }
        }
    }
}
