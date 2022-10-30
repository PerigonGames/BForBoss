using UnityEngine;

namespace BForBoss
{
    public class ItemTimerRespawnBehaviour : ItemRespawnBehaviour
    {
        [SerializeField] private float _respawnTime = 10f;
        private float _elapsedRespawnTime = 0;
        
        public override void Reset()
        {
            _elapsedRespawnTime = _respawnTime;
            base.Reset();
        }

        private void Update()
        {
            if (_canRespawn)
            {
                return;
            }
            
            _elapsedRespawnTime -= Time.deltaTime;
            if (_elapsedRespawnTime <= 0)
            {
                _canRespawn = true;
            }
        }
    }
}
