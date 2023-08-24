using Sirenix.OdinInspector;
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
        [ColorPalette]
        [SerializeField] private Color _firstPhaseColor = Color.green;
        [ColorPalette]
        [SerializeField] private Color _secondPhaseColor = Color.yellow;
        [ColorPalette]
        [SerializeField] private Color _thirdPhaseColor = Color.red;
        
        public void SetState(DerekHealthViewState state)
        {
            _invincibleView.gameObject.SetActive(state.IsInvulnerable);
            _dynamicView.fillAmount = state.HealthPercentage;
            _dynamicView.color = MapHealthToColor(state.HealthPercentage);
        }

        private Color MapHealthToColor(float health)
        {
            if (health > 0.66)
            {
                return _firstPhaseColor;
            }

            if (health > 0.33f)
            {
                return _secondPhaseColor;
            }

            return _thirdPhaseColor;
        }
    }
}
