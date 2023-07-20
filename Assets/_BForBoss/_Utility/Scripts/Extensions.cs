using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Utility
{
    public static class Extensions
    {
        public static void SetAlpha(this SpriteRenderer r, float alpha)
        {
            var color = r.color;
            color.a = alpha;
            r.color = color;
        }
        
        public static float ClampToRange(this Vector2 range, float value)
        {
            return Mathf.Clamp(value, range.x, range.y);
        }
    }
}
