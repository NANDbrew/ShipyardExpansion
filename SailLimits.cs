using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipyardExpansion
{
    public static class SailLimits
    {
        public static readonly int[] stretchableSquares = { 4, 32, 48, 49, 50, 51, 52, 59, 80, 113, 114, 115, 116, };
        public static readonly int[] stretchableJibs = { 110, 111, };
        public static readonly int[] flippableSquares = { };//{ 27, 28, 29, 100, 101, 102 };

        public static readonly Dictionary<int, float[]> angleLimits = new Dictionary<int, float[]> 
        {
            { 2, new float[2] { 340f, 20f } },
            { 10, new float[2] { 340f, 20f } },
            { 11, new float[2] { 340f, 20f } },
            { 12, new float[2] { 340f, 15f } },
            { 33, new float[2] { 335f, 10f } },
            { 43, new float[2] { 335f, 10f } },
            { 44, new float[2] { 335f, 10f } },
            { 45, new float[2] { 335f, 10f } },
            { 60, new float[2] { 335f, 15f } },
            { 61, new float[2] { 335f, 15f } },
            { 121, new float[2] { 335f, 10f } },
            { 127, new float[2] { 340f, 15f } },
            { 128, new float[2] { 340f, 15f } },
            { 129, new float[2] { 340f, 15f } },
            { 130, new float[2] { 340f, 15f } },
            { 156, new float[2] { 340f, 10f } },
            { 157, new float[2] { 340f, 8f } },
            { 158, new float[2] { 340f, 10f } },

        };

        public static readonly Dictionary<int, float[]> sizeLimits = new Dictionary<int, float[]>
        {
            { -1, new float[2]{ 0.5f, 3.5f } },

            { 50, new float[2]{ 0.4f, 2.5f } },
            { 51, new float[2]{ 0.25f, 2.5f } },
            { 82, new float[2]{ 0.85f, 3.5f } },

            { 70, new float[2]{ 0.5f, 3.5f } },
            { 71, new float[2]{ 0.5f, 3.5f } },
            { 72, new float[2]{ 0.5f, 3.5f } },
            { 95, new float[2]{ 1f, 3.5f } },

            { 113, new float[2]{ 0.35f, 2.0f } },
            { 114, new float[2]{ 0.5f, 2.0f } },
            { 115, new float[2]{ 0.5f, 2.0f } },
            { 116, new float[2]{ 0.5f, 2.0f } },
        };

        public static readonly Dictionary<int, float[]> ratioLimits = new Dictionary<int, float[]>
        {
            //{ 17, new float[2]{ 0.5f, 3.5f } },

            { 110, new float[2]{ 0.6f, 2f } },
            { 111, new float[2]{ 0.6f, 2f } },
        };
    }
}
