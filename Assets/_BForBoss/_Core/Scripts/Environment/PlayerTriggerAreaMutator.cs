
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    [Serializable]
    public class PlayerTriggerAreaMutation
    {
        [Title("Repeated Effects", "Continuous effect on Player over time", TitleAlignments.Split)]
        [SerializeField, Min(0.0f), Tooltip("Duration between player getting effected on")]
        private float _secondsBetweenEffect = 1.0f;

        [Title("Health Effect", horizontalLine: false)]
        [SerializeField, Tooltip("The amount to change the player's health by")]
        private int _healthChangeAmount = 0;

        [Title("Immediate Effects", "Immediate effect on Player's stats upon triggering", TitleAlignments.Split)]
        [Title("Speed Effect", horizontalLine: false)]
        [SerializeField, Range(0.0f, 5.0f), Tooltip("modifer to multiply player's speed by")]
        private float _speedMultiplier = 1f;

        public float secondsBetweenEffect => _secondsBetweenEffect;
        public int healthChangeAmount => _healthChangeAmount;

        public float speedMultiplier => _speedMultiplier;
    }
}