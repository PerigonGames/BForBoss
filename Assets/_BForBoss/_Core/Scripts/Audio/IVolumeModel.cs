using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    public interface IVolumeModel
    {
        void SetVolume(float volume);
        float GetVolume();
    }
}
