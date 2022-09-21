using System;
using DG.Tweening;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BForBoss
{
    public abstract class BaseWorldManager : MonoBehaviour
    {
        private const string ADDITIVE_WEAPON_SCENE_NAME = "AdditiveWeaponManager";
        private const string ADDITIVE_USER_INTERFACE_SCENE_NAME = "AdditiveUserInterfaceScene";
        private const string ADDITIVE_DEBUG_SCENE_NAME = "AdditiveDebugScene";
        
        protected readonly StateManager _stateManager = StateManager.Instance;

        [Title("Base Component")]
        [SerializeField] protected PlayerBehaviour _playerBehaviour = null;

        [SerializeField] private InputActionAsset _actionAsset;

        private PGInputSystem _inputSystem;
        private EnvironmentManager _environmentManager;

        private WeaponSceneManager _weaponSceneManager;
        private UserInterfaceManager _userInterfaceManager;

        private UserInterfaceManager UserInterfaceManager
        {
            get
            {
                if (_userInterfaceManager == null)
                {
                    _userInterfaceManager = FindObjectOfType<UserInterfaceManager>();
                }

                return _userInterfaceManager;
            }
        }

        protected WeaponSceneManager WeaponSceneManager
        {
            get
            {
                if (_weaponSceneManager == null)
                {
                    _weaponSceneManager = FindObjectOfType<WeaponSceneManager>();
                }

                return _weaponSceneManager;
            }
        }

        protected abstract Vector3 SpawnLocation { get; }
        protected abstract Quaternion SpawnLookDirection { get; }

        protected virtual void CleanUp()
        {
            DOTween.KillAll();
            if (_environmentManager != null)
            {
                _environmentManager.CleanUp();
            }
        }

        protected virtual void Reset()
        {
            _playerBehaviour.Reset();
            _playerBehaviour.SpawnAt(SpawnLocation, SpawnLookDirection);
            if (_environmentManager != null)
            {
                _environmentManager.Reset();
            }
            VisualEffectsManager.Instance.Reset();
            _stateManager.SetState(State.Play);
        }

        protected virtual void Awake()
        {
            SceneManager.sceneLoaded += OnAdditiveSceneLoaded;
            _inputSystem = new PGInputSystem(_actionAsset);
            _inputSystem.OnPausePressed += HandlePausePressed;
            _stateManager.OnStateChanged += HandleStateChange;
            _environmentManager = gameObject.AddComponent<EnvironmentManager>();
            SceneManager.LoadScene(ADDITIVE_WEAPON_SCENE_NAME, LoadSceneMode.Additive);
            SceneManager.LoadScene(ADDITIVE_USER_INTERFACE_SCENE_NAME, LoadSceneMode.Additive);
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            SceneManager.LoadScene(ADDITIVE_DEBUG_SCENE_NAME, LoadSceneMode.Additive);
#endif
        }

        protected virtual void Start()
        {
            _playerBehaviour.Initialize(_inputSystem);            
            _environmentManager.Initialize();
            _stateManager.SetState(State.PreGame);
        }
        
        protected virtual void OnDestroy()
        {
            _stateManager.OnStateChanged -= HandleStateChange;
            _inputSystem.OnPausePressed -= HandlePausePressed;
            SceneManager.sceneLoaded -= OnAdditiveSceneLoaded;
        }

        protected virtual void OnValidate()
        {
            if (_playerBehaviour == null)
            {
                PanicHelper.Panic(new Exception("_playerBehaviour is missing from World Manager"));
            }
        }
        
        private void OnAdditiveSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.name)
            {
                case ADDITIVE_USER_INTERFACE_SCENE_NAME:
                    UserInterfaceManager.Initialize();
                    break;
                case ADDITIVE_WEAPON_SCENE_NAME:
                    WeaponSceneManager.Initialize(_playerBehaviour, _inputSystem);
                    break; 
                default:
                    return;
            }
        }
        
        private void HandlePausePressed()
        {
            if (_stateManager.GetState() == State.Play)
            {
                _stateManager.SetState(State.Pause);
            }
            else if(_stateManager.GetState() == State.Pause)
            {
                _stateManager.SetState(State.Play);
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
                case State.Debug:
                case State.Pause:
                {
                    HandleStatePause();
                    break;
                }
                case State.Death:
                {
                    HandleOnDeath();
                    break;
                }
                case State.EndGame:
                    HandleOnEndGame();
                    break;
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
            _inputSystem.SetToPlayerControls();
        }

        protected virtual void HandleStatePause()
        {
            Time.timeScale = 0.0f;
            _inputSystem.SetToUIControls();
        }

        protected virtual void HandleOnDeath()
        {
            _inputSystem.SetToUIControls();
        }

        protected virtual void HandleOnEndGame()
        {
        }
    }
}
