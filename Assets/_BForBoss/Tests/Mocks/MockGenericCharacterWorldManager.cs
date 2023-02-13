using BForBoss;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests
{
    public class MockGenericCharacterWorldManager : MonoBehaviour
    {
        public InputActionAsset ActionAsset;
        private PGInputSystem _pgInputSystem;
        
        private void Awake()
        {
            var character = FindObjectOfType<PlayerMovementBehaviour>();
            _pgInputSystem = new PGInputSystem(ActionAsset);
            character.Initialize(_pgInputSystem);
            _pgInputSystem.SetToPlayerControls();
        }

        private void OnDestroy()
        {
            _pgInputSystem.ForceUnbind();
        }
    }
}
