using System;
using System.Collections;
using System.Collections.Generic;
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
        
        // Start is called before the first frame update
        void Awake()
        {
            _effect = GetComponent<VisualEffect>();
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
            _effect.Stop();
            OnEffectStop?.Invoke();
        }
    }
}
