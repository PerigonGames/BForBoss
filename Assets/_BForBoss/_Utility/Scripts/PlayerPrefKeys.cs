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
            public const string USERNAME = "user_name";
            public const string TIMER = "timer";
            public const string SHOULD_UPLOAD = "should_upload";
        }

        public struct AudioSettings
        {
            public const string MAIN_VOLUME = "main_volume";
            public const string MUSIC_VOLUME = "music_volume";
            public const string SFX_VOLUME = "sfx_volume";
        }

        public static IList<string> GetAllKeys()
        {
            var keys = GetConstStringValuesFromStruct<InputSettings>().ToList();
            keys.AddRange(GetConstStringValuesFromStruct<ThirdPerson>());
            keys.AddRange(GetConstStringValuesFromStruct<LeaderboardSettings>());
            keys.AddRange(GetConstStringValuesFromStruct<AudioSettings>());
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

