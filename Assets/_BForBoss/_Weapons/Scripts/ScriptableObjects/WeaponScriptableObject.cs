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
    }
    
    [CreateAssetMenu(fileName = "WeaponProperties", menuName = "PerigonGames/Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject, IWeaponProperties
    {
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _bulletSpread = 1f;
        [PreviewField]
        [SerializeField] private Sprite _crosshairImage = null;

        public float RateOfFire => _rateOfFire;
        [CanBeNull] public Sprite Crosshair => _crosshairImage;
        public float BulletSpread => _bulletSpread;
    }
}
