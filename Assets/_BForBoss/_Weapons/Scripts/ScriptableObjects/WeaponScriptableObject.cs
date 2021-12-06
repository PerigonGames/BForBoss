using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeaponProperties
    {
        float RateOfFire { get; }
        [CanBeNull] Sprite Crosshair { get; }
        float BulletSpread { get; }
        int NumberOfBullets { get; }
    }
    
    [CreateAssetMenu(fileName = "WeaponProperties", menuName = "PerigonGames/Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject, IWeaponProperties
    {
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _bulletSpread = 1f;
        [SerializeField] private int _numberOfBullets = 1;
        [PreviewField]
        [SerializeField] private Sprite _crosshairImage = null;

        public float RateOfFire => _rateOfFire;
        public float BulletSpread => _bulletSpread;
        public int NumberOfBullets => _numberOfBullets;
        [CanBeNull] public Sprite Crosshair => _crosshairImage;
    }
}
