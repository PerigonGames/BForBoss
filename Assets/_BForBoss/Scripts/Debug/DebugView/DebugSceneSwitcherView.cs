using BForBoss;
using UnityEngine;

public class DebugSceneSwitcherView : DebugView
{
    public DebugSceneSwitcherView(Rect masterRect) : base(masterRect)
    {
        
    }

    protected override void DrawWindow()
    {
        GUILayout.Label("Hey There");
    }
}
