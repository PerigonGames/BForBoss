using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public class InputSettingsViewModel
    {
        private readonly IInputSettings _settings = null;
        private readonly IPerigonAnalytics _perigonAnalytics = null;

        public float GetMouseHorizontal => _settings.MouseHorizontalSensitivity;
        public float GetMouseVertical => _settings.MouseVerticalSensitivity;
        public float GetControllerHorizontal => _settings.ControllerHorizontalSensitivity;
        public float GetControllerVeritcal => _settings.ControllerVerticalSensitivity;
        public bool GetIsInverted => _settings.IsInverted;
        
        public InputSettingsViewModel(IInputSettings settings, IPerigonAnalytics analytics = null)
        {
            _perigonAnalytics = analytics ?? PerigonAnalytics.Instance;
            _settings = settings;
            SetControlSettings();
        }

        public void RevertSettings()
        {
            _settings.RevertAllSettings();
            SetControlSettings();
        }

        public void ApplySettings(float horizontalMouse, float verticalMouse, float horizontalController, float verticalController, bool isInverted)
        {
            _settings.MouseHorizontalSensitivity = horizontalMouse;
            _settings.MouseVerticalSensitivity = verticalMouse;
            _settings.ControllerHorizontalSensitivity = horizontalController;
            _settings.ControllerVerticalSensitivity = verticalController;
            _settings.IsInverted = isInverted;

            SetControlSettings();
        }

        private void SetControlSettings()
        {
            _perigonAnalytics.SetControlSettings(
                GetMouseHorizontal,
                GetMouseVertical,
                GetControllerHorizontal,
                GetControllerVeritcal,
                GetIsInverted);
        }
    }
}
