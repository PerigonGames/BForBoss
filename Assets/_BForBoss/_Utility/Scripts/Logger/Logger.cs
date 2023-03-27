using UnityEngine;

namespace Perigon.Utility
{
    public enum LoggerColor
    {
        Default,
        Black,
        Red,
        Blue,
        Green,
        Yellow
    }
    
    public static partial class Logger
    {
        public static void LogString(string toLog, LoggerColor color = LoggerColor.Default,  string key = "Misc")
        {
#if !UNITY_EDITOR && DEBUG
            Debug.Log(ColorizeLog(toLog, color));
#elif UNITY_EDITOR
            LogInEditor(ColorizeLog(toLog, color), key, Debug.Log);
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
        
        public static void LogWarning(string toLog, LoggerColor color = LoggerColor.Default, string key = "Misc")
        {
#if !UNITY_EDITOR && DEBUG
            Debug.LogWarning(ColorizeLog(toLog, color));
#elif UNITY_EDITOR
            LogInEditor(ColorizeLog(toLog, color), key, Debug.LogWarning);
#endif
        }
        
        public static void LogError(string toLog, LoggerColor color = LoggerColor.Default,  string key = "Misc")
        {
#if !UNITY_EDITOR && DEBUG
            Debug.LogError(ColorizeLog(toLog, color));
#elif UNITY_EDITOR
            LogInEditor(ColorizeLog(toLog, color), key, Debug.LogError);
#endif
        }

        private static string ColorizeLog(string log, LoggerColor color)
        {
            string colorText;

            switch (color)
            {
                case LoggerColor.Black:
                    colorText = "black";
                    break;
                case LoggerColor.Red:
                    colorText = "red";
                    break;
                case LoggerColor.Blue:
                    colorText = "blue";
                    break;
                case LoggerColor.Green:
                    colorText = "green";
                    break;
                case LoggerColor.Yellow:
                    colorText = "yellow";
                    break;
                case LoggerColor.Default:
                default:
                    colorText = GUI.color == Color.black ? "black" : "white";
                    break;
            }
            

            return $"<color={colorText}>{log}</color>";
        }
    }
}
