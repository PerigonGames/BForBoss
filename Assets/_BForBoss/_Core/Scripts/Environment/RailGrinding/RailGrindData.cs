using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace BForBoss
{
    [RequireComponent(typeof(SplineContainer))]
    public class RailGrindData : MonoBehaviour
    {
        private SplineContainer _splineContainer;

        private void Awake()
        {
            _splineContainer = GetComponent<SplineContainer>();
        }

        public float CalculateTargetRailPointAsTime(Vector3 playerPosition)
        {
            float3 floatPlayerPosition = new float3(playerPosition.x, playerPosition.y, playerPosition.z);
            var point = SplineUtility.GetNearestPoint(
                _splineContainer.Spline, 
                floatPlayerPosition, 
                out var nearest,
                out var time);
            return time;
        }
    }
}
