using Perigon.Utility;

namespace Tests
{
    public class MockInputSettings : IInputSettings
    {
        public int CalledRevertAllSettings = 0;
        public int CalledEnableActions = 0;
        public int CalledDisableActions = 0;
        
        public bool IsInverted { get; set; }
        public float MouseHorizontalSensitivity { get; set; }
        public float MouseVerticalSensitivity { get; set; }
        public float ControllerHorizontalSensitivity { get; set; }
        public float ControllerVerticalSensitivity { get; set; }
        public void RevertAllSettings()
        {
            CalledRevertAllSettings++;
        }

        public void DisableActions()
        {
            CalledDisableActions++;
        }

        public void EnableActions()
        {
            CalledEnableActions++;
        }
    }
}
