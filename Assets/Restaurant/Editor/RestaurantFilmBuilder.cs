using System;
using System.Reflection;
using RestaurantPreview;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class RestaurantFilmBuilder
{
    [MenuItem("Restaurant/Prepare Concept Film")]
    public static void Prepare()
    {
        TMPro.TMP_PackageResourceImporter.ImportResources(true,false,false);
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var cam=new GameObject("Main Camera",typeof(Camera)).GetComponent<Camera>();cam.tag="MainCamera";cam.orthographic=true;cam.orthographicSize=8.5f;cam.transform.position=new Vector3(6,13,-15);cam.transform.LookAt(new Vector3(0,.5f,0));cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=Models.C("#F3E8D4");cam.rect=new Rect(0,.20f,1,.62f);cam.allowHDR=true;cam.allowMSAA=true;
        cam.gameObject.AddComponent<AudioListener>();
        var data=cam.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;data.antialiasing=AntialiasingMode.SubpixelMorphologicalAntiAliasing;data.antialiasingQuality=AntialiasingQuality.High;
        var key=new GameObject("Soft afternoon key",typeof(Light)).GetComponent<Light>();key.type=LightType.Directional;key.color=Models.C("#FFF0D5");key.intensity=1.75f;key.transform.rotation=Quaternion.Euler(48,-35,0);key.shadows=LightShadows.Soft;key.shadowStrength=.37f;key.shadowBias=.035f;key.shadowNormalBias=.18f;
        var fill=new GameObject("Window fill",typeof(Light)).GetComponent<Light>();fill.type=LightType.Directional;fill.intensity=.32f;fill.color=Models.C("#D3E4E5");fill.transform.rotation=Quaternion.Euler(65,130,0);fill.shadows=LightShadows.None;
        RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=Models.C("#D9E1D7");RenderSettings.ambientEquatorColor=Models.C("#C9D2C2");RenderSettings.ambientGroundColor=Models.C("#A7A89D");RenderSettings.ambientIntensity=1.0f;RenderSettings.skybox=null;
        var ground=GameObject.CreatePrimitive(PrimitiveType.Plane);ground.name="Backdrop ground";ground.transform.position=new Vector3(0,-.47f,0);ground.transform.localScale=Vector3.one*20;
        var groundMaterial=Models.Mat("#EFE3CA");AssetDatabase.CreateAsset(groundMaterial,"Assets/Restaurant/Backdrop.mat");ground.GetComponent<Renderer>().sharedMaterial=groundMaterial;
        var volume=new GameObject("Warm restaurant grade",typeof(Volume)).GetComponent<Volume>();volume.isGlobal=true;volume.priority=10;
        var profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,"Assets/Restaurant/CozyGrade.asset");
        var tone=profile.Add<Tonemapping>();tone.mode.Override(TonemappingMode.Neutral);AssetDatabase.AddObjectToAsset(tone,profile);
        var grade=profile.Add<ColorAdjustments>();grade.saturation.Override(5);grade.contrast.Override(4);grade.postExposure.Override(.1f);AssetDatabase.AddObjectToAsset(grade,profile);
        var warm=profile.Add<WhiteBalance>();warm.temperature.Override(7);AssetDatabase.AddObjectToAsset(warm,profile);
        var bloom=profile.Add<Bloom>();bloom.intensity.Override(.09f);bloom.threshold.Override(1.1f);AssetDatabase.AddObjectToAsset(bloom,profile);
        EditorUtility.SetDirty(profile);volume.sharedProfile=profile;
        var urp=GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if(!urp)throw new Exception("URP missing");urp.supportsHDR=true;urp.msaaSampleCount=4;urp.shadowDistance=40;urp.mainLightShadowmapResolution=2048;EditorUtility.SetDirty(urp);
        var film=new GameObject("Idle Restaurant Tycoon Concept",typeof(RestaurantFilm)).GetComponent<RestaurantFilm>();film.mainCamera=cam;
        PlayerSettings.productName="Idle Restaurant Tycoon";PlayerSettings.companyName="Restaurant Prototype";PlayerSettings.runInBackground=true;PlayerSettings.colorSpace=ColorSpace.Linear;PlayerSettings.defaultScreenWidth=720;PlayerSettings.defaultScreenHeight=1280;
        AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene,"Assets/Restaurant/RestaurantConcept.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Restaurant/RestaurantConcept.unity",true)};
        Portrait();Debug.Log("RESTAURANT_SCENE_READY");
    }
    public static void Portrait()
    {
        var a=typeof(Editor).Assembly;var t=a.GetType("UnityEditor.GameViewSizes");var sizes=typeof(ScriptableSingleton<>).MakeGenericType(t).GetProperty("instance").GetValue(null);var f=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
        var group=t.GetMethod("GetGroup").Invoke(sizes,new[]{t.GetProperty("currentGroupType",f).GetValue(sizes)});
        var kind=a.GetType("UnityEditor.GameViewSizeType");var size=Activator.CreateInstance(a.GetType("UnityEditor.GameViewSize"),f,null,new object[]{Enum.Parse(kind,"FixedResolution"),720,1280,"Restaurant Film 720"},null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});int n=(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null);
        var window=EditorWindow.GetWindow(a.GetType("UnityEditor.GameView"));window.GetType().GetProperty("selectedSizeIndex",f).SetValue(window,n-1);window.Show();window.Focus();window.Repaint();
    }
}
