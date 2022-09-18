#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Perigon.Utility
{
    [InitializeOnLoad]
    public static partial class Logger
    {
        public class LoggerSettings
        {
            private const string USE_LOGGING_KEY = "USE_LOGGING";
            private const string LOGGER_PREFIX = "LOGGER-";
            private const string LOGGER_HASHSTRING_KEY = "LOGGER_HASHSTRING";
            private const string MENU_ITEM_NAME = "Tools/Toggle Logging";
            
            public Dictionary<string, bool> _loggerValues;
            public HashSet<string> _loggerNamespaces;
            
            public static bool _useLogging = false;

            public bool AllSelected => _loggerValues.All((pair) => pair.Value);
            public bool NoneSelected => _loggerValues.All((pair) => !pair.Value);
            
            public void SetAll(bool value)
            {
                _loggerValues = _loggerValues.ToDictionary(p => p.Key, p => value);
            }
            
            [MenuItem(MENU_ITEM_NAME)]
            private static void ToggleLogging()
            {
                var logValue = EditorPrefs.GetBool(USE_LOGGING_KEY, false);
                SetLoggingState(!logValue);
            }

            public static void SetLoggingState(bool logState)
            {
                Menu.SetChecked(MENU_ITEM_NAME, logState);
                EditorPrefs.SetBool(USE_LOGGING_KEY, logState);
                _useLogging = logState;
            }

            public static LoggerSettings GetRuntimeSettings()
            {
                _useLogging = EditorPrefs.GetBool(USE_LOGGING_KEY, false);
                return new LoggerSettings()
                {
                    _loggerNamespaces = new HashSet<string>(EditorPrefs.GetString(LOGGER_HASHSTRING_KEY, "").Split(','))
                };
            }

            public static LoggerSettings GetEditorSettings()
            {
                _useLogging = EditorPrefs.GetBool(USE_LOGGING_KEY, false);
                var settings = new LoggerSettings
                {
                    _loggerValues = new Dictionary<string, bool>(),
                };

                foreach (var key in LoggerKeysAsset.instance.LoggingKeys)
                {
                    settings._loggerValues[key] = EditorPrefs.GetBool(LOGGER_PREFIX + key, true);
                }
                return settings;
            }

            public static void SaveSettings(LoggerSettings settings)
            {
                EditorPrefs.SetBool(USE_LOGGING_KEY, _useLogging);
                foreach (var pair in settings._loggerValues)
                {
                    EditorPrefs.SetBool(LOGGER_PREFIX + pair.Key, pair.Value);
                }

                var trueVals = settings._loggerValues
                    .Where(p => p.Value)
                    .Select(p => p.Key);
                var hashString = new StringBuilder();
                hashString.AppendJoin(',', trueVals);
                EditorPrefs.SetString(LOGGER_HASHSTRING_KEY, hashString.ToString());
            }
        }

        private const string MENU_ITEM_NAME = "Tools/Toggle Logging";
        private static LoggerSettings _settings;
        
        static Logger()
        {
            _settings = LoggerSettings.GetRuntimeSettings();
            EditorApplication.delayCall += () =>
                Menu.SetChecked(MENU_ITEM_NAME, LoggerSettings._useLogging);
        }

        private static void LogInEditor(string toLog, string key, Action<string> debugMethod)
        {
            if (!LoggerSettings._useLogging)
                return;
            key = key.ToLower();
            if (_settings._loggerNamespaces.Contains(key))
            {
                debugMethod(toLog);
            }else if (!LoggerKeysAsset.instance.HasKey(key))
            {
                Debug.LogWarning("Using a debug key that doesn't exist! Go to Preferences > Perigon > Debug Logging to fix this");
            }
        }
    }
}
#endif