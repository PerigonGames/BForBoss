using System.Collections.Generic;
using System.Linq;
using Perigon.Utility;
using PerigonGames;

namespace BForBoss.Labor
{
    public class LaborSystem
    {        
        //TODO
        // Make sure when failing, resets same Grouped Ring
        // Make sure when reset, it stops timer
        // make sure to have delay when reset same grouped ring
        private readonly float PenaltyDelayedStart;

        private Queue<ILabor> _laborsToComplete;
        public ILabor CurrentLabor { get; private set; }

        public bool IsComplete { get; private set; }
        private bool _laborFailed;

        public LaborSystem(
            IEnumerable<ILabor> labors, 
            float penaltyDelayedStart = 0, 
            bool randomize = false)
        {
            PenaltyDelayedStart = penaltyDelayedStart;
            if (randomize)
            {
                var randomList = labors.ToList();
                randomList.ShuffleFisherYates();
                _laborsToComplete = new Queue<ILabor>(randomList);
            }
            else
            {
                _laborsToComplete = new Queue<ILabor>(labors);
            }

            _laborFailed = true;
        }

        public void Activate()
        {
            if (IsComplete)
            {
                return;
            }

            if (CurrentLabor == null)
            {
                SetNextLaborActive();
            }
            else if (_laborFailed)
            {
                CurrentLabor.Activate();
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
            _laborFailed = false;
            Logger.LogString("Group Ring Completed, next group started", key: "Labor");
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
                _laborFailed = true;
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
