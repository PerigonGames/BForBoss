using BForBoss;
using NUnit.Framework;

namespace Tests.Input
{
    public class InputSettingsViewModelTests
    {
        [Test]
        public void Test_InputSettings_MouseKeyboardConstructorSetup()
        {
            //Given
            var inputSettings = new MockInputConfiguration
            {
                MouseHorizontalSensitivity = 0.1f,
                MouseVerticalSensitivity = 0.2f,
                ControllerHorizontalSensitivity = 0.3f,
                ControllerVerticalSensitivity = 0.4f,
                IsInverted = true
            };

            //When
            var viewModel = new MouseKeyboardInputSettingsViewModel(inputSettings);
            
            //Then
            Assert.AreEqual(viewModel.GetHorizontal, 10f, "Mouse Horizontal value should be the same as input settings");
            Assert.AreEqual(viewModel.GetVertical, 20f, "Mouse Vertical value should be the same as input settings");
            Assert.IsTrue(viewModel.GetIsInverted, "Input should be inverted");
        }
        
        [Test]
        public void Test_InputSettings_ControllerConstructorSetup()
        {
            //Given
            var inputSettings = new MockInputConfiguration
            {
                MouseHorizontalSensitivity = 0.1f,
                MouseVerticalSensitivity = 0.2f,
                ControllerHorizontalSensitivity = 0.3f,
                ControllerVerticalSensitivity = 0.4f,
                IsInverted = true
            };

            //When
            var viewModel = new ControllerInputSettingsViewModel(inputSettings);
            
            //Then
            Assert.IsTrue(TestUtilities.WithinBounds(viewModel.GetHorizontal, 30f), "Controller Horizontal value should be the same as input settings");
            Assert.IsTrue(TestUtilities.WithinBounds(viewModel.GetVertical, 40f), "Controller Vertical value should be the same as input settings");
            Assert.IsTrue(viewModel.GetIsInverted, "Input should be inverted");
        }
        
        [Test]
        public void Test_RevertSettings_InputSettingsRevertSettingCalled()
        {
            //Given
            var inputSettings = new MockInputConfiguration();
            var viewModel = new MouseKeyboardInputSettingsViewModel(inputSettings);

            //When
            viewModel.RevertSettings();
            
            //Then
            Assert.AreEqual(1, inputSettings.CalledRevertAllSettings, "Revert Settings should be called once");
        }
        
        [Test]
        public void Test_MouseKeyboardApplySettings()
        {
            //Given
            var inputSettings = new MockInputConfiguration
            {
                MouseHorizontalSensitivity = 0.1f,
                MouseVerticalSensitivity = 0.2f,
                ControllerHorizontalSensitivity = 0.3f,
                ControllerVerticalSensitivity = 0.4f,
                IsInverted = true
            };
            var viewModel = new MouseKeyboardInputSettingsViewModel(inputSettings);

            //When
            viewModel.ApplySettings(9, 9, false);
            
            //Then
            Assert.AreEqual(viewModel.GetHorizontal, 9f, "Mouse Horizontal value should be overriden");
            Assert.AreEqual(viewModel.GetVertical, 9f, "Mouse Vertical value should be overriden");
            Assert.IsFalse(viewModel.GetIsInverted, "Input should not be inverted");
        }
        
        [Test]
        public void Test_ControllerApplySettings()
        {
            //Given
            var inputSettings = new MockInputConfiguration
            {
                MouseHorizontalSensitivity = 0.1f,
                MouseVerticalSensitivity = 0.2f,
                ControllerHorizontalSensitivity = 0.3f,
                ControllerVerticalSensitivity = 0.4f,
                IsInverted = true
            };
            var viewModel = new ControllerInputSettingsViewModel(inputSettings);

            //When
            viewModel.ApplySettings(9, 9, false);
            
            //Then
            Assert.AreEqual(9f, viewModel.GetHorizontal , "Controller Horizontal value should be overriden");
            Assert.AreEqual(9f, viewModel.GetVertical,  "Controller Vertical value should be overriden");
            Assert.IsFalse(viewModel.GetIsInverted, "Input should not be inverted");
        }
    }
}
