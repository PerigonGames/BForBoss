using System;
using System.Collections.Generic;
using System.Linq;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class PatrolBehaviour : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _patrolPosition = new List<GameObject>();
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _waitTimeOnArrival = 3;
        
        private Patrol _patrol = null;

        public void CleanUp()
        {
            _patrol.CleanUp();
        }

        public void Reset()
        {
            _patrol.Reset();
        }

        private void Awake()
        {
            if (_patrolPosition.IsNullOrEmpty())
            {
                PanicHelper.Panic(new Exception("List of patrol position is empty or null for Patrol Behaviour"));
            }
            _patrol = new Patrol(_patrolPosition.Select(x => x.transform.position).ToArray(), _speed, _waitTimeOnArrival);    
            transform.position = _patrolPosition[0].transform.position;
        }

        private void FixedUpdate()
        {
            transform.position = _patrol.MoveTowards(transform.position, Time.deltaTime);
        }
    }
    
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
                Debug.Log("waiting");
                UpdateWaitTime(time);
                return position;
            }
            
            var nextPosition = Vector3.MoveTowards(position, _currentDestination, _speed * time);
            if (nextPosition == _currentDestination)
            {
                Debug.Log("Got next destination: " + _currentDestination);
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
                Debug.Log("Finished Waiting");
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
