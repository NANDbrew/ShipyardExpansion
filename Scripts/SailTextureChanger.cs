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
        public static string[] names = new string[]
        {
            "ParticleCloudWhite",
            "dhow small cloth paint",
            "medi small cloth paint Diffuse Color",
            "junk small cloth paint Diffuse Color",
            "sail junk medium paint Diffuse Color",
            "medi medium sail paint Diffuse Color",
            "dhow medium cloth paint"
        };
        public static Dictionary<string, Texture> textures = new Dictionary<string, Texture>
        {
/*            { names[0], null },
            { names[1], null },
            { names[2], null },
            { names[3], null },
            { names[4], null },
            { names[5], null },*/
        };

        public static Dictionary<int, List<string>> allowedTexMap = new Dictionary<int, List<string>>
        {
/*            { 6, new List<string>{ names[0], names[2] } },
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
            { 107, new List<string>{ names[0], names[3] } },*/
            //{ 12, new List<string>{ names[0], names[1] } },

        };

        //public static List<Texture> sailTextures = new List<Texture>();
        public string textureIndex;
        public SkinnedMeshRenderer cloth;
        public List<string> allowedTextures;
        private int currentAllowed;

        public void Setup()
        {
            var sail = GetComponent<Sail>();
            cloth = sail.cloth.GetComponent<SkinnedMeshRenderer>();
            if (cloth != null)
            {
                textures[cloth.sharedMaterial.mainTexture.name] = cloth.sharedMaterial.mainTexture;

                textureIndex = cloth.sharedMaterial.mainTexture.name;
            }

            if (allowedTexMap.TryGetValue(sail.prefabIndex, out var output))
            {
                allowedTextures = output.ToList();
            }
            else
            {
                allowedTextures = new List<string> { textureIndex, names[0] };
            }

#if DEBUG
            allowedTextures = names.ToList();
#endif
        }

        public void SetTexture(string index)
        {
            if (!textures.ContainsKey(index))
            {
                Debug.LogError($"Sail texture dictionary does not contain \"{index}\"");
                return;
            }
            currentAllowed = Math.Max(allowedTextures.IndexOf(index), 0);
            textureIndex = index;
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
