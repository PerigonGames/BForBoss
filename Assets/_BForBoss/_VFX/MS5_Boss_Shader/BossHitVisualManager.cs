using Perigon.Weapons;
using UnityEngine;
using UnityEngine.VFX;

namespace BForBoss
{
    public class BossHitVisualManager : MonoBehaviour, IBulletCollision
    {
        [SerializeField]
        private GameObject bossDebris;
        private VisualEffect _bossDebrisVFX;
        
        [SerializeField]
        private float animationSpeed = 1.0f;
        
        private Material bossMat;
        private float currentAnimationTime = 1.0f;
        
        void Start()
        {
            bossMat = GetComponent<Renderer>().material;
        }

        
        void Update()
        {
            currentAnimationTime += Time.deltaTime * animationSpeed;
            if (currentAnimationTime > 1.0f){
                currentAnimationTime = 1.0f;
            }
            
            bossMat.SetFloat("_AnimationTime", currentAnimationTime);
        }
        
        
        void IBulletCollision.OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            currentAnimationTime = 0.0f;
            
            GameObject debris = Instantiate(bossDebris, transform);
            _bossDebrisVFX = debris.GetComponent<VisualEffect>();
            _bossDebrisVFX.SetVector3("normal", collisionNormal);
            _bossDebrisVFX.SetVector3("position", collisionPoint);
            Destroy(debris, 1);
        }
        
    }
}