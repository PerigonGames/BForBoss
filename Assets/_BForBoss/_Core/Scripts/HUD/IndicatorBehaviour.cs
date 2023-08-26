using System;
using UnityEngine;
using UnityEngine.Pool;

namespace BForBoss
{
    public class IndicatorBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float _minScaleSize = 1f;
        [SerializeField]
        private float _maxScaleSize = 3f;
        [SerializeField]
        private float _distanceToStartScaling = 30f;
        
        private IObjectPool<IndicatorBehaviour> _objectPool;

        public void SetSizeBasedOnDistance(float distance)
        {
            var mappedValue = MapDistanceToScale(distance);
            var scale = Math.Clamp(mappedValue, _minScaleSize, _maxScaleSize);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        
        private float MapDistanceToScale(float input)
        {
            if (input >= 0 && input <= _distanceToStartScaling)
            {
                return _maxScaleSize - (input / _distanceToStartScaling) * (_maxScaleSize - _minScaleSize);
            }

            if (input > _distanceToStartScaling)
            {
                return _minScaleSize;
            }

            return _maxScaleSize;
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
