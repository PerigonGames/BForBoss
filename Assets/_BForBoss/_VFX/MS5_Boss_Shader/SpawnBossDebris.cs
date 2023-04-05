using Perigon.Weapons;
using UnityEngine;
using UnityEngine.VFX;

namespace BForBoss
{
    public class SpawnBossDebris : MonoBehaviour, IBulletCollision
    {
        [SerializeField]
        private GameObject bossDebris;

        private VisualEffect _bossDebrisVFX;

        void IBulletCollision.OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            GameObject debris = Instantiate(bossDebris, transform);
            _bossDebrisVFX = debris.GetComponent<VisualEffect>();
            _bossDebrisVFX.SetVector3("normal", collisionNormal);
            _bossDebrisVFX.SetVector3("position", collisionPoint);
            Destroy(debris, 1);
        }
    }
}