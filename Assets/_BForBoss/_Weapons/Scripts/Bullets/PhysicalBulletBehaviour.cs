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
        
        private void ResetRigidbodyVelocity(IBullet _)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isActive)
            {
                var contact = collision.GetContact(0);
                HitObject(collision.collider, contact.point, contact.normal );
                Deactivate(); 
            }
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