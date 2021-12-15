using Perigon.Character;
using Perigon.Utility;
using Perigon.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class GunRangeWorldManager : MonoBehaviour
    {
        [Title("Component")] 
        [SerializeField] private FirstPersonPlayer _player = null;
        [SerializeField] private EquipmentBehaviour _equipmentBehaviour = null;
        [SerializeField] private AmmunitionCountViewBehaviour _ammunitionCountView = null;
        [SerializeField] private PauseMenu _pauseMenu = null;
        private FreezeActionsUtility _freezeActionsUtility = null;
        private readonly StateManager _stateManager = StateManager.Instance;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [Title("Debug")]
        [SerializeField] private GameObject _debugCanvas;
#endif
        
        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            if (FindObjectOfType<DebugWindow>() == null)
            {
                Instantiate(_debugCanvas).gameObject.GetComponent<DebugWindow>();
            }
#endif
            _stateManager.OnStateChanged += HandleStateChange;
        }

        private void Start()
        {
            _player.Initialize();
            _equipmentBehaviour.Initialize();
            _ammunitionCountView.Initialize(_equipmentBehaviour);
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
