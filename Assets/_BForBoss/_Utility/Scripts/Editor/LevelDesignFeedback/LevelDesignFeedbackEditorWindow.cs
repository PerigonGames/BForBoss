using UnityEditor;
using UnityEngine;

public class LevelDesignFeedbackEditorWindow : EditorWindow
{
    private static LevelDesignFeedbackEditorWindow _window = null;

    private const float ELEMENT_SPACING = 5f;
    private const int NUMBER_OF_FEEDBACK_LINES = 5;
    
    private Texture2D _image;
    private string _title;
    private string _feedback;

    [MenuItem("BForBoss/Level Design/Add Feedback")]
    public static void OpenWindow()
    {
        _window = (LevelDesignFeedbackEditorWindow) GetWindow(typeof(LevelDesignFeedbackEditorWindow));
        _window.titleContent = new GUIContent("Add Level Design Feedback");
        _window.minSize = new Vector2(250, 600);
        _window.maxSize = new Vector2(400, 950);
        _window.Show();
    }

    private void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            DrawPreviewImage();

            EditorGUILayout.Space(ELEMENT_SPACING);

            DrawDescription();
            
            GUILayout.FlexibleSpace();

            DrawButtons();
            
            EditorGUILayout.Space(ELEMENT_SPACING);
        }
    }

    private void DrawPreviewImage()
    {
        Rect previewImageRect;
        EditorGUILayout.Space(ELEMENT_SPACING);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
                
            previewImageRect = new Rect(position.width * 0.05f,position.y, position.width * 0.9f, position.height * 0.4f);
                
            if (_image == null)
            {
                EditorGUI.DrawRect(previewImageRect, Color.gray);
            }
            else
            {
                Color guiColor = GUI.color;
                GUI.color = Color.clear;
                EditorGUI.DrawTextureTransparent(previewImageRect, _image, ScaleMode.ScaleToFit, 0);
                GUI.color = guiColor;
            }

            if (_image != null && WasElementDoubleClicked(previewImageRect))
            {
                Debug.Log("How you Doing?");
                // new SceneViewCameraWindow(SceneView.currentDrawingSceneView)
                
                PopupWindow.Show(previewImageRect, new FeedbackScreenshotEditContent(_image));
                
            }

            // if (Event.current.type == EventType.Repaint)
            // {
            //     previewImageRect = GUILayoutUtility.GetLastRect();
            // }

            GUILayout.FlexibleSpace();
        }


        EditorGUILayout.Space(previewImageRect.y + previewImageRect.height);
        EditorGUILayout.Space(ELEMENT_SPACING);

        _image = (Texture2D) EditorGUILayout.ObjectField(_image, typeof(Texture2D), false);
    }

    private void DrawDescription()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 35f;
            EditorGUILayout.PrefixLabel("Title", GUI.skin.button, EditorStyles.boldLabel);
            _title = EditorGUILayout.TextField(_title, GUILayout.ExpandWidth(true));
            EditorGUIUtility.labelWidth = labelWidth;
        }
            
        EditorGUILayout.Space(ELEMENT_SPACING);
        EditorGUILayout.LabelField("Feedback",EditorStyles.boldLabel);
        _feedback = EditorGUILayout.TextArea(_feedback, GUI.skin.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * NUMBER_OF_FEEDBACK_LINES));
    }

    private void DrawButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.Space(ELEMENT_SPACING);

            if (GUILayout.Button("Close"))
            {
                _window.Close();
            }
                
            GUILayout.FlexibleSpace();

            GUI.enabled = _image != null && !string.IsNullOrEmpty(_title) && !string.IsNullOrEmpty(_feedback);
                
            if (GUILayout.Button("Create Feedback"))
            {
                Debug.Log("Feedback added");
            }

            GUI.enabled = true;

            EditorGUILayout.Space(ELEMENT_SPACING);
        }
    }

    private bool WasElementDoubleClicked(Rect elementRect)
    {
        Event evt = Event.current;
        bool hasMouseClick = evt.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive, elementRect)) == EventType.MouseDown;

        return hasMouseClick && evt.button == 0 && evt.clickCount == 2 && elementRect.Contains(evt.mousePosition);
    }
}

public class FeedbackScreenshotEditContent : PopupWindowContent
{
    private Texture2D _screenshot;
    
    private readonly Vector2 _windowSize;

    public FeedbackScreenshotEditContent(Texture2D screenShot)
    {
        _screenshot = screenShot;
        
        float popupWidth = ((float)Display.main.systemWidth / 2f) * EditorGUIUtility.pixelsPerPoint;
        float popupHeight = ((float)Display.main.systemHeight / 2f) * EditorGUIUtility.pixelsPerPoint;
        _windowSize = new Vector2(popupWidth, popupHeight);
    }

    public override void OnGUI(Rect rect)
    {
        
    }

    public override Vector2 GetWindowSize()
    {
        return _windowSize;
    }
}
