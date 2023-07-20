using System;

namespace BForBoss.Labor
{
    public interface ILabor
    {
        public event Action<bool> OnLaborCompleted;

        public void Activate();

        public void Reset();
    }
}
