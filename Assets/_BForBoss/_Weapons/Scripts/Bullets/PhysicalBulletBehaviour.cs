using UnityEngine;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalBulletBehaviour : BulletBehaviour
    {
        private Rigidbody _rb;
        private IBullet _bulletBehaviour;

        private void SetRigidbodyVelocity()
        {
            _rb.velocity = transform.forward * BulletProperties.Speed;
        }
        
        private void ResetRigidbodyVelocity(IBullet _)
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
            _bulletBehaviour = this;
        }

        private void OnEnable()
        {
            _bulletBehaviour.OnBulletSpawn += SetRigidbodyVelocity;
            _bulletBehaviour.OnBulletDeactivate += ResetRigidbodyVelocity;
        }
        
        private void OnDisable()
        {
            _bulletBehaviour.OnBulletSpawn -= SetRigidbodyVelocity;
            _bulletBehaviour.OnBulletDeactivate -= ResetRigidbodyVelocity;
        }


    }
}
