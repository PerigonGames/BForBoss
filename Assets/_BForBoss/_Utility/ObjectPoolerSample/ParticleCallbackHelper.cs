using Perigon.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleCallbackHelper : MonoBehaviour
    {
        private ParticleSystem system;
        public ObjectPooler<ParticleSystem> pool;

        void Start()
        {
            system = GetComponent<ParticleSystem>();
            var main = system.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            pool.Reclaim(system);
        }
    }
}
