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
        BulletTypes TypeOfBullet { get; }

    }

    [CreateAssetMenu(fileName = "WeaponProperties", menuName = "PerigonGames/Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject, IWeaponProperties
    {
        [SerializeField] private string _nameOfWeapon = "";
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _bulletSpread = 1f;
        [SerializeField] private float _reloadDuration = 0.5f;
        [SerializeField] private int _bulletsPerShot = 1;
        [SerializeField] private int _ammunitionAmount = 20;
        [SerializeField] private BulletTypes _typeOfBullet = BulletTypes.NoPhysics;
        [PreviewField]
        [SerializeField] private Sprite _crosshair = null;

        public string NameOfWeapon => _nameOfWeapon;
        public float RateOfFire => _rateOfFire;
        public float BulletSpread => _bulletSpread;
        public float ReloadDuration => _reloadDuration;
        public int BulletsPerShot => _bulletsPerShot;
        public int AmmunitionAmount => _ammunitionAmount;
        public Sprite Crosshair => _crosshair;
        public BulletTypes TypeOfBullet => _typeOfBullet;
    }
}
