using System;
using UnityEngine;

namespace BForBoss
{
    public class FreeRoamCameraDebugView : DebugView
    {
        private GUIStyle _boldInstructionStyle = null;
        private Action<bool> _onCamerOptionsChanged = null;
        private Action _onBackButtonPressed = null;

        private bool _shouldInvertMouseYAxis = false;
        private bool _shouldResumeTime = false;

        public override string PrettyName => "Free Roam Camera";

        public FreeRoamCameraDebugView(Rect masterRect, Action<bool> onCamerOptionsChanged, Action onBackButtonPressed) : base(masterRect)
        {
            _onCamerOptionsChanged = onCamerOptionsChanged;
            _onBackButtonPressed = onBackButtonPressed;
        }

        public override void ResetData()
        {
            base.ResetData();

            if (_shouldResumeTime)
            {
                _shouldResumeTime = false;
                Time.timeScale = 0.0f;
            }
            
            _onBackButtonPressed?.Invoke();
        }

        protected override void DrawWindow()
        {
            _boldInstructionStyle ??= new GUIStyle(GUI.skin.label)
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.BoldAndItalic,
                alignment = TextAnchor.MiddleCenter
            };

            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("");
                DrawInstruction("Use", "WASD", "to move");
                DrawInstruction("Use", "Q", "to pan down");
                DrawInstruction("Use", "E", "to pan up");
                DrawInstruction("Hold", "Left Shift", "to boost");
                DrawInstruction("Use", "Scroll Wheel", "to modify boost multiplier");
                DrawInstruction("Hold", "Right mouse", "to rotate");
                DrawInstruction("Press", "SpaceBar", "to reset");
                DrawInstruction("Press", "Backquote", "to escape");

                _shouldInvertMouseYAxis = GUILayout.Toggle(_shouldInvertMouseYAxis, "Invert Y-Axis");

                if (GUI.changed)
                {
                    _onCamerOptionsChanged?.Invoke(_shouldInvertMouseYAxis);
                    GUI.changed = false;
                }

                _shouldResumeTime = GUILayout.Toggle(_shouldResumeTime, "Resume Time");

                if (GUI.changed)
                {
                    Time.timeScale = _shouldResumeTime ? 1.0f : 0.0f;

                    if (_shouldResumeTime)
                    {
                        Debug.Log("The Player will be invincible while time is resumed");
                    }
                }
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
