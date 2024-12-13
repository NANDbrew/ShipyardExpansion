using BepInEx;
using SE_Bridge;
using ShipyardExpansion.Scripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace ShipyardExpansion
{
    public static class AssetTools
    {
        public static AssetBundle bundle;
        const string assetDir = "ShipyardExpansion";
        const string assetFile = "shipyard_expansion.assets";
        const string libFile = "SE_Bridge.dll";

        public static void LoadAssetBundles()    //Load the bundle
        {
            Debug.Log("attempting to load other assembly");
            string libFirstTry = Path.Combine(Paths.PluginPath, assetDir, libFile);
            string libSecondTry = Path.Combine(Paths.PluginPath, libFile);
            Assembly.LoadFrom(File.Exists(libFirstTry) ? libFirstTry : libSecondTry);

            string firstTry = Path.Combine(Paths.PluginPath, assetDir, assetFile);
            string secondTry = Path.Combine(Paths.PluginPath, assetFile);

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
            /*            if (opt == null)
                        {
                            opt = data.gameObject.AddComponent<BoatPartOption>();
                            opt.optionName = data.optionName;
                            opt.basePrice = data.basePrice;
                            opt.installCost = data.installCost;
                            opt.mass = data.mass;
                            opt.requires = HandlePartOptions(data.requires, walkColParent, parts);
                            opt.requiresDisabled = HandlePartOptions(data.requiresDisabled, walkColParent, parts);
                            opt.walkColObject = walkColParent.transform.Find(data.name).gameObject;
                            opt.childOptions = data.childOptions;
                        }*/
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

        public static Dictionary<string, BoatPart> HandleImportsOld(GameObject thing, BoatCustomParts partsList)
        {
            Dictionary<string, BoatPart> modParts = new Dictionary<string, BoatPart>();

            foreach (Transform obj in thing.GetComponentsInChildren<Transform>(true))
            {
                if (obj.GetComponent<Mast>())
                {
                    obj.gameObject.SetActive(true);
                }

                if (obj.GetComponent<SE_PartOptionData>() is SE_PartOptionData optData && obj.GetComponent<BoatPartOption>() is BoatPartOption opt)
                {
                    if (optData.requiresVanilla2.Length == 2) opt.requires.Add(partsList.availableParts[optData.requiresVanilla2[0]].partOptions[optData.requiresVanilla2[1]]);
                    if (optData.requiresVanilla1.Length == 2) opt.requires.Add(partsList.availableParts[optData.requiresVanilla1[0]].partOptions[optData.requiresVanilla1[1]]);
                    if (optData.parentPartIndex > -1)
                    {
                        partsList.availableParts[optData.parentPartIndex].partOptions.Add(opt);
                    }
                    optData.enabled = false;
                }

                if (obj.GetComponent<SE_LadderData>() is SE_LadderData ladderData)
                {
                    Transform target = ladderData.target;
                    target.SetParent(thing.transform.parent);
                    var ladder = ladderData.gameObject.AddComponent<NANDLadder>();
                    ladder.target = target;
                    ladderData.enabled = false;
                }

                if (obj.GetComponent<SE_PartData>() is SE_PartData partData)
                {
                    BoatPart part = new BoatPart
                    {
                        //activeOption = data.activeOption,
                        category = partData.category,
                        partOptions = partData.partOptions,
                    };
                    partsList.availableParts.Add(part);
                    partData.enabled = false;
                    modParts.Add(obj.name, part);
                }

                if (obj.GetComponent<BoatEmbarkCollider>())
                {
                    obj.transform.SetParent(thing.transform.parent);
                    obj.gameObject.SetActive(true);
                }

                if (obj.GetComponent<WindClothSimple>() is WindClothSimple cloth)
                {
                    cloth.shipRigidbody = partsList.GetComponent<Rigidbody>();
                }

            }
            Debug.Log("modParts.Count = " + modParts.Count);
            return modParts;
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
                        optData.enabled = false;
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
                    optData.enabled = false;
                }
                else Debug.LogError("huh? " + optData.name);

            }
            foreach (Transform obj in thing.GetComponentsInChildren<Transform>())
            {
              /*  if (obj.GetComponent<Mast>() is Mast mast)
                {
                    //obj.gameObject.SetActive(true);
                    mast.Awake();
                }*/

                if (obj.GetComponent<SE_LadderData>() is SE_LadderData ladderData)
                {
                    Transform target = ladderData.target;
                    target.SetParent(thing.transform.parent);
                    var ladder = ladderData.gameObject.AddComponent<NANDLadder>();
                    ladder.target = target;
                    ladderData.enabled = false;
                }


                if (obj.GetComponent<WindClothSimple>() is WindClothSimple cloth)
                {
                    cloth.shipRigidbody = partsList.GetComponent<Rigidbody>();
                }
            }


            Debug.Log("modParts.Count = " + modParts.Count);
            return modParts;
        }
    }

        /*public static Mast HandleMast(SE_MastData data, Transform walkColParent, Rigidbody shipRigidBody)
        {
            data.gameObject.SetActive(false);
            Mast mast = data.gameObject.AddComponent<Mast>();

            mast.maxSails = data.maxSails;
            mast.mastHeight = data.mastHeight;
            mast.extraBottomHeight = data.extraBottomHeight;
            mast.orderIndex = data.orderIndex;
            mast.onlyStaysails = data.onlyStaysails;
            mast.onlySquareSails = data.onlySquareSails;
            mast.startingSailColor = data.startingSailColor;

            mast.leftAngleWinch = HandleWinches(data.leftAngleWinch);
            mast.rightAngleWinch = HandleWinches(data.rightAngleWinch);
            if (data.midAngleWinch != null) mast.midAngleWinch = HandleWinches(data.midAngleWinch);
            mast.reefWinch = HandleWinches(data.reefWinch);

            mast.midRopeAtt = data.midRopeAtt;
            mast.mastReefAtt = data.mastReefAtt;
            mast.mastReefAttExtension = data.mastReefAttExtension;
            mast.mastCols = data.mastCols;

            mast.startSailPrefab = data.startSailPrefab;
            mast.startSailPrefabs = data.startSailPrefabs;
            mast.startSailHeightOffset = data.startSailHeightOffset;
            mast.startSailsHeightOffsets = data.startSailsHeightOffsets;
            mast.walkColMast = walkColParent.transform.Find(mast.name).GetComponentInChildren<Collider>().transform;
            mast.sails = new List<GameObject>();
            mast.shipRigidbody = shipRigidBody;

            data.gameObject.SetActive(true);

            return mast;
        }*/
    }

