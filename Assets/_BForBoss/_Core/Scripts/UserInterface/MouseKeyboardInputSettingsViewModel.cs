using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class MouseKeyboardInputSettingsViewModel: BaseInputSettingsViewModel
    {
        public override float GetHorizontal => _configuration.MouseHorizontalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        public override float GetVertical => _configuration.MouseVerticalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        
        public MouseKeyboardInputSettingsViewModel(IInputConfiguration configuration = null, IBForBossAnalytics analytics = null) : base(configuration, analytics)
        {
        }

        public override void ApplySettings(float horizontal, float vertical, bool isInverted)
        {
            _configuration.MouseHorizontalSensitivity = horizontal / MAPPED_SENSITIVITY_MULTIPLIER;
            _configuration.MouseVerticalSensitivity = vertical / MAPPED_SENSITIVITY_MULTIPLIER;
            _configuration.IsInverted = isInverted;
            SetInputSettingsAnalytics();
        }
    }
}
