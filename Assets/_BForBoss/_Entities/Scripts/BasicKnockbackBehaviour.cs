using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Entities
{
    public interface IKnockback
    {
        void ApplyKnockback(float force, Vector3 originPosition);
    }
    
    [RequireComponent(typeof(Rigidbody))]
    public class BasicKnockbackBehaviour : MonoBehaviour, IKnockback
    {
        [SerializeField] private float _radiusMultiplier = 2f;
        private Rigidbody _rb;
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            
        }

        public void ApplyKnockback(float force, Vector3 originPosition)
        {
            var distance = Vector3.Distance(transform.position, originPosition);
            _rb.AddExplosionForce(force, originPosition, distance * _radiusMultiplier);
        }
    }
}
