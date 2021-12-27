using System;
using System.Linq;
using System.Reflection;
using PerigonGames;
using UnityEditor;
using UnityEngine;

namespace BForBoss
{
    [CustomPropertyDrawer(typeof(Resolve))]
    public class ResolverPropertyDrawer : PropertyDrawer
    {
        private const float RESOLVER_WIDTH = 80f;
        private const float OBJECT_FIELD_BUTTON_WIDTH = 18f; //Change depending on field Depth
        private const float RESOLVER_BUTTON_WIDTH = 20f;

        private bool _isContentInitialized = false;
        private bool _includeInactiveGameObjects;
        
        //Button Content
        private GUIContent _childResolveContent;
        // private GUIContent _sceneResolveContent;
        private GUIContent _parentResolveContent;
        // private GUIContent _siblingResolveContent;
        private GUIContent _selfResolveContent;
        private GUIContent _clearContent;
        
        //Button Rects
        private Rect _childResolveRect;
        // private Rect _sceneResolveRect;
        private Rect _parentResolveRect;
        // private Rect _siblingResolveRect;
        private Rect _selfResolveRect;
        private Rect _clearContentRect;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsResolvable(property))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!_isContentInitialized)
            {
                InitializeContent();

                Resolve resolveAttribute = (Resolve) attribute;
                _includeInactiveGameObjects = resolveAttribute.IncludeInactive;
            }
            
            position.Set(position.x, position.y, position.width - RESOLVER_WIDTH, position.height);
            EditorGUI.PropertyField(position,property,label);
            
            CreateRects(position);

            DrawResolverButtons(property);
        }

        private void DrawResolverButtons(SerializedProperty property)
        {
            int depth = property.depth;
            // Self (G) | Children (C) | (OPTIONAL) Parent (P) | (OPTIONAL) Sibilings (R) | None (X)
            
            if (GUI.Button(_childResolveRect, _childResolveContent))
            {
                MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;

                var components = mb.GetComponentsInChildren(fieldInfo.FieldType, _includeInactiveGameObjects)
                    .Where(component => component.gameObject != mb.gameObject).ToArray();
                
                property.objectReferenceValue = components.IsNullOrEmpty() ? null : components[0];
            }
            
            if (GUI.Button(_parentResolveRect, _parentResolveContent))
            {
                MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;

                var components = mb.GetComponentsInParent(fieldInfo.FieldType, _includeInactiveGameObjects)
                    .Where(component => component.gameObject != mb.gameObject).ToArray();
                
                property.objectReferenceValue = components.IsNullOrEmpty() ? null : components[0];
            }
            
            if (GUI.Button(_selfResolveRect, _selfResolveContent))
            {
                MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;
            
                var components = mb.GetComponents(fieldInfo.FieldType);
                property.objectReferenceValue = components.IsNullOrEmpty() ? null : components[0];
            }
            
            if (GUI.Button(_clearContentRect, _clearContent))
            {
                property.objectReferenceValue = null;
            }
        }

        private void InitializeContent()
        {
            _childResolveContent = new GUIContent("C", "Get Component from Children");
            _parentResolveContent = new GUIContent("P", "Get Component from Parent");
            _selfResolveContent = new GUIContent("G", "Get Component from Self");
            _clearContent = new GUIContent("X", "Clear Component");

            _isContentInitialized = true;
        }

        private void CreateRects(Rect propertyRect)
        {
            _childResolveRect = new Rect(propertyRect.width + OBJECT_FIELD_BUTTON_WIDTH, propertyRect.y, RESOLVER_BUTTON_WIDTH, propertyRect.height);
            _parentResolveRect = new Rect(_childResolveRect.x + _childResolveRect.width, propertyRect.y, RESOLVER_BUTTON_WIDTH, propertyRect.height);
            _selfResolveRect = new Rect(_parentResolveRect.x + _parentResolveRect.width, propertyRect.y, RESOLVER_BUTTON_WIDTH, propertyRect.height);
            _clearContentRect = new Rect(_selfResolveRect.x + _selfResolveRect.width, propertyRect.y, RESOLVER_BUTTON_WIDTH, propertyRect.height);
        }

        private bool IsResolvable(SerializedProperty property)
        {
            return IsBeingInspected() && IsAppropriateType(property);
        }

        private bool IsBeingInspected()
        {
            if (fieldInfo.IsNotSerialized || fieldInfo.IsStatic)
            {
                return false;
            }

            bool isSerializeField = fieldInfo.GetCustomAttribute(typeof(SerializeField)) != null;
            return (fieldInfo.IsPublic || ((fieldInfo.IsFamily || fieldInfo.IsPrivate) && isSerializeField));
        }

        private bool IsAppropriateType(SerializedProperty property)
        {
            Type fieldType = fieldInfo.FieldType;
            
            //List/Array property.propertyType describes the element type not the IEnumerable Type (i.e. int instead of List<int>)
            bool isAppropriateType = !fieldType.IsValueType && fieldType != typeof(string) && property.propertyType == SerializedPropertyType.ObjectReference;
            
            return isAppropriateType;
        }
    }
}
