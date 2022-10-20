using System;
using System.Collections;
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
        
        public void StartEffect()
        {
            _effect.Reinit();
            _effect.Play();
            StartCoroutine(StopAfterTime());
        }

        public void StopEffect()
        {
            _effect.Stop();
        }
        
        private IEnumerator StopAfterTime()
        {
            yield return new WaitForSeconds(_duration);
            if (_effect != null)
            {
                _effect.Stop();
            }
            OnEffectStop?.Invoke();
        }
        
        private void Awake()
        {
            _effect = GetComponent<VisualEffect>();
        }

        private void OnEnable()
        {
            _effect.Stop();
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(StopAfterTime));
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
