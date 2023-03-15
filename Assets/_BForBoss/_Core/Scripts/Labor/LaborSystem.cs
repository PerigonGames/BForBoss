using System;
using System.Collections.Generic;

namespace BForBoss.Labor
{
    public class LaborSystem
    {
        public Action onLaborCompleted;
        public Action onQueueCompleted;
        
        private Queue<ILabor> _laborsToComplete;
        public ILabor CurrentLabor { get; private set; }

        public void Initialize(IEnumerable<ILabor> labors)
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
            onLaborCompleted?.Invoke();
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
            onQueueCompleted?.Invoke();
        }
    }
}
