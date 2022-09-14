using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class MouseKeyboardInputSettingsViewModel: BaseInputSettingsViewModel
    {
        public override float GetHorizontal => _settings.MouseHorizontalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        public override float GetVertical => _settings.MouseVerticalSensitivity * MAPPED_SENSITIVITY_MULTIPLIER;
        
        public MouseKeyboardInputSettingsViewModel(IInputSettings settings, IBForBossAnalytics analytics = null) : base(settings, analytics)
        {
        }

        public override void ApplySettings(float horizontal, float vertical, bool isInverted)
        {
            _settings.MouseHorizontalSensitivity = horizontal / MAPPED_SENSITIVITY_MULTIPLIER;
            _settings.MouseVerticalSensitivity = vertical / MAPPED_SENSITIVITY_MULTIPLIER;
            _settings.IsInverted = isInverted;
            SetInputSettingsAnalytics();
        }
    }
}
