using UnityEditor;
using UnityEngine;

namespace BForBoss
{
    [CustomPropertyDrawer(typeof(Resolve))]
    public class ResolverPropertyDrawer : PropertyDrawer
    {
        private const float RESOLVER_WIDTH = 20f;
        private const float OBJECT_FIELD_BUTTON_WIDTH = 18f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.Set(position.x, position.y, position.width - RESOLVER_WIDTH, position.height);

            EditorGUI.PropertyField(position,property,label);
            
            if (GUI.Button(new Rect(position.width + OBJECT_FIELD_BUTTON_WIDTH, position.y, RESOLVER_WIDTH, position.height), "X"))
            {
                Debug.Log("Working Implementation");
            }
        }
    }
}
