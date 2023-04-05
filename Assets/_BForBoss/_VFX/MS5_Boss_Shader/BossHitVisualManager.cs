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
        private float AnimationSpeed = 1.0f;
        
        private Material bossMat;
        private float currentTime = 1.0f;
        
        void Start()
        {
            bossMat = GetComponent<Renderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            currentTime += Time.deltaTime * AnimationSpeed;
            if (currentTime > 1.0f){
                currentTime = 1.0f;
            }
            
            bossMat.SetFloat("_AnimationTime", currentTime);
        }
        
        
        void IBulletCollision.OnCollided(Vector3 collisionPoint, Vector3 collisionNormal)
        {
            
            currentTime = 0.0f;
            
            GameObject debris = Instantiate(bossDebris, transform);
            _bossDebrisVFX = debris.GetComponent<VisualEffect>();
            _bossDebrisVFX.SetVector3("normal", collisionNormal);
            _bossDebrisVFX.SetVector3("position", collisionPoint);
            Destroy(debris, 1);
        }
    }
}