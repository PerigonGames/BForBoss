using Perigon.Utility;
using UnityEngine;

namespace Perigon.Character
{
    public interface IThirdPerson
    {
        void SetThirdPersonActive(bool isActive);
        bool IsThirdPerson { get; }
    }
    public partial class FirstPersonPlayer : IThirdPerson
    {
        private const int DEFAULT_IS_THIRD_PERSON = 0;
        
        public void SetThirdPersonActive(bool isActive)
        {
            var isThirdPerson = isActive ? 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKeys.ThirdPerson.IS_THIRD_PERSON, isThirdPerson);
            IsThirdPerson = isActive;
        }
        
        private void SetupThirdPerson()
        {
            IsThirdPerson = PlayerPrefs.GetInt(PlayerPrefKeys.ThirdPerson.IS_THIRD_PERSON, DEFAULT_IS_THIRD_PERSON) == 1;
            ToggleThirdPerson();
        }
        
        private void ToggleThirdPerson()
        {
            cmCrouchedCamera.gameObject.SetActive(!IsThirdPerson && IsCrouching());
            cmWalkingCamera.gameObject.SetActive(!IsThirdPerson && !IsCrouching());
            cmThirdPersonCamera.gameObject.SetActive(IsThirdPerson);
            TogglePlayerModel();
        }
    }
}