using DG.Tweening;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace Perigon.Weapons
{
    public class BulletHoleVFX : BaseWallHitVFX
    {
        [SerializeField] private Sprite[] _bulletHoleSprites;
        private SpriteRenderer _spriteRenderer;
        private ParticleSystem _particleSystem;
        private RandomUtility _rand;

        private const float MAX_ALPHA = 1f;
        private const float MIN_ALPHA = 0f;
        private const float WALL_HIT_VFX_HIT_FADE_DURATION = 2.0f;

        public override void Spawn()
        {
            if (_rand.NextTryGetElement(_bulletHoleSprites, out var bulletHole))
            {
                _spriteRenderer.sprite = bulletHole;
            }
            _particleSystem.Play();
            
            var tween = _spriteRenderer.DOFade(MIN_ALPHA, WALL_HIT_VFX_HIT_FADE_DURATION);
            tween.OnComplete(ReleaseToPool);
            tween.Play();
        }
        public override void Reset()
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _spriteRenderer.SetAlpha(MAX_ALPHA);
        }
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _rand = new RandomUtility();
        }
    }
}
