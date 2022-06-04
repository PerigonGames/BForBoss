using System;
using Perigon.Character;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine.SceneManagement;
#endif

namespace BForBoss
{
    public abstract class BaseWorldManager : MonoBehaviour
    {
        protected readonly StateManager _stateManager = StateManager.Instance;

        [Title("Base Component")] 
        [SerializeField] protected FirstPersonPlayer _player = null;

        [Title("Base User Interface")] 
        [SerializeField] protected PauseMenu _pauseMenu;

        private ICharacterSpawn _character = null;
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
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)            
            SceneManager.LoadScene("AdditiveDebugScene", LoadSceneMode.Additive);
#endif
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
                PanicHelper.Panic(new Exception("FirstPersonPlayer is missing from World Manager"));
            }
            
            if (_pauseMenu == null)
            {
                PanicHelper.Panic(new Exception("PauseMenu is missing from World Manager"));
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
