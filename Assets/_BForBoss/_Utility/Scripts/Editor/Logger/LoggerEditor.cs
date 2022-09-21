using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Perigon.Utility.Editor
{
    public class LoggerEditor
    {

        private static string _newKey;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Perigon/Debug Logging", SettingsScope.User)
            {
                label = "Debug Logging",
                guiHandler = (searchContext) =>
                {
                    var settings = Logger.LoggerSettings.GetEditorSettings();
                    
                    EditorGUI.BeginChangeCheck();
                    DrawSettings(settings);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Logger.LoggerSettings.SaveSettings(settings);
                    }
                },
                
                keywords = new HashSet<string>(new[] {"Perigon", "Debug", "Logging", "Log"})
            };
            return provider;
        }

        private static void DrawSettings(Logger.LoggerSettings settings)
        {
            EditorGUI.indentLevel += 1;
            var useLogging = EditorGUILayout.ToggleLeft("Enable Global Logging", Logger.LoggerSettings._useLogging);
            Logger.LoggerSettings.SetLoggingState(useLogging);

            EditorGUILayout.Space();
            
            var oldAllSelected = settings.AllSelected;
            var oldNoneSelected = settings.NoneSelected;
            var allSelected = EditorGUILayout.ToggleLeft("All", oldAllSelected);
            var noneSelected = EditorGUILayout.ToggleLeft("None", oldNoneSelected);
            if(allSelected != oldAllSelected) settings.SetAll(true);
            if(noneSelected != oldNoneSelected) settings.SetAll(false);
            
            var pairs = new List<KeyValuePair<string, bool>>(settings._loggerValues);
            foreach (var pair in pairs)
            {
                GUILayout.BeginHorizontal();
                settings._loggerValues[pair.Key] = EditorGUILayout.ToggleLeft(pair.Key, pair.Value);
                if (GUILayout.Button("Remove Key", GUILayout.ExpandWidth(false)))
                {
                    LoggerKeysAsset.instance.RemoveKey(pair.Key);
                    Logger.LoggerSettings.SaveSettings(settings);
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();
            _newKey = EditorGUILayout.TextField("Key To Add", _newKey);
            if (GUILayout.Button("Add Key", GUILayout.ExpandWidth(false)))
            {
                LoggerKeysAsset.instance.AddKey(_newKey);
                _newKey = "";
                Logger.LoggerSettings.SaveSettings(settings);
            }
            GUILayout.EndHorizontal();
            
            EditorGUI.indentLevel -= 1;
        }
    }
    
    
}
