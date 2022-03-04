using FMODUnity;
using Perigon.UserInterface;
using UnityEngine;

namespace BForBoss
{
    public class AudioSettingsViewBehaviour : MonoBehaviour
    {

        [SerializeField] private SliderBehaviour _mainVolumeSlider;
        [SerializeField] private SliderBehaviour _musicVolumeSlider;
        [SerializeField] private SliderBehaviour _sfxVolumeSlider;
        
        
        public void Initialize()
        {
            
        }
    }
}
