using UnityEngine;

namespace Perigon.Utility
{
    public static partial class Logger
    {
        public static void LogString(string toLog)
        {
#if !UNITY_EDITOR && DEBUG
            Debug.Log(toLog);
#elif UNITY_EDITOR
            LogInEditor(toLog, Debug.Log);
#endif
        }
        
        public static void LogWarning(string toLog)
        {
#if !UNITY_EDITOR && DEBUG
            Debug.LogWarning(toLog);
#elif UNITY_EDITOR
            LogInEditor(toLog, Debug.LogWarning);
#endif
        }
        
        public static void LogError(string toLog)
        {
#if !UNITY_EDITOR && DEBUG
            Debug.LogWarning(toLog);
#elif UNITY_EDITOR
            LogInEditor(toLog, Debug.LogError);
#endif
        }
    }
}
