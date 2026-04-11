using HarmonyLib;
using SE_Bridge;
using ShipyardExpansion.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ShipyardExpansion
{
    public static class AssetTools
    {
        public static AssetBundle bundle;
        public static AssetBundle bundle2;
        static string basePath = "";
        const string assetDir = "ShipyardExpansion";
        const string assetFile = "shipyard_expansion.assets";
        const string assetFile2 = "shipyard_expansion_ui.assets";
        const string libFile = "SE_Bridge.dll";
        //static object[] mats;
        public static void LoadAssetBundles()    //Load the bundle
        {
            basePath = Directory.GetParent(Plugin.instance.Info.Location).FullName;
            string libFirstTry = Path.Combine(basePath, assetDir, libFile);
            string libSecondTry = Path.Combine(basePath, libFile);
            try
            {
                Assembly.LoadFrom(File.Exists(libFirstTry) ? libFirstTry : libSecondTry);
                Debug.Log("ShipyardExpansion: SE bridge loaded successfully");
            } 
            catch { Debug.LogError("SE: failed to load other assembly!"); }

            string firstTry = Path.Combine(Directory.GetParent(basePath).FullName, assetDir, assetFile);
            string secondTry = Path.Combine(basePath, assetFile);
            try
            {
                bundle = AssetBundle.LoadFromFile(File.Exists(firstTry) ? firstTry : secondTry);
                Debug.Log("ShipyardExpansion: loaded bundle " + bundle.ToString());
            }
            catch { Debug.LogError("Bundle 1 not loaded! Did you place it in the correct folder?"); }

            string firstTry2 = Path.Combine(Directory.GetParent(basePath).FullName, assetDir, assetFile2);
            string secondTry2 = Path.Combine(basePath, assetFile2);
            bundle2 = AssetBundle.LoadFromFile(File.Exists(firstTry2) ? firstTry2 : secondTry2);
            if (bundle2 == null)
            {
                Debug.LogError("Bundle 2 not loaded! Did you place it in the correct folder?");
            }
            else { Debug.Log("ShipyardExpansion: loaded bundle " + bundle2.ToString()); }

            // stupid hack to fix fogless shader
            var mats = bundle.LoadAllAssets(typeof(Material));
            foreach (Material m in mats)
            {

                var shaderName = m.shader.name;
                //Debug.LogWarning("trying to refresh shader: " + shaderName + " in material " + m.name);
                var newShader = Shader.Find(shaderName);
                if (newShader != null)
                {
                    m.shader = newShader;
                    //Debug.LogWarning("refreshed shader: " + shaderName + " in material " + m.name);

                }
                else
                {
                    Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + m.name);
                }
            }

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
#if DEBUG
            Debug.Log("Translating partOption requirements for " + opt.name);
#endif
            if (optData.requiresVanilla1.Length == 2)
            {
                opt.requires.Add(partsList.availableParts[optData.requiresVanilla1[0]].partOptions[optData.requiresVanilla1[1]]);
#if DEBUG
                Debug.Log("did 1");
#endif
            }
            if (optData.requiresVanilla2.Length == 2)
            {
                opt.requires.Add(partsList.availableParts[optData.requiresVanilla2[0]].partOptions[optData.requiresVanilla2[1]]);
#if DEBUG
                Debug.Log("did 2");
#endif
            }
            if (optData.requiresDisabledVanilla1.Length == 2)
            {
                opt.requiresDisabled.Add(partsList.availableParts[optData.requiresDisabledVanilla1[0]].partOptions[optData.requiresDisabledVanilla1[1]]);
#if DEBUG
               Debug.Log("did anti 1");
#endif

            }
            if (optData.requiresDisabledVanilla2.Length == 2)
            {
                opt.requiresDisabled.Add(partsList.availableParts[optData.requiresDisabledVanilla2[0]].partOptions[optData.requiresDisabledVanilla2[1]]);
#if DEBUG
               Debug.Log("did anti 2");
#endif

            }
            //optData.enabled = false;

        }

        private static void HandleMastCols(SE_PartOptionData optData, BoatPartOption opt, BoatCustomParts partsList)
        {
            var masts = new List<Mast>();
            foreach (var req in opt.requires)
            {
                if (req.GetComponent<Mast>() is Mast mast)
                {
                    masts.Add(mast);
                }
            }
            if (optData.mastVanilla1.Length == 2 && partsList.availableParts[optData.mastVanilla1[0]].partOptions[optData.mastVanilla1[1]].GetComponent<Mast>() is Mast m1)
            {
                masts.Add(m1);
            }
            if (optData.mastVanilla2.Length == 2 && partsList.availableParts[optData.mastVanilla2[0]].partOptions[optData.mastVanilla2[1]].GetComponent<Mast>() is Mast m2)
            {
                masts.Add(m2);
            }
            foreach (var mast in masts)
            {
                mast.mastCols = mast.mastCols.AddRangeToArray(optData.colliders.ToArray());
            }
        }

        public static Dictionary<string, BoatPart> HandleImports(GameObject thing, BoatCustomParts partsList)
        {
            Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();

            SE_BoatCustomData boatData = thing.GetComponent<SE_BoatCustomData>();

            BoatRefs boatRefs = partsList.GetComponent<BoatRefs>();

            BoatEmbarkCollider embarkCol = partsList.GetComponentInChildren<BoatEmbarkCollider>();

            BoatDamage damage = boatRefs.GetComponent<BoatDamage>();

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
                    }

                }
                partData.enabled = false;
