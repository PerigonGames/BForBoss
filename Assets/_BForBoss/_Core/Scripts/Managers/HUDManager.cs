using System;
using Perigon.Entities;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class HUDManager : MonoBehaviour
    {
        [Resolve] [SerializeField] private PlayerHealthViewBehaviour _playerHealthViewBehaviour;
        [Resolve] [SerializeField] private EnergySystemViewBehaviour _energySystemViewBehaviour;
        public IEnergyDataSubject EnergyDataSubject;
        
        public void Initialize(ILifeCycle playerLifeCycle)
        {
            StateManager.Instance.OnStateChanged += HandleOnStateChanged;
            _playerHealthViewBehaviour.Initialize(playerLifeCycle);
            _energySystemViewBehaviour.Initialize(EnergyDataSubject);
        }

        private void HandleOnStateChanged(State gameState)
        {
            //TODO - Do we want to keep the HUD on in end game?
            //I kinda like it, but not super strong feeling
            //_playerHealthViewBehaviour.gameObject.SetActive(gameState != State.EndGame);
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
