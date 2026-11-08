using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace IdleRestaurant {
 [UnityEngine.Scripting.Preserve,Serializable] public class OutletState {
  public bool open; public int rank,waves,lastStarWave,pendingWaveGems; public double pendingBonus; public int[] upgrades=new int[9];public int[] managers={-1,-1,-1};
  public ServiceGuest[] service;public double spawnClock;public int ordered,cooked,seated;public double orderClock,kitchenClock,diningClock,walkClock,clearClock,waveIncome;
 }
 [UnityEngine.Scripting.Preserve,Serializable] public class ResearchState { public int level;public double finish; }
 [UnityEngine.Scripting.Preserve,Serializable] public class SaveState {
  public int version=2,selected,gems,prestigeCoins,tutorial,packDry,slots=1; public double cash,totalEarned,seconds,utc,boostUntil,adReady;
  public int goalBoardVersion; public int[] goalKinds; public double[] goalBaselines,goalTargets;
  public int guideStep,guestsServed,serviceStars,recruitSequence; public bool introComplete;public bool music=true,sfx=true,haptics=true,reducedMotion,largeText,premiumOffline,income2,income3; public int quality=1;
  public OutletState[] outlets; public ResearchState[] research;public double[] prestigeFinish=new double[8]; public int[] managerCopies,managerLevels,prestige=new int[8];
  public string[] receipts=Array.Empty<string>(); public int dailyDay,dailyAds,taskClaimed,taskBook,waveGemDay,waveGemsEarned;public double pendingOffline;
 }
 [UnityEngine.Scripting.Preserve,Serializable] class SaveEnvelope { public string payload,sha256; }
 public sealed class SaveStore {
  readonly string path;public bool HadUnreadableSave{get;private set;}bool canWrite=true;
  public SaveStore(string file){path=file;}
  public static string Hash(string value){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-","").ToLowerInvariant();}
  public void Write(SaveState state) {
   if(!canWrite)throw new IOException("Damaged saves could not be preserved; writing is disabled");
   string payload=Newtonsoft.Json.JsonConvert.SerializeObject(state),json=Newtonsoft.Json.JsonConvert.SerializeObject(new SaveEnvelope{payload=payload,sha256=Hash(payload)});
   Directory.CreateDirectory(Path.GetDirectoryName(path));string temp=path+".tmp";File.WriteAllText(temp,json);
   if(File.Exists(path)) File.Replace(temp,path,path+".bak");else File.Move(temp,path);
  }
  public SaveState Read(out bool recovered) {
   HadUnreadableSave=false;recovered=false;bool seen=false;foreach(var file in new[]{path,path+".bak"}){try{if(!File.Exists(file))continue;seen=true;var e=Newtonsoft.Json.JsonConvert.DeserializeObject<SaveEnvelope>(File.ReadAllText(file));if(e==null||e.sha256!=Hash(e.payload))continue;var state=Newtonsoft.Json.JsonConvert.DeserializeObject<SaveState>(e.payload);if(state==null||(state.version!=1&&state.version!=2))continue;recovered=file!=path;return state;}catch(Exception){}}
   if(seen){HadUnreadableSave=true;canWrite=false;string suffix=".unreadable-"+DateTime.UtcNow.Ticks;foreach(var file in new[]{path,path+".bak"})if(File.Exists(file))File.Copy(file,file+suffix);canWrite=true;}
   return null;
  }
 }
}
