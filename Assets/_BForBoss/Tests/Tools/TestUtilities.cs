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
            return Vector3.Distance(firstArg, secondArg) < bound;
        }
    }  
}

