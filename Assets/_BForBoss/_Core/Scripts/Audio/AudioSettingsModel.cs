using Perigon.Utility;
using UnityEngine;

namespace BForBoss.Audio
{
    public class AudioSettingsModel
    {
        private const float DEFAULT_VOLUME = 1f;
        private const string MUSIC_BUS_NAME = "Music";
        private const string SFX_BUS_NAME = "SFX";

        private IVolumeModel _masterVolume;
        private IVolumeModel _musicVolume;
        private IVolumeModel _sfxVolume;

        public float MainVolume
        {
            get => _masterVolume.GetVolume();
            set
            {
                _masterVolume.SetVolume(value);
                PlayerPrefs.SetFloat(PlayerPrefKeys.AudioSettings.MAIN_VOLUME, value);
            }
        }
        
        public float MusicVolume
        {
            get => _musicVolume.GetVolume();
            set 
            {
                _musicVolume.SetVolume(value);
                PlayerPrefs.SetFloat(PlayerPrefKeys.AudioSettings.MUSIC_VOLUME, value);
            }
        }

        public float SFXVolume
        {
            get => _sfxVolume.GetVolume();
            set
            {
                _sfxVolume.SetVolume(value);
                PlayerPrefs.SetFloat(PlayerPrefKeys.AudioSettings.SFX_VOLUME, value);
            }
        }
        
        
        public void RevertToDefault()
        {
            MainVolume = DEFAULT_VOLUME;
            MusicVolume = DEFAULT_VOLUME;
            SFXVolume = DEFAULT_VOLUME;
        }

        public AudioSettingsModel()
        {
            _masterVolume = new MasterVolumeModel();
            _sfxVolume = new VCAVolumeModel(SFX_BUS_NAME);
            _musicVolume = new VCAVolumeModel(MUSIC_BUS_NAME);
            SetValues();
        }

        private void SetValues()
        {
            _masterVolume.SetVolume(PlayerPrefs.GetFloat(PlayerPrefKeys.AudioSettings.MAIN_VOLUME, DEFAULT_VOLUME));
            _musicVolume.SetVolume(PlayerPrefs.GetFloat(PlayerPrefKeys.AudioSettings.MUSIC_VOLUME, DEFAULT_VOLUME));
            _sfxVolume.SetVolume(PlayerPrefs.GetFloat(PlayerPrefKeys.AudioSettings.SFX_VOLUME, DEFAULT_VOLUME));
        }
    }
}
