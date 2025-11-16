using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(Sail), "GetSailArea")]
    internal static class SailAreaPatch
    {
        static bool Prefix(Sail __instance, ref float __result)
        {
            var smr = __instance.cloth.GetComponent<SkinnedMeshRenderer>();

            int[] triangles = smr.sharedMesh.triangles;
            Vector3[] vertices = smr.sharedMesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = smr.transform.TransformVector(vertices[i]);
            }

            float num = 0.0f;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 vector = vertices[triangles[i]];
                Vector3 lhs = vertices[triangles[i + 1]] - vector;
                Vector3 rhs = vertices[triangles[i + 2]] - vector;
                num += Vector3.Cross(lhs, rhs).magnitude;
            }

            __result = num / 2.0f;

            //Debug.Log(__instance.name + " area = " + __result);
            return false;
        }

    }
}
