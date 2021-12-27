using System;
using System.Linq;
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
            if (!IsResolvable(property))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            position.Set(position.x, position.y, position.width - RESOLVER_WIDTH, position.height);
            EditorGUI.PropertyField(position,property,label);
            
            if (GUI.Button(new Rect(position.width + OBJECT_FIELD_BUTTON_WIDTH, position.y, RESOLVER_WIDTH, position.height), "X"))
            {
                Debug.Log("Working Implementation");
            }
        }

        private bool IsResolvable(SerializedProperty property)
        {
            return IsBeingInspected() && IsReferenceType(property);
        }

        private bool IsBeingInspected()
        {
            if (fieldInfo.IsNotSerialized || fieldInfo.IsStatic)
            {
                return false;
            }

            bool IsSerializeField = fieldInfo.CustomAttributes.Any(attributeData => attributeData.AttributeType == typeof(SerializeField));
            return (fieldInfo.IsPublic || ((fieldInfo.IsFamily || fieldInfo.IsPrivate) && IsSerializeField));
        }

        private bool IsReferenceType(SerializedProperty property)
        {
            Type fieldType = fieldInfo.FieldType;
            return !fieldType.IsValueType && fieldType != typeof(string) && property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}
