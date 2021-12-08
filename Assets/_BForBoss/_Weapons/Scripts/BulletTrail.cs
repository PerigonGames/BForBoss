using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    public class BulletTrail : MonoBehaviour
    {
        private TrailRenderer _trail;
        private BulletBehaviour _bullet;
        
        void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _bullet = GetComponentInParent<BulletBehaviour>();
        }

        private void StartTrails()
        {
            _trail.emitting = true;
        }
        
        private void StopTrails()
        {
            _trail.emitting = false;
            _trail.Clear();
        }

        private void OnEnable()
        {
            _bullet.OnBulletSpawn += StartTrails;
            _bullet.OnBulletDeactivate += StopTrails;
        }
        
        private void OnDisable()
        {
            _bullet.OnBulletSpawn -= StartTrails;
            _bullet.OnBulletDeactivate -= StopTrails;
        }
    }
}
