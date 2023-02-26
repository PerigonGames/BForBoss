using UnityEngine;

namespace BForBoss
{
    [CreateAssetMenu(fileName = "Energy Data", menuName = "PerigonGames/EnergySystem/EnergyData", order = 1)]
    public class EnergySO : ScriptableObject
    {
        [SerializeField, Tooltip("Player's starting value of Energy"), Min(0f)] 
        private float _startingEnergyValue = 0f;

        [SerializeField, Tooltip("Max Energy Attainable")]
        private float _maxEnergyValue = 100f;

        [SerializeField, Tooltip("How often does the player gain/lose energy per second when in the middle of a transaction")]
        private float _rateOfTransaction = 1f;

        public EnergyData MapToData()
        {
            return new EnergyData(
                value:_startingEnergyValue,
                maxEnergyValue: _maxEnergyValue,
                rateOfTransaction: _rateOfTransaction);
        }
    }
}
