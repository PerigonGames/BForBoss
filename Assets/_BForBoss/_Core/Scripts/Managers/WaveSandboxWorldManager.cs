using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WaveSandboxWorldManager : BaseWorldManager
    {
        [Title("Waves Configuration")]
        [SerializeField, Tooltip("Cooldown time between waves")]
        private float _secondsBetweenWaves = 2.5f;
        [SerializeField, Tooltip("Number of enemies per wave is the number of enemies from the previous wave multiplied by this multiplier")]
        private float _enemyAmountMultiplier = 1.2f;
        [SerializeField, Tooltip("Number of enemies for the first wave")]
        private int _initialNumberOfEnemies = 10;

        [Title("Component")]
        [SerializeField] private EnemyContainer _enemyContainer;
        [SerializeField] private EnemySpawnersManager _enemySpawnersManager;

        [Title("HUD")]
        [SerializeField] private WaveViewBehaviour _waveView;
        [SerializeField] private TimerViewBehaviour _timerView;
        private WaveModel _waveModel;
        private WaveManager _waveManager;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;

        protected override void Reset()
        {
            base.Reset();
            _waveView.Reset();
            _waveModel.Reset();
            _enemyContainer.Reset();
            _enemySpawnersManager.Reset();
            _waveManager.Reset();
            _timerView.Reset();
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _enemyContainer.CleanUp();
            _enemySpawnersManager.CleanUp();
        }

        protected override void Awake()
        {
            base.Awake();
            _waveManager = gameObject.AddComponent<WaveManager>();
            _waveModel = new WaveModel(_initialNumberOfEnemies);
        }

        protected override void Start()
        {
            base.Start();
            _waveView.Initialize(_waveModel);
            _enemyContainer.Initialize(() => _playerBehaviour.PlayerMovement.camera.transform.position);
            _enemySpawnersManager.Initialize(_enemyContainer, _waveModel);
            _waveManager.Initialize(_waveModel, _enemySpawnersManager, _secondsBetweenWaves, _enemyAmountMultiplier);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_enemyContainer == null)
            {
                PanicHelper.Panic(new Exception("Enemy Container missing from WaveSandboxWorldManager"));
            }

            if (_enemySpawnersManager == null)
            {
                PanicHelper.Panic(new Exception("Enemy Spawner Manager missing from the world manager"));
            }

            if (_waveView == null)
            {
                PanicHelper.Panic(new Exception("Wave View UI missing from the world Manager"));
            }

            if (_timerView == null)
            {
                PanicHelper.Panic(new Exception("TimerViewUI is missing from the worldManager"));
            }
        }

        protected override void HandleStatePause()
        {
            base.HandleStatePause();
            _timerView.PauseTimer();
        }

        protected override void HandleStatePlay()
        {
            base.HandleStatePlay();
            _timerView.StartTimer();
        }

        protected override void HandleOnEndGame()
        {
            base.HandleOnEndGame();
            _timerView.PauseTimer();
        }
    }
}
