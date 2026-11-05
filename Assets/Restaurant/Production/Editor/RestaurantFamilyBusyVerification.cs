using System;using System.Collections;using System.Collections.Generic;using System.IO;using System.Linq;using UnityEngine;using UnityEditor;using IdleRestaurant;
public static class RestaurantFamilyBusyVerification {
 public static string Status="Not started";[MenuItem("Restaurant/Family Game/Test five districts at maximum upgrades")]public static void BeginMax(){level=30;Begin();}static int level;[MenuItem("Restaurant/Family Game/Test five districts at base upgrades")]public static void Begin(){UnityEngine.Object.FindAnyObjectByType<RestaurantApp>().StartCoroutine(Run());}
 static IEnumerator Run(){var a=UnityEngine.Object.FindAnyObjectByType<RestaurantApp>();var results=new List<object>();Status="Running";string error="";
 foreach(int outlet in new[]{7,15,23,31,39}){var state=new RestaurantGame(a.catalog).State;state.introComplete=true;state.guideStep=6;foreach(var room in state.outlets)room.open=false;state.selected=outlet;state.outlets[outlet].open=true;for(int c=0;c<9;c++)state.outlets[outlet].upgrades[c]=level;a.Game.State=state;a.Navigate("play");a.world.Show(outlet);a.Game.BeginService();float begin=Time.realtimeSinceStartup;int startFrame=Time.frameCount;Status="Testing outlet "+(outlet+1);double expected=22*(a.Game.FamilyPay(outlet,0)+a.Game.FamilyPay(outlet,1)+a.Game.FamilyPay(outlet,2))*(1+a.Game.Balance.serviceBonus);
 while(state.outlets[outlet].waves==0&&Time.realtimeSinceStartup-begin<160)yield return null;
 bool passed=state.outlets[outlet].waves==1&&state.guestsServed==22&&state.gems==1&&(level==30?state.totalEarned>=expected:Math.Abs(state.totalEarned-expected)<expected*1e-8);
 results.Add(new{outlet,passed,seconds=state.seconds,frames=Time.frameCount-startFrame,meals=state.guestsServed,tickets=state.gems,expected,earned=state.totalEarned,guests=passed?null:state.outlets[outlet].service});
 if(!passed){error="Service or payout failed at outlet "+outlet;break;}yield return null;
 }
 Status=error==""?"PASS":"FAIL: "+error;Directory.CreateDirectory("Verification");File.WriteAllText("Verification/FamilyBusyServices"+(level==30?"Max":"")+".json",Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,Status,results,error,scope="Actual Editor guest movement and furniture colliders in all five districts, 22 guests each; base exact payouts, max-level payout includes deterministic tips"},Newtonsoft.Json.Formatting.Indented));Debug.Log("FAMILY_BUSY "+Status);a.Save();
 }
}
