using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    public interface ISpawnerControl
    {
        void PauseSpawning();
        void ResumeSpawning();
    }
}
