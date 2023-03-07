using System;
using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private PlayerHealthViewBehaviour _playerHealthViewBehaviour;

        public void Initialize(ILifeCycle playerLifeCycle)
        {
            StateManager.Instance.OnStateChanged += HandleOnStateChanged;
            _playerHealthViewBehaviour.Initialize(playerLifeCycle);
        }

        private void HandleOnStateChanged(State gameState)
        {
            _playerHealthViewBehaviour.gameObject.SetActive(gameState != State.EndGame);
        }

        private void Awake()
        {
            if (_playerHealthViewBehaviour == null)
            {
                PanicHelper.Panic(new Exception($"{nameof(_playerHealthViewBehaviour)} missing from HUDManager"));
            }
        }

        private void OnDestroy()
        {
            StateManager.Instance.OnStateChanged -= HandleOnStateChanged;
        }
    }
}
