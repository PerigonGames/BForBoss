using FMODUnity;
using Perigon.UserInterface;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class AudioSettingsViewBehaviour : MonoBehaviour
    {
        private const float MULTIPLIER_VALUE = 100f;
        
        [SerializeField] private SliderBehaviour _mainVolumeSlider;
        [SerializeField] private SliderBehaviour _musicVolumeSlider;
        [SerializeField] private SliderBehaviour _sfxVolumeSlider;

        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _revertButton;

        private AudioSettingsModel _audioSettingsModel = null;

        public void Initialize()
        {
            _audioSettingsModel = new AudioSettingsModel();
            SetUI();
            BindUI();
        }

        private void SetUI()
        {
            Debug.Log(_audioSettingsModel.MusicVolume);
            _mainVolumeSlider.SliderValue = _audioSettingsModel.MainVolume * MULTIPLIER_VALUE;
            _musicVolumeSlider.SliderValue = _audioSettingsModel.MusicVolume * MULTIPLIER_VALUE;
            _sfxVolumeSlider.SliderValue = _audioSettingsModel.SFXVolume * MULTIPLIER_VALUE;
            _applyButton.interactable = false;
        }

        private void BindUI()
        {
            _applyButton.onClick.AddListener(ApplyValues);
            _revertButton.onClick.AddListener(() =>
            {
                _audioSettingsModel.RevertToDefault();
                SetUI();
            });
            _mainVolumeSlider.OnValueChangedAction += () => _applyButton.interactable = true;
            _musicVolumeSlider.OnValueChangedAction += () => _applyButton.interactable = true;
            _sfxVolumeSlider.OnValueChangedAction += () => _applyButton.interactable = true;
        }

        private void ApplyValues()
        {
            _audioSettingsModel.MainVolume = _mainVolumeSlider.SliderValue / MULTIPLIER_VALUE;
            _audioSettingsModel.MusicVolume = _musicVolumeSlider.SliderValue / MULTIPLIER_VALUE;
            _audioSettingsModel.SFXVolume = _sfxVolumeSlider.SliderValue / MULTIPLIER_VALUE;
            _applyButton.interactable = false;
        }
    }
}
