using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Utility
{
    public static class Extensions
    {
        public static T GetRandomElement<T>(this IList<T> array)
        {
            return array[Random.Range(0, array.Count)];
        }

        public static void SetAlpha(this SpriteRenderer r, float alpha)
        {
            var color = r.color;
            color.a = alpha;
            r.color = color;
        }
    }
}
