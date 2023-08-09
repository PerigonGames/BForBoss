using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [CreateAssetMenu(fileName = "Phase Data", menuName = "PerigonGames/Boss/Derek/PhaseData", order = 1)]
    public class DerekPhaseDataSO : SerializedScriptableObject
    {
        //Health
        [Header("Health")]
        [SerializeField /*,PropertyRange(0, "_healthThresholdMaximum")*/ ,MinValueAttribute(0.0f), MaxValueAttribute(1.0f), Tooltip("The percentage of health where Derek will increment its phase")] 
        private float _healthThreshold;
        [SerializeField, MinValueAttribute(0.0f), Tooltip("How long should Derek's vulnerability last this phase")] private float _vulnerabilityDuration = 1.0f;
        
        //Movement
        [Header("Movement")]
        [SerializeField, Tooltip("How fast should Derek Rotate"), MinValueAttribute(0.0f)] private float _rotationRate;
        
        //Missile Information
        [Header("Missiles")]
        [SerializeField, MinValueAttribute(0.1f), Tooltip("How long should Derek wait before firing its next shot")] private float _intervalBetweenShots;
        //private DerekMissileBehaviour.MovementProfile _missileMovementProfile;
        [SerializeField, MinValueAttribute(0.1f), Tooltip("Multiplier set on the Autopilot speed of the missile")] private float _missileSpeedMultiplier;
        
        public float HealthThreshold => _healthThreshold;
        public float VulnerabilityDuration => _vulnerabilityDuration;
        
        public float RotationRate => _rotationRate;
        
        public float IntervalBetweenShots => _intervalBetweenShots;
        public float MissileSpeedMultiplier => _missileSpeedMultiplier;
    }
}
