using System;
using System.Collections;
using System.Collections.Generic;
using Perigon.Entities;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(Rigidbody))]
    public class FloatingEnemyKnockbackBehaviour : MonoBehaviour, IKnockback
    {
        private const float EPSILON_SQUARED = float.Epsilon * float.Epsilon;
        
        [SerializeField] private float _knockbackRadiusModifier = 10f;
        [SerializeField] private float _knockbackUpwardsForceModifier = 10f;
        [SerializeField] private float _knockbackMaxDuration = 2f;
        [SerializeField] private bool _applyGravity = true;
        
        private float _knockbackCurrentTime;
        
        private Rigidbody _rb;

        private ILifeCycle _lifeCycle;
        
        private Action OnKnockbackFinished;
        private Action OnKnockbackStarted;

        public void Initialize(ILifeCycle lifecycle, Action onKnockbackStarted = null, Action onKnockbackFinished = null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _lifeCycle = lifecycle;
            OnKnockbackFinished = onKnockbackFinished;
            OnKnockbackStarted = onKnockbackStarted;
        }
        
        public void ApplyKnockback(float force, Vector3 originPosition)
        {
            if (!_lifeCycle.IsAlive)
                return;
            ToggleKnockbackState(true);
            _rb.AddExplosionForce(force, originPosition, _knockbackRadiusModifier, _knockbackUpwardsForceModifier);
        }

        public void UpdateKnockback(float deltaTime)
        {
            _knockbackCurrentTime += deltaTime;
            if (_knockbackCurrentTime > _knockbackMaxDuration || _rb.velocity.sqrMagnitude < EPSILON_SQUARED)
            {
                ToggleKnockbackState(false);
            }
        }

        public void Reset()
        {
            ToggleKnockbackState(false);
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void ToggleKnockbackState(bool active)
        {
            if (active)
                _knockbackCurrentTime = 0f;
            _rb.isKinematic = !active;
            if(_applyGravity)
                _rb.useGravity = active;
            if(active)
                OnKnockbackStarted?.Invoke();
            else
                OnKnockbackFinished?.Invoke();
        }
    }
}
