using UnityEditor;
using UnityEngine;

public static class LevelDesignFeedbackWindowListener
{
    public static KeyCode WindowKeyCode = KeyCode.F10;
    private static LevelDesignFeedbackEditorWindow _window = null;
    
    public static void OpenLevelDesignFeedbackWindow()
    {
        if (!CanOpenFeedbackWindow() || _window != null)
        {
            Debug.Log("Track No");
            return;
        }
        
        Debug.Log("Track yes");
        _window = EditorWindow.GetWindow<LevelDesignFeedbackEditorWindow>(false);
        _window.OnWindowClose = OnWindowClosed;
        _window.Show();
    }

    private static void OnWindowClosed()
    {
        _window = null;
    }

    private static bool CanOpenFeedbackWindow()
    {
        return (EditorApplication.isPlaying || EditorApplication.isPaused) && Application.isFocused;
    }
}
