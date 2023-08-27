using System;
using PixelPlay.OffScreenIndicator;
using UnityEngine;

namespace BForBoss
{
    public interface IIncomingAttacker
    {
        IndicatorBehaviour Indicator { get; set; }
        Vector3 Position { get; }
        bool IsActive { get; }
    }
    
    [RequireComponent(typeof(IndicatorObjectPool))]
    public class IncomingAttackIndicatorView : MonoBehaviour
    {
        [Range(0.5f, 0.9f)]
        [Tooltip("Distance offset of the indicators from the centre of the screen")]
        [SerializeField] private float _screenBoundOffset = 0.9f;
        
        private IndicatorObjectPool _indicatorPool;
        private Camera _mainCamera;
        private Vector3 _screenCentre;
        private Vector3 _screenBounds;

        private IIncomingAttacker[] IncomingAttacks => FindObjectsOfType<DerekMissileBehaviour>(includeInactive: true);

        public void Reset()
        {
            foreach (var attacker in IncomingAttacks)
            {
                ClearInactiveAttackerIndicator(attacker);
            }
        }

        private void Awake()
        {
            _indicatorPool = GetComponent<IndicatorObjectPool>();
            _mainCamera = Camera.main;
            _screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
            _screenBounds = _screenCentre * _screenBoundOffset;
        }

        private void LateUpdate()
        {
            foreach (var attacker in IncomingAttacks)
            {
                if (attacker.IsActive)
                {
                    Vector3 screenPosition = OffScreenIndicatorCore.GetScreenPosition(_mainCamera, attacker.Position);
                    IndicatorBehaviour indicator = null;

                    float angle = float.MinValue;
                    OffScreenIndicatorCore.GetArrowIndicatorPositionAndAngle(ref screenPosition, ref angle,
                        _screenCentre, _screenBounds);
                    indicator = GetIndicator(attacker);
                    indicator.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                    indicator.transform.position = screenPosition;
                    indicator.SetSizeBasedOnDistance(
                        Vector3.Distance(_mainCamera.transform.position, attacker.Position));
                }
                else
                {
                    ClearInactiveAttackerIndicator(attacker);
                }
            }
        }

        private IndicatorBehaviour GetIndicator(IIncomingAttacker attacker)
        {
            if (attacker.Indicator != null)
            {
                return attacker.Indicator;
            }
            var indicator = _indicatorPool.GetIndicator();
            attacker.Indicator = indicator;
            return indicator;
        }

        private void ClearInactiveAttackerIndicator(IIncomingAttacker attacker)
        {
            attacker.Indicator?.ReleaseToPool();
            attacker.Indicator = null;
        }
    }
}
