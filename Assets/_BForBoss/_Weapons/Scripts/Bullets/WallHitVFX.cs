using Perigon.Utility;
using DG.Tweening;
using PerigonGames;
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

        private RandomUtility _rand;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _rand = new RandomUtility();
        }

        public void Initialize(ObjectPooler<WallHitVFX> pool)
        {
            _pool = pool;
        }

        public void Spawn(float fadeDuration)
        {
            if (_rand.NextTryGetElement(_bulletHoleSprites, out var bulletHole))
            {
                _spriteRenderer.sprite = bulletHole;
            }
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
