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
                    EditorGUI.DrawTextureTransparent(previewImageRect, _image, ScaleMode.ScaleToFit, 0);
                }
                
                GUILayout.FlexibleSpace();
            }


            EditorGUILayout.Space(previewImageRect.y + previewImageRect.height);
            EditorGUILayout.Space(ELEMENT_SPACING);

            _image = (Texture2D) EditorGUILayout.ObjectField(_image, typeof(Texture2D), false);

            EditorGUILayout.Space(ELEMENT_SPACING);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                _title = EditorGUILayout.TextField("Title", _title, GUILayout.ExpandWidth(true));
            }
            
            EditorGUILayout.Space(ELEMENT_SPACING);
            EditorGUILayout.LabelField("Feedback",EditorStyles.boldLabel);
            _feedback = EditorGUILayout.TextArea(_feedback, GUI.skin.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * NUMBER_OF_FEEDBACK_LINES));
            
            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space(ELEMENT_SPACING);

                if (GUILayout.Button("Close"))
                {
                    _window.Close();
                }
                
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create Feedback"))
                {
                    Debug.Log("Feedback added");
                }

                EditorGUILayout.Space(ELEMENT_SPACING);
            }
            
            EditorGUILayout.Space(ELEMENT_SPACING);
        }
    }
}
