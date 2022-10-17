using UnityEngine;

namespace BForBoss
{
    public abstract class DebugView
    {
        public bool IsShown = false;
        
        protected string _windowHeader = "Debug Options";
        protected Vector2 _scrollPosition = Vector2.zero;

        private Rect _masterRect;
        private Rect _contentRect;
        private Rect _titleRect;
        private Rect _backButtonRect;
        
        private const float BACK_BUTTON_HEIGHT = 20f;
        private const float TITLE_HEIGHT = 20f;

        public Rect MasterRect
        {
            set
            {
                _masterRect = value;
                InitializeRects();
            }
        }
        
        public abstract string PrettyName { get; }
        
        public virtual void ResetData()
        {
            IsShown = false;
        }

        protected DebugView(Rect masterRect)
        {
            _masterRect = masterRect;
            InitializeRects();
        }
        
        public void DrawGUI()
        {
            if (!IsShown)
            {
                return; 
            }
            
            using (new GUILayout.AreaScope(_masterRect, string.Empty, GUI.skin.box))
            {
                DrawBackButton();
                DrawTitle();
                DrawWindow();
            }
        }

        protected virtual void DrawTitleContent()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(PrettyName);
                GUILayout.FlexibleSpace();
            }
        }
        
        protected abstract void DrawWindowContent();
        

        protected virtual void InitializeRects()
        {
            _backButtonRect = new Rect(_masterRect.x, 0, _masterRect.width, BACK_BUTTON_HEIGHT);
            _titleRect = new Rect(_masterRect.x, BACK_BUTTON_HEIGHT, _masterRect.width, TITLE_HEIGHT);
            _contentRect = new Rect(_masterRect.x, BACK_BUTTON_HEIGHT + TITLE_HEIGHT, _masterRect.width,
                _masterRect.height - BACK_BUTTON_HEIGHT - TITLE_HEIGHT);
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

        private void DrawTitle()
        {
            using (new GUILayout.AreaScope(_titleRect))
            {
                DrawTitleContent();
            }
        }

        private void DrawWindow()
        {
            using (new GUILayout.AreaScope(_contentRect))
            {
                using (var scrollScope = new GUILayout.ScrollViewScope(_scrollPosition))
                {
                    _scrollPosition = scrollScope.scrollPosition;
                    DrawWindowContent();
                }
            }
        }
    }
}
