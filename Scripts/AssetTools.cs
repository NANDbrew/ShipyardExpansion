using BepInEx;
using SE_Bridge;
using ShipyardExpansion.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace ShipyardExpansion
{
    public static class AssetTools
    {
        public static AssetBundle bundle;
        static string basePath = "";
        const string assetDir = "ShipyardExpansion";
        const string assetFile = "shipyard_expansion.assets";
        const string libFile = "SE_Bridge.dll";

        public static void LoadAssetBundles()    //Load the bundle
        {
            basePath = Directory.GetParent(Plugin.instance.Info.Location).FullName;
            Debug.Log("attempting to load other assembly");
            string libFirstTry = Path.Combine(basePath, assetDir, libFile);
            string libSecondTry = Path.Combine(basePath, libFile);
            Assembly.LoadFrom(File.Exists(libFirstTry) ? libFirstTry : libSecondTry);

            string firstTry = Path.Combine(Directory.GetParent(basePath).FullName, assetDir, assetFile);
            string secondTry = Path.Combine(basePath, assetFile);

            bundle = AssetBundle.LoadFromFile(File.Exists(firstTry) ? firstTry : secondTry);
            if (bundle == null)
            {
                Debug.LogError("Bundle not loaded! Did you place it in the correct folder?");
            }
            else { Debug.Log("ShipyardExpansion: loaded bundle " + bundle.ToString()); }

        }

        public static GPButtonRopeWinch[] HandleWinches(GameObject[] data)
        {
            if (data == null)
            {
                Debug.LogError("SE builder: winch list is null!");
                return new GPButtonRopeWinch[0];
            }
            GPButtonRopeWinch[] winches = new GPButtonRopeWinch[data.Length];
            for (int i = 0; i < winches.Length; i++)
            {
                if (data[i].GetComponent<GPButtonRopeWinch>() is GPButtonRopeWinch component)
                {
                    winches[i] = component;
                }
                else
                {
                    winches[i] = data[i].AddComponent<GPButtonRopeWinch>();
                }
            }
            return winches;

        }
        public static BoatPartOption HandlePartOption(SE_PartOptionData data, Transform walkColParent, BoatCustomParts parts)
        {
            BoatPartOption opt = data.GetComponent<BoatPartOption>();

            if (data.requiresVanilla1.Length == 2) opt.requires.Add(parts.availableParts[data.requiresVanilla1[0]].partOptions[data.requiresVanilla1[1]]);
            if (data.requiresVanilla2.Length == 2) opt.requires.Add(parts.availableParts[data.requiresVanilla2[0]].partOptions[data.requiresVanilla2[1]]);

            return opt;
        }

        public static List<BoatPartOption> HandlePartOptions(List<GameObject> data, Transform walkColParent, BoatCustomParts parts)
        {
            List<BoatPartOption> opts = new List<BoatPartOption>();
            foreach (var datum in data)
            {
                opts.Add(HandlePartOption(datum.GetComponent<SE_PartOptionData>(), walkColParent, parts));
            }
            return opts;
        }
        public static List<BoatPartOption> HandlePartOptions(SE_PartOptionData[] data, Transform walkColParent, BoatCustomParts parts)
        {
            List<BoatPartOption> opts = new List<BoatPartOption>();
            foreach (var datum in data)
            {
                opts.Add(HandlePartOption(datum, walkColParent, parts));
            }
            return opts;
        }

        private static void ReqTranslate(SE_PartOptionData optData, BoatPartOption opt, BoatCustomParts partsList)
        {
            Debug.Log("Translating partOption requirements for " + opt.name);
            if (optData.requiresVanilla1.Length == 2)
            {
                opt.requires.Add(partsList.availableParts[optData.requiresVanilla1[0]].partOptions[optData.requiresVanilla1[1]]);
                Debug.Log("did 1");
            }
            if (optData.requiresVanilla2.Length == 2)
            {
                opt.requires.Add(partsList.availableParts[optData.requiresVanilla2[0]].partOptions[optData.requiresVanilla2[1]]);
                Debug.Log("did 2");
            }
            if (optData.requiresDisabledVanilla1.Length == 2)
            {
                opt.requiresDisabled.Add(partsList.availableParts[optData.requiresDisabledVanilla1[0]].partOptions[optData.requiresDisabledVanilla1[1]]);
                Debug.Log("did anti 1");

            }
            if (optData.requiresDisabledVanilla2.Length == 2)
            {
                opt.requiresDisabled.Add(partsList.availableParts[optData.requiresDisabledVanilla2[0]].partOptions[optData.requiresDisabledVanilla2[1]]);
                Debug.Log("did anti 2");

            }
            //optData.enabled = false;
        }
        public static Dictionary<string, BoatPart> HandleImports(GameObject thing, BoatCustomParts partsList)
        {
            Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();

            SE_BoatCustomData boatData = thing.GetComponent<SE_BoatCustomData>();

            foreach (SE_PartData partData in boatData.parts)
            {
                BoatPart part = new BoatPart
                {
                    //activeOption = data.activeOption,
                    category = partData.category,
                    partOptions = partData.partOptions,
                };
                partsList.availableParts.Add(part);
                modParts.Add(partData.name, part);
                foreach (BoatPartOption opt in part.partOptions)
                {
                    if (opt.GetComponent<SE_PartOptionData>() is SE_PartOptionData optData)
                    {
                        ReqTranslate(optData, opt, partsList);
                        //optData.enabled = false;
                    }
                }
                partData.enabled = false;
                Debug.Log(partData.name);
                Debug.Log(partsList.availableParts.Count);
            }

            foreach (SE_PartOptionData optData in boatData.options)
            {
                Debug.Log(optData.name);
                if (optData.GetComponent<BoatPartOption>() is BoatPartOption opt && optData.parentPartIndex > -1)
                {
                    ReqTranslate(optData, opt, partsList);
                    partsList.availableParts[optData.parentPartIndex].partOptions.Add(opt);
                    //optData.enabled = false;
#if DEBUG
                    if (opt.walkColObject.layer != 8) Debug.Log("part " + opt.gameObject.name + " walk col is not layer 8");
#endif
                }
                else Debug.LogError("huh? " + optData.name);

            }
            Debug.Log("working on ladders and flags");
            foreach (Transform obj in thing.GetComponentsInChildren<Transform>())
            {
                if (obj.GetComponent<SE_LadderData>() is SE_LadderData ladderData)
                {
                    Debug.Log("adding ladder: " + ladderData.name);
                    var ladder = ladderData.gameObject.AddComponent<NANDLadder>();
                    ladder.targets = ladderData.targets;
                    for (int i = 0; i < ladder.targets.Length; i++)
                    {
                        //Transform target = ;
                        ladder.targets[i].SetParent(thing.transform.parent);
                        //ladder.targets[i] = target;

                    }
                    ladderData.enabled = false;
                }


                if (obj.GetComponent<WindClothSimple>() is WindClothSimple cloth)
                {
                    cloth.shipRigidbody = partsList.GetComponent<Rigidbody>();
                }
            }
            if (boatData.walkColMesh != null)
            {
                var col = partsList.gameObject.GetComponentInChildren<BoatEmbarkCollider>();
                Debug.Log("SE found embarkCol: " + col);
                col.GetComponent<MeshCollider>().sharedMesh = boatData.walkColMesh;
                col.GetComponent<MeshFilter>().sharedMesh = boatData.walkColMesh;
                //partsList.StartCoroutine(ReplaceEmbarkMesh(partsList.gameObject.GetComponentInChildren<BoatEmbarkCollider>(), boatData.walkColMesh));
            }
            Debug.Log("modParts.Count = " + modParts.Count);
            return modParts;
        }
        public static IEnumerator ReplaceEmbarkMesh(BoatEmbarkCollider col, Mesh mesh)
        {
            yield return new WaitUntil(() => GameState.playing && !GameState.justStarted);
            Debug.Log("SE found embarkCol: " + col);
            col.GetComponent<MeshCollider>().sharedMesh = mesh;
            col.GetComponent<MeshFilter>().sharedMesh = mesh;
            
#if DEBUG
            Debug.Log("Replaced embarkCol mesh: " + col.GetComponentInParent<BoatDamage>().name);
#endif
        }
    }
}

