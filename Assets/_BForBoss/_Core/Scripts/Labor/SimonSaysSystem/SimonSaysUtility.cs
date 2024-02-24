using System;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public static class SimonSaysUtility
    {
        public static Color DefaultNoneColor = Color.grey; 
        private static int ColorLength = Enum.GetValues(typeof(SimonSaysColor)).Length;
        
        public static SimonSaysColor GetRandomColor()
        {
            var randomizer = new RandomUtility();
            return (SimonSaysColor) randomizer.NextInt(0, ColorLength - 1);
        }
    }
}
