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
        
        private void TogglePlayerModel()
        {
            animate = IsThirdPerson;
            camera.cullingMask = IsThirdPerson ? _thirdPersonMask : FirstPersonMask;
        }
    }
}