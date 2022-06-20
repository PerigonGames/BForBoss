using UnityEngine;

namespace BForBoss
{
    public abstract class DebugView
    {
        public bool IsInitialized = false;
        
        protected Rect _masterRect;
        protected Rect _baseRect;
        protected Rect _backButtonRect;

        protected string _windowHeader = "Debug Options";
        
        private const float BACK_BUTTON_HEIGHT = 20f;

        public Rect MasterRect
        {
            set
            {
                _masterRect = value;
                CreateBaseRect();
            }
        }
        
        public abstract string PrettyName { get; }
        
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
            if (!IsInitialized)
            {
                return;
            }
            
            using (new GUILayout.AreaScope(_masterRect, string.Empty, GUI.skin.box))
            {
                DrawBackButton();
                DrawWindow();
            }
        }
        

        protected abstract void DrawWindow();

        protected virtual void CreateBaseRect()
        {
            _baseRect = new Rect(_masterRect.x, BACK_BUTTON_HEIGHT, _masterRect.width,
                _masterRect.height - BACK_BUTTON_HEIGHT);
            _backButtonRect = new Rect(_masterRect.x, 0, _masterRect.width, BACK_BUTTON_HEIGHT);
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
