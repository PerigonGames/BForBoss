using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class MouseKeyboardInputSettingsViewModel: BaseInputSettingsViewModel
    {
        public override float GetHorizontal => _settings.MouseHorizontalSensitivity;
        public override float GetVertical => _settings.MouseVerticalSensitivity;
        
        public MouseKeyboardInputSettingsViewModel(IInputSettings settings, IPerigonAnalytics analytics = null) : base(settings, analytics)
        {
        }

        public override void ApplySettings(float horizontal, float vertical, bool isInverted)
        {
            _settings.MouseHorizontalSensitivity = horizontal / 10;
            _settings.MouseVerticalSensitivity = vertical / 10;
            _settings.IsInverted = isInverted;
            SetInputSettingsAnalytics();
        }
    }
}
