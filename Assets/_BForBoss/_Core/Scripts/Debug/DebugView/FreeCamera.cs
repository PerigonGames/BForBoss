using System;
using UnityEngine;

namespace BForBoss
{
    public class FreeCamera : DebugView
    {
        public FreeCamera(Rect masterRect, Action onWindowOpened) : base(masterRect)
        {
            onWindowOpened?.Invoke();
        }

        protected override void DrawWindow()
        {
            // Add Instructions
            GUILayout.TextField("Press ESC key to stop using the free roam camera");
        }
    }
}
