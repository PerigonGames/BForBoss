using System;
using UnityEngine;

namespace BForBoss
{
    public class FreeRoamCameraDebugView : DebugView
    {
        private GUIStyle _boldInstructionStyle = null;
        private Action<bool> _onOptionsChanged = null;
        private Action _onBackButtonPressed = null;

        private bool _isMouseRotationYInverted = true;

        public override string PrettyName => "Free Roam Camera";

        public FreeRoamCameraDebugView(Rect masterRect, Action onWindowOpened, Action<bool> onOptionsChanged, Action onBackButtonPressed) : base(masterRect)
        {
            _onOptionsChanged = onOptionsChanged;
            _onBackButtonPressed = onBackButtonPressed;
            onWindowOpened?.Invoke();
        }

        public override void ResetData()
        {
            base.ResetData();
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

                _isMouseRotationYInverted = GUILayout.Toggle(_isMouseRotationYInverted, "Invert Y-Axis");

                if (GUI.changed)
                {
                    _onOptionsChanged?.Invoke(_isMouseRotationYInverted);
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
