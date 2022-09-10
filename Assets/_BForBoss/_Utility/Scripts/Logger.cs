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
            LogInEditor(toLog);
#endif
        }
    }
}
