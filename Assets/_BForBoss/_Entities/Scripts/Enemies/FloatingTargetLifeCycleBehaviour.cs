using System;
using UnityEngine;

namespace Perigon.Entities
{
    public class FloatingTargetBehaviour : EnemyBehaviour
    {
        [SerializeField] private HealthbarViewBehaviour _healthbar;
        public override void Initialize(Action onDeathCallback)
        {
            base.Initialize(onDeathCallback);
            if (_healthbar != null)
            {
                _healthbar.Initialize(_lifeCycle);
            }
        }

        public override void Reset()
        {
            base.Reset();
            gameObject.SetActive(true);
            _healthbar.Reset();
        }

        private void Awake()
        {
            if (_healthbar == null)
            {
                Debug.LogWarning("A FloatingTargetBehaviour is missing a health bar");
            }
        }

        protected override void LifeCycleFinished()
        {
            if (_onReleaseToSpawner != null)
            {
                _onReleaseToSpawner.Invoke(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
