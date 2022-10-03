using UnityEngine;

namespace Perigon.Entities
{
    [CreateAssetMenu(fileName = "HealthProperties", menuName = "PerigonGames/Health", order = 1)]
    public class HealthScriptableObject : ScriptableObject
    {
        [SerializeField] private float _amount = 100f;

        public float Amount => _amount;
    }
}
