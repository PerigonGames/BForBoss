using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace BForBoss
{
    public class IndicatorBehaviour : MonoBehaviour
    {
        private const float MAX_SCALE_SIZE = 1f;
        [SerializeField, MaxValue(1.0f)]
        private float _minScaleSize = 0.1f;
        [SerializeField, Min(0.1f)]
        private float _distanceToStartScaling = 30f;

        [SerializeField] 
        private Image _backgroundArrow;
        [SerializeField] 
        private Transform _dangerSign;
        
        private IObjectPool<IndicatorBehaviour> _objectPool;

        public void SetSizeBasedOnDistance(float distance)
        {
            var mappedValue = MapDistanceToScale(distance);
            var scale = Math.Clamp(mappedValue, _minScaleSize, MAX_SCALE_SIZE);
            transform.localScale = new Vector3(scale, scale, scale);
            _backgroundArrow.color = MapDistanceToColor(distance);
        }
        
        private float MapDistanceToScale(float input)
        {
            if (input >= 0 && input <= _distanceToStartScaling)
            {
                return MAX_SCALE_SIZE - (input / _distanceToStartScaling) * (MAX_SCALE_SIZE - _minScaleSize);
            }

            if (input > _distanceToStartScaling)
            {
                return _minScaleSize;
            }

            return MAX_SCALE_SIZE;
        }

        private Color MapDistanceToColor(float input)
        {
            if (input >= 0 && input <= _distanceToStartScaling)
            {
                return Color.red;
            }

            if (input > _distanceToStartScaling)
            {
                return Color.yellow;
            }

            return Color.yellow;
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
