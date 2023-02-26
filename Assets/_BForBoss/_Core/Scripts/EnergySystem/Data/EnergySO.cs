using UnityEngine;

namespace BForBoss
{
    [CreateAssetMenu(fileName = "Energy Data", menuName = "PerigonGames/EnergySystem/EnergyData", order = 1)]
    public class EnergySO : ScriptableObject
    {
        [SerializeField, Tooltip("Player's current value of Energy"), Min(0f)] 
        private float _value = 0f;

        [SerializeField, Tooltip("Max Energy Attainable")]
        private float _maxEnergyValue = 100f;

        [SerializeField, Tooltip("How often does the player gain energy per second when in the middle of accruement")]
        private float _rateOfAccruement = 1f;

        public EnergyData MapToData()
        {
            return new EnergyData(
                value:_value,
                maxEnergyValue: _maxEnergyValue,
                rateOfAccruement: _rateOfAccruement);
        }
    }
}
