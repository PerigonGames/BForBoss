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
