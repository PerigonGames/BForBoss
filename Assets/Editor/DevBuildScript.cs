using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.

public class DevBuildScript : MonoBehaviour
{
    [MenuItem("Build/Dev Build Windows")]
    public static void MyBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        List<string> scenePaths = new List<string>();
        
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            scenePaths.Add(e.path);
            Debug.Log("Scene added to build: " + e.path);
        }

        buildPlayerOptions.scenes = scenePaths.ToArray();
        buildPlayerOptions.locationPathName = "C:/Users/Developer/OneDrive/_PerigonGames/new_dev_build/windowsBuild.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.Development;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}
