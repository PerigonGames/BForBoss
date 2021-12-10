using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.AI
{
    public abstract class LifeCycleBehaviour : MonoBehaviour
    {
        [SerializeField] private HealthScriptableObject _health = null;

        protected LifeCycle _lifeCycle;

        protected virtual void Awake()
        {
            _lifeCycle = new LifeCycle(_health);
        }

        protected abstract void LifeCycleFinished();

        protected virtual void OnEnable()
        {
            _lifeCycle.OnDeath += LifeCycleFinished;
        }

        protected virtual void OnDisable()
        {
            _lifeCycle.OnDeath -= LifeCycleFinished;
        }
    }
}
