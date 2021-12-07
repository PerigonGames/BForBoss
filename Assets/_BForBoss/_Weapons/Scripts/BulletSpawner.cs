using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Perigon.Weapons
{

    public enum BulletTypes
    {
        RifleNoPhysics,
        RiflePhysics
    }
    
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private BulletBehaviour[] _bulletPrefabs;
        [InlineEditor]
        [SerializeField] private BulletPropertiesScriptableObject _bulletPropertiesScriptableObject;
        // placeholder for more robust bullet switching
        [SerializeField] private bool _usePhysics = false;
        
        private ObjectPooler<BulletBehaviour>[] _pools = null;

        public IBullet SpawnBullet()
        {
            if(_pools == null) 
                SetupPools();
            var bulletType = _usePhysics ? BulletTypes.RiflePhysics : BulletTypes.RifleNoPhysics;
            return _pools[(int)bulletType].Get();
        }

        private void SetupPools()
        {
            _pools = new ObjectPooler<BulletBehaviour>[_bulletPrefabs.Length];
            for (int i = 0; i < _bulletPrefabs.Length; i++)
            {
                var prefab = _bulletPrefabs[i];
                var index = i;
                _pools[i] = new ObjectPooler<BulletBehaviour>($"{prefab.name}Pool", () => GenerateBullet(prefab,index),
                    (bullet) => bullet.gameObject.SetActive(true),
                    (bullet) => bullet.gameObject.SetActive(false));
            }
        }

        private BulletBehaviour GenerateBullet(BulletBehaviour prefab, int poolIndex)
        {
            var newBullet = Instantiate(prefab);
            newBullet.Initialize(_bulletPropertiesScriptableObject, _pools[poolIndex]);
            return newBullet;
        }
    }
}
