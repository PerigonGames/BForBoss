using System;
using System.Collections.Generic;
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

        private int layerMask = ~LayerMask.GetMask("Player");
        
        public float CurrentCooldown => _currentCooldown;

        public MeleeWeapon(IMeleeProperties meleeProperties, Action<bool> onHitEntity = null)
        {
            _meleeProperties = meleeProperties;
        }

        public void DecrementCooldown(float deltaTime)
        {
            _currentCooldown -= _currentCooldown > 0 ? deltaTime : _currentCooldown;
        }

        public bool TryAttackMany(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return false;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            _hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, layerMask, ref _enemyBuffer);
            return true;
        }
        
        public bool TryAttackOne(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return false;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            _hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, layerMask, ref _enemyBuffer);
            if (_hits > 1)
                _hits = 1; //ensure we only damage first enemy
            return true;
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

        private Vector3 DamageEnemy(Collider enemyCollider, Vector3 position)
        {
            if (enemyCollider.TryGetComponent(out IKnockback knockback))
            {
                knockback.ApplyKnockback(_meleeProperties.MeleeKnockbackForce, position);
            }

            if(enemyCollider.TryGetComponent(out IWeaponHolder weaponHolder))
            {
                weaponHolder.DamagedBy(_meleeProperties.Damage);
                _onHitEntity?.Invoke(!weaponHolder.IsAlive);
            }
            return enemyCollider.ClosestPointOnBounds(position);
        }
    }
}
