using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [CreateAssetMenu(fileName = "Energy Configuration Data", menuName = "PerigonGames/EnergySystem/ConfigurationData", order = 2)]
    public class EnergySystemConfigurationSO : ScriptableObject
    {
        [FoldoutGroup("Accruement properties")] 
        [SerializeField]
        private float _wallRunEnergy = 1f;
        [FoldoutGroup("Accruement properties")]
        [SerializeField] 
        private float _dashEnergy = 1f;
        [FoldoutGroup("Accruement properties")]
        [SerializeField] 
        private float _slideEnergy = 1f;
        
        [FoldoutGroup("Expense properties")] 
        [SerializeField]
        private float _shotEnergy = 1f;
        [FoldoutGroup("Expense properties")] 
        [SerializeField]
        private float _slowMoEnergy =1f;

        public EnergySystemConfigurationData MapToData()
        {
            return new EnergySystemConfigurationData(
                wallRunEnergy:_wallRunEnergy,
                dashEnergy:_dashEnergy,
                slideEnergy:_slideEnergy,
                shotEnergy:_shotEnergy,
                slowMoEnergy:_slowMoEnergy);
        }
    }
}
