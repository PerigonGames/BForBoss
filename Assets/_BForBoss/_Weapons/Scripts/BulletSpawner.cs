using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private BulletBehaviour _bulletPrefab;
        [InlineEditor]
        [SerializeField] private BulletPropertiesScriptableObject _bulletPropertiesScriptableObject;

        private ObjectPooler<BulletBehaviour> _pool = null;

        public IBullet SpawnBullet()
        {
            _pool ??= new ObjectPooler<BulletBehaviour>($"{_bulletPrefab.name}Pool", GenerateBullet,
                (bullet) => bullet.gameObject.SetActive(true),
                (bullet) => bullet.gameObject.SetActive(false));
            return _pool.Get();
        }

        private BulletBehaviour GenerateBullet()
        {
            var newBullet = Instantiate(_bulletPrefab);
            newBullet.Initialize(_bulletPropertiesScriptableObject, _pool);
            return newBullet;
        }
    }
}
