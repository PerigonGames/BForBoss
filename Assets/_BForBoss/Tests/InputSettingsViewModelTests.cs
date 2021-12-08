using BForBoss;
using NUnit.Framework;

namespace Tests.Input
{
    public class InputSettingsViewModelTests
    {
        [Test]
        public void Test_InputSettings_ConstructorSetup()
        {
            //Given
            var inputSettings = new MockInputSettings
            {
                MouseHorizontalSensitivity = 0.1f,
                MouseVerticalSensitivity = 0.2f,
                ControllerHorizontalSensitivity = 0.3f,
                ControllerVerticalSensitivity = 0.4f,
                IsInverted = true
            };

            //When
            var viewModel = new InputSettingsViewModel(inputSettings, new MockAnalytics());
            
            //Then
            Assert.AreEqual(viewModel.GetMouseHorizontal, 0.1f, "Mouse Horizontal value should be the same as input settings");
            Assert.AreEqual(viewModel.GetMouseVertical, 0.2f, "Mouse Vertical value should be the same as input settings");
            Assert.AreEqual(viewModel.GetControllerHorizontal, 0.3f, "Controller Horizontal value should be the same as input settings");
            Assert.AreEqual(viewModel.GetControllerVeritcal, 0.4f, "Controller Vertical value should be the same as input settings");
            Assert.IsTrue(viewModel.GetIsInverted, "Input should be inverted");
        }
        
        [Test]
        public void Test_RevertSettings_InputSettingsRevertSettingCalled()
        {
            //Given
            var inputSettings = new MockInputSettings();
            var viewModel = new InputSettingsViewModel(inputSettings, new MockAnalytics());

            //When
            viewModel.RevertSettings();
            
            //Then
            Assert.AreEqual(1, inputSettings.CalledRevertAllSettings, "Revert Settings should be called once");
        }
        
        [Test]
        public void Test_AppleSettings()
        {
            //Given
            var inputSettings = new MockInputSettings
            {
                MouseHorizontalSensitivity = 0.1f,
                MouseVerticalSensitivity = 0.2f,
                ControllerHorizontalSensitivity = 0.3f,
                ControllerVerticalSensitivity = 0.4f,
                IsInverted = true
            };
            var viewModel = new InputSettingsViewModel(inputSettings, new MockAnalytics());

            //When
            viewModel.ApplySettings(9, 9, 9, 9, false);
            
            //Then
            Assert.AreEqual(viewModel.GetMouseHorizontal, 9f, "Mouse Horizontal value should be overriden");
            Assert.AreEqual(viewModel.GetMouseVertical, 9f, "Mouse Vertical value should be overriden");
            Assert.AreEqual(viewModel.GetControllerHorizontal, 9f, "Controller Horizontal value should be overriden");
            Assert.AreEqual(viewModel.GetControllerVeritcal, 9f, "Controller Vertical value should be overriden");
            Assert.IsFalse(viewModel.GetIsInverted, "Input should not be inverted");
        }
    }
}
