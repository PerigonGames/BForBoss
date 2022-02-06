using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PerigonGames;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneWizard : EditorWindow, IHasCustomMenu
{
    private const string PROEJCT_PARENT_FOLDER = "BForBoss/";
    private const string PROJECT_NAME = "\\" + "_BForBoss";
    private const string SCENE_FILE_EXTENSION = ".unity";

    private bool _needsToRefreshElements = false;
    private SceneWizardConfig _config;
    private List<SceneConfigSetup> _favoriteSceneConfigs = new List<SceneConfigSetup>();
    private SceneConfigSetup _currentSceneConfig;
    private Vector2 _scrollView;

    private Texture2D _notFavouriteSymbol;
    private Texture2D _favouriteSymbol;

    [MenuItem("BForBoss/SceneSwitcher")]
    private static void Init()
    {
        SceneWizard window = (SceneWizard)GetWindow(typeof(SceneWizard));
        window.titleContent = new GUIContent("Scene Switcher");
        window.minSize = new Vector2(440, 350);
        window.Show();
    }
    
    public void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Refresh Scenes"), false, ReloadScenes);
    }

    private void OnProjectChanged()
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
        _favoriteSceneConfigs = new List<SceneConfigSetup>();
        LoadFromPath(Application.dataPath + PROJECT_NAME);
    }

    private void LoadFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        string[] files = Directory.GetFiles(path);
        foreach (string fp in files)
        {
            if (fp.Contains(SCENE_FILE_EXTENSION))
            {
                string assetPath = fp.Split(new string[] {PROEJCT_PARENT_FOLDER}, StringSplitOptions.None)[1];
                
                SceneAsset sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

                if (sceneLoaded != null)
                {
                    string relativeScenePathName = assetPath.Split(new string[] {"Assets" + "\\"}, StringSplitOptions.None)[1];
                    int lastFolderCharacterIndex = relativeScenePathName.LastIndexOf('\\');
                    string parentFolderName = relativeScenePathName.Remove(lastFolderCharacterIndex);

                    SceneConfigSetup scs = new SceneConfigSetup()
                    {
                        name = sceneLoaded.name,
                        path = assetPath,
                        parentFolder = parentFolderName
                    };

                    if (string.Equals(SceneManager.GetActiveScene().path.Replace('/', '\\'), assetPath))
                    {
                        _currentSceneConfig = scs;
                    }

                    if (EditorPrefs.GetBool(scs.path, false))
                    {
                        _favoriteSceneConfigs.Add(scs);
                    }

                    _config.scenes.Add(scs);
                }
            }
        }

        string[] dirs = Directory.GetDirectories(path);
        foreach (string dir in dirs)
        {
            LoadFromPath(dir);
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
        
        if (_config.scenes.IsNullOrEmpty())
        {
            return;
        }
        
        string lastFolderName = string.Empty;

        using (new EditorGUILayout.VerticalScope())
        {
            GUILayout.Space(5f);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Scene Switcher", new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 30,
                    fixedWidth = 300,
                    fixedHeight = 25
                });
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(15f);
            
            if (!_favoriteSceneConfigs.IsNullOrEmpty())
            {
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                {
                    GUILayout.Label(new GUIContent("Favourite Scenes", _favouriteSymbol), EditorStyles.boldLabel,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }

                foreach (SceneConfigSetup sceneConfig in _favoriteSceneConfigs)
                {
                    DrawSceneElement(sceneConfig, true);
                }

                GUILayout.Space(15f);
            }

            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollView))
            { 
                _scrollView = scrollViewScope.scrollPosition;
                foreach (SceneConfigSetup scene in _config.scenes)
                {
                    if (scene.parentFolder != lastFolderName)
                    {
                        EditorGUI.indentLevel++;
                            
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                        {
                            GUILayout.Label(scene.parentFolder, EditorStyles.boldLabel);
                        }

                        EditorGUI.indentLevel--;
                            
                        lastFolderName = scene.parentFolder;
                    }
                    
                    DrawSceneElement(scene, false);
                }
            }
            
            GUILayout.Space(3f);
        }
    }
    
    private void DrawSceneElement(SceneConfigSetup scene, bool disableFavouriteToggle)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            SceneAsset sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);

            using (new EditorGUI.DisabledGroupScope(disableFavouriteToggle))
            {
                bool isFavoriteScene = EditorPrefs.GetBool(scene.path, false);
                
                if (GUILayout.Button(isFavoriteScene ? _favouriteSymbol : _notFavouriteSymbol, GUIStyle.none,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(20f), GUILayout.ExpandHeight(true)))
                {
                    if (!isFavoriteScene)
                    {
                        EditorPrefs.SetBool(scene.path, true);
                        _favoriteSceneConfigs.Add(scene);
                    }
                    else
                    {
                        EditorPrefs.DeleteKey(scene.path);
                        _favoriteSceneConfigs.Remove(scene);
                    }
                }
            }
            
            using (new EditorGUI.DisabledGroupScope(true))
            {
                Color defaultColor = GUI.color;
                GUI.color = (_currentSceneConfig != null && _currentSceneConfig.Equals(scene)) ? Color.green : defaultColor;
                EditorGUILayout.ObjectField(GUIContent.none, sceneLoaded, typeof(SceneAsset), false);
                GUI.color = defaultColor;
            }

            if (GUILayout.Button("Open Single"))
            { 
                if (SceneManager.GetActiveScene().isDirty) 
                { 
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) 
                    { 
                        OpenSceneSingleMode(scene);
                    }
                }
                else
                {
                    OpenSceneSingleMode(scene);
                }
            }

            if (GUILayout.Button("Open Additively"))
            {
                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
            }
        }
    }
    
    private void OpenSceneSingleMode(SceneConfigSetup sceneConfig)
    {
        EditorSceneManager.OpenScene(sceneConfig.path, OpenSceneMode.Single);
        _currentSceneConfig = sceneConfig;
    }

    private void LoadSymbols()
    {
        _notFavouriteSymbol = EditorGUIUtility.IconContent("d_Favorite On Icon").image as Texture2D;
        _favouriteSymbol = EditorGUIUtility.IconContent("d_Favorite Icon").image as Texture2D;
    }
    
    private void OnEnable()
    {
        LoadSymbols();
        RefreshElements();
        EditorApplication.projectChanged += OnProjectChanged;
    }
    
    private void OnDestroy()
    {
        EditorApplication.projectChanged -= OnProjectChanged;
    }
}
