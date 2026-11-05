using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using IdleRestaurant;
public static class RestaurantStoreArt {
 [MenuItem("Restaurant/Production/Create Original App Icon")]
 public static void Create(){
  const string folder="Assets/Restaurant/Production/Store";Directory.CreateDirectory(folder);var c=AssetDatabase.LoadAssetAtPath<RestaurantCatalog>(RestaurantProductionBuilder.CatalogPath);var preview=new PreviewRenderUtility();preview.camera.orthographic=true;preview.camera.orthographicSize=.47f;preview.camera.nearClipPlane=.05f;preview.camera.farClipPlane=10;preview.camera.transform.position=new Vector3(.7f,.62f,-.85f);preview.camera.transform.LookAt(new Vector3(0,.22f,0));preview.camera.clearFlags=CameraClearFlags.SolidColor;preview.camera.backgroundColor=new Color(.22f,.48f,.35f,1);preview.lights[0].intensity=1.5f;preview.lights[0].transform.rotation=Quaternion.Euler(45,-30,0);preview.lights[1].intensity=.6f;preview.ambientColor=new Color(.6f,.65f,.57f);
  var food=Object.Instantiate(c.outlets[0].food);preview.AddSingleGO(food);preview.BeginPreview(new Rect(0,0,512,512),GUIStyle.none);preview.Render(true);var rendered=preview.EndPreview();var previous=RenderTexture.active;RenderTexture.active=rendered as RenderTexture;var image=new Texture2D(512,512,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,512,512),0,0);image.Apply();File.WriteAllBytes(folder+"/RestaurantIcon.png",image.EncodeToPNG());Object.DestroyImmediate(image);RenderTexture.active=previous;preview.Cleanup();AssetDatabase.Refresh();var icon=AssetDatabase.LoadAssetAtPath<Texture2D>(folder+"/RestaurantIcon.png");var sizes=PlayerSettings.GetIconSizes(NamedBuildTarget.Unknown,IconKind.Any);PlayerSettings.SetIcons(NamedBuildTarget.Unknown,sizes.Select(_=>icon).ToArray(),IconKind.Any);var androidSizes=PlayerSettings.GetIconSizes(NamedBuildTarget.Android,IconKind.Any);if(androidSizes.Length>0)PlayerSettings.SetIcons(NamedBuildTarget.Android,androidSizes.Select(_=>icon).ToArray(),IconKind.Any);AssetDatabase.SaveAssets();Debug.Log("RESTAURANT_ORIGINAL_ICON_READY");
 }
}
