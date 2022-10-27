using BForBoss;
using UnityEngine;

namespace Perigon.Entities
{
    public class HealingItemBehaviour : ItemPickupBehaviour
    {
        [SerializeField] private float _healAmount = 50f;

        public override void CleanUp()
        {
            base.CleanUp();
            _itemRespawnBehaviour.Reset();
        }

        protected override bool DidPickUpItem(Collider other)
        {
            return other.TryGetComponent(out PlayerLifeCycleBehaviour lifeCycle) && !lifeCycle.IsFullHealth;
        }

        protected override void OnPickedUpItem(Collider other)
        {
            if (other.TryGetComponent(out PlayerLifeCycleBehaviour lifeCycle))
            {
                lifeCycle.HealBy(_healAmount);
            }
        }

        private void Update()
        {
            if (!_isSpawned && _itemRespawnBehaviour.CanRespawn)
            {
                IsShown(true);
            }
        }
    }
}
