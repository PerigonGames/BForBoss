using UnityEngine;

namespace BForBoss
{
    public interface IThirdPerson
    {
        void SetThirdPersonActive(bool isActive);
        bool IsThirdPerson { get; }
    }
    public partial class FirstPersonPlayer : IThirdPerson
    {
        private partial struct PlayerPrefKey
        {
            public const string IsThirdPerson = "is_third_person";
        }

        private const int Default_Is_Third_Person = 0;

        private void SetupThirdPerson()
        {
            IsThirdPerson = PlayerPrefs.GetInt(PlayerPrefKey.IsThirdPerson, Default_Is_Third_Person) == 1;
            ToggleThirdPerson();
        }
        
        public void SetThirdPersonActive(bool isActive)
        {
            var isThirdPerson = isActive ? 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKey.IsThirdPerson, isThirdPerson);
            IsThirdPerson = isActive;
        }
    }
}