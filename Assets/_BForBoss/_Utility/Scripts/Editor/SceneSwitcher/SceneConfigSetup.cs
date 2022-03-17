using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SceneConfigSetup
{
    public string path;
    public string parentFolder;
    
    public bool Equals(SceneConfigSetup other)
    {
        return string.Equals(path, other.path);
    }
}

public static class SceneSwitcherProjectSettings
{
    public struct Settings
    {
        public string baseSearchFolder;
        
        public Settings(string searchFolder)
        {
            baseSearchFolder = searchFolder;
        }
    }

    private const string BASE_SEARCH_FOLDER_PREF_KEY = "Perigon_Tools_Scene_Switcher_Base_Search_Folder_Key";
    
    public static void SaveSettings(Settings settings)
    {
        EditorPrefs.SetString(BASE_SEARCH_FOLDER_PREF_KEY, settings.baseSearchFolder);
    }

    public static Settings GetOrCreateSettings()
    {
        return new Settings(EditorPrefs.GetString(BASE_SEARCH_FOLDER_PREF_KEY, Application.dataPath));
    }
}
