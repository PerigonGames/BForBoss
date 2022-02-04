using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PerigonGames;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneWizard : EditorWindow
{
    private static bool _needsToRefreshElements = false;
    
    private SceneWizardConfig _config;
    private Vector2 _scrollView;

    [MenuItem("BForBoss/SceneSwitcher")]
    private static void Init()
    {
        SceneWizard window = (SceneWizard)GetWindow(typeof(SceneWizard));
        window.Show();
    }

    private void OnSceneDirtied(Scene scene)
    {
        _needsToRefreshElements = true;
    }

    private void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
    {
        _needsToRefreshElements = true;
    }
    
    private void OnSceneSaved(Scene scene)
    {
        _needsToRefreshElements = true;
    }

    private void RefreshConfig()
    {
        if (_config != null)
        {
            return;
        }
        
        
        if (!File.Exists(Application.dataPath + "/SceneWizard/SceneWizard_Config.asset"))
        {
            SceneWizardConfig newConfig = ScriptableObject.CreateInstance<SceneWizardConfig>();
            AssetDatabase.CreateAsset(newConfig, "Assets/SceneWizard/SceneWizard_Config.asset");
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
        }

        string[] foundAssets = AssetDatabase.FindAssets("SceneWizard_Config", new[] { "Assets/" });
        if (foundAssets.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
            _config = AssetDatabase.LoadAssetAtPath<SceneWizardConfig>(path);
        }
    }

    private void ReloadScenes()
    {
        _config.scenes = new List<SceneConfigSetup>();
        LoadFromPath(_config.folderPath);
    }

    private void LoadFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        string[] files = Directory.GetFiles(path);
        foreach (var fp in files)
        {
            if (fp.Contains(".unity"))
            {
                var assetPath = "Assets" + fp.Split(new string[] {"Assets"}, StringSplitOptions.None)[1];

                var sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

                if (sceneLoaded != null)
                {
                    var pathSplit = assetPath.Replace("\\", "/").Split('/');

                    SceneConfigSetup scs = new SceneConfigSetup()
                    {
                        name = sceneLoaded.name,
                        path = assetPath,
                        parentFolder = pathSplit[pathSplit.Length - 2]
                    };

                    _config.scenes.Add(scs);
                }
            }
        }


        if (_config.allowSubfolders)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                LoadFromPath(dir);
            }
        }
    }

    private void OnFocus()
    {
        if (_needsToRefreshElements)
        {
            RefreshElements();
            _needsToRefreshElements = false;
        }
    }

    private void RefreshElements()
    {
        RefreshConfig();
        ReloadScenes();
    }

    private void OnGUI()
    {
        if (_needsToRefreshElements)
        {
            RefreshElements();
            _needsToRefreshElements = false;
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Scene Wizard", EditorStyles.boldLabel);

        GUILayout.Space(20);

        TextAnchor prevAlignment = GUI.skin.button.alignment;
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {

            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
            {
                if (string.IsNullOrEmpty(_config.folderPath))
                {
                    if (GUILayout.Button("Select a folder"))
                    {
                        string path = EditorUtility.OpenFolderPanel("Select a folder to load Scenes from", "", "");
                        _config.folderPath = path;
                    }
                }
                else
                {
                    if (GUILayout.Button("Change folder"))
                    {
                        string path = EditorUtility.OpenFolderPanel("Select a folder to load Scenes from", "", "");

                        if (!string.IsNullOrEmpty(path))
                        {
                            _config.folderPath = path;
                            _config.scenes = new List<SceneConfigSetup>();
                            _needsToRefreshElements = true;
                        }
                    }

                    if (GUILayout.Button("Refresh"))
                    {
                        ReloadScenes();
                    }

                    if (GUILayout.Button("Clear"))
                    {
                        _config.folderPath = string.Empty;
                        _config.scenes = new List<SceneConfigSetup>();
                    }
                }
            }

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                _config.allowSubfolders = EditorGUILayout.Toggle("Allow Subfolders", _config.allowSubfolders);

                if (changeCheckScope.changed)
                {
                    _needsToRefreshElements = true;
                }
            }
        }
        
        if (!_config.scenes.IsNullOrEmpty())
        {
            string lastFolderName = string.Empty;
            GUILayout.Space(15);
            
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollView))
                {
                    _scrollView = scrollViewScope.scrollPosition;
                    EditorGUI.indentLevel++;
                    foreach (var scene in _config.scenes)
                    {
                        if (scene.parentFolder != lastFolderName)
                        {
                            if (string.IsNullOrEmpty(lastFolderName))
                            {
                                GUILayout.Space(8);
                            }

                            EditorGUI.indentLevel--;
                            
                            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                            {
                                GUILayout.Label(scene.parentFolder, EditorStyles.boldLabel);
                            }
                            
                            lastFolderName = scene.parentFolder;
                            EditorGUI.indentLevel++;
                        }
                
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            SceneAsset sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                            
                            using (new EditorGUI.DisabledGroupScope(true))
                            {
                                EditorGUILayout.ObjectField(GUIContent.none, sceneLoaded, typeof(SceneAsset), false);
                            }
                            
                            if (GUILayout.Button("Open Single"))
                            {
                                if (SceneManager.GetActiveScene().isDirty)
                                {
                                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                    {
                                        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                                    }
                                }
                                else
                                {
                                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                                }
                            }

                            if (GUILayout.Button("Open Additively"))
                            {
                                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                            }
                        }
                    }
                }
            }
        }

        GUI.skin.button.alignment = prevAlignment;
    }

    private void OnEnable()
    {
        RefreshElements();
        EditorSceneManager.newSceneCreated += OnNewSceneCreated;
        EditorSceneManager.sceneDirtied += OnSceneDirtied;
        EditorSceneManager.sceneSaved += OnSceneSaved;
    }

    private void OnDestroy()
    {
        EditorSceneManager.newSceneCreated -= OnNewSceneCreated;
        EditorSceneManager.sceneDirtied -= OnSceneDirtied;
        EditorSceneManager.sceneSaved -= OnSceneSaved;
    }
}
