using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public struct DerekHealthViewState
    {
        public float HealthPercentage;
        public bool IsInvulnerable;

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
            _dynamicView.color = MapHealthToColor(state.HealthPercentage);
        }

        private Color MapHealthToColor(float health)
        {
            if (health <= 1.0)
            {
                return Color.green;
            }

            if (health <= 0.66f)
            {
                return Color.yellow;
            }

            if (health <= 0.33f)
            {
                return Color.red;
            }

            return Color.white;
        }
    }
}
