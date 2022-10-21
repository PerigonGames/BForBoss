using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class ItemPickupBehaviour : MonoBehaviour
    {
        [Resolve][SerializeField] private GameObject _itemToPickUp = null;
        [SerializeField] private float _respawnTime = 10f;

        protected bool _isSpawned = true;
        
        private float _elapsedRespawnTime = 0;
        private BoxCollider _boxCollider = null;

        public void Reset()
        {
            IsShown(_isSpawned);
        }

        public void CleanUp()
        {
            _isSpawned = true;
            _elapsedRespawnTime = 0;
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

        protected virtual void Update()
        {
            if (_isSpawned)
            {
                return;
            }

            _elapsedRespawnTime -= Time.deltaTime;
            if (_elapsedRespawnTime <= 0)
            {
                IsShown(true);
                _elapsedRespawnTime = _respawnTime;
            }
        }

        protected void IsShown(bool show)
        {
            _isSpawned = show;
            _itemToPickUp.SetActive(show);
            _boxCollider.enabled = show;
        }

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }
    }
}
