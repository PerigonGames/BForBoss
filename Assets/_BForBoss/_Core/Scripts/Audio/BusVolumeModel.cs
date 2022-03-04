using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace BForBoss
{
    public class BusVolumeModel : IVolumeModel
    {
        private const string BUS_PREFIX = "bus:/";
        private Bus _bus;
        
        public BusVolumeModel(string name)
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
}
