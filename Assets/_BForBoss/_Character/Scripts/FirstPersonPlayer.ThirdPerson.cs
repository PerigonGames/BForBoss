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
        private const int Default_Is_Third_Person = 0;
        
        public void SetThirdPersonActive(bool isActive)
        {
            var isThirdPerson = isActive ? 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKeys.ThirdPerson.IsThirdPerson, isThirdPerson);
            IsThirdPerson = isActive;
        }
        
        private void SetupThirdPerson()
        {
            IsThirdPerson = PlayerPrefs.GetInt(PlayerPrefKeys.ThirdPerson.IsThirdPerson, Default_Is_Third_Person) == 1;
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