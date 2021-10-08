using ECM2.Characters;
using UnityEngine;

namespace BForBoss
{
    public class FirstPersonPlayer : FirstPersonCharacter
    {

        [Header("Cinemachine")]
        public GameObject cmWalkingCamera;
        public GameObject cmCrouchedCamera;

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
    }
}
