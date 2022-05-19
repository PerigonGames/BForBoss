using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IMeleeProperties
    {
        float Damage { get; }
        float AttackCoolDown { get; }

        int OverlapCapsule(Vector3 position, Vector3 forwardDirection, ref Collider[] buffer);
    }
    
    [CreateAssetMenu(fileName = "MeleeProperties", menuName = "PerigonGames/Melee", order = 1)]
    public class MeleeScriptableObject : ScriptableObject, IMeleeProperties
    {
        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _coolDown = 0.5f;
        
        [field: SerializeField] public float Range { get; set; } = 1f;
        [field: SerializeField] public float Height { get; set; } = 2f;

        public float Damage => _damage;
        public float AttackCoolDown => _coolDown;

        public int OverlapCapsule(Vector3 position, Vector3 forwardDirection, ref Collider[] buffer)
        {
            var point1 = position;
            point1 += forwardDirection * 0.5f * Range;
            var point2 = point1;
            point1.y += Height;
            return Physics.OverlapCapsuleNonAlloc(point1, point2, Range, buffer);
        }

        public Vector3 GetColliderCenter(Vector3 position, Vector3 forwardDirection)
        {
            var center = position;
            center += forwardDirection * Range * 0.5f;
            center.y += Height * 0.5f;
            return center;
        }
    }
}
