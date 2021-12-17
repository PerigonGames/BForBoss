using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public abstract class BaseInputSettingsViewModel
    {        
        private readonly IPerigonAnalytics _perigonAnalytics = null;
        protected readonly IInputSettings _settings = null;
        
        public abstract float GetHorizontal { get; }
        public abstract float GetVertical { get; }
        
        public bool GetIsInverted => _settings.IsInverted;

        protected BaseInputSettingsViewModel(IInputSettings settings, IPerigonAnalytics analytics = null)
        {
            _perigonAnalytics = analytics ?? PerigonAnalytics.Instance;
            _settings = settings;
            SetInputSettingsAnalytics();
        }

        public abstract void ApplySettings(float horizontal, float vertical, bool isInverted);

        public void RevertSettings()
        {
            _settings.RevertAllSettings();
            SetInputSettingsAnalytics();
        }
        
        protected void SetInputSettingsAnalytics()
        {
            _perigonAnalytics.SetMouseKeyboardSettings(
                GetHorizontal,
                GetVertical,
                GetIsInverted);
        }
    }
}
