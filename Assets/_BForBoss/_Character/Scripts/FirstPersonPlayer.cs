using ECM2.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class FirstPersonPlayer : FirstPersonCharacter
    {

        [Header("Cinemachine")]
        public GameObject cmWalkingCamera;
        public GameObject cmCrouchedCamera;

        [Title("Optional Behaviour")]
        private PlayerDashBehaviour _dashBehaviour = null;

        protected override void OnAwake()
        {            
            _dashBehaviour = GetComponent<PlayerDashBehaviour>();
            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _dashBehaviour.Initialize(this);
        }

        protected override void SetupPlayerInput()
        {
            base.SetupPlayerInput();
            var dashAction = actions.FindAction("Dash");
            _dashBehaviour.SetupPlayerInput(dashAction);
        }

        protected override void AnimateEye()
        {
            // Base class animates the camera for crouching here, cinemachine handles that
        }

        protected override void OnCrouched()
        {
            base.OnCrouched();
            cmWalkingCamera.SetActive(false);
            cmCrouchedCamera.SetActive(true);
        }

        protected override void OnUncrouched()
        {
            base.OnUncrouched();
            cmCrouchedCamera.SetActive(false);
            cmWalkingCamera.SetActive(true);
        }

        protected override void OnMove()
        {
            base.OnMove();
            _dashBehaviour.OnDashing();
        }

        protected override void HandleInput()
        {
            base.HandleInput();
            _dashBehaviour.HandleInput();
        }
    }
}
