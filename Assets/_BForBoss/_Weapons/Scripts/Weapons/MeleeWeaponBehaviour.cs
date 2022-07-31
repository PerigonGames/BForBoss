using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Perigon.VFX;

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
        [Tooltip("Used for previewing melee radius in editor")]
        [SerializeField] private Transform _playerTransform;

        [SerializeField] private bool _canAttackMany = true;
        [SerializeField] private TimedVFXEffect _meleeVFXPrefab = null;

        private MeleeWeapon _weapon;
        private InputAction _meleeActionInputAction;
        private Func<Transform> _getTransform;
        private Action _onSuccessfulAttack;
        private ObjectPooler<TimedVFXEffect> _meleeVFXPool;

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

            if (_meleeVFXPrefab != null)
            {
                _meleeVFXPool = new ObjectPooler<TimedVFXEffect>(
                    () =>
                    {
                        var newVFX = Instantiate(_meleeVFXPrefab);
                        newVFX.OnEffectStop += () =>
                        {
                            _meleeVFXPool.Reclaim(newVFX);
                        };
                        return newVFX;
                    },
                    (effect =>
                    {
                        effect.gameObject.SetActive(true);
                    }),
                    (effect =>
                    {
                        effect.gameObject.SetActive(false);
                    }));
            }

            BindActions();
        }

        private void OnMeleeInputAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var t = _getTransform();
                var isAttackSuccessful = _canAttackMany ?
                    _weapon.TryAttackMany(t.position, t.forward) :
                    _weapon.TryAttackOne(t.position, t.forward);

                if (isAttackSuccessful)
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

        private void OnValidate()
        {
            if (_playerTransform == null)
            {
                Debug.LogWarning("Player Transform Missing from MeleeWeaponBehaviour");
            }
        }

        private void OnDrawGizmos()
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
                Gizmos.DrawSphere(center, radius);
                center.y -= 2 * radius;
                center.y += _meleeScriptable.Height;
                Gizmos.DrawSphere(center, radius);
            }
        }

        public void ApplyDamage()
        {
            var t = _getTransform();
            var pointsHit = _weapon.ApplyDamage(t.position + t.up); // use player's torso instead of feet

            if (_meleeVFXPool == null)
                return;
            foreach(var point in pointsHit)
            {
                var vfx = _meleeVFXPool.Get();
                vfx.transform.SetPositionAndRotation(point, Quaternion.LookRotation(-t.forward));
                vfx.StartEffect();
            }
        }
    }
}
