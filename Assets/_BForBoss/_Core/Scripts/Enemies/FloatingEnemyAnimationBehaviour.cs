using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public interface IFloatingEnemyAnimation
    {
        void SetShootingAnimation();
        void SetIdleAnimation();
        void SetMovementAnimation();
    }
    
    public class FloatingEnemyAnimationBehaviour : MonoBehaviour, IFloatingEnemyAnimation
    {
        private const string ANIMATION_MOVEMENT_KEY = "Movement";
        private const string ANIMATION_SHOOTING_KEY = "Shooting";

        [Resolve] [SerializeField] private Animator _animator = null;

        
        public void Initialize()
        {
            this.PanicIfNullObject(_animator, nameof(_animator));
        }

        public void SetMovementAnimation()
        {
            _animator.SetBool(ANIMATION_MOVEMENT_KEY, true);
            _animator.SetBool(ANIMATION_SHOOTING_KEY, false);
        }
        
        public void SetIdleAnimation()
        {
            _animator.SetBool(ANIMATION_MOVEMENT_KEY, false);
            _animator.SetBool(ANIMATION_SHOOTING_KEY, false);
        }

        public void SetShootingAnimation()
        {
            _animator.SetBool(ANIMATION_MOVEMENT_KEY, false);
            _animator.SetBool(ANIMATION_SHOOTING_KEY, true);
        }
    }
}
