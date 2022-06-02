using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    
    public interface IMeleeWeapon
    {
        float CurrentCooldown { get; }
        float MaxCooldown { get; }
        bool CanMelee { get; }
    }
    
    public class MeleeWeaponBehaviour : MonoBehaviour, IMeleeWeapon
    {
        [InlineEditor]
        [SerializeField] private MeleeScriptableObject _meleeScriptable;
        [SerializeField] private Transform _playerTransform;

        [SerializeField] private bool _canAttackMany = true;

        private MeleeWeapon _weapon;
        private InputAction _meleeActionInputAction;
        private Func<Transform> _getTransform;
        private Action _onSuccessfulAttack;

        public float CurrentCooldown => _weapon?.CurrentCooldown ?? 0f;
        public float MaxCooldown => _meleeScriptable != null ? _meleeScriptable.AttackCoolDown : 1f;
        public bool CanMelee => _weapon?.CanMelee ?? false;

        public void Initialize(InputAction meleeAttackAction, 
            Func<Transform> getTransform,
            IMeleeProperties properties = null, 
            Action onSuccessfulAttack = null)
        {
            _meleeActionInputAction = meleeAttackAction;
            _getTransform = getTransform;
            _onSuccessfulAttack = onSuccessfulAttack;
            _weapon = new MeleeWeapon(properties ?? _meleeScriptable);
            BindActions();
        }

        private void OnMeleeInputAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var t = _playerTransform ? _playerTransform : _getTransform();
                var attack = _canAttackMany ? 
                    _weapon.AttackManyIfPossible(t.position, t.forward) : 
                    _weapon.AttackOneIfPossible(t.position, t.forward);

                if (attack)
                {
                    _onSuccessfulAttack?.Invoke();
                }
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
                Debug.LogWarning("Player transform is not set on melee weapon, gizmos will not be drawn");
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

        private void OnDrawGizmosSelected()
        {
            if (_meleeScriptable != null && _playerTransform != null)
            {
                var t = _playerTransform;
                Gizmos.color = Color.blue;
                Gizmos.matrix = t.localToWorldMatrix;
                var center = Vector3.zero;
                var radius = _meleeScriptable.HalfRange;
                center.y += radius;
                center.z += radius;
                Gizmos.DrawWireSphere(center, radius);
                center.y -= 2 * radius;
                center.y += _meleeScriptable.Height;
                Gizmos.DrawWireSphere(center, radius);
            }
        }

        public void ApplyDamageDelayed()
        {
            _weapon.ApplyDamageDelayed();
        }
    }
}
