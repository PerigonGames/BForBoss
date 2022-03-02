using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Perigon
{
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
