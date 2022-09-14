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
            private const string DEFAULT_FILTER_STRING = "BForBoss,Perigon"; // todo : make this project specific
            
            private const string FILTER_STRING_KEY = "LOGGER_FILTER_STRING";
            private const string USE_LOGGING_KEY = "USE_LOGGING";
            private const string NO_NAMESPACE_LOGGING_VALUE = "LOGGER_NO_NAMESPACE_VALUE";
            private const string LOGGER_PREFIX = "LOGGER-";
            private const string LOGGER_HASHSTRING_KEY = "LOGGER_HASHSTRING";
            
            public string _filterString;
            public bool _noNameSpaceValue;
            public Dictionary<string, bool> _loggerValues;
            public HashSet<string> _loggerNamespaces;
            
            public static bool _useLogging = false;

            private string[] FilterList => _filterString.Split(',');

            public bool AllSelected => _loggerValues.All((pair) => pair.Value) && _noNameSpaceValue;
            public bool NoneSelected => _loggerValues.All((pair) => !pair.Value) && !_noNameSpaceValue;
            
            public void ApplyFilter()
            {
                _loggerValues = _loggerValues.Where((pair) => FilterNamespace(pair.Key, FilterList))
                    .ToDictionary(p => p.Key, p => p.Value);
            }
            
            private static bool FilterNamespace(string nameSpace, params string[] filterList)
            {
                if (filterList.Length == 0 || string.IsNullOrEmpty(nameSpace)) return true;
                foreach (var root in filterList)
                {
                    if(string.IsNullOrEmpty(root)) continue;
                    if (nameSpace.StartsWith(root))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void SetAll(bool value)
            {
                _noNameSpaceValue = value;
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
                LoggerSettings._useLogging = logState;
            }

            public static LoggerSettings GetRuntimeSettings()
            {
                _useLogging = EditorPrefs.GetBool(USE_LOGGING_KEY, false);
                return new LoggerSettings()
                {
                    _noNameSpaceValue = EditorPrefs.GetBool(NO_NAMESPACE_LOGGING_VALUE, false),
                    _loggerNamespaces = new HashSet<string>(EditorPrefs.GetString(LOGGER_HASHSTRING_KEY, "").Split(','))
                };
            }

            public static LoggerSettings GetEditorSettings(IList<string> namespaces)
            {
                _useLogging = EditorPrefs.GetBool(USE_LOGGING_KEY, false);
                var settings = new LoggerSettings
                {
                    _noNameSpaceValue = EditorPrefs.GetBool(NO_NAMESPACE_LOGGING_VALUE, false),
                    _filterString = EditorPrefs.GetString(FILTER_STRING_KEY, DEFAULT_FILTER_STRING),
                    _loggerValues = new Dictionary<string, bool>(),
                };

                foreach (var name in namespaces)
                {
                    if(FilterNamespace(name, settings.FilterList))
                        settings._loggerValues[name] = EditorPrefs.GetBool(LOGGER_PREFIX + name, true);
                }
                return settings;
            }

            public static void SaveSettings(LoggerSettings settings)
            {
                EditorPrefs.SetBool(USE_LOGGING_KEY, _useLogging);
                EditorPrefs.SetBool(NO_NAMESPACE_LOGGING_VALUE, settings._noNameSpaceValue);
                EditorPrefs.SetString(FILTER_STRING_KEY, settings._filterString);
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

        private static void LogInEditor(string toLog, Action<string> debugMethod)
        {
            if (!LoggerSettings._useLogging)
                return;
            StackTrace stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(2).GetMethod();
            var nameSpace = callingMethod.DeclaringType?.Namespace;
            if (string.IsNullOrEmpty(nameSpace))
            {
                if(_settings._noNameSpaceValue)
                    debugMethod(toLog);
            }else if (_settings._loggerNamespaces.Contains(nameSpace))
            {
                debugMethod(toLog);
            }
        }
    }
}
#endif