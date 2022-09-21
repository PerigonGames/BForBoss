using UnityEngine;

namespace Perigon.Utility
{
    public static partial class Logger
    {
        public static void LogString(string toLog, string key = "Misc")
        {
#if !UNITY_EDITOR && DEBUG
            Debug.Log(toLog);
#elif UNITY_EDITOR
            LogInEditor(toLog, key, Debug.Log);
#endif
        }
        
        public static void LogFormat(string toLog, string key = "Misc", params object[] args)
        {
#if !UNITY_EDITOR && DEBUG
            Debug.LogFormat(toLog, args);
#elif UNITY_EDITOR
            LogInEditor(toLog, key, (str) => Debug.LogFormat(str, args));
#endif
        }
        
        public static void LogWarning(string toLog, string key = "Misc")
        {
#if !UNITY_EDITOR && DEBUG
            Debug.LogWarning(toLog);
#elif UNITY_EDITOR
            LogInEditor(toLog, key, Debug.LogWarning);
#endif
        }
        
        public static void LogError(string toLog, string key = "Misc")
        {
#if !UNITY_EDITOR && DEBUG
            Debug.LogWarning(toLog);
#elif UNITY_EDITOR
            LogInEditor(toLog, key, Debug.LogError);
#endif
        }
    }
}
