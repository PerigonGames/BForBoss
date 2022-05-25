using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class WeaponBehaviour
    {
        private const float WALL_HIT_ZFIGHT_BUFFER = 0.01f;

        private void FireRayCastBullets(int numberOfBullets)
        {
            var camOrigin = MainCamera.transform.position;
            //TODO - origin -> direction -> random direction
            for (int i = 0; i < numberOfBullets; i++)
            {

                var forward = new Vector3(_weapon.GenerateRayCastAngle()/10, _weapon.GenerateRayCastAngle()/10, 1);
                if (Physics.Raycast(camOrigin, MainCamera.transform.TransformDirection(forward), out var hit, Mathf.Infinity, ~TagsAndLayers.Layers.TriggerArea))
                {
                    if (hit.collider.TryGetComponent(out LifeCycleBehaviour lifeCycle))
                    {
                        lifeCycle.Damage(_weapon.DamagePerRayCast);
                        _crosshair.ActivateHitMarker(!lifeCycle.IsAlive);
                    }
                    else
                    {
                        OnBulletHitWall(hit);
                    }
                }
            }
        }

        private void OnBulletHitWall(RaycastHit hit)
        {
            var wallHitVFX = _wallHitVFXSpawner.SpawnWallHitVFX();
            wallHitVFX.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
            wallHitVFX.transform.Translate(0f, 0f, WALL_HIT_ZFIGHT_BUFFER, Space.Self);
            wallHitVFX.Spawn(2.0f);
        }
    }
}
