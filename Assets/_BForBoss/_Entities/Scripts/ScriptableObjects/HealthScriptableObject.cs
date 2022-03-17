using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Entities
{

    public interface IHealth
    {
        public float Health { get; }
    }
    
    [CreateAssetMenu(fileName = "HealthProperties", menuName = "PerigonGames/Health", order = 1)]
    public class HealthScriptableObject : ScriptableObject, IHealth
    {
        [SerializeField] private float _health = 100f;

        public float Health => _health;
    }
}
