using System.Collections.Generic;
using UnityEditor;

public static class SceneSwitcherSettingsProvider
{
    private const string PROVIDER_LABEL = "Scene Switcher Settings";
    private static readonly IEnumerable<string> _keyWords = new List<string>{"Scene", "Switch", "Change"};
    private static SceneSwitcherProjectSettings.Settings _settings;
    
    [SettingsProvider]
    public static SettingsProvider CreateSceneSwitcherSettingsProvider()
    {
        SettingsProvider provider = new SettingsProvider("Project/SceneSwitcherSettings", SettingsScope.Project, _keyWords)
        {
            label = PROVIDER_LABEL,
            guiHandler = searchContext =>
            {
                _settings = SceneSwitcherProjectSettings.GetOrCreateSettings();
                
                using (var changeCheck = new EditorGUI.ChangeCheckScope())
                {
                    _settings.baseSearchFolder = EditorGUILayout.TextField("Root Folder", _settings.baseSearchFolder);
    
                    if (changeCheck.changed)
                    {
                        SceneSwitcherProjectSettings.SaveSettings(_settings);
                    }
                }
            }
        };
    
        return provider;
    }
}
