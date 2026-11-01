using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{

    public class SailTextureChanger : MonoBehaviour
    { 
        // vanilla sail textures for reference
        public static readonly string[] names = new string[]
        {
            "ParticleCloudWhite", // blank white
            "dhow small cloth paint", // stripey lateens/jibs
            "medi small cloth paint Diffuse Color", // stripey squares & fan-striped jibs
            "junk small cloth paint Diffuse Color", // kakam junk sails
            "sail junk medium paint Diffuse Color", // large & medium junk sails
            "medi medium sail paint Diffuse Color", // brig sails
            "dhow medium cloth paint" // sanbuq lateens
        };

        public static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        
        /// <summary>
        /// add a texture to the dictionary. Texture must have a unique name
        /// </summary>
        /// <param name="texture"></param>
        /// <returns>Success</returns>
        public static bool AddTexture(Texture texture)
        {
            if (textures.ContainsKey(texture.name))
            {
                Debug.LogError($"SE: Sail texture map already contains a texture named \"{texture.name}\"");
                return false;
            }
            textures.Add(texture.name, texture);
            return true;
        }

        /// <summary>
        /// add a texture name to the allowed lists of the specified sail indexes
        /// </summary>
        /// <param name="textureName"></param>
        /// <param name="sails"></param>
        public static void AllowTexture(string textureName, int[] sails)
        {
            for (int i = 0; i < sails.Length; i++)
            {
                try
                {
                    PrefabsDirectory.instance.sails[sails[i]].GetComponent<SailTextureChanger>().allowedTextures.Add(textureName);
                }
                catch (Exception e){ Debug.Log($"SE: failed to add allowed texture to sail {sails[i]} because: {e.Message}"); }
            }
        }

        /// <summary>
        /// add a texture name to the allowed lists of ALL sails
        /// </summary>
        /// <param name="textureName"></param>
        public static void AllowTextureToAll(string textureName)
        {
            for (int i = 0; i < PrefabsDirectory.instance.sails.Length; i++)
            {
                try
                {
                    PrefabsDirectory.instance.sails[i].GetComponent<SailTextureChanger>().allowedTextures.Add(textureName);
                }
                catch (Exception e) { Debug.Log($"SE: failed to add allowed texture to sail {i} because: {e.Message}"); }
            }
        }

        public bool AllowTexture(Texture texture)
        {
            if (allowedTextures.Contains(texture.name)) return false;
            allowedTextures.Add(texture.name);
            return true;
        }

        // vanilla sails that start white, but will work with other textures
        internal static Dictionary<int, List<string>> allowedTexMap = new Dictionary<int, List<string>>
        {
            { 6, new List<string>{ names[0], names[2] } },
            { 7, new List<string>{ names[0], names[2] } },
            { 8, new List<string>{ names[0], names[2] } },
            { 9, new List<string>{ names[0], names[2] } },
            { 24, new List<string>{ names[0], names[2] } },
            { 25, new List<string>{ names[0], names[2] } },
            { 36, new List<string>{ names[0], names[2] } },
            { 53, new List<string>{ names[0], names[2] } },
            { 95, new List<string>{ names[0], names[2] } },
            { 96, new List<string>{ names[0], names[2] } },
            { 97, new List<string>{ names[0], names[2] } },
            { 99, new List<string>{ names[0], names[2] } },
            { 127, new List<string>{ names[0], names[3] } },
            { 128, new List<string>{ names[0], names[3] } },
            { 129, new List<string>{ names[0], names[3] } },
            { 130, new List<string>{ names[0], names[3] } },
            { 17, new List<string>{ names[0], names[3] } },
            { 18, new List<string>{ names[0], names[3] } },
            { 107, new List<string>{ names[0], names[3] } },
            { 12, new List<string>{ names[0], names[1] } },

        };

        public string textureIndex;
        public SkinnedMeshRenderer cloth;
        public List<string> allowedTextures;
        private int currentAllowed;

        public void Awake()
        {
            cloth = GetComponent<Sail>().cloth.GetComponent<SkinnedMeshRenderer>();
        }

        // mostly used for registering default textures
        public void Setup()
        {
            var sail = GetComponent<Sail>();
            cloth = sail.cloth.GetComponent<SkinnedMeshRenderer>();
            if (cloth != null)
            {
                textures[cloth.sharedMaterial.mainTexture.name] = cloth.sharedMaterial.mainTexture;

                textureIndex = cloth.sharedMaterial.mainTexture.name;
            }

            if (textureIndex == names[0])
            {
                if (allowedTexMap.TryGetValue(sail.prefabIndex, out var output))
                {
                    allowedTextures = output.ToList();
                }
                else
                {
                    allowedTextures = new List<string> { textureIndex };
                }
            }
            else
            {
                allowedTextures = new List<string> { textureIndex, names[0] };
            }

#if DEBUG
            allowedTextures = names.ToList();
#endif
        }
        public void SetTexture(string newTextureName)
        {
            if (!textures.ContainsKey(newTextureName))
            {
                Debug.LogError($"Sail texture dictionary does not contain \"{newTextureName}\"");
                return;
            }
            currentAllowed = Math.Max(allowedTextures.IndexOf(newTextureName), 0);
            textureIndex = newTextureName;
            UpdateMaterial();
        }

        public void SetTexture(int index)
        {
            if (index >= allowedTextures.Count) Debug.LogError("Texture index is out of range");
            currentAllowed = index;
            textureIndex = allowedTextures[currentAllowed];
            UpdateMaterial();

        }
        public string NextTexture()
        {
            currentAllowed++;
            if (currentAllowed >= allowedTextures.Count) currentAllowed = 0;
            textureIndex = allowedTextures[currentAllowed];
            UpdateMaterial();
            return textureIndex;
        }

        public void UpdateMaterial()
        {
            if (cloth != null && textures.TryGetValue(textureIndex, out Texture newTex))
            {
                if (newTex == null)
                {
                    Debug.LogError($"Sail texture \"{textureIndex}\" is null");
                    return;
                }
                cloth.material.mainTexture = newTex;
            }
        }
    }


}
