using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PerigonGames;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BForBoss
{
    [CustomPropertyDrawer(typeof(ResolveAttribute))]
    public class ResolverPropertyDrawer : PropertyDrawer
    {
        private const float RESOLVER_WIDTH = 80f;
        private const float OBJECT_FIELD_BUTTON_WIDTH = 18f;
        private const float RESOLVER_BUTTON_WIDTH = 20f;
        private const float ENUMERABLE_BOX_OFFSET = 25f;
        private const float STRUCT_OFFSET = 15f;

        private bool _isContentInitialized = false;
        private bool _includeInactiveGameObjects;
        
        private bool _isList = false;
        private bool _isArray = false;
        private bool _isDeclaredInStruct = false;
        
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
        
        private enum ResolveType
        {
            FromChildren,
            FromParent,
            FromSelf
        }
        
        
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

                ResolveAttribute resolveAttributeAttribute = (ResolveAttribute) attribute;
                _includeInactiveGameObjects = resolveAttributeAttribute.IncludeInactive;
            }
            
            //The way to tell if property is an array/list or not via property.propertypath
            //https://answers.unity.com/questions/603882/serializedproperty-isnt-being-detected-as-an-array.html

            Type fieldType = fieldInfo.FieldType;
            _isArray = fieldType.IsArray;
            _isList = fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>);
            
            //Check if field is being declared within Struct
            Type declaringType = fieldInfo.DeclaringType;
            _isDeclaredInStruct = declaringType != null && declaringType.IsValueType && !declaringType.IsPrimitive;

            position.Set(position.x, position.y, position.width - RESOLVER_WIDTH, position.height);
            EditorGUI.PropertyField(position,property,label);

            CreateRects(position);

            DrawResolverButtons(property);
        }

        private void DrawResolverButtons(SerializedProperty property)
        {
            // Self (G) | Children (C) | Parent (P) | (OPTIONAL) Sibilings (R) | None (X)
            
            if (GUI.Button(_childResolveRect, _childResolveContent))
            {
                SetResolvedComponent(property, ResolveType.FromChildren);
            }
            
            if (GUI.Button(_parentResolveRect, _parentResolveContent))
            {
                SetResolvedComponent(property, ResolveType.FromParent);
            }
            
            if (GUI.Button(_selfResolveRect, _selfResolveContent))
            {
                SetResolvedComponent(property, ResolveType.FromSelf);
            }
            
            if (GUI.Button(_clearContentRect, _clearContent))
            {
                Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "Cleared Resolved Component");
                property.objectReferenceValue = null;
            }
        }

        private void SetResolvedComponent(SerializedProperty property, ResolveType resolveType)
        {
            Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "Auto Resolved Component");
            MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;
            Type fieldType = _isArray ? fieldInfo.FieldType.GetElementType() :
                _isList ? fieldInfo.FieldType.GetGenericArguments().Single() : fieldInfo.FieldType;
            
            Component[] components = null;
            switch (resolveType)
            {
                case ResolveType.FromChildren:
                {
                    components = mb.GetComponentsInChildren(fieldType, _includeInactiveGameObjects)
                        .Where(component => component.gameObject != mb.gameObject).ToArray();
                    break;
                }
                case ResolveType.FromParent:
                {
                    components = mb.GetComponentsInParent(fieldType, _includeInactiveGameObjects)
                        .Where(component => component.gameObject != mb.gameObject).ToArray();
                    break;
                }
                case ResolveType.FromSelf:
                {
                    components = mb.GetComponents(fieldType);
                    break;
                }
            }

            if (components.IsNullOrEmpty())
            {
                return;
            }
            
            if (components.Length > 1)
            {
                void OnItemSelected(int componentID)
                {
                    property.serializedObject.Update();
                    property.objectReferenceInstanceIDValue = componentID;
                    property.serializedObject.ApplyModifiedProperties();
                }

                var dropdown = new ComponentResolverDropdown(components, OnItemSelected , new AdvancedDropdownState());
                dropdown.Show(_childResolveRect);
            }
            else
            {
                property.serializedObject.Update();
                property.objectReferenceValue = components[0];
                property.serializedObject.ApplyModifiedProperties();
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
            float depthOffset = (_isList || _isArray)
                ? _isDeclaredInStruct ? ENUMERABLE_BOX_OFFSET + STRUCT_OFFSET : ENUMERABLE_BOX_OFFSET
                : 0;
            
            _childResolveRect = new Rect(propertyRect.width + OBJECT_FIELD_BUTTON_WIDTH + depthOffset, propertyRect.y, RESOLVER_BUTTON_WIDTH, propertyRect.height);
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
            
            //Todo: Find a long term solution to not allow attribute being placed on structs moving forward 
            if (fieldType.IsValueType && !fieldType.IsPrimitive)
            {
                Debug.LogError("Unable to Resolve Struct Declared fields directly." +
                               "\n Please add [Resolve] onto specific fields within the struct instead");
            }

            //List/Array property.propertyType describes the element type not the IEnumerable Type (i.e. int instead of List<int>)
            return !fieldType.IsValueType && property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}

public class ComponentResolverDropdown : AdvancedDropdown
{
    private Component[] _components;
    private Action<int> _onItemSelected;
    private readonly Vector2 MINIMUM_SIZE = new Vector2(40f,200f);

    public ComponentResolverDropdown(Component[] components, Action<int> onItemSelected, AdvancedDropdownState state) : base(state)
    {
        _components = components;
        _onItemSelected = onItemSelected;
        minimumSize = MINIMUM_SIZE;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        AdvancedDropdownItem root = new AdvancedDropdownItem("Components");

        foreach (Component component in _components)
        {
            AdvancedDropdownItem dropdownItem = new AdvancedDropdownItem($"{component.gameObject.name} : {component.GetType().Name}");
            dropdownItem.id = component.GetInstanceID();
            
            root.AddChild(dropdownItem);
        }
        
        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        _onItemSelected?.Invoke(item.id);
    }
}
