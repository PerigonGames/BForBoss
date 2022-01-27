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
            public const string IS_INVERTED = "is_inverted";
            public const string MOUSE_HORIZONTAL_SENSITIVITY = "mouse_horizontal_sensitivity";
            public const string MOUSE_VERTICAL_SENSITIVITY = "mouse_vertical_sensitivity";
            public const string CONTROLLER_HORIZONTAL_SENSITIVITY = "controller_horizontal_sensitivity";
            public const string CONTROLLER_VERTICAL_SENSITIVITY = "controller_vertical_sensitivity";
        }

        public struct ThirdPerson
        {
            public const string IS_THIRD_PERSON = "is_third_person";
        }
        
        public struct LeaderboardSettings
        {
            public const string USERNAME = "UserName";
            public const string TIMER = "Timer";
            public const string INPUT = "Input";
            public const string SHOULDUPLOAD = "ShouldUpload";
        }

        public static IList<string> GetAllKeys()
        {
            var keys = GetConstStringValuesFromStruct<InputSettings>().ToList();
            keys.AddRange(GetConstStringValuesFromStruct<ThirdPerson>());
            keys.AddRange(GetConstStringValuesFromStruct<LeaderboardSettings>());
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

