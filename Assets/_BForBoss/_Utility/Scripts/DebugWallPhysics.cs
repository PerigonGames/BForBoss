using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Perigon.Utility
{

    [RequireComponent(typeof(Collider))]
    public class DebugWallPhysics : MonoBehaviour
    {
        public Vector3 ContactPoint
        {
            get => _contactPoint;
            set
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider>();
                }

                _contactPoint = _collider.ClosestPointOnBounds(value);
            }
        }

        public Vector3 VelocityPoint { get; private set; }

        public Vector3 ProjectionPoint => _projectedVelocity + ContactPoint;
        public float ProjectionMagnitude => _projectedVelocity.magnitude;

        public Vector3 Velocity => VelocityPoint - ContactPoint;

        private Vector3 _contactPoint;
        private Collider _collider;
        private Vector3 _projectedVelocity;
        private Vector3 _normal;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            
        }

        public void SetContactPoint(Vector3 newPoint)
        {
            if (newPoint == ContactPoint) return;
            
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }

            ContactPoint = _collider.ClosestPoint(newPoint);
            _normal = GetNormal(_collider);
        }

        public void SetVelocityPoint(Vector3 newPoint)
        {
            if (newPoint == VelocityPoint) return;
            
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }

            VelocityPoint = newPoint;
            _projectedVelocity = Projection(Velocity, _normal);
        }

        private Vector3 GetNormal(Collider col)
        {
            var ray = new Ray(Vector3.zero, _contactPoint);
            if (col.Raycast(ray, out RaycastHit hitInfo, 100f))
            {
                return hitInfo.normal;
            }
            return Vector3.negativeInfinity;
        }

        private static Vector3 Projection(Vector3 velocity, Vector3 planeNormal)
        {
            var magnitude = velocity.magnitude;
            velocity.y = 0;
            return Vector3.ProjectOnPlane(velocity, planeNormal).normalized * magnitude;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ContactPoint, Velocity);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(ContactPoint, _normal);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(ContactPoint, _projectedVelocity);
            
        }
    }
}
