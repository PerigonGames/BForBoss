using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public class MeleeWeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private MeleeScriptableObject _meleeScriptable;
        [SerializeField] private Transform _playerTransform;

        [SerializeField] private bool _attackMany = true;

        private MeleeWeapon _weapon;
        private InputAction _meleeActionInputAction;

        public void Initialize(InputAction meleeAttackAction, IMeleeProperties properties = null)
        {
            _meleeActionInputAction = meleeAttackAction;
            _weapon = new MeleeWeapon(properties ?? _meleeScriptable);
            BindActions();
        }

        private void OnMeleeInputAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var t = _playerTransform ? _playerTransform : transform;
                if(_attackMany)
                    _weapon.AttackManyIfPossible(t.position, t.forward);
                else
                    _weapon.AttackOneIfPossible(t.position, t.forward);
            }
        }

        private void BindActions()
        {
            _meleeActionInputAction.performed += OnMeleeInputAction;
            _meleeActionInputAction.canceled += OnMeleeInputAction;
        }

        private void Awake()
        {
            if (_playerTransform == null)
            {
                Debug.LogWarning("Player transform is not set on melee weapon, melee attacks may not work as expected");
            }
        }

        private void Update()
        {
            _weapon.DecrementCooldown(Time.deltaTime);
        }

        private void OnEnable()
        {
            if (_meleeActionInputAction != null)
            {
                BindActions();
            }
        }

        private void OnDisable()
        {
            if (_meleeActionInputAction != null)
            {
                _meleeActionInputAction.performed -= OnMeleeInputAction;
                _meleeActionInputAction.canceled -= OnMeleeInputAction;
            }
        }

        private void OnDrawGizmos()
        {
            if (_meleeScriptable != null)
            {
                var t = _playerTransform ? _playerTransform : transform;
                Gizmos.color = Color.blue;
                Gizmos.matrix = t.localToWorldMatrix;
                var center = Vector3.zero;
                var radius = _meleeScriptable.Range * 0.5f;
                center.y += radius;
                center.z += radius;
                Gizmos.DrawWireSphere(center, radius);
                center.y -= 2 * radius;
                center.y += _meleeScriptable.Height;
                Gizmos.DrawWireSphere(center, radius);
            }
        }
    }
}
