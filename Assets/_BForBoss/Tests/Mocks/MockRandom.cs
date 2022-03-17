using System.Collections.Generic;
using PerigonGames;

namespace Tests
{
    public class MockRandom : IRandomUtility
    {
        public double RandomDouble = 0;
        
        public int NextInt()
        {
            return 0;
        }

        public int NextInt(int maxValue)
        {
            return 0;
        }

        public int NextInt(int minValue, int maxValue)
        {
            return 0;
        }

        public double NextDouble()
        {
            return RandomDouble;
        }

        public bool CoinFlip()
        {
            return true;
        }

        public bool CoinFlip(float probability)
        {
            return true;
        }

        public bool NextTryGetElement<T>(IList<T> list, out T element)
        {
            element = list[0];
            return false;
        }
    }
}
