using System;
using System.Collections.Generic;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public class MeleeWeapon
    {
        private const int BUFFER_SIZE = 10;
            
        private readonly IMeleeProperties _meleeProperties;

        private Collider[] _enemyBuffer = new Collider[BUFFER_SIZE];
        private float _currentCooldown = 0f;
        private Action<bool> _onHitEntity;
        public bool CanMelee => _currentCooldown <= 0f;
        private int _hits;

        public float CurrentCooldown => _currentCooldown;

        public MeleeWeapon(IMeleeProperties meleeProperties, Action<bool> onHitEntity = null)
        {
            _meleeProperties = meleeProperties;
        }

        public void DecrementCooldown(float deltaTime)
        {
            _currentCooldown -= _currentCooldown > 0 ? deltaTime : _currentCooldown;
        }

        public int TryAttackMany(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            return TryAttack(playerPosition, playerForwardDirection);
        }
        
        public int TryAttackOne(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            var hits = TryAttack(playerPosition, playerForwardDirection);

            if (hits < 0) return hits;
            _hits = hits > 1 ? 1 : 0; //ensure we only damage first enemy
            return _hits;
        }
        
        public IList<Vector3> ApplyDamage(Vector3 position)
        {
            if (_enemyBuffer.IsNullOrEmpty())
                return null;
            var pointsHit = new List<Vector3>();
            for (int i = 0; i < _hits; i++)
            {
                var collider = _enemyBuffer[i];
                pointsHit.Add(DamageEnemy(_enemyBuffer[i], position));
            }

            return pointsHit;
        }

        private int TryAttack(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return -1;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            _hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, TagsAndLayers.Layers.PlayerMask, ref _enemyBuffer);
            return _hits;
        }

        private Vector3 DamageEnemy(Collider enemyCollider, Vector3 position)
        {
            if (enemyCollider.TryGetComponent(out IKnockback knockback))
            {
                knockback.ApplyKnockback(_meleeProperties.MeleeKnockbackForce, position);
            }

            if(enemyCollider.TryGetComponent(out IWeaponHolder weaponHolder))
            {
                weaponHolder.DamageBy(_meleeProperties.Damage);
                _onHitEntity?.Invoke(!weaponHolder.IsAlive);
            }
            return enemyCollider.ClosestPointOnBounds(position);
        }
    }
}
