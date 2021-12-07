using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalBulletBehaviour : BulletBehaviour
    {
        private Rigidbody _rb;

        private void SetRigidbodyVelocity()
        {
            _rb.velocity = transform.forward * _properties.Speed;
        }
        
        private void ResetRigidbodyVelocity()
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        
        protected override void HandleCollision(Vector3 position)
        {
            
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.contacts[0].point);
            Deactivate();
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            OnBulletSpawn += SetRigidbodyVelocity;
            OnBulletDeactivate += ResetRigidbodyVelocity;
        }
        
        private void OnDisable()
        {
            OnBulletSpawn -= SetRigidbodyVelocity;
            OnBulletDeactivate -= ResetRigidbodyVelocity;
        }


    }
}
