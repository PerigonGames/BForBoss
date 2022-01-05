using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Perigon.Utility
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
        private GUIContent _parentResolveContent;
        private GUIContent _selfResolveContent;
        private GUIContent _clearContent;
        
        //Button Rects
        private Rect _childResolveRect;
        private Rect _parentResolveRect;
        private Rect _selfResolveRect;
        private Rect _clearContentRect;

        private GUIStyle _clearContentGUIStyle;
        
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
            //https://stackoverflow.com/questions/1827425/how-to-check-programmatically-if-a-type-is-a-struct-or-a-class
            _isDeclaredInStruct = declaringType != null && declaringType.IsValueType && !declaringType.IsPrimitive &&
                                  declaringType != typeof(decimal) && declaringType != typeof(DateTime) &&
                                  !declaringType.IsEnum;

            position.Set(position.x, position.y, position.width - RESOLVER_WIDTH, position.height);
            EditorGUI.PropertyField(position,property,label);

            CreateRects(position);

            DrawResolverButtons(property);
        }

        private void DrawResolverButtons(SerializedProperty property)
        {
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
            
            if (GUI.Button(_clearContentRect, _clearContent, _clearContentGUIStyle))
            {
                Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "Cleared Resolved Component");
                property.objectReferenceValue = null;
            }
        }

        private void SetResolvedComponent(SerializedProperty property, ResolveType resolveType)
        {
            MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;
            Type fieldType = _isArray ? fieldInfo.FieldType.GetElementType() :
                _isList ? fieldInfo.FieldType.GetGenericArguments().Single() : fieldInfo.FieldType;

            ComponentInformation[] components = fieldType == typeof(GameObject)
                ? GetResolvedGameObjects(mb, resolveType)
                : GetResolvedComponents(mb, fieldType, resolveType);

            if (components.IsNullOrEmpty())
            {
                return;
            }
            
            void OnItemSelected(int componentID)
            {
                property.serializedObject.Update();
                property.objectReferenceInstanceIDValue = componentID;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "Auto Resolved Component");

            if (components.Length > 1)
            {
                var dropDown = new ResolverDropDown(components, OnItemSelected, new AdvancedDropdownState());
                dropDown.Show(_childResolveRect);
            }
            else
            {
                property.serializedObject.Update();
                property.objectReferenceInstanceIDValue = components[0].instanceID;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private ComponentInformation[] GetResolvedGameObjects(MonoBehaviour mb, ResolveType resolveType)
        {
            List<ComponentInformation> components = new List<ComponentInformation>();

            switch (resolveType)
            {
                case ResolveType.FromChildren:
                {
                    Transform[] transforms = mb.GetComponentsInChildren<Transform>(_includeInactiveGameObjects)
                        .Where(transform => transform.gameObject != mb.gameObject).ToArray();

                    foreach (Transform transform in transforms)
                    {
                        components.Add(new ComponentInformation(transform.gameObject));
                    }
                    
                    break;
                }
                case ResolveType.FromParent:
                {
                    Transform[] transforms = mb.GetComponentsInParent<Transform>(_includeInactiveGameObjects)
                        .Where(transform => transform.gameObject != mb.gameObject).ToArray();

                    foreach (Transform transform in transforms)
                    {
                        components.Add(new ComponentInformation(transform.gameObject));
                    }
                    
                    break;
                }
                case ResolveType.FromSelf:
                {
                    components = new List<ComponentInformation>{new ComponentInformation(mb.gameObject)};
                    break;
                }
            }

            return components.ToArray();
        }

        private ComponentInformation[] GetResolvedComponents(MonoBehaviour mb, Type fieldType, ResolveType resolveType)
        {
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

            if (components == null)
            {
                return null;
            }
            
            ComponentInformation[] componentInformations = new ComponentInformation[components.Length];

            for (int i = 0; i < componentInformations.Length; i++)
            {
                componentInformations[i] = new ComponentInformation(components[i]);
            }

            return componentInformations;
        }
        

        private void InitializeContent()
        {
            _childResolveContent = new GUIContent("C", "Get Component from Children");
            _parentResolveContent = new GUIContent("P", "Get Component from Parent");
            _selfResolveContent = new GUIContent("S", "Get Component from Self");
            _clearContent = EditorGUIUtility.IconContent("d_winbtn_mac_close_h@2x", "|Clear Component");

            _clearContentGUIStyle = new GUIStyle()
            {
                stretchHeight = true,
                stretchWidth = true
            };

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
                Debug.LogWarning("Unable to Resolve Struct Declared fields directly." +
                               "\n Please add [Resolve] onto specific fields within the struct instead");
            }

            bool isFieldDeclaredInScriptableObject = fieldInfo.DeclaringType.InheritsFrom<ScriptableObject>();

            //List/Array property.propertyType describes the element type not the IEnumerable Type (i.e. int instead of List<int>)
            return !fieldType.IsValueType && !isFieldDeclaredInScriptableObject &&
                   !fieldType.InheritsFrom<ScriptableObject>() &&
                   property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}

internal class ResolverDropDown : AdvancedDropdown
{
    private readonly Vector2 MINIMUM_SIZE = new Vector2(40f,200f);
    private ComponentInformation[] _components;
    private Action<int> _onItemSelected;
    
    public ResolverDropDown(ComponentInformation[] components, Action<int> onItemSelected, AdvancedDropdownState state) : base(state)
    {
        _components = components;
        _onItemSelected = onItemSelected;
        minimumSize = MINIMUM_SIZE;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        AdvancedDropdownItem root = new AdvancedDropdownItem("Components");

        foreach (ComponentInformation component in _components)
        {
            AdvancedDropdownItem dropDownItem = new AdvancedDropdownItem($"{component.name} : {component.type.Name}");
            dropDownItem.id = component.instanceID;
            
            root.AddChild(dropDownItem);
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        _onItemSelected.Invoke(item.id);
    }
}

internal readonly struct ComponentInformation
{
    public readonly string name;
    public readonly Type type;
    public readonly int instanceID;
    
    public ComponentInformation(Component component)
    {
        name = component.name;
        type = component.GetType();
        instanceID = component.GetInstanceID();
    }

    public ComponentInformation(GameObject gameObject)
    {
        name = gameObject.name;
        type = gameObject.GetType();
        instanceID = gameObject.GetInstanceID();
    }
}
