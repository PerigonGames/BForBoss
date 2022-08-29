using System;
using System.Collections.Generic;
using System.Linq;
using Perigon.Entities;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class EnemyBehaviourManager : MonoBehaviour
    {
        private List<EnemyBehaviour> _enemies = null;
        private Func<Vector3> _getPlayerPosition;
        private BulletSpawner _bulletSpawner;

        public Func<Vector3> GetPlayerPosition => _getPlayerPosition;
        public BulletSpawner BulletSpawner => _bulletSpawner;

        public void Reset()
        {
            if (_enemies == null)
            {
                return;
            }
            
            foreach (EnemyBehaviour enemy in _enemies)
            {
                enemy.Reset();
            }
        }

        public void Initialize(Func<Vector3> getPlayerPosition)
        {
            _getPlayerPosition = getPlayerPosition;
            _bulletSpawner = GetComponent<BulletSpawner>();

            if (_enemies == null)
            {
                return;
            }

            foreach (EnemyBehaviour enemy in _enemies)
            {
                //Todo: Find a way to initialize Enemy based on its Type.
                FloatingTargetBehaviour floatingEnemy = enemy as FloatingTargetBehaviour;

                if (floatingEnemy == null)
                {
                    continue;
                }

                floatingEnemy.Initialize(_getPlayerPosition, _bulletSpawner, null);
            }
        }

        private void Awake()
        {
            _enemies = FindObjectsOfType<EnemyBehaviour>().ToList();
        }
    }
}
