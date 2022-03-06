using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BForBoss
{
    public class VCAVolumeModel : IVolumeModel
    {
        private const string VCA_PREFIX = "vca:/";
        private VCA _vca;

        private string _name;
        
        public VCAVolumeModel(string name)
        {
            _name = name;
            _vca = RuntimeManager.GetVCA(VCA_PREFIX + name);
        }

        public void SetVolume(float volume)
        {
            if (!_vca.isValid())
            {
                Debug.LogWarning(_name + " is not a valid VCA");
                return;
            }
            _vca.setVolume(volume);
        }

        public float GetVolume()
        {
            if (_vca.isValid() && _vca.getVolume(out float volume) == RESULT.OK)
            {
                return volume;
            }
            Debug.LogWarning(_name + " is not a valid VCA");
            return 0f;
        }
    }
}
