using System;
using Perigon.Character;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BForBoss
{
    public abstract class BaseWorldManager : MonoBehaviour
    {
        protected readonly StateManager _stateManager = StateManager.Instance;

        [Title("Base Component")]
        [SerializeField] protected PlayerBehaviour _playerBehaviour = null;

        [SerializeField] private InputActionAsset _actionAsset;
        
        private IInputConfiguration _inputConfiguration = null;
        private PGInputSystem _inputSystem;
        private EnvironmentManager _environmentManager = null;

        private WeaponSceneManager _weaponSceneManager = null;
        private UserInterfaceManager _userInterfaceManager = null;

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
            if (_environmentManager != null)
            {
                _environmentManager.CleanUp();
            }
        }

        protected virtual void Reset()
        {
            _stateManager.SetState(State.Play);
            _playerBehaviour.SpawnAt(SpawnLocation, SpawnLookDirection);
            if (_environmentManager != null)
            {
                _environmentManager.Reset();
            }
            VisualEffectsManager.Instance.Reset();
        }

        protected virtual void Awake()
        {
            _inputSystem = new PGInputSystem(_actionAsset);
            _stateManager.OnStateChanged += HandleStateChange;
            _inputConfiguration = new InputConfiguration();
            _environmentManager = gameObject.AddComponent<EnvironmentManager>();
            _environmentManager.Initialize();
            SceneManager.LoadScene("AdditiveWeaponManager", LoadSceneMode.Additive);
            SceneManager.LoadScene("AdditiveUserInterfaceScene", LoadSceneMode.Additive);
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            SceneManager.LoadScene("AdditiveDebugScene", LoadSceneMode.Additive);
#endif
        }
        
        protected virtual void Update()
        {
            if (Keyboard.current.escapeKey.isPressed)
            {
                _stateManager.SetState(State.Pause);
            }
        }

        protected virtual void Start()
        {
            SetupSubManagers();
            WeaponSceneManager.Initialize(_playerBehaviour, _inputSystem);
            UserInterfaceManager.Initialize(_inputConfiguration);
            _stateManager.SetState(State.PreGame);
        }

        private void SetupSubManagers()
        {
            _playerBehaviour.Initialize(_inputConfiguration as InputConfiguration, onDeath: () =>
            {
                StateManager.Instance.SetState(State.Death);
            });
        }

        protected virtual void OnDestroy()
        {
            _stateManager.OnStateChanged -= HandleStateChange;
        }

        protected virtual void OnValidate()
        {
            if (_playerBehaviour == null)
            {
                PanicHelper.Panic(new Exception("_playerBehaviour is missing from World Manager"));
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
            _playerBehaviour.SpawnAt(SpawnLocation, SpawnLookDirection);
            _stateManager.SetState(State.Play);
        }
    }
}
