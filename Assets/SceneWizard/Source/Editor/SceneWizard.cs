using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneWizard : EditorWindow
{
    SceneWizardConfig config;

    Vector2 scrollView;

    [MenuItem("Window/EMD Tools/Scene Wizard")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SceneWizard window = (SceneWizard)EditorWindow.GetWindow(typeof(SceneWizard));
        window.Show();
    }

    void RefreshConfig()
    {
        if (config == null)
        {
            if (!File.Exists(Application.dataPath + "/SceneWizard/SceneWizard_Config.asset"))
            {
                SceneWizardConfig newConfig = ScriptableObject.CreateInstance<SceneWizardConfig>();
                AssetDatabase.CreateAsset(newConfig, "Assets/SceneWizard/SceneWizard_Config.asset");
                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();
            }

            var foundAssets = AssetDatabase.FindAssets("SceneWizard_Config", new[] { "Assets/" });
            if (foundAssets.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
                config = AssetDatabase.LoadAssetAtPath<SceneWizardConfig>(path);
            }
        }
    }

    void ReloadScenes()
    {
        config.scenes = new List<SceneConfigSetup>();
        LoadFromPath(config.folderPath);
    }

    void LoadFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        string[] files = Directory.GetFiles(path);
        foreach (var fp in files)
        {
            if (fp.Contains(".unity"))
            {
                //string[] splits = fp.Split(new string[] {"Assets"}, StringSplitOptions.None);
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

                    config.scenes.Add(scs);
                }
            }
        }


        if (config.allowSubfolders)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                LoadFromPath(dir);
            }
        }
    }

    private void OnEnable()
    {
        RefreshConfig();
        ReloadScenes();
    }

    private void OnFocus()
    {
        RefreshConfig();
        ReloadScenes();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Scene Wizard", EditorStyles.boldLabel);

        GUILayout.Space(20);
        RefreshConfig();
        ReloadScenes();

        var prevAlignment = GUI.skin.button.alignment;
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        if (config.folderPath == "" || config.folderPath == null)
        {
            if (GUILayout.Button("Select a folder"))
            {
                // EditorUtility.DisplayDialog("Select Folder", "You must select a folder first!", "OK");

                string path = EditorUtility.OpenFolderPanel("Select a folder to load Scenes from", "", "");
                config.folderPath = path;
            }
        }
        else
        {
            if (GUILayout.Button("Change folder"))
            {
                // EditorUtility.DisplayDialog("Select Folder", "You must select a folder first!", "OK");
                string path = EditorUtility.OpenFolderPanel("Select a folder to load Scenes from", "", "");

                if (!string.IsNullOrEmpty(path))
                {
                    config.folderPath = path;
                    config.scenes = new List<SceneConfigSetup>();
                }
            }

            if (config.scenes == null || config.scenes.Count <= 0)
            {
                ReloadScenes();
            }
            if (GUILayout.Button("Refresh"))
            {
                ReloadScenes();
            }

            if (GUILayout.Button("Clear"))
            {
                config.folderPath = "";
                config.scenes = new List<SceneConfigSetup>();
            }
        }


        EditorGUILayout.EndHorizontal();

        config.allowSubfolders = EditorGUILayout.Toggle("Allow Subfolders", config.allowSubfolders);

        EditorGUILayout.EndVertical();


        if (config.scenes != null && config.scenes.Count > 0)
        {
            string lastFolderName = "";
            GUILayout.Space(15);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Height(position.height * 0.6f));

            EditorGUI.indentLevel++;
            foreach (var scene in config.scenes)
            {
                if (scene.parentFolder != lastFolderName)
                {
                    if (lastFolderName != "")
                        GUILayout.Space(8);

                    EditorGUI.indentLevel--;

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    GUILayout.Label(scene.parentFolder, EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();
                    lastFolderName = scene.parentFolder;
                    EditorGUI.indentLevel++;
                }

                EditorGUILayout.BeginHorizontal();

                //    EditorGUILayout.LabelField(scene.name + " : ");
                var sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(GUIContent.none, sceneLoaded, typeof(SceneAsset), false);
                EditorGUI.EndDisabledGroup();


                if (GUILayout.Button("Open Single"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Single);
                }

                if (GUILayout.Button("Open Additively"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                }

                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        GUI.skin.button.alignment = prevAlignment;
    }
}
