using System.Collections.Generic;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class PlayerPrefResetter : DebugView
    {
        private Vector2 _scrollPosition = Vector2.zero;

        private IList<string> _keys;

        public PlayerPrefResetter(Rect masterRect) : base(masterRect)
        {
            _keys = PlayerPrefKeys.GetAllKeys();
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
    }
}
