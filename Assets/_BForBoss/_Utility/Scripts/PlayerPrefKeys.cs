using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Perigon.Utility
{
    public static class PlayerPrefKeys 
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

        public static IList<string> GetAllKeys()
        {
            var keys = GetConstStringValuesFromStruct<InputSettings>().ToList();
            keys.AddRange(GetConstStringValuesFromStruct<ThirdPerson>());
            keys.AddRange(GetConstStringValuesFromStruct<GameplaySettings>());
            return keys;
        }

        /// <summary>
        /// Gets all constant string values defined in the struct
        /// </summary>
        /// <typeparam name="T">Struct containing player pref keys</typeparam>
        /// <returns>Array of Keys</returns>
        private static IEnumerable<string> GetConstStringValuesFromStruct<T>() where T : struct
        {
            Type playerPrefStruct = typeof(T);

            FieldInfo[] fields = playerPrefStruct.GetFields(BindingFlags.Public |
         BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fields
                .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                .Select(field => (string)field.GetRawConstantValue());
        }
    }
}

