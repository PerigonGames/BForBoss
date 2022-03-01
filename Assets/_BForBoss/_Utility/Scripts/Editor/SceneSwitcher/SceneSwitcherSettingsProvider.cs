using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SceneSwitcherSettingsProvider
{
    private const string PROVIDER_LABEL = "Scene Switcher Settings";
    public static readonly string SETTINGS_PATH = "Preferences/SceneSwitcherSettings";
    private static readonly IEnumerable<string> _keyWords = new List<string>{"Scene", "Switch", "Change"};
    private static SceneSwitcherProjectSettings.Settings _settings;
    
    [SettingsProvider]
    public static SettingsProvider CreateSceneSwitcherSettingsProvider()
    {
        SettingsProvider provider = new SettingsProvider(SETTINGS_PATH, SettingsScope.User, _keyWords)
        {
            label = PROVIDER_LABEL,
            guiHandler = searchContext =>
            {
                _settings = SceneSwitcherProjectSettings.GetOrCreateSettings();

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Root Search Folder");

                    if (GUILayout.Button("Choose Directory"))
                    {
                        string currentRootFolder = _settings.baseSearchFolder;
                        _settings.baseSearchFolder = EditorUtility.OpenFolderPanel("Choose Root Folder", _settings.baseSearchFolder, string.Empty);

                        if (!IsValidRootFolder(_settings.baseSearchFolder))
                        {
                            Debug.LogError("Invalid Directory Choice");
                            _settings.baseSearchFolder = currentRootFolder;
                        }
                        
                        SceneSwitcherProjectSettings.SaveSettings(_settings);
                        RefreshEditorWindowIfOpen();
                    }
                    
                    EditorGUILayout.Space();
                }

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.LabelField(_settings.baseSearchFolder);
                }

                EditorGUILayout.HelpBox("A valid directory must be located within the main Assets Folder",
                    MessageType.Info, true);
            }
        };
    
        return provider;
    }

    private static bool IsValidRootFolder(string rootFolderPath)
    {
        return !string.IsNullOrEmpty(rootFolderPath) && Directory.Exists(rootFolderPath) &&
               rootFolderPath.StartsWith(Application.dataPath);
    }

    private static void RefreshEditorWindowIfOpen()
    {
        if (!EditorWindow.HasOpenInstances<SceneSwitcherEditorWindow>())
        {
            return;
        }
        
        SceneSwitcherEditorWindow window = EditorWindow.GetWindow<SceneSwitcherEditorWindow>();
        window.OnSettingsProviderChanged();
    }
}
