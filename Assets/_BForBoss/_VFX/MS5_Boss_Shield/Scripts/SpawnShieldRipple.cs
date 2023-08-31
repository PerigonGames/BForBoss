using FMODUnity;
using Perigon.Weapons;
using UnityEngine;
using UnityEngine.VFX;

namespace BForBoss
{
    public class SpawnShieldRipple : MonoBehaviour, IBulletCollision
    {
        [SerializeField]
        private GameObject shieldRipples;

        [SerializeField] private EventReference _rippleSFX;

        private VisualEffect _shieldRipplesVFX;

        void IBulletCollision.OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            if(!_rippleSFX.IsNull) RuntimeManager.PlayOneShot(_rippleSFX, collisionPoint);
            GameObject ripples = Instantiate(shieldRipples, transform);
            _shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            _shieldRipplesVFX.SetVector3("SphereCenter", collisionPoint);
            Destroy(ripples, 2);
        }
    }
}