using Perigon.VFX;
using UnityEngine;

namespace Perigon.Weapons
{
    public class BulletSplashVFX : BaseWallHitVFX
    {
        [SerializeField] private TimedVFXEffect _visualEffect;
        
        public override void Spawn()
        {
            _visualEffect.StartEffect();
            _visualEffect.OnEffectStop += OnEffectStop;
        }
        
        public override void Reset()
        {
            _visualEffect.OnEffectStop -= OnEffectStop;
            _visualEffect.StopEffect();
        }

        private void OnEffectStop()
        {
            _visualEffect.OnEffectStop -= OnEffectStop;
            ReleaseToPool();
        }
    }
}
