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
        private IEnergySystem _mockEnergySystem = new MockEnergySystem();
        
        private void Awake()
        {
            var character = FindObjectOfType<PlayerMovementBehaviour>();
            _pgInputSystem = new PGInputSystem(ActionAsset);
            character.Initialize(_mockEnergySystem, _pgInputSystem);
            _pgInputSystem.SetToPlayerControls();
        }

        private void OnDestroy()
        {
            _pgInputSystem.ForceUnbind();
        }
    }
}
