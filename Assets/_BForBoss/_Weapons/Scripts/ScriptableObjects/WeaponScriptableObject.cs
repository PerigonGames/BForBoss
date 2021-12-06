using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeaponProperties
    {
        float RateOfFire { get; }
        Sprite Crosshair { get; }
        float BulletSpread { get; }
        float ReloadDuration { get; }
        int BulletsPerShot { get; }
        int AmmunitionAmount { get; }
    }
    
    [CreateAssetMenu(fileName = "WeaponProperties", menuName = "PerigonGames/Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject, IWeaponProperties
    {
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _bulletSpread = 1f;
        [SerializeField] private float _reloadDuration = 0.5f;
        [SerializeField] private int _numberOfBullets = 1;
        [SerializeField] private int _ammunitionAmount = 20;
        [PreviewField]
        [SerializeField] private Sprite _crosshairImage = null;

        public float RateOfFire => _rateOfFire;
        public float BulletSpread => _bulletSpread;
        public float ReloadDuration => _reloadDuration;
        public int BulletsPerShot => _numberOfBullets;
        public int AmmunitionAmount => _ammunitionAmount; 
        public Sprite Crosshair => _crosshairImage;
    }
}
