using Perigon.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BForBoss
{
    public class SpawnParticles : MonoBehaviour
    {
        [SerializeField] ParticleSystem prefabToSpawn;
        [SerializeField] InputAction clickAction;
        private Camera cam;

        private ObjectPooler<ParticleSystem> pool;

        void Awake()
        {
            pool = new ObjectPooler<ParticleSystem>("ParticlePool", CreateNew, GetParticleSystem, RemoveParticleSystem);
            cam = Camera.main;
        }

        private ParticleSystem CreateNew()
        {
            var system = Instantiate(prefabToSpawn);
            var callbackHelper = system.gameObject.AddComponent<ParticleCallbackHelper>();
            callbackHelper.pool = pool;
            return system;

        }

        private void GetParticleSystem(ParticleSystem system)
        {
            system.gameObject.SetActive(true);
        }

        private void RemoveParticleSystem(ParticleSystem system)
        {
            system.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            clickAction.Enable();
            clickAction.performed += ClickAction_performed;
        }

        private void OnDisable()
        {
            clickAction.Disable();
            clickAction.performed -= ClickAction_performed;
        }

        private void ClickAction_performed(InputAction.CallbackContext obj)
        {
            Debug.Log("Clicked");
            var pos = Mouse.current.position.ReadValue();
            if (cam == null) return;

            var ray = cam.ScreenPointToRay(pos);
            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                var newSystem = pool.Get();
                newSystem.transform.position = hitInfo.point;
                newSystem.Play();
            }
        }
    }
}
