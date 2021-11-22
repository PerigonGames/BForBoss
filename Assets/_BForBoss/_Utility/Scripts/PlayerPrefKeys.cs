namespace Perigon.Utility
{
    public class PlayerPrefKeys 
    {
        public struct InputSettings
        {
            public const string Is_Inverted = "is_inverted";
            public const string Mouse_Horizontal_Sensitivity = "mouse_horizontal_sensitivity";
            public const string Mouse_Vertical_Sensitivity = "mouse_vertical_sensitivity";
            public const string Controller_Horizontal_Sensitivity = "controller_horizontal_sensitivity";
            public const string Controller_Vertical_Sensitivity = "controller_vertical_sensitivity";
        }

        public struct ThirdPerson
        {
            public const string IsThirdPerson = "is_third_person";
        }

        public struct GameplaySettings
        {
            public const string ShowFPS = "ShowFPS";
            public const string ShowRAMUsage = "ShowRAMUsage";
            public const string ShowPCSpecs = "ShowPCSpecs";
        }
    }
}

