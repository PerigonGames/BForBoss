using System.Collections.Generic;
using System.Linq;
using PerigonGames;
using UnityEngine;
using Logger = Perigon.Utility.Logger;

namespace BForBoss.Labor
{
    public class LaborSystem
    {        
        //TODO
        // Make sure when reset, it stops timer
        // make sure to have delay when reset same grouped ring
        private Queue<ILabor> _laborsToComplete;
        public ILabor CurrentLabor { get; private set; }

        public bool IsComplete { get; private set; }

        public LaborSystem(
            IEnumerable<ILabor> labors, 
            bool randomize = false)
        {
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
        }

        public void Activate()
        {
            if (IsComplete)
            {
                Logger.LogWarning("Labour System is called to be activated, even when it's completed");
                return;
            }

            if (CurrentLabor == null)
            {
                SetNextLaborActive();
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
            Logger.LogString("Group Ring started", key: "Labor");
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
                CurrentLabor.Activate();
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
