using System;
using UnityEngine;

namespace Tests
{
    public static class TestUtilities
    {
        public static bool WithinBounds(float firstArg, float secondArg, float bound = 0.1f)
        {
            return Math.Abs(firstArg - secondArg) < bound;
        }
        
        public static bool WithinBounds(Vector3 firstArg, Vector3 secondArg, float bound = 0.1f)
        {
            var x = Math.Abs(firstArg.x - secondArg.x) < bound;
            var y = Math.Abs(firstArg.y - secondArg.y) < bound;
            var z = Math.Abs(firstArg.z - secondArg.z) < bound;
            return x && y && z;
        }
    }  
}

