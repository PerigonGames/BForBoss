using System;
using System.Collections.Generic;
using System.Linq;
using ECM2.Components;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class PatrolBehaviour : PlatformMovement
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
            transform.position = _patrolPosition[0].transform.position;
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

        protected override void OnMove()
        {
            position = _patrol.MoveTowards(transform.position, Time.deltaTime);
        }
    }
}
