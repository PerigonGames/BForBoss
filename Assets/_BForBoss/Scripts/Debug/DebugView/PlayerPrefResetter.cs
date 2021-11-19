using System;
using UnityEngine;

namespace BForBoss
{
    public class PlayerPrefResetter : DebugView
    {
        private Vector2 _scrollPosition = Vector2.zero;

        private string[] _keys;

        public PlayerPrefResetter(Rect masterRect) : base(masterRect)
        {
            _keys = GetKeysFromStruct<UploadPlayerScoreDataSource.PlayerPrefKey>();
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

                    using (var scrollViewScope = new GUILayout.ScrollViewScope(_scrollPosition))
                    {
                        _scrollPosition = scrollViewScope.scrollPosition;
                        for (int i = 0, count = _keys.Length; i < count; i++)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button($"Delete {_keys[i]}"))
                                {
                                    DeleteSpecificKey(_keys[i]);
                                }
                                GUILayout.FlexibleSpace();
                            }
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
        /// Real fancy and a bit cursed method of iterating through a struct to get all the values.
        /// Rests on a couple kinda dangerous assumptions - 
        /// 1) All fields are strings. If we wanted we could check this, but I think given the use case its okay
        /// 2) All fields are consts. 
        /// </summary>
        /// <typeparam name="T">Struct containing player pref keys</typeparam>
        /// <returns>Array of Keys</returns>
        private string[] GetKeysFromStruct<T>() where T : struct
        {
            var keys = new T();
            Type playerPrefStruct = typeof(T);

            System.Reflection.FieldInfo[] fields = playerPrefStruct.GetFields();

            string[] toReturn = new string[fields.Length];

            for(int i = 0; i < fields.Length; i++)
            {
                toReturn[i] = fields[i].GetValue(keys).ToString();
            }
            return toReturn;
        }
    }
}
