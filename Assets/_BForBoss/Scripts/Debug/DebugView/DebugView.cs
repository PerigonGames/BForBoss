using UnityEngine;

namespace BForBoss
{
    public abstract class DebugView
    {
        public bool IsInitialized = false;
        
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
        
        public virtual void ResetData()
        {
            IsInitialized = false;
        }

        protected DebugView(Rect masterRect)
        {
            _masterRect = masterRect;
            CreateBaseRect();
            IsInitialized = true;
        }
        
        public void DrawGUI()
        {
            DrawBackButton();
            DrawWindow();
        }
        

        protected abstract void DrawWindow();

        protected virtual void CreateBaseRect()
        {
            _baseRect = new Rect(_masterRect.x, _backButtonHeight, _masterRect.width,
                _masterRect.height - _backButtonHeight);
            _backButtonRect = new Rect(_masterRect.x, 0, _masterRect.width, _backButtonHeight);
        }
        
        private void DrawBackButton()
        {
            using (new GUILayout.AreaScope(_backButtonRect))
            {
                if (GUILayout.Button("Back", GUILayout.Width(_backButtonRect.width)))
                {
                    ResetData();
                }
            }
        }
    }
}
