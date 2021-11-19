using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BForBoss
{
    public class PlayerPrefResetter : DebugView
    {
        private Vector2 _scrollPosition = Vector2.zero;

        private IList<string> _keys;

        public PlayerPrefResetter(Rect masterRect) : base(masterRect)
        {
            _keys = GetConstStringValuesFromStruct<UploadPlayerScoreDataSource.PlayerPrefKey>();
            // if we add player pref fields anywhere else make sure to append them to the array here
        }

        protected override void DrawWindow()
        {
            using (new GUILayout.AreaScope(_baseRect))
            {
                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Player Pref Tool");
                        GUILayout.FlexibleSpace();
                    }

                    if(GUILayout.Button("Delete All"))
                    {
                        ClearPlayerPrefs();
                    }

                    using var scrollViewScope = new GUILayout.ScrollViewScope(_scrollPosition);
                    _scrollPosition = scrollViewScope.scrollPosition;
                    foreach(string key in _keys)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button($"Delete {key}"))
                            {
                                DeleteSpecificKey(key);
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                }
            }
        }

        private void ClearPlayerPrefs()
        {
            Debug.Log("Deleting all player prefs!");
            PlayerPrefs.DeleteAll();
        }

        private void DeleteSpecificKey(string key)
        {
            Debug.Log($"Deleting {key} player pref");
            PlayerPrefs.DeleteKey(key);
        }

        /// <summary>
        /// Gets all constant string values defined in the struct
        /// </summary>
        /// <typeparam name="T">Struct containing player pref keys</typeparam>
        /// <returns>Array of Keys</returns>
        private static List<string> GetConstStringValuesFromStruct<T>() where T : struct
        {
            Type playerPrefStruct = typeof(T);

            FieldInfo[] fields = playerPrefStruct.GetFields(BindingFlags.Public |
         BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fields
                .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                .Select(field => (string)field.GetRawConstantValue()).ToList();
        }
    }
}
