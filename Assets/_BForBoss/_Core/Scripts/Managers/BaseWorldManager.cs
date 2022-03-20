using System;
using System.Collections;
using Perigon.Character;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public abstract class BaseWorldManager : MonoBehaviour
    {
        [Title("Base Component")] 
        [SerializeField] protected FirstPersonPlayer _player = null;

        [Title("Base User Interface")] 
        [SerializeField] protected PauseMenu _pauseMenu;

        private ICharacterSpawn _character = null;
        protected readonly StateManager _stateManager = StateManager.Instance;
        protected FreezeActionsUtility _freezeActionsUtility = null;

        protected abstract Vector3 SpawnLocation { get; }
        protected abstract Quaternion SpawnLookDirection { get; }

        protected virtual void CleanUp()
        {
            Debug.Log("Cleaning Up");
        }
        
        protected virtual void Reset()
        {
            _stateManager.SetState(State.Play);
            _character.SpawnAt(SpawnLocation, SpawnLookDirection);
        }

        protected virtual void Awake()
        {
            _stateManager.OnStateChanged += HandleStateChange;
            _character = _player;
        }

        protected virtual void Start()
        {
            SetupSubManagers();
            _pauseMenu.Initialize(_player, _freezeActionsUtility);
            _stateManager.SetState(State.PreGame);
        }
        
        private void SetupSubManagers()
        {
            _player.Initialize();
            _freezeActionsUtility = new FreezeActionsUtility(_player);
        }

        protected virtual void OnDestroy()
        {
            _stateManager.OnStateChanged -= HandleStateChange;
        }

        protected virtual void OnValidate()
        {
            if (_player == null)
            {
                Debug.LogError("FirstPersonPlayer is missing from World Manager");
            }
            
            if (_pauseMenu == null)
            {
                Debug.LogError("PauseMenu is missing from World Manager");
            }
        }
        
        private void HandleStateChange(State newState)
        {
            switch (newState)
            {
                case State.PreGame:
                {
                    HandleStatePreGame();
                    break;
                }
                case State.Play:
                {
                    HandleStatePlay();
                    break;
                }
                case State.Pause:
                {
                    HandleStatePause();
                    break;
                }
                case State.EndRace:
                {
                    HandleOnEndOfRace();
                    break;
                }
                case State.Death:
                {
                    HandleOnDeath();
                    break;
                }
            }
        }

        protected virtual void HandleStatePreGame()
        {
            CleanUp();
            Reset();
        }

        protected virtual void HandleStatePlay()
        {
            Time.timeScale = 1.0f;
        }
        
        protected virtual void HandleStatePause()
        {
            Time.timeScale = 0.0f;
        }

        protected virtual void HandleOnEndOfRace()
        {
            
        }

        protected virtual void HandleOnDeath()
        {
            _character.SpawnAt(SpawnLocation, SpawnLookDirection);
            _stateManager.SetState(State.Play);
        }
    }
}
