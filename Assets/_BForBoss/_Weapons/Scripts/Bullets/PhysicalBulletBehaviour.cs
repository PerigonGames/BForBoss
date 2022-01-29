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
            _rb.velocity = transform.forward * BulletProperties.Speed;
        }
        
        private void ResetRigidbodyVelocity()
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var contact = collision.GetContact(0);
            HitObject(collision.collider, contact.point, contact.normal );
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
