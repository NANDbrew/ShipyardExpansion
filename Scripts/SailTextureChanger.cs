using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    public class SailTextureChanger : MonoBehaviour
    {
        public static Dictionary<int, int[]> allowedTexMap = new Dictionary<int, int[]>
        {
            { 6, new int[]{ 0, 2 } },
            { 7, new int[]{ 0, 2 } },
            { 8, new int[]{ 0, 2 } },
            { 9, new int[]{ 0, 2 } },
            { 24, new int[]{ 0, 2 } },
            { 25, new int[]{ 0, 2 } },
            { 53, new int[]{ 0, 2 } },
            { 95, new int[]{ 0, 2 } },
            { 96, new int[]{ 0, 2 } },
            { 97, new int[]{ 0, 2 } },
            { 99, new int[]{ 0, 2 } },
            { 127, new int[]{ 0, 3 } },
            { 128, new int[]{ 0, 3 } },
            { 129, new int[]{ 0, 3 } },
            { 130, new int[]{ 0, 3 } },
            { 17, new int[]{ 0, 3 } },
            { 18, new int[]{ 0, 3 } },
            { 107, new int[]{ 0, 3 } },
            { 12, new int[]{ 0, 1 } },

        };

        public static List<Texture> sailTextures = new List<Texture>();
        public int textureIndex;
        public SkinnedMeshRenderer cloth;
        public List<int> allowedTextures;
        private int currentAllowed;

        public void Setup()
        {
            var sail = GetComponent<Sail>();
            cloth = sail.cloth.GetComponent<SkinnedMeshRenderer>();
            if (cloth != null)
            {
                if (!sailTextures.Contains(cloth.sharedMaterial.mainTexture))
                {
                    sailTextures.Add(cloth.sharedMaterial.mainTexture);
                }

                textureIndex = sailTextures.IndexOf(cloth.sharedMaterial.mainTexture);
            }

            if (allowedTexMap.TryGetValue(sail.prefabIndex, out var output))
            {
                allowedTextures = output.ToList();
            }
            else
            {
                allowedTextures = new List<int> { textureIndex, 0 };
            }
        }

        public void SetTexture(int index)
        {
            textureIndex = index;
            currentAllowed = allowedTextures.IndexOf(index);
            UpdateMaterial();
        }

        public int NextTexture()
        {
            currentAllowed++;
            if (currentAllowed >= allowedTextures.Count) currentAllowed = 0;
            textureIndex = allowedTextures[currentAllowed];
            UpdateMaterial();
            return textureIndex;
        }

        public void UpdateMaterial()
        {
            if (cloth != null)
            {
                cloth.material.mainTexture = sailTextures[textureIndex];
            }
        }
    }


}
