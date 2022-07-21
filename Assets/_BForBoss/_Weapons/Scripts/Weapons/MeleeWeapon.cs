using System;
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

        public bool TryAttackMany(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return false;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            _hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, ref _enemyBuffer);
            return true;
        }
        
        public bool TryAttackOne(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return false;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            _hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, ref _enemyBuffer);
            if (_hits > 1)
                _hits = 1; //ensure we only damage first enemy
            return true;
        }
        
        public void ApplyDamage()
        {
            if (_enemyBuffer.IsNullOrEmpty())
                return;
            for (int i = 0; i < _hits; i++)
            {
                DamageEnemy(_enemyBuffer[i]);
            }
        }

        private void DamageEnemy(Collider enemyCollider)
        {
            if(enemyCollider.TryGetComponent(out IWeaponHolder weaponHolder))
            {
                weaponHolder.DamagedBy(_meleeProperties.Damage);
                _onHitEntity?.Invoke(!weaponHolder.IsAlive);
            }
        }
    }
}
