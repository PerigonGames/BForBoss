using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class ItemPickupBehaviour : MonoBehaviour
    {
        [Resolve, SerializeField] protected ItemRespawnBehaviour _itemRespawnBehaviour;
        [Resolve, SerializeField] private GameObject _itemToPickUp = null;

        protected bool _isSpawned = true;
        
        private BoxCollider _boxCollider = null;

        public void Reset()
        {
            IsShown(_isSpawned);
        }

        public virtual void CleanUp()
        {
            _isSpawned = true;
        }

        protected abstract bool DidPickUpItem(Collider other);

        protected abstract void OnPickedUpItem(Collider other);
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (DidPickUpItem(other))
            {
                OnPickedUpItem(other);
                IsShown(false);
            }
        }
        
        protected void IsShown(bool show)
        {
            _isSpawned = show;
            _itemToPickUp.SetActive(show);
            _boxCollider.enabled = show;
            _itemRespawnBehaviour.Reset();
        }

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }
    }
}
