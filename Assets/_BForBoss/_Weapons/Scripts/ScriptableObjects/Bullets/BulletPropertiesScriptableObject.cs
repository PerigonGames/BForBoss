using UnityEngine;

namespace Perigon.Weapons
{
    public interface IBulletProperties
    {
        public float Damage { get; }
        public float Speed { get; }
        public float MaxDistance { get; }
        public float BulletHoleTimeToLive { get; }
    }

    [CreateAssetMenu(fileName = "BulletProperties", menuName = "PerigonGames/Bullet", order = 2)]
    public class BulletPropertiesScriptableObject : ScriptableObject, IBulletProperties
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _maxDistance = Mathf.Infinity;
        [SerializeField] private float _bulletHoleTimeToLive = 10f;

        float IBulletProperties.Damage => _damage;
        float IBulletProperties.Speed => _speed;
        float IBulletProperties.MaxDistance => _maxDistance;
        float IBulletProperties.BulletHoleTimeToLive => _bulletHoleTimeToLive;
    }
}
