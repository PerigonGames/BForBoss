using System;

namespace BForBoss.Labor
{
    public interface ILabor
    {
        public event Action OnLaborCompleted;

        public void Activate();
    }
}
