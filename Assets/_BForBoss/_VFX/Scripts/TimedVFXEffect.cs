using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Perigon.VFX
{
    [RequireComponent(typeof(VisualEffect))]
    public class TimedVFXEffect : MonoBehaviour
    {
        private VisualEffect _effect;

        [SerializeField] private float _duration = 1.0f;

        public Action OnEffectStop;
        
        private void Awake()
        {
            _effect = GetComponent<VisualEffect>();
        }

        private void OnEnable()
        {
            _effect.Stop();
        }

        public void StartEffect()
        {
            _effect.Reinit();
            _effect.Play();
            StopAfterTime();
        }

        private async void StopAfterTime()
        {
            await Task.Delay(TimeSpan.FromSeconds(_duration));
            if (_effect != null)
            {
                _effect.Stop();
            }
            OnEffectStop?.Invoke();
        }
    }
}
