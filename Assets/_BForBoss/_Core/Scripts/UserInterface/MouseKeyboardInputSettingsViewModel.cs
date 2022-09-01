using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class MouseKeyboardInputSettingsViewModel: BaseInputSettingsViewModel
    {
        public override float GetHorizontal => Configuration.MouseHorizontalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        public override float GetVertical => Configuration.MouseVerticalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        
        public MouseKeyboardInputSettingsViewModel(IInputConfiguration configuration, IBForBossAnalytics analytics = null) : base(configuration, analytics)
        {
        }

        public override void ApplySettings(float horizontal, float vertical, bool isInverted)
        {
            Configuration.MouseHorizontalSensitivity = horizontal / MAPPED_SENSITIVITY_MULTIPLIER;
            Configuration.MouseVerticalSensitivity = vertical / MAPPED_SENSITIVITY_MULTIPLIER;
            Configuration.IsInverted = isInverted;
            SetInputSettingsAnalytics();
        }
    }
}
