using System;
using System.Collections.Generic;
using BForBoss.Labor;
using PerigonGames;

namespace BForBoss.RingSystem
{
    public class NestedRingSystem : ILabor
    {
        public event Action<bool> OnLaborCompleted;
        private RingSystem _currentSystem;
        private Queue<RingSystem> _queue;
        private readonly RingSystem[] _allSystems;

        private readonly bool _isRandomized;

        private readonly float _time;

        public NestedRingSystem(RingSystem[] systems, bool isRandomized = false, float time = 0f)
        {
            _allSystems = systems;
            _isRandomized = isRandomized;
            _time = time;
            SetupSystems(_allSystems);

            foreach (var system in _allSystems)
            {
                system.Reset();
            }
        }

        public void Reset()
        {
            foreach (var system in _allSystems)
            {
                system.Reset();
                system.OnLaborCompleted -= OnSystemCompleted;
            }

            _currentSystem = null;
            SetupSystems(_allSystems);
        }

        public void Activate()
        {
            TrySetupNextSystem();
            
            if (_time > 0f)
            {
                CountdownTimer.Instance.StartCountdown(_time, CountdownFinish);
            }
        }

        private void TrySetupNextSystem()
        {
            if (_queue.Count == 0)
            {
                InvokeLaborCompleted(true);
                return;
            }
            _currentSystem = _queue.Dequeue();
            _currentSystem.Activate();
            _currentSystem.OnLaborCompleted += OnSystemCompleted;
        }

        private void OnSystemCompleted(bool success)
        {
            _currentSystem.OnLaborCompleted -= OnSystemCompleted;
            if(success)
                TrySetupNextSystem();
            else 
                InvokeLaborCompleted(false);
        }

        private void InvokeLaborCompleted(bool success)
        {
            if(_time > 0f)
                CountdownTimer.Instance.StopCountdown();
            OnLaborCompleted?.Invoke(success);
        }
        
        private void SetupSystems(IList<RingSystem> systems)
        {
            if (_isRandomized)
            {
                systems.ShuffleFisherYates();
            }
            _queue = new Queue<RingSystem>(systems);
        }

        private void CountdownFinish()
        {
            Perigon.Utility.Logger.LogString("Ran out of time! Resetting the labor", key: "Labor");
            Reset();
            OnLaborCompleted?.Invoke(false);
        }
    }
}