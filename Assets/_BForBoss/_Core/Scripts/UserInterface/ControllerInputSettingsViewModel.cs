using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class ControllerInputSettingsViewModel: BaseInputSettingsViewModel
    {
        public override float GetHorizontal => _settings.ControllerHorizontalSensitivity;
        public override float GetVertical => _settings.ControllerVerticalSensitivity;
        
        public ControllerInputSettingsViewModel(IInputSettings settings, IBForBossAnalytics analytics = null) : base(settings, analytics)
        {
        }

        public override void ApplySettings(float horizontal, float vertical, bool isInverted)
        {
            _settings.ControllerHorizontalSensitivity = horizontal;
            _settings.ControllerVerticalSensitivity = vertical;
            _settings.IsInverted = isInverted;
            SetInputSettingsAnalytics();
        }
    }
}
