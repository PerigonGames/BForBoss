using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
     public class Patrol
    {
        private enum PatrolState
        {
            Moving,
            Waiting
        }
        
        private readonly Queue<Vector3> _queueOfLocations = new Queue<Vector3>();
        private readonly float _speed = 0;
        private readonly float _waitTimeOnArrival = 0;
        private readonly Vector3[] _arrayOfLocations;

        private float _elapsedWaitTime = 0;
        private PatrolState _state = PatrolState.Moving;
        private Vector3 _currentDestination = Vector3.zero;

        private bool NeedToWait => _elapsedWaitTime < _waitTimeOnArrival; 

        public Patrol(Vector3[] patrolLocations, 
            float speed = 0f,
            float waitTimeOnArrival = 0f)
        {
            _speed = speed;
            _waitTimeOnArrival = waitTimeOnArrival;
            _arrayOfLocations = patrolLocations;
            SetupLocations();
        }

        public Vector3 MoveTowards(Vector3 position, float time)
        {
            if (_state == PatrolState.Waiting && NeedToWait)
            {
                UpdateWaitTime(time);
                return position;
            }
            
            var nextPosition = Vector3.MoveTowards(position, _currentDestination, _speed * time);
            if (nextPosition == _currentDestination)
            {
                _state = PatrolState.Waiting;
                GetNextDestination();
            }

            return nextPosition;
        }

        private void UpdateWaitTime(float time)
        {
            _elapsedWaitTime += time;
            if (_elapsedWaitTime > _waitTimeOnArrival)
            {
                _elapsedWaitTime = 0;
                _state = PatrolState.Moving;
            }
        }

        public void CleanUp()
        {
            _currentDestination = Vector3.zero;
            _queueOfLocations.Clear();
        }

        public void Reset()
        {
            SetupLocations();
        }

        private void SetupLocations()
        {
            _queueOfLocations.Clear();
            foreach (var location in _arrayOfLocations)
            {
                _queueOfLocations.Enqueue(location);
            }

            GetNextDestination();
        }

        private void GetNextDestination()
        {
            if (_queueOfLocations.Count > 0)
            {
                _currentDestination = _queueOfLocations.Dequeue();
                _queueOfLocations.Enqueue(_currentDestination);
            }
        }
    }
}
