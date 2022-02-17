using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public abstract class BaseInputSettingsViewModel
    {        
        private readonly IBForBossAnalytics _analytics = null;
        protected readonly IInputSettings _settings = null;
        
        public abstract float GetHorizontal { get; }
        public abstract float GetVertical { get; }
        
        public bool GetIsInverted => _settings.IsInverted;

        protected BaseInputSettingsViewModel(IInputSettings settings, IBForBossAnalytics analytics = null)
        {
            _analytics = analytics ?? BForBossAnalytics.Instance;
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
            _analytics.SetMouseKeyboardSettings(
                GetHorizontal,
                GetVertical,
                GetIsInverted);
        }
    }
}
