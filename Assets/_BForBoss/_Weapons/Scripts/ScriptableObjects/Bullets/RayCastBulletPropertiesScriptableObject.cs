using UnityEngine;

namespace Perigon.Weapons
{
    
    [CreateAssetMenu(fileName = "BulletProperties", menuName = "PerigonGames/RayCastBullet", order = 2)]
    public class RayCastBulletPropertiesScriptableObject : ScriptableObject, IBulletProperties
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _maxDistance = 100;
        [SerializeField] private float _bulletHoleTimeToLive = 10f;

        public float Damage => _damage;
        public float Speed => 0;
        public float MaxTravelDistance => _maxDistance;
        public float BulletHoleTimeToLive => _bulletHoleTimeToLive;
    }
}
