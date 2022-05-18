using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    public interface IMeleeProperties
    {
        float Damage { get; }
        float AttackCoolDown { get; }

        int OverlapCube(Vector3 position, Quaternion rotation, Collider[] buffer);
    }
    
    [CreateAssetMenu(fileName = "MeleeProperties", menuName = "PerigonGames/Melee", order = 1)]
    public class MeleeScriptableObject : ScriptableObject, IMeleeProperties
    {
        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _coolDown = 0.5f;
        
        [field: SerializeField] public float Range { get; set; } = 1f;
        [field: SerializeField] public float Height { get; set; } = 1f;
        [field: SerializeField] public float Width { get; set; } = 1f;
        
        public float Damage => _damage;
        public float AttackCoolDown => _coolDown;

        public Vector3 HalfExtents => new Vector3(Width, Height, Range) * 0.5f;

        public int OverlapCube(Vector3 position, Quaternion rotation, Collider[] buffer)
        {
            return Physics.OverlapBoxNonAlloc(GetColliderCenter(position), HalfExtents, buffer, rotation);
        }

        public Vector3 GetColliderCenter(Vector3 position)
        {
            var center = position;
            center.y += Height * 0.5f;
            center.z += Range * 0.5f;
            return center;
        }
    }
}
