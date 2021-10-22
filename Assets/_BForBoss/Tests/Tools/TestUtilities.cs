using System;

namespace Tests
{
    public static class TestUtilities
    {
        public static bool WithinBounds(float firstArg, float secondArg, float bound = 0.1f)
        {
            return Math.Abs(firstArg - secondArg) < bound;
        }
    }  
}

