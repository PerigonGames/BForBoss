using System;
using System.Collections.Generic;
using System.Linq;
using PerigonGames;

namespace BForBoss.Labor
{
    public class LaborSystem : IDisposable
    {
        private Queue<ILabor> _laborsToComplete;
        public ILabor CurrentLabor { get; private set; }

        public bool IsComplete { get; private set; } = false;
        public bool IsPaused { get; private set; } = false;

        public LaborSystem(IEnumerable<ILabor> labors, bool randomize = false, bool autoStart = false)
        {
            if (randomize)
            {
                var randomList = labors.ToList();
                randomList.ShuffleFisherYates();
                _laborsToComplete = new Queue<ILabor>(randomList);
            }
            else
                _laborsToComplete = new Queue<ILabor>(labors);
            if(autoStart)
                    SetNextLaborActive();
            else IsPaused = true;
        }

        public void Start()
        {
            if(IsComplete) return;

            if (CurrentLabor == null)
                SetNextLaborActive();
            else if(IsPaused)
                CurrentLabor.Activate();
        }
        
        public void Dispose()
        {
            if (CurrentLabor != null)
            {
                CurrentLabor.OnLaborCompleted -= OnLaborCompleted;
            }
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
            IsPaused = false;
        }

        private void OnLaborCompleted(bool success)
        {
            if (success)
            {
                if (_laborsToComplete.Count > 0)
                    SetNextLaborActive();
                else
                    OnQueueCompleted();
            }
            else
            {
                CurrentLabor.Reset();
                IsPaused = true;
            }
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
    }
}
