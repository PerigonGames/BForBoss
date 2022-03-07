using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class ControllerInputSettingsViewModel: BaseInputSettingsViewModel
    {
        public override float GetHorizontal => _settings.ControllerHorizontalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        public override float GetVertical => _settings.ControllerVerticalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        
        public ControllerInputSettingsViewModel(IInputSettings settings, IBForBossAnalytics analytics = null) : base(settings, analytics)
        {
        }

        public override void ApplySettings(float horizontal, float vertical, bool isInverted)
        {
            _settings.ControllerHorizontalSensitivity = horizontal / MAPPED_SENSITIVITY_MULTIPLIER;
            _settings.ControllerVerticalSensitivity = vertical / MAPPED_SENSITIVITY_MULTIPLIER;
            _settings.IsInverted = isInverted;
            SetInputSettingsAnalytics();
        }
    }
}
