using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    public class BulletPropertiesScriptableObject : ScriptableObject
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _maxDistance = Mathf.Infinity;
        [SerializeField] private float _bulletHoleTimeToLive = 10f;

        public float Damage => _damage;
        public float Speed => _speed;
        public float MaxDistance => _maxDistance;
        public float BulletHoleTimeToLive => _bulletHoleTimeToLive;
    }
}
