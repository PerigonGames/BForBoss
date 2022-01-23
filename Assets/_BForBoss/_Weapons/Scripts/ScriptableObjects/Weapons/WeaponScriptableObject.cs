using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeaponProperties
    {
        string NameOfWeapon { get; }
        float RateOfFire { get; }
        Sprite Crosshair { get; }
        float BulletSpread { get; }
        float ReloadDuration { get; }
        int BulletsPerShot { get; }
        int AmmunitionAmount { get; }
        float VisualRecoilForce { get; }
        BulletTypes TypeOfBullet { get; }

    }

    [CreateAssetMenu(fileName = "WeaponProperties", menuName = "PerigonGames/Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject, IWeaponProperties
    {
        [SerializeField] private string _nameOfWeapon = "";
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _bulletSpread = 1f;
        [SerializeField]
        [Range(0.01f, 10f)] private float _reloadDuration = 0.5f;
        [SerializeField] private int _bulletsPerShot = 1;
        [SerializeField] 
        [Range(1, 1000)] private int _ammunitionAmount = 20;
        [SerializeField] private BulletTypes _typeOfBullet = BulletTypes.NoPhysics;
        [Title("Visuals")]
        [PreviewField]
        [SerializeField] private Sprite _crosshair = null;

        [SerializeField] private float _visualRecoil = 0.5f;
        string IWeaponProperties.NameOfWeapon => _nameOfWeapon;
        float IWeaponProperties.RateOfFire => _rateOfFire;
        float IWeaponProperties.BulletSpread => _bulletSpread;
        float IWeaponProperties.ReloadDuration => _reloadDuration;
        int IWeaponProperties.BulletsPerShot => _bulletsPerShot;
        int IWeaponProperties.AmmunitionAmount => _ammunitionAmount;
        float IWeaponProperties.VisualRecoilForce => _visualRecoil;
        Sprite IWeaponProperties.Crosshair => _crosshair;
        public BulletTypes TypeOfBullet => _typeOfBullet;
    }
}
