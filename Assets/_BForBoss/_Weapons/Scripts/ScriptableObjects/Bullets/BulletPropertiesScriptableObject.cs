using UnityEngine;

namespace Perigon.Weapons
{
    public interface IBulletProperties
    {
        public float Damage { get; }
        public float Speed { get; }
        public float MaxTravelDistance { get; }
        public float BulletHoleTimeToLive { get; }
    }

    [CreateAssetMenu(fileName = "BulletProperties", menuName = "PerigonGames/Bullet", order = 2)]
    public class BulletPropertiesScriptableObject : ScriptableObject, IBulletProperties
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _maxDistance = Mathf.Infinity;
        [SerializeField] private float _bulletHoleTimeToLive = 10f;

        public float Damage => _damage;
        public float Speed => _speed;
        public float MaxTravelDistance => _maxDistance;
        public float BulletHoleTimeToLive => _bulletHoleTimeToLive;
    }
}
