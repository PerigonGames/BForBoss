using System;
using Perigon.Utility;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class ShootAtInteractiveButtonBehaviour : MonoBehaviour, IBulletCollision
    {
        [SerializeField] private GameObject _labelGO;
        
        private Action _onButtonShot;
        private bool _canBeInteractedWith = false;
        
        public void Initialize(Action onButtonShot)
        {
            _onButtonShot = onButtonShot;
            _canBeInteractedWith = true;
            
            if (_labelGO == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_labelGO)} is null"));
            }
        }

        public void Reset()
        {
            ToggleInteractivity(true);
        }

        public void OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            if (!_canBeInteractedWith)
            {
                return;
            }
            
            ToggleInteractivity(false);
            _onButtonShot?.Invoke();
        }

        private void ToggleInteractivity(bool canBeInteractedWith)
        {
            _canBeInteractedWith = canBeInteractedWith;
            _labelGO.SetActive(canBeInteractedWith);
        }
    }
}
