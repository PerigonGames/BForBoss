using System;
using Perigon.Entities;
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

        public float CurrentCooldown => _currentCooldown;

        public MeleeWeapon(IMeleeProperties meleeProperties, Action<bool> onHitEntity = null)
        {
            _meleeProperties = meleeProperties;
        }

        public void DecrementCooldown(float deltaTime)
        {
            _currentCooldown -= _currentCooldown > 0 ? deltaTime : _currentCooldown;
        }

        public void AttackManyIfPossible(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            var hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, ref _enemyBuffer);
            if (hits <= 0) 
                return;
            
            for(int i = 0; i < hits; i++)
            {
                DamageEnemy(_enemyBuffer[i]);
            }
        }
        
        public void AttackOneIfPossible(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            var hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, ref _enemyBuffer);
            if (hits <= 0) 
                return;
            
            DamageEnemy(_enemyBuffer[0]);
        }

        private void DamageEnemy(Collider enemyCollider)
        {
            if(enemyCollider.TryGetComponent(out LifeCycleBehaviour lifeCycle))
            {
                lifeCycle.Damage(_meleeProperties.Damage);
                _onHitEntity?.Invoke(!lifeCycle.IsAlive);
            }
        }
    }
}