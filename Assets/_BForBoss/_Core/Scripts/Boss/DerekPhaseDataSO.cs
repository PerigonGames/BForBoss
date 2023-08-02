using BForBoss.RingSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BForBoss
{
    [CreateAssetMenu(fileName = "Phase Data", menuName = "PerigonGames/Boss/Derek/PhaseData", order = 1)]
    public class DerekPhaseDataSO : SerializedScriptableObject
    {
        //Health
        [Header("Health")]
        [SerializeField /*,PropertyRange(0, "_healthThresholdMaximum")*/ ,MinValue(0.0f), MaxValue(1.0f), Tooltip("The percentage of health where Derek will increment its phase")] 
        private float _healthThreshold;
        [SerializeField, MinValue(0.0f), Tooltip("How long should Derek's vulnerability last this phase")] private float _vulnerabilityTimer = 1.0f;
        
        //Movement
        [Header("Movement")]
        [SerializeField, Tooltip("How fast should Derek Rotate")] private float _rotationRate;
        
        //Missile Information
        [Header("Missiles")]
        [SerializeField, Min(0.1f), Tooltip("How long should Derek wait before firing its next shot")] private float _intervalBetweenShots;
        //private DerekMissileBehaviour.MovementProfile _missileMovementProfile;
        [SerializeField, Min(1.0f), Tooltip("The Speed of the missile after it has reached its apex and begins trailing the player")] private float _missileSpeed;
        
        //Labor Information
        [Header("Labors")]
        [OdinSerialize, Tooltip("The Labors to initialize for this phase")] private RingLaborManager.RingGrouping[] _ringLaborSystems;

        // private float _healthThresholdMaximum = 1.0f;
        // public void SetHealthThresholdMaximum(float newThresholdMaximum)
        // {
        //     _healthPercentageThreshold = newThresholdMaximum;
        // }


        public float HealthThreshold => _healthThreshold;
        public float VulnerabilityTimer => _vulnerabilityTimer;
        
        public float RotationRate => _rotationRate;
        
        public float IntervalBetweenShots => _intervalBetweenShots;
        public float MissileSpeed => _missileSpeed;

        public RingLaborManager.RingGrouping[] RingLaborSystems => _ringLaborSystems;
    }
}
