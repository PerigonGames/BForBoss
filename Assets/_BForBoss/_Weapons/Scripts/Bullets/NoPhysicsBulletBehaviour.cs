using UnityEngine;

namespace Perigon.Weapons
{
    public class NoPhysicsBulletBehaviour : BulletBehaviour
    {
        private bool TryMoveForward(out Vector3 translationForward)
        {
            Debug.Log("Try Moving Forward");
            var distance = _properties.Speed * Time.deltaTime;
            translationForward = transform.forward * distance;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance, ~Mask))
            {
                Debug.Log("Hitting Something: " + hit.collider.name);
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
