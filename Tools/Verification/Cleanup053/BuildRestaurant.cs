using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
public static class BuildRestaurant053
{
    public static object Main()
    {
        if (EditorApplication.isPlaying) throw new Exception("Stop Play mode before building.");
        PlayerSettings.bundleVersion="0.5.3";PlayerSettings.Android.bundleVersionCode=10;
        PlayerSettings.runInBackground=false;
        Directory.CreateDirectory("Verification/Cleanup053");
        var start=DateTime.UtcNow;
        var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes=new[]{"Assets/Restaurant/Production/RestaurantGame.unity"},locationPathName="Builds/IdleRestaurant-Android-v0.5.3.apk",target=BuildTarget.Android,options=BuildOptions.None });
        var evidence=new {startedUtc=start,finishedUtc=DateTime.UtcNow,result=report.summary.result.ToString(),errors=report.summary.totalErrors,warnings=report.summary.totalWarnings,bytes=report.summary.totalSize,seconds=report.summary.totalTime.TotalSeconds,messages=report.steps.SelectMany(s=>s.messages).Where(m=>m.type==UnityEngine.LogType.Warning||m.type==UnityEngine.LogType.Error).Select(m=>new{type=m.type.ToString(),m.content}).ToArray()};
        File.WriteAllText("Verification/Cleanup053/AndroidBuild.json",Newtonsoft.Json.JsonConvert.SerializeObject(evidence,Newtonsoft.Json.Formatting.Indented));
        if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Restaurant Android build failed; see AndroidBuild.json");
        File.Copy("Builds/IdleRestaurant-Android-v0.5.3.apk","Builds/IdleRestaurant-Android.apk",true);
        AssetDatabase.SaveAssets();
        return evidence;
    }
}
