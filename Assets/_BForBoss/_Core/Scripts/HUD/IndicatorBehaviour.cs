using System;
using UnityEngine;
using UnityEngine.Pool;

namespace BForBoss
{
    public class IndicatorBehaviour : MonoBehaviour
    {
        private const float MinSize = 0.5f;
        private const float MaxSize = 3f;
        private const float DistanceToStartScaling = 30;
        
        private IObjectPool<IndicatorBehaviour> _objectPool;

        public void SetSizeBasedOnDistance(float distance)
        {
            var mappedValue = MapDistanceToScale(distance);
            var scale = Math.Clamp(mappedValue, MinSize, MaxSize);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        
        private float MapDistanceToScale(float input)
        {
            if (input is >= 0 and <= DistanceToStartScaling)
            {
                return MaxSize - (input / DistanceToStartScaling) * (MaxSize - MinSize);
            }

            if (input > DistanceToStartScaling)
            {
                return MinSize;
            }

            return MaxSize;
        }
        
        public void Initialize(IObjectPool<IndicatorBehaviour> pool)
        {
            _objectPool = pool;
        }

        public void ReleaseToPool()
        {
            _objectPool.Release(this);
        }
    }
}
