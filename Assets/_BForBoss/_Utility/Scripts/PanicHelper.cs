using System;
using UnityEditor;
using UnityEngine;

namespace Perigon.Utility
{
    public static class PanicHelper
    {
        public static void Panic(Exception exception)
        {
            System.Diagnostics.Debugger.Break();
            // Print the exception
            Debug.LogException(exception);
            // Quit the app
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit(1);
#endif
        }
    }
}