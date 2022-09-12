using Perigon.Character;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests
{
    public class MockGenericCharacterWorldManager : MonoBehaviour
    {
        public InputActionAsset _actionAsset;
        private PGInputSystem _pgInputSystem;
        
        private void Start()
        {
            var character = FindObjectOfType<PlayerMovementBehaviour>();
            _pgInputSystem = new PGInputSystem(_actionAsset);
            character.Initialize(_pgInputSystem);
            _pgInputSystem.SetToPlayerControls();
        }

        private void OnDestroy()
        {
            _pgInputSystem.ForceUnbind();
        }
    }
}
