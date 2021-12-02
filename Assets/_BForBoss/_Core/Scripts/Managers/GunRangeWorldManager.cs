using System;
using Perigon.Character;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class GunRangeWorldManager : MonoBehaviour
    {
        [Title("Component")] 
        [SerializeField] private FirstPersonPlayer _player = null;
        [SerializeField] private PauseMenu _pauseMenu = null;
        private FreezeActionsUtility _freezeActionsUtility = null;
        private readonly StateManager _stateManager = StateManager.Instance;

        private void Awake()
        {
            _stateManager.OnStateChanged += HandleStateChange;
        }

        private void Start()
        {
            _player.Initialize();
            _freezeActionsUtility = new FreezeActionsUtility(_player);
            _pauseMenu.Initialize(_player, _player, _freezeActionsUtility);
            _stateManager.SetState(State.Play);
        }

        private void HandleStateChange(State newState)
        {
            switch (newState)
            {
                case State.Play:
                {
                    Time.timeScale = 1.0f;
                    break;
                }
                case State.Pause:
                {
                    Time.timeScale = 0.0f;
                    break;
                }
                default:
                    _stateManager.SetState(State.Play);
                    break;
            }
        }
    }
}
