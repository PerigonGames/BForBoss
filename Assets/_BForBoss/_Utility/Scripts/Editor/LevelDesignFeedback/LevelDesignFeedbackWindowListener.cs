using UnityEditor;
using UnityEngine;

namespace Perigon.Utility
{
    public static class LevelDesignFeedbackWindowListener
    {
        public static KeyCode OpenEditorKeyCode = KeyCode.F10;
        private static LevelDesignFeedbackEditorWindow _window = null;

        private static bool _isApplicationPaused = false;

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
            EditorApplication.isPaused = _isApplicationPaused;
        }

        private static bool CanOpenFeedbackWindow()
        {
            _isApplicationPaused = EditorApplication.isPaused;
            return (EditorApplication.isPlaying || _isApplicationPaused) && Application.isFocused;
        }
    }
}
