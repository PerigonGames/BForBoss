using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public struct DerekHealthViewState
    {
        public readonly float HealthPercentage;
        public readonly bool IsInvulnerable;
        public readonly Color HealthBarColor;

        public DerekHealthViewState(
            float healthPercentage, 
            bool isInvulnerable,
            Color healthBarColor)
        {
            HealthPercentage = healthPercentage;
            IsInvulnerable = isInvulnerable;
            HealthBarColor = healthBarColor;
        }

        public DerekHealthViewState Apply(float? healthPercentage = null, bool? isInvulnerable = null, Color? healthBarColor = null)
        {
            return new DerekHealthViewState(healthPercentage ?? HealthPercentage
                , isInvulnerable ?? IsInvulnerable, healthBarColor ?? HealthBarColor);
        }
    }
    public class DerekHealthView : MonoBehaviour
    {
        [SerializeField] private Image _invincibleView;
        [SerializeField] private Image _dynamicView;

        public void SetState(DerekHealthViewState state)
        {
            _invincibleView.gameObject.SetActive(state.IsInvulnerable);
            _dynamicView.fillAmount = state.HealthPercentage;
            _dynamicView.color = state.HealthBarColor;
        }
    }
}