using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace BForBoss
{
    public class VCAVolumeModel : IVolumeModel
    {
        private const string VCA_PREFIX = "vca:/";
        private VCA _vca;
        
        public VCAVolumeModel(string name)
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
}
