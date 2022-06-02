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
        private bool _applyDamageDelayed;
        private int hits;
        
        public float CurrentCooldown => _currentCooldown;

        public MeleeWeapon(IMeleeProperties meleeProperties, bool applyDamageDelayed, Action<bool> onHitEntity = null)
        {
            _applyDamageDelayed = applyDamageDelayed;
            _meleeProperties = meleeProperties;
        }

        public void DecrementCooldown(float deltaTime)
        {
            _currentCooldown -= _currentCooldown > 0 ? deltaTime : _currentCooldown;
        }

        public bool AttackManyIfPossible(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return false;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, ref _enemyBuffer);
            if (hits <= 0) 
                return true;

            if (!_applyDamageDelayed)
            {
                for (int i = 0; i < hits; i++)
                {
                    DamageEnemy(_enemyBuffer[i]);
                }
            }

            return true;
        }
        
        public bool AttackOneIfPossible(Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            if (!CanMelee)
                return false;
            _currentCooldown += _meleeProperties.AttackCoolDown;
            
            var hits = _meleeProperties.OverlapCapsule(playerPosition, playerForwardDirection, ref _enemyBuffer);
            if (hits <= 0) 
                return true;

            if (_applyDamageDelayed)
            {
                hits = 1; //ensure we only damage first enemy
            }
            else
            {
                DamageEnemy(_enemyBuffer[0]);
            }

            return true;
        }

        private void DamageEnemy(Collider enemyCollider)
        {
            if(enemyCollider.TryGetComponent(out LifeCycleBehaviour lifeCycle))
            {
                lifeCycle.Damage(_meleeProperties.Damage);
                _onHitEntity?.Invoke(!lifeCycle.IsAlive);
            }
        }

        public void ApplyDamageDelayed()
        {
            if (!_applyDamageDelayed || _enemyBuffer == null)
                return;
            for (int i = 0; i < hits; i++)
            {
                DamageEnemy(_enemyBuffer[i]);
            }
        }
    }
}
