using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.

public class CIBuildScript : MonoBehaviour
{
    public static void MakeProductionBuild()
    {
        MyBuild(BuildOptions.None);
    }

    public static void MakeDevelopmentBuild()
    {
        MyBuild(BuildOptions.Development);
    }
    
    private static void MyBuild(BuildOptions option)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        List<string> scenePaths = new List<string>();
        
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            //Do not include AdditiveDebugScene in ProdBuild
            if (e.path.Contains("AdditiveDebugScene") && option != BuildOptions.Development)
            {
                continue;
            }
            scenePaths.Add(e.path);
            Debug.Log("Scene added to build: " + e.path);
        }

        buildPlayerOptions.scenes = scenePaths.ToArray();
        //buildPlayerOptions.locationPathName = "build/StandaloneWindows64/windows.exe";
        buildPlayerOptions.locationPathName = "C:/Users/Developer/Documents/TempUnityBuilds/windowsBuild.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = option;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            throw new BuildFailedException("Build failed");
        }
    }
}
