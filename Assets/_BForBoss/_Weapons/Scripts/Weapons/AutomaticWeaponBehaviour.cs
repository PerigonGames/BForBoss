using FMODUnity;
using UnityEngine;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class AutomaticWeaponBehaviour : WeaponBehaviour
    {
        private const string FIRE_RATE_PARAM = "FireRate";
        private StudioEventEmitter _weaponFiringAudio;

        private int _shotsFired;
        private bool _isFiring;


        public override void Reset()
        {
            base.Reset();
            _isFiring = false;
            _shotsFired = 0;
        }
        
        protected override void Awake()
        {
            base.Awake();
            _weaponFiringAudio = GetComponent<StudioEventEmitter>();
        }
        
        protected override void OnFireInputAction(bool isFiring)
        {
            _isFiring = isFiring;
            
            if (!_isFiring)
            {
                _timeSinceFire = 0;
                _shotsFired = 0;
            }
        }

        protected override void Update()
        {
            if (_isFiring)
            {
                _timeSinceFire += _weapon.ScaledDeltaTime(Time.deltaTime, Time.timeScale);
                _shotsFired += _weapon.TryFire() ? 1 : 0;
                _weaponFiringAudio.SetParameter(FIRE_RATE_PARAM, _shotsFired / Mathf.Max(_timeSinceFire, 1f));
            }
            base.Update();
            if (!_isFiring && _weaponFiringAudio.IsPlaying())
            {
                _weaponFiringAudio.Stop();
            }
        }

        protected override void PlayFiringAudio()
        {
            if (!_weaponFiringAudio.IsPlaying())
            {
                _weaponFiringAudio.EventReference = _weaponConfigurationData.WeaponShotAudio
                _weaponFiringAudio.SetParameter(FIRE_RATE_PARAM, 0f)
                _weaponFiringAudio.Play();
            }
        }

        protected override void PlayReloadingAudio()
        {
            _weaponFiringAudio.Stop();
            base.PlayReloadingAudio();
        }
    }
}
