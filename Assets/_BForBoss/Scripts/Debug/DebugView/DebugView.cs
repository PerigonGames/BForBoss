using UnityEngine;

namespace BForBoss
{
    public abstract class DebugView
    {
        protected Rect _masterRect;
        protected Rect _baseRect;
        protected Rect _backButtonRect;

        protected bool _isInitialized = false;

        private const float _backButtonHeight = 20f;

        public Rect MasterRect
        {
            set
            {
                _masterRect = value;
                CreateBaseRect();
            }
        }

        protected DebugView(Rect masterRect)
        {
            _masterRect = masterRect;
            CreateBaseRect();
            DebugWindow.OnGUIUpdate += DrawGUI;
        }

        protected virtual void Initialize(Rect masterRect)
        {
            if (_isInitialized)
            {
                return;
            }
            
            _masterRect = masterRect;
            _isInitialized = true;
        }

        public virtual void ResetData()
        {
            _isInitialized = false;
            DebugWindow.OnGUIUpdate -= OnGUIUpdate;
        }

        protected abstract void DrawWindow();

        protected virtual void CreateBaseRect()
        {
            _baseRect = new Rect(_masterRect.x, _backButtonHeight, _masterRect.width,
                _masterRect.height - _backButtonHeight);
            _backButtonRect = new Rect(_masterRect.x, 0, _masterRect.width, _backButtonHeight);
        }

        protected virtual void DrawGUI()
        {
            DrawBackButton();
            DrawWindow();
        }


        private void OnGUIUpdate()
        {
            DrawBackButton();
            DrawGUI();
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
