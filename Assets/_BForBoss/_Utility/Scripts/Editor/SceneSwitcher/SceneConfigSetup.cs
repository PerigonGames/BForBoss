using System.IO;
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

    private const string FILE_NAME = "SceneSwitcherSettings.json";
    private static readonly string ROOT_FOLDER = Application.dataPath;
    private static readonly string SETTINGS_FOLDER = Directory.GetParent(Application.dataPath).FullName +
                                                     Path.DirectorySeparatorChar + "ProjectSettings" +
                                                     Path.DirectorySeparatorChar;
    private static readonly string SETTINGS_FILE = SETTINGS_FOLDER + FILE_NAME;

    public static void SaveSettings(Settings settings)
    {
        string rawSettings = EditorJsonUtility.ToJson(settings, true);

        if (!Directory.Exists(SETTINGS_FOLDER))
        {
            Debug.LogError("Unable to locate ProjectSettings Folder. Terminating Save");
            return;
        }
        
        File.WriteAllText(SETTINGS_FILE, rawSettings);
    }

    public static Settings GetOrCreateSettings()
    {
        Settings settings;
        if (AreSettingsAvailable())
        {
            string settingsJson = File.ReadAllText(SETTINGS_FILE);
            settings = JsonUtility.FromJson<Settings>(settingsJson);
        }
        else
        {
            settings = new Settings(ROOT_FOLDER);
            SaveSettings(settings);
            AssetDatabase.RefreshSettings();
        }

        return settings;
    }

    private static bool AreSettingsAvailable()
    {
        return File.Exists(SETTINGS_FILE);
    }

}
