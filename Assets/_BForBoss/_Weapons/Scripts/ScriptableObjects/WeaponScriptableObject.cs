using UnityEngine;

namespace Perigon.Weapons
{
    public interface IWeapon
    {
        float RateOfFire { get; }
    }
    
    [CreateAssetMenu(fileName = "WeaponProperties", menuName = "PerigonGames/Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject, IWeapon
    {
        [SerializeField] private float _rateOfFire = 0.1f;

        public float RateOfFire => _rateOfFire;
    }
}
