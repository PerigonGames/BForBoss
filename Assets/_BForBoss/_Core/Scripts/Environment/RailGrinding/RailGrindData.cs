using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace BForBoss
{
    [RequireComponent(typeof(SplineContainer))]
    public class RailGrindData : MonoBehaviour
    {
        private SplineContainer _splineContainer;
        private bool _normalRailDirection;
        public bool NormalRailDirection => _normalRailDirection;
        public float RailLength => _splineContainer.CalculateLength();

        private void Awake()
        {
            _splineContainer = GetComponent<SplineContainer>();
        }
        
        private Vector3 LocalToWorldConversion(float3 localPoint)
        {
            return transform.TransformPoint(localPoint);
        }
        
        private float3 WorldToLocalConversion(Vector3 worldPoint)
        {
            return transform.InverseTransformPoint(worldPoint);
        }

        public Vector3 CalculateNextPosition(float progress)
        {
            SplineUtility.Evaluate(_splineContainer.Spline, progress, out var position, out var tangent, out var up);
            return LocalToWorldConversion(position);
        }

        public Vector3 CalculateForward(float normalizedTime)
        {
            SplineUtility.Evaluate(_splineContainer.Spline, normalizedTime, out var pos, out var forward, out var up);
            return forward;
        }

        public float CalculateTargetRailPoint(Vector3 playerPosition, out Vector3 worldPosOnSpline)
        {
            SplineUtility.GetNearestPoint(_splineContainer.Spline, WorldToLocalConversion(playerPosition), out var nearestPoint, out var time);
            worldPosOnSpline = LocalToWorldConversion(nearestPoint);
            return time;
        }
        
        public void CalculateDirection(float3 railForward, Vector3 playerForward)
        {
            _normalRailDirection = Vector3.Angle(railForward, playerForward.normalized) <= 90f;
        }
    }
}
