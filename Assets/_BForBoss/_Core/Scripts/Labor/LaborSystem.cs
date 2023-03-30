using System;
using System.Collections.Generic;

namespace BForBoss.Labor
{
    public class LaborSystem : IDisposable
    {
        private Queue<ILabor> _laborsToComplete;
        public ILabor CurrentLabor { get; private set; }

        public bool IsComplete { get; private set; } = false;

        public LaborSystem(IEnumerable<ILabor> labors)
        {
            _laborsToComplete = new Queue<ILabor>(labors);
            SetNextLaborActive();
        }

        private void SetNextLaborActive()
        {
            if (CurrentLabor != null)
            {
                CurrentLabor.OnLaborCompleted -= OnLaborCompleted;
            }

            CurrentLabor = _laborsToComplete.Dequeue();
            CurrentLabor.OnLaborCompleted += OnLaborCompleted;
            CurrentLabor.Activate();

        }

        private void OnLaborCompleted()
        {
            if(_laborsToComplete.Count > 0)
                SetNextLaborActive();
            else
                OnQueueCompleted();
        }

        private void OnQueueCompleted()
        {
            if (CurrentLabor != null)
            {
                CurrentLabor.OnLaborCompleted -= OnLaborCompleted;
            }

            CurrentLabor = null;
            IsComplete = true;
        }

        public void Dispose()
        {
            if (CurrentLabor != null)
            {
                CurrentLabor.OnLaborCompleted -= OnLaborCompleted;
            }
        }
    }
}
