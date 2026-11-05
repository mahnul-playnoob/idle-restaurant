using System;
using System.Collections;
using System.IO;
using UnityEngine;
using IdleRestaurant;
public static class RestaurantMemoryVerification {
 public static void Begin(){var app=UnityEngine.Object.FindAnyObjectByType<RestaurantApp>();if(!app)throw new Exception("Enter Play mode");app.StartCoroutine(Run(app));}
 static IEnumerator Run(RestaurantApp app){int selected=app.Game.State.selected;app.world.Show(selected);yield return new WaitForSecondsRealtime(.2f);int before=Resources.FindObjectsOfTypeAll<Mesh>().Length;int owners=UnityEngine.Object.FindObjectsByType<RestaurantRuntimeMeshOwner>(FindObjectsSortMode.None).Length;for(int i=0;i<10;i++){app.world.Show(selected);yield return new WaitForSecondsRealtime(.1f);}yield return new WaitForSecondsRealtime(.2f);int after=Resources.FindObjectsOfTypeAll<Mesh>().Length;int finalOwners=UnityEngine.Object.FindObjectsByType<RestaurantRuntimeMeshOwner>(FindObjectsSortMode.None).Length;bool pass=after<=before+4&&owners==finalOwners;Directory.CreateDirectory("Verification");File.WriteAllText("Verification/OutletReloadMemory.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,passed=pass,reloads=10,meshesBefore=before,meshesAfter=after,ownersBefore=owners,ownersAfter=finalOwners,scope="Editor native mesh retention across repeated same-outlet reloads; not an Android memory profile"},Newtonsoft.Json.Formatting.Indented));Debug.Log("RESTAURANT_MEMORY_TEST "+pass+" meshes="+before+" -> "+after);}
}
