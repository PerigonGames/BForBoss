using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class ControllerInputSettingsViewModel: BaseInputSettingsViewModel
    {
        public override float GetHorizontal => Configuration.ControllerHorizontalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        public override float GetVertical => Configuration.ControllerVerticalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        
        public ControllerInputSettingsViewModel(IInputConfiguration configuration, IBForBossAnalytics analytics = null) : base(configuration, analytics)
        {
        }

        public override void ApplySettings(float horizontal, float vertical, bool isInverted)
        {
            Configuration.ControllerHorizontalSensitivity = horizontal / MAPPED_SENSITIVITY_MULTIPLIER;
            Configuration.ControllerVerticalSensitivity = vertical / MAPPED_SENSITIVITY_MULTIPLIER;
            Configuration.IsInverted = isInverted;
            SetInputSettingsAnalytics();
        }
    }
}
