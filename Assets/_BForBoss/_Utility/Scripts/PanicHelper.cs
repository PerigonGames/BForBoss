using System;
using System.Collections;
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
        
        public static void PanicIfNullObject<T>(this T effectedClass, System.Object fieldToCheck, string fieldName) where T : class
        {
            if (fieldToCheck == null)
            {
                Exception exception = new Exception($"{fieldName} is null on {effectedClass.ToString()}");
                Panic(exception);
            }
        }

        public static void PanicIfNullOrEmptyList<T>(this T effectedClass, IList listToCheck, string fieldName) where T : class
        {
            if (listToCheck == null || listToCheck.Count == 0)
            {
                Exception exception = new Exception($"{fieldName} is null or empty on {effectedClass.ToString()}");
                Panic(exception);
            }
        }
    }
}