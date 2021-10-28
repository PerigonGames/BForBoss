using System.Collections;
using System.Collections.Generic;
using UnityEngine;    

public abstract class DebugView
{
    protected Rect _masterRect;
    protected Rect _baseRect;
    protected Rect _backButtonRect;

    private const float _backButtonHeight = 20f;
    
    public Rect MasterRect
    {
        set
        {
            _masterRect = value;
            CreateBaseRect();
        }
    }

    protected abstract void DrawWindow(int id);

    protected virtual void CreateBaseRect()
    {
        
    }
}
