using Perigon.Utility;

namespace Tests
{
    public class MockInputConfiguration : IInputConfiguration
    {
        public int CalledRevertAllSettings = 0;
        
        public bool IsInverted { get; set; }
        public float MouseHorizontalSensitivity { get; set; }
        public float MouseVerticalSensitivity { get; set; }
        public float ControllerHorizontalSensitivity { get; set; }
        public float ControllerVerticalSensitivity { get; set; }
        public void RevertAllSettings()
        {
            CalledRevertAllSettings++;
        }

        public void SwapToUIActions()
        {
        }

        public void SwapToPlayerActions()
        {
        }
    }
}
