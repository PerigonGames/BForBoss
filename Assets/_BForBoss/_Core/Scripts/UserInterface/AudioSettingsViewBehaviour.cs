using Perigon.UserInterface;
using UnityEngine;
using UnityEngine.UI;
using BForBoss.Audio;

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
            _mainVolumeSlider.SliderValue = _audioSettingsModel.MainVolume * MULTIPLIER_VALUE;
            _musicVolumeSlider.SliderValue = _audioSettingsModel.MusicVolume * MULTIPLIER_VALUE;
            _sfxVolumeSlider.SliderValue = _audioSettingsModel.SFXVolume * MULTIPLIER_VALUE;
            _applyButton.interactable = false;
        }

        private void BindUI()
        {
            _mainVolumeSlider.OnValueChangedAction -= AllowApplying;
            _musicVolumeSlider.OnValueChangedAction -= AllowApplying;
            _sfxVolumeSlider.OnValueChangedAction -= AllowApplying;
            _applyButton.onClick.RemoveAllListeners();
            _revertButton.onClick.RemoveAllListeners();
            
            _applyButton.onClick.AddListener(ApplyValues);
            _revertButton.onClick.AddListener(() =>
            {
                _audioSettingsModel.RevertToDefault();
                SetUI();
            });
            _mainVolumeSlider.OnValueChangedAction += AllowApplying;
            _musicVolumeSlider.OnValueChangedAction += AllowApplying;
            _sfxVolumeSlider.OnValueChangedAction += AllowApplying;
        }

        private void AllowApplying()
        {
            _applyButton.interactable = true;
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