#if DEBUG
                Debug.Log(partData.name);
                Debug.Log(partsList.availableParts.Count);
#endif
            }

            foreach (SE_PartOptionData optData in boatData.options)
            {
#if DEBUG
                Debug.Log(optData.name);
#endif
                if (optData.GetComponent<BoatPartOption>() is BoatPartOption opt)
                {
                    if (optData.parentPartIndex > -1)
                    {
                        ReqTranslate(optData, opt, partsList);
                        partsList.availableParts[optData.parentPartIndex].partOptions.Add(opt);
                    }
                    HandleMastCols(optData, opt, partsList);

#if DEBUG
                    if (opt.walkColObject.layer != 8) Debug.Log("part " + opt.gameObject.name + " walk col is not layer 8");
#endif
                }
                else Debug.LogError("huh? " + optData.name);

            }
            foreach (var ladderData in boatData.ladders)
            {
#if DEBUG
                Debug.Log("adding ladder: " + ladderData.name);
#endif
                var ladder = ladderData.gameObject.AddComponent<NANDLadder>();
                ladder.walkCol = boatRefs.walkCol;
                ladder.targets = ladderData.targets;
                foreach (var target in ladder.targets)
                {
                    target.SetParent(boatRefs.boatModel);
                    //target.GetComponent<Renderer>().enabled = false;
                    target.gameObject.layer = 8;
                }
                ladderData.enabled = false;
            }
            if (boatData.embarkColMesh != null)
            {
                //var col = partsList.gameObject.GetComponentInChildren<BoatEmbarkCollider>();
#if DEBUG
                Debug.Log("SE found embarkCol: " + embarkCol);
#endif
                embarkCol.GetComponent<MeshCollider>().sharedMesh = boatData.embarkColMesh;
                embarkCol.GetComponent<MeshFilter>().sharedMesh = boatData.embarkColMesh;
                //partsList.StartCoroutine(ReplaceEmbarkMesh(partsList.gameObject.GetComponentInChildren<BoatEmbarkCollider>(), boatData.embarkColMesh));
            }
#if DEBUG
            Debug.Log("modParts.Count = " + modParts.Count);
#endif
            foreach (var table in boatData.tabletops)
            {
                var comp = table.AddComponent<StaticTable>();
                comp.allowPlacingItems = true;
            }

            foreach (var pump in boatData.pumps)
            {
                pump.damage = damage;
            }
            return modParts;
        }
        public static IEnumerator ReplaceEmbarkMesh(BoatEmbarkCollider col, Mesh mesh)
        {
            yield return new WaitUntil(() => GameState.playing && !GameState.justStarted);
#if DEBUG
            Debug.Log("SE found embarkCol: " + col);
#endif
            col.GetComponent<MeshCollider>().sharedMesh = mesh;
            col.GetComponent<MeshFilter>().sharedMesh = mesh;
            
#if DEBUG
            Debug.Log("Replaced embarkCol mesh: " + col.GetComponentInParent<BoatDamage>().name);
#endif
        }

        public static void PreparePrefab(GameObject prefab, BoatRefs boatRefs)
        {
            SE_BoatCustomData boatData = prefab.GetComponent<SE_BoatCustomData>();
#if DEBUG
            Debug.Log("working on ladders and flags");
#endif
            Rigidbody boatRigidbody = boatRefs.gameObject.GetComponent<Rigidbody>();
            foreach (var mast in boatData.masts)
            {
                mast.shipRigidbody = boatRigidbody;
            }

            foreach (var cloth in boatData.flags)
            {
                cloth.shipRigidbody = boatRigidbody;
            }

            foreach (var swapper in boatData.meshSwappers)
            {
                swapper.targetParent = swapper.targetWalkCol ? boatRefs.walkCol : boatRefs.boatModel;
            }
/*            foreach (var trapdoor in boatData.doors)
            {
                trapdoor.importedActualBoat = boatRefs.boatModel;
                trapdoor.embarkCol = embarkCol;
            }*/
            
        }

    }
}

