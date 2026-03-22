using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardExpansion
{
    internal class SailClothRotations
    {
        public static readonly Dictionary<int, Vector3> clothRotations = new Dictionary<int, Vector3>
        {
            //{ 17, new float[2]{ 0.5f, 3.5f } },

            { 0, new Vector3(90, 0, 0) },
            { 81, new Vector3(90, 0, 0) },
            { 82, new Vector3(90, 0, 0) },

            /*{ 1, new Vector3(90, 0, 0) },
            { 3, new Vector3(90, 0, 0) },
            { 28, new Vector3(90, 0, 0) },
            { 46, new Vector3(90, 0, 0) },
            { 47, new Vector3(90, 0, 0) },*/
        };
        public static readonly Dictionary<int, int[]> colDirs = new Dictionary<int, int[]>
        {
            //{ 17, new float[2]{ 0.5f, 3.5f } },

            { 0, new int[3] { 0, 2, 1 } },
            { 81, new int[3] { 0, 2, 1 } },
            { 82, new int[3] { 0, 2, 1 } },

            /*{ 1, new Vector3(90, 0, 0) },
            { 3, new Vector3(90, 0, 0) },
            { 28, new Vector3(90, 0, 0) },
            { 46, new Vector3(90, 0, 0) },
            { 47, new Vector3(90, 0, 0) },*/
        };
    }
}
