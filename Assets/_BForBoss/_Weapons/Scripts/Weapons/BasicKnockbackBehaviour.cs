
using UnityEngine;

namespace Perigon.Weapons
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
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            Debug.Log("Print Statssssesssfmdccdd");
            Debug.Log("Print Stfdffftssssefmdccdd");
            Debug.Log("Print ");
            Debug.Log("Print Sssefmdccdd");
        }

        public void ApplyKnockback(float force, Vector3 originPosition)
        {
            var distance = Vector3.Distance(transform.position, originPosition);
            _rb.AddExplosionForce(force, originPosition, distance * _radiusMultiplier);
        }
    }
}
