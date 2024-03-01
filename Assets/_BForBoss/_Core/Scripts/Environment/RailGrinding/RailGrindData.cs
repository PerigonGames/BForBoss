using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace BForBoss
{
    [RequireComponent(typeof(SplineContainer))]
    public class RailGrindData : MonoBehaviour
    {
        private SplineContainer _splineContainer;
        public bool normalDir;
        public float RailLength => _splineContainer.CalculateLength();

        private void Awake()
        {
            _splineContainer = GetComponent<SplineContainer>();
        }
        
        private Vector3 LocalToWorldConversion(float3 localPoint)
        {
            Vector3 worldPos = transform.TransformPoint(localPoint);
            return worldPos;
        }
        
        private float3 WorldToLocalConversion(Vector3 worldPoint)
        {
            float3 localPos = transform.InverseTransformPoint(worldPoint);
            return localPos;
        }

        public Vector3 NextPosition(float progress)
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
            float angle = Vector3.Angle(railForward, playerForward.normalized);
            if (angle > 90f)
                normalDir = false;
            else
                normalDir = true;
        }
    }
}
