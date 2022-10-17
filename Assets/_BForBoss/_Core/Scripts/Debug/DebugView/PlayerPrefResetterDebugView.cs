using System.Collections.Generic;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss
{
    public class PlayerPrefResetterDebugView : DebugView
    {
        private IList<string> _keys;

        public override string PrettyName => "Player Pref Resetter";

        public PlayerPrefResetterDebugView(Rect masterRect) : base(masterRect)
        {
            _keys = PlayerPrefKeys.GetAllKeys();
        }

        protected override void DrawWindowContent()
        {
            using (new GUILayout.VerticalScope())
            {
                if (GUILayout.Button("Delete All"))
                {
                    ClearPlayerPrefs();
                }
                
                foreach (string key in _keys)
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
