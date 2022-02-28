using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Perigon.UserInterface
{
    public class VolumeSliderBehaviour : MonoBehaviour
    {
        private const float REMAP_VALUE = 50f;
        private const string BUS_PREFIX = "bus:/";
        private const string VCA_PREFIX = "vca:/";
        
        [SerializeField] private string _volumeName;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private VolumeType _volumeType = VolumeType.VCA;
        
        private Slider _customSlider = null;
        private TMP_InputField _inputField = null;

        private VolumeWrapper _volume;

        private Slider CustomSlider
        {
            get
            {
                if (_customSlider == null)
                {
                    _customSlider = GetComponentInChildren<Slider>();
                }

                return _customSlider;
            }
        }

        private TMP_InputField CustomInputField
        {
            get
            {
                if (_inputField == null)
                {
                    _inputField = GetComponentInChildren<TMP_InputField>();
                }

                return _inputField;
            }
        }

        private float SliderValue
        {
            get
            {
                return CustomSlider.value / REMAP_VALUE;
            }
            set
            {
                CustomSlider.value = value * REMAP_VALUE;
                SetVolume(value);
            }
        }

        private void Awake()
        {
            CustomSlider.onValueChanged.AddListener(HandleOnSliderValueChanged);
            CustomInputField.onEndEdit.AddListener(HandleOnInputFieldEnded);

            if (_titleText != null)
            {
                _titleText.text = _volumeName;
            }

            switch (_volumeType)
            {
                case VolumeType.VCA:
                    _volume = new VCAVolume(_volumeName);
                    break;
                case VolumeType.Bus:
                    _volume = new BusVolume(_volumeName);
                    break;
                case VolumeType.MasterBus:
                    _volume = new MasterVolume();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SliderValue = GetVolume();

        }

        private float GetVolume()
        {
            if (_volume.GetVolume(out float volume))
            {
                return volume;
            }
            return -1;
        }

        private void SetVolume(float volume)
        {
            _volume.SetVolume(volume);
        }

        private void OnDestroy()
        {
            CustomSlider.onValueChanged.RemoveListener(HandleOnSliderValueChanged);
            CustomInputField.onEndEdit.RemoveListener(HandleOnInputFieldEnded);
        }

        private void HandleOnSliderValueChanged(float value)
        {
            CustomInputField.text = value.ToString("F0");
            SetVolume(SliderValue);
        }

        private void HandleOnInputFieldEnded(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                CustomInputField.text = CustomSlider.value.ToString("F0");
            }
            else
            {
                CustomSlider.value = float.Parse(value);
                SetVolume(SliderValue);
            }
        }
        
        private enum VolumeType
        {
            VCA, Bus, MasterBus
        }
    }

    public interface VolumeWrapper
    {
        void SetVolume(float volume);
        bool GetVolume(out float volume);
    }

    public class VCAVolume : VolumeWrapper
    {
        private const string VCA_PREFIX = "vca:/";
        private VCA _vca;
        
        public VCAVolume(string name)
        {
            _vca = RuntimeManager.GetVCA(VCA_PREFIX + name);
        }

        public void SetVolume(float volume)
        {
            if (!_vca.isValid())
                return;
            _vca.setVolume(volume);
        }

        public bool GetVolume(out float volume)
        {
            if (_vca.isValid() && _vca.getVolume(out float _, out float finalVolume) == RESULT.OK)
            {
                volume = finalVolume;
                return true;
            }
            volume = 0f;
            return false;
        }
    }
    
    public class BusVolume : VolumeWrapper
    {
        private const string BUS_PREFIX = "bus:/";
        private Bus _bus;
        
        public BusVolume(string name)
        {
            _bus = RuntimeManager.GetBus(BUS_PREFIX + name);
        }

        public void SetVolume(float volume)
        {
            if (!_bus.isValid())
                return;
            _bus.setVolume(volume);
        }

        public bool GetVolume(out float volume)
        {
            if (_bus.isValid() && _bus.getVolume(out float _, out float finalVolume) == RESULT.OK)
            {
                volume = finalVolume;
                return true;
            }
            volume = 0f;
            return false;
        }
    }
    
    public class MasterVolume : BusVolume
    {
        public MasterVolume() : base("")
        {
            
        }
    }
}
