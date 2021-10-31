using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BForBoss
{
    public class SceneSwitcher : DebugView
    {
        private const string SCENE_NAME_DELIMITER = "/";
        private const string SCENE_NAME_EXTENSION = ".unity";
        
        private List<string> _buildSceneNames = new List<string>();

        public SceneSwitcher(Rect masterRect) : base(masterRect)
        {
            GetBuildSceneNames();
            
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.sceneListChanged += GetBuildSceneNames;
#endif
        }

        public override void ResetData()
        {
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.sceneListChanged -= GetBuildSceneNames;
#endif
            base.ResetData();
        }

        protected override void DrawWindow()
        {
            using (new GUILayout.AreaScope(_baseRect))
            {
                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Scenes");
                        GUILayout.FlexibleSpace();
                    }
                    
                    for (int i = 0, count = _buildSceneNames.Count; i < count; i++)
                    {
                        if (GUILayout.Button(_buildSceneNames[i]))
                        {
                            ChangeScene(i);
                        }
                    }
                }
            }
        }

        private void ChangeScene(int buildIndex)
        {
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }

        private void GetBuildSceneNames()
        {
            _buildSceneNames = new List<string>();

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                _buildSceneNames.Add(GetAppropriateSceneName(SceneUtility.GetScenePathByBuildIndex(i)));
            }
        }

        private string GetAppropriateSceneName(string buildPath)
        {
            if (string.IsNullOrEmpty(buildPath))
            {
                Debug.LogError("Scene Build Name Not Available");
                return string.Empty;
            }
            
            string sceneNameWithExtension = buildPath.Substring(buildPath.LastIndexOf(SCENE_NAME_DELIMITER, StringComparison.Ordinal) + 1);
            return sceneNameWithExtension.Replace(SCENE_NAME_EXTENSION, string.Empty);
        }
        
    }
}
