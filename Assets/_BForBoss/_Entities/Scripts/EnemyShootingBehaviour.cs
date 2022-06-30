using System;
using UnityEngine;

namespace Perigon.Entities
{
    public class EnemyShootingBehaviour : MonoBehaviour
    {
        private enum ShootingState
        {
            RotateTowards,
            Shoot
        }
        
        [SerializeField] private float _shootDistance = 5;
        [SerializeField] private float _lookRotationSpeed = 100;
        [SerializeField] private float _shootCountDown = 1;

        private float _elapsedShootCountDown = 0;
        private float _shootCoolDown = 0;
        private Func<Vector3> Destination = null;
        private ShootingState _shootingState = ShootingState.RotateTowards;
        
        public void Initialize(Func<Vector3> getPlayerPosition)
        {
            Destination = getPlayerPosition;
        }

        public void Reset()
        {
            _elapsedShootCountDown = _shootCountDown;
        }

        public void ShootingUpdate()
        {
            switch (_shootingState)
            {
                case ShootingState.RotateTowards:
                    RotateTowardPlayer();
                    ShootIfPossible();
                    break;
                case ShootingState.Shoot:
                    CountDownToShoot();
                    break;
            }
        }

        private void RotateTowardPlayer()
        {
            Vector3 targetDirection = Destination() - transform.position;
            float singleStep = _lookRotationSpeed * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        private void ShootIfPossible()
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit, _shootDistance))
            {
                if (hit.collider.name == "First Person Character")
                {
                    _shootingState = ShootingState.Shoot;
                }
            }
        }

        private void CountDownToShoot()
        {
            _elapsedShootCountDown -= Time.deltaTime;
            if (_elapsedShootCountDown <= 0)
            {
                // Shoot
                _shootingState = ShootingState.RotateTowards;
                _elapsedShootCountDown = _shootCountDown;
            }
        }
    }
}
