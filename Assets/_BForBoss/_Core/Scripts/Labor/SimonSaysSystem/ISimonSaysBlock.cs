using System;
using UnityEngine;

namespace BForBoss
{
    public interface ISimonSaysBlock
    {
        event Action<ISimonSaysBlock, SimonSaysColor> OnBlockCompleted;
        void Reset();
        void SetColorData(SimonSaysColorData colorData);
    }
}
