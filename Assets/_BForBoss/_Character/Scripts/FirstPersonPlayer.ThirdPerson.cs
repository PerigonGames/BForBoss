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
        private const string PLAYER_MODEL_LAYER = "PlayerModel";
        private const string FIRST_PERSON_WEAPON_LAYER = "FirstPersonWeapon";
        private const int DEFAULT_IS_THIRD_PERSON = 0;
        
        private int _thirdPersonMask;

        private int FirstPersonMask => ~(1 << LayerMask.NameToLayer(PLAYER_MODEL_LAYER) | 1 << LayerMask.NameToLayer(FIRST_PERSON_WEAPON_LAYER));

        void IThirdPerson.SetThirdPersonActive(bool isActive)
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.ThirdPerson.IS_THIRD_PERSON, isActive ? 1 : 0);
            SetIsThirdPerson(isActive);
        }
        
        private void SetupThirdPerson()
        {
            SetIsThirdPerson(PlayerPrefs.GetInt(PlayerPrefKeys.ThirdPerson.IS_THIRD_PERSON, DEFAULT_IS_THIRD_PERSON) == 1);
            ToggleThirdPerson();
        }
        
        private void ToggleThirdPerson()
        {
            IThirdPerson thirdPerson = this;
            bool isThirdPerson = thirdPerson.IsThirdPerson;
            cmCrouchedCamera.gameObject.SetActive(!isThirdPerson && IsCrouching());
            cmWalkingCamera.gameObject.SetActive(!isThirdPerson && !IsCrouching());
            cmThirdPersonCamera.gameObject.SetActive(isThirdPerson);
            TogglePlayerModel();
        }
        
        private void TogglePlayerModel()
        {
            IThirdPerson thirdPerson = this;
            bool isThirdPerson = thirdPerson.IsThirdPerson;
            
            animate = isThirdPerson;
            camera.cullingMask = isThirdPerson ? _thirdPersonMask : FirstPersonMask;
        }

        private void SetIsThirdPerson(bool isActive)
        {
            if (_isThirdPerson == isActive)
            {
                return;
            }

            _isThirdPerson = isActive;
            ToggleThirdPerson();
        }
    }
}