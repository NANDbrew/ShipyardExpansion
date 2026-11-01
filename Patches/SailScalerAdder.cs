using HarmonyLib;
using OVRSimpleJSON;
using ShipyardExpansion.Scripts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
//using static System.Net.WebRequestMethods;

namespace ShipyardExpansion.Scripts
{
    [HarmonyPatch(typeof(PrefabsDirectory), "Start")]
    internal static class CompAdder2
    {
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(GameObject[] ___sails)
        {
            var textures = AssetTools.LoadSailTextures();
            for (int i = 0; i < ___sails.Length; i++)
            {
                if (___sails[i] != null)
                {
                    if (___sails[i].GetComponent<SailScaler>() == null)
                    {
                        var sc = ___sails[i].gameObject.AddComponent<SailScaler>();
                        sc.Setup();
                    }
                    if (___sails[i].GetComponent<SailTextureChanger>() == null)
                    {
                        var tc = ___sails[i].gameObject.AddComponent<SailTextureChanger>();
                        tc.Setup();

                        if (textures.ContainsKey(i))
                        {
                            tc.allowedTextures.AddRange(textures[i]);
                        }
                    }
                }
            }
        }


    }
}
