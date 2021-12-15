using Perigon.Utility;
using DG.Tweening;
using UnityEngine;

namespace Perigon.Weapons
{
    public class WallHitVFX : MonoBehaviour
    {
        private const float MAX_ALPHA = 1f;
        private const float MIN_ALPHA = 0f;
        
        [SerializeField] private Sprite[] _bulletHoleSprites;
        private SpriteRenderer _spriteRenderer;
        private ParticleSystem _particleSystem;

        private ObjectPooler<WallHitVFX> _pool;

        // Start is called before the first frame update
        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public void Setup(ObjectPooler<WallHitVFX> pool)
        {
            _pool = pool;
        }

        public void Spawn(float fadeDuration)
        {
            _spriteRenderer.sprite = _bulletHoleSprites.GetRandomElement();
            _particleSystem.Play();
            
            var tween = _spriteRenderer.DOFade(MIN_ALPHA, fadeDuration);
            tween.OnComplete(() => _pool.Reclaim(this));
            tween.Play();
        }

        public void Reset()
        {
            _particleSystem.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            _spriteRenderer.SetAlpha(MAX_ALPHA);
        }
    }
}
