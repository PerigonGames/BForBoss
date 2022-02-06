using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneConfigSetup
{
    public string name;
    public string path;
    public string parentFolder;

    private bool _isFavouriteScene = false;

    public bool IsFavouriteScene
    {
        get => _isFavouriteScene;
        set => _isFavouriteScene = value;
    }
    
    public bool Equals(SceneConfigSetup other)
    {
        return string.Equals(path, other.path);
    }
}

public class SceneWizardConfig : ScriptableObject
{
    public List<SceneConfigSetup> scenes;
}
