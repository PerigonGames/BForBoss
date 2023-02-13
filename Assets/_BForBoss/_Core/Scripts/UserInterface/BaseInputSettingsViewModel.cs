using Perigon.Analytics;
using Perigon.Utility;

namespace BForBoss
{
    public abstract class BaseInputSettingsViewModel
    {        
        /// <summary>
        /// ECM2's sensitivity normally goes through 0.01 -> 2.0
        /// It looks too small and sensitive, so multiplying by 10 to go through 0.1 -> 25 
        /// </summary>
        protected const float MAPPED_SENSITIVITY_MULTIPLIER = 100f;
        private readonly IBForBossAnalytics _analytics = null;
        protected readonly IInputConfiguration _configuration = null;
        
        public abstract float GetHorizontal { get; }
        public abstract float GetVertical { get; }
        
        public bool GetIsInverted => _configuration.IsInverted;

        protected BaseInputSettingsViewModel(IInputConfiguration configuration = null, IBForBossAnalytics analytics = null)
        {
            _analytics = analytics ?? BForBossAnalytics.Instance;
            _configuration = configuration ?? new InputConfiguration();
            SetInputSettingsAnalytics();
        }

        public abstract void ApplySettings(float horizontal, float vertical, bool isInverted);

        public void RevertSettings()
        {
            _configuration.RevertAllSettings();
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
