using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Perigon.Utility.Editor
{
    public class LoggerEditor
    {
        private static GUIContent _rootNamespacesLabel = new("Root Namespaces",
            "Only namespaces under these will be used (comma seperated)");

        private static GUIContent _noNamespacesLabel = new("<No Namespace>", "Show logs for scripts that are not in a namespace");

        private static List<string> _allNamespaces = null;
        private static List<string> _filteredNamespaces = null;
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Perigon/Debug Logging", SettingsScope.User)
            {
                label = "Debug Logging",
                guiHandler = (searchContext) =>
                {
                    var settings = Logger.LoggerSettings.GetEditorSettings(GetNamespaces());
                    
                    EditorGUI.BeginChangeCheck();
                    settings.ApplyFilter();
                    DrawSettings(settings);
                    if (EditorGUI.EndChangeCheck())
                    {
                        settings.ApplyFilter();
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
            settings._filterString = EditorGUILayout.TextField(_rootNamespacesLabel, settings._filterString);
            
            var oldAllSelected = settings.AllSelected;
            var oldNoneSelected = settings.NoneSelected;
            var allSelected = EditorGUILayout.ToggleLeft("All", oldAllSelected);
            var noneSelected = EditorGUILayout.ToggleLeft("None", oldNoneSelected);
            if(allSelected != oldAllSelected) settings.SetAll(true);
            if(noneSelected != oldNoneSelected) settings.SetAll(false);
            
            settings._noNameSpaceValue = EditorGUILayout.ToggleLeft(_noNamespacesLabel, settings._noNameSpaceValue);
            var pairs = new List<KeyValuePair<string, bool>>(settings._loggerValues);
            foreach (var pair in pairs)
            {
                settings._loggerValues[pair.Key] = EditorGUILayout.ToggleLeft(pair.Key, pair.Value);
            }
            EditorGUI.indentLevel -= 1;
        }

        private static List<string> GetNamespaces()
        {
            if (_allNamespaces == null)
            {
                var unityAssemblies = CompilationPipeline.GetAssemblies().Select((assembly => assembly.name));
                _allNamespaces = AppDomain.CurrentDomain.GetAssemblies()
                    .Where((assembly) => unityAssemblies.Contains(assembly.GetName().Name))
                    .SelectMany(assembly => assembly.GetTypes())
                    .Select(t => t.Namespace)
                    .Distinct()
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
                _allNamespaces.Sort();
            }

            return _allNamespaces;
        }
        


        
    }
    
    
}
