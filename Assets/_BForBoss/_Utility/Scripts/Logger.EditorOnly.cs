#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Perigon.Utility
{
    [InitializeOnLoad]
    public static partial class Logger
    {
        private const string MENU_ITEM_NAME = "Tools/Toggle Logging";
        private const string EDITOR_PREF_KEY = "USE_LOGGING";
        
        private static bool _useLogging = false;
        
        static Logger()
        {
            _useLogging = EditorPrefs.GetBool(EDITOR_PREF_KEY, false);
            EditorApplication.delayCall += () =>
                Menu.SetChecked(MENU_ITEM_NAME, _useLogging);
        }
        
        [MenuItem(MENU_ITEM_NAME)]
        private static void ToggleLogging() 
        {
            SetLogging(!_useLogging);
        }

        private static void SetLogging(bool logState)
        {
            Menu.SetChecked(MENU_ITEM_NAME, logState);
            EditorPrefs.SetBool(EDITOR_PREF_KEY, logState);
            _useLogging = logState;
        }

        private static void LogInEditor(string toLog)
        {
            if (!_useLogging)
                return;
            StackTrace stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(2).GetMethod();
            var nameSpace = callingMethod.DeclaringType?.Namespace;
            Debug.Log(toLog);
        }
    }
}
#endif