using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public struct DerekHealthViewState
    {
        public readonly float HealthPercentage;
        public readonly bool IsInvulnerable;

        public DerekHealthViewState(
            float healthPercentage, 
            bool isInvulnerable)
        {
            HealthPercentage = healthPercentage;
            IsInvulnerable = isInvulnerable;
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
        }

        public void SetHealthBarColor(Color healthBarColor)
        {
            _dynamicView.color = healthBarColor;
        }
    }
}