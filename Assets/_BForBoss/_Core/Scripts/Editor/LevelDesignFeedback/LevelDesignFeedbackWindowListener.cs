using UnityEditor;
using UnityEngine;

public static class LevelDesignFeedbackWindowListener
{
    public static KeyCode WindowKeyCode = KeyCode.F10;
    private static LevelDesignFeedbackEditorWindow _window = null;
    
    public static void OpenLevelDesignFeedbackWindow(Texture2D screenShot)
    {
        if (!CanOpenFeedbackWindow() || _window != null)
        {
            return;
        }
        
        _window = LevelDesignFeedbackEditorWindow.OpenWindow(screenShot);
        _window.OnWindowClosed = OnWindowClosed;
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
