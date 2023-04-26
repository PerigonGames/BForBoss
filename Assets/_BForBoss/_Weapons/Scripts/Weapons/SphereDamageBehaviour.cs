using System;
using UnityEngine;

namespace Perigon.Weapons
{
    public class SphereDamageBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private float _radiusSize = 10;
        
        public void DamageArea(float damage)
        {
            var colliders = Physics.OverlapSphere(transform.position, _radiusSize);
            foreach (var collider in colliders)
            {
                if(collider.TryGetComponent(out IWeaponHolder weaponHolder))
                {
                    weaponHolder.DamageBy(damage);
                } 
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, _radiusSize);
        }
    }
}
