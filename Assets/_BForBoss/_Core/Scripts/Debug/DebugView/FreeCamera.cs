using System;
using UnityEngine;

namespace BForBoss
{
    public class FreeCamera : DebugView
    {
        private GUIStyle _boldInstructionStyle = null;
        
        public FreeCamera(Rect masterRect, Action onWindowOpened) : base(masterRect)
        {
            onWindowOpened?.Invoke();
        }

        protected override void DrawWindow()
        {
            if (_boldInstructionStyle == null)
            {
                _boldInstructionStyle = new GUIStyle(GUI.skin.label)
                {
                    normal = new GUIStyleState
                    {
                        textColor = Color.white
                    },
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
            
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("");
                DrawInstruction("Use", "WASD", "to move");
                DrawInstruction("Use", "Q", "to pan down");
                DrawInstruction("Use", "E", "to pan down");
                DrawInstruction("Hold", "Left Shift", "to boost");
                DrawInstruction("Hold", "Right mouse", "to rotate");
                DrawInstruction("Press", "`", "to escape");
            }
            
            GUI.UnfocusWindow();
        }

        private void DrawInstruction(string prefix, string instruction, string suffix)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"{prefix} ", GUILayout.ExpandWidth(false));
                GUILayout.Label($"{instruction} ", _boldInstructionStyle,GUILayout.ExpandWidth(false));
                GUILayout.Label(suffix);
            }
        }
    }
}
