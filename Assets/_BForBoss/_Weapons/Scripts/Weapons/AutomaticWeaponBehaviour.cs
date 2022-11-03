using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class AutomaticWeaponBehaviour : WeaponBehaviour
    {

        private const string FIRE_RATE_PARAM = "FireRate";
        private StudioEventEmitter _weaponFiringAudio;

        private int _shotsFired;
        
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
                _shotsFired += _weapon.FireIfPossible() ? 1 : 0;
                float shotsPerSecond = _shotsFired / Mathf.Max(_timeSinceFire, 1f);
                _weaponFiringAudio.SetParameter(FIRE_RATE_PARAM, shotsPerSecond);
            }
            _weapon.DecrementElapsedTimeRateOfFire(Time.deltaTime, Time.timeScale);
            _weapon.ReloadWeaponCountDownIfNeeded(Time.deltaTime, Time.timeScale);
            if (!_isFiring && _weaponFiringAudio.IsPlaying())
            {
                _weaponFiringAudio.Stop();
            }
        }

        protected override void PlayFiringAudio()
        {
            if (!_weaponFiringAudio.IsPlaying())
            {
                _weaponFiringAudio.EventReference = _weapon.ShotAudio;
                _weaponFiringAudio.SetParameter(FIRE_RATE_PARAM, 0f);
                _weaponFiringAudio.Play();
                
            }
        }

        protected override void HandleOnStartReloading()
        {
            _weaponFiringAudio.Stop();
            base.HandleOnStartReloading();
        }
    }
}
