namespace Perigon.Utility
{
    public interface IInputConfiguration
    {
        bool IsInverted { get; set; }
        float MouseHorizontalSensitivity { get; set; }
        float MouseVerticalSensitivity { get; set; }
        float ControllerHorizontalSensitivity { get; set; }
        float ControllerVerticalSensitivity { get; set; }
        void RevertAllSettings();
    }
}
