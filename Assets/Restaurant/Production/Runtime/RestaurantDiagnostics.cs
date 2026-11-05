using System;
using System.IO;
using UnityEngine;
namespace IdleRestaurant {
 public static class RestaurantDiagnostics {
  public static void Event(string name,object fields=null){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   try{string path=Path.Combine(Application.persistentDataPath,"development-events.jsonl");if(File.Exists(path)&&new FileInfo(path).Length>262144){File.Copy(path,path+".bak",true);File.WriteAllText(path,"");}File.AppendAllText(path,Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,name,fields})+"\n");}catch(IOException){}
#endif
  }
 }
}
