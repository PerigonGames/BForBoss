using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class DerekWorldManager : BaseWorldManager
    {
        [SerializeField] private DerekContextManager _derekContextManager;
        private CountdownViewBehaviour _countdownTimer;

        protected override Vector3 SpawnLocation => _spawnLocation.position;
        protected override Quaternion SpawnLookDirection => _spawnLocation.rotation;

        protected override void Reset()
        {
            base.Reset();
            _derekContextManager.Reset();
        }

        protected override void Start()
        {
            base.Start();
            _derekContextManager.Initialize(_playerBehaviour.PlayerMovement);
            StateManager.Instance.SetState(State.PreGame);
            TutorialViewsManager.Instance.Show(TutorialState.FirstTutorial);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.PanicIfNullObject(_derekContextManager, nameof(_derekContextManager));
        }
    }
}
