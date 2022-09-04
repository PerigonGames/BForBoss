using System;
using UnityEngine;
using UnityEditor;

namespace Perigon.Weapons.Editor
{
    [CustomEditor(typeof(BulletSpawner))]
    public class BulletSpawnerInspector : UnityEditor.Editor
    {
        private SerializedProperty _prefabListProp;
        private SerializedProperty _layerProperty;
        private SerializedProperty _layerMaskProperty;
        private string[] _bulletTypeNames;

        private static bool _showPrefabs = false;
        
        private void OnEnable()
        {
            _prefabListProp = serializedObject.FindProperty("_bulletPrefabs");
            _layerProperty = serializedObject.FindProperty("_layer");
            _layerMaskProperty = serializedObject.FindProperty("_layerMask");
            _bulletTypeNames = Enum.GetNames(typeof(BulletTypes));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var numNames = _bulletTypeNames.Length;
            if (_prefabListProp.arraySize != numNames)
            {
                while (_prefabListProp.arraySize > numNames)
                {
                    _prefabListProp.DeleteArrayElementAtIndex(numNames);
                }

                for (int i = _prefabListProp.arraySize; i < numNames; i++)
                {
                    _prefabListProp.InsertArrayElementAtIndex(i);
                }
            }

            _showPrefabs = EditorGUILayout.Foldout(_showPrefabs, new GUIContent("Bullet Prefabs"));
            if (_showPrefabs)
            {
                for (int i = 0; i < numNames; i++)
                {
                    EditorGUILayout.PropertyField(_prefabListProp.GetArrayElementAtIndex(i),
                        new GUIContent($"{_bulletTypeNames[i]} Bullet Prefab"));
                }
            }

            EditorGUILayout.PropertyField(_layerProperty);
            EditorGUILayout.PropertyField(_layerMaskProperty);

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
