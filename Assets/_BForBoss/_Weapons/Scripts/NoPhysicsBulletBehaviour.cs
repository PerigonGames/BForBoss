using UnityEngine;

namespace Perigon.Weapons
{
    public class NoPhysicsBulletBehaviour : BulletBehaviour
    {
        
        private bool TryMoveForward(out Vector3 translationForward)
        {
            var distance = _properties.Speed * Time.deltaTime;
            translationForward = transform.forward * distance;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance))
            {
                HitObject(hit.collider, hit.point, hit.normal);
                return false;
            }
            return true;
        }

        private void LateUpdate()
        {
            if (TryMoveForward(out var translationForward))
            {
                transform.position += translationForward;
            }
            else
            {
                Deactivate();
            }
        }
    }
}
