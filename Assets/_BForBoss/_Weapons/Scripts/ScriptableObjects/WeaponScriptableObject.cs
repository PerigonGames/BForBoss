using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeapon
    {
        float RateOfFire { get; }
        Sprite Crosshair { get; }
        float BulletSpread { get; }
    }
    
    [CreateAssetMenu(fileName = "WeaponProperties", menuName = "PerigonGames/Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject, IWeapon
    {
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _bulletSpread = 1f;
        [PreviewField]
        [SerializeField] private Sprite _crosshairImage = null;

        public float RateOfFire => _rateOfFire;
        public float BulletSpread => _bulletSpread;
        public Sprite Crosshair => _crosshairImage;
    }
}
