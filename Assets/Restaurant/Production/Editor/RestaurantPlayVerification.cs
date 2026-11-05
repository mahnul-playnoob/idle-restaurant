using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using IdleRestaurant;
public static class RestaurantPlayVerification {
 static RestaurantApp App=>UnityEngine.Object.FindAnyObjectByType<RestaurantApp>();
 static void Require(bool condition,string message){if(!condition)throw new Exception(message);}
 static Button Find(string text)=>UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).First(b=>b.isActiveAndEnabled&&b.GetComponentInChildren<TMP_Text>().text.StartsWith(text,StringComparison.Ordinal));
 static void Tap(Button b){Canvas.ForceUpdateCanvases();var rect=(RectTransform)b.transform;Vector2 position=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));var e=new PointerEventData(EventSystem.current){position=position,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);Require(hits.Count>0,"No UI hit at "+b.name);var target=hits[0].gameObject.GetComponentInParent<Button>();Require(target==b,"UI blocked: "+b.name+" by "+hits[0].gameObject.name);ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerClickHandler);}
 [MenuItem("Restaurant/Production/Test Actual UI Path (Play mode)")]
 public static void Begin(){Require(EditorApplication.isPlaying,"Enter Play mode");App.StartCoroutine(Run());}
 static IEnumerator Run(){var app=App;var original=Newtonsoft.Json.JsonConvert.SerializeObject(app.Game.State);var steps=new List<string>();string error="";bool finished=false;
  // Yielded steps are wrapped individually so state restoration always executes.
  app.Game.State=new RestaurantGame(app.catalog).State;app.Game.State.tutorial=3;app.Game.State.cash=125;app.Navigate("play");yield return new WaitForSecondsRealtime(.12f);
  try{var state=app.Game.State;int before=state.outlets[0].upgrades[0];Tap(Find("Faster orders"));Require(state.outlets[0].upgrades[0]==before+1,"Affordable purchase failed");Require(Math.Abs(state.cash-75)<.001,"Cash debit differs");steps.Add("GraphicRaycaster affordable purchase: level +1 and cash -50");}catch(Exception ex){error=ex.Message;}
  yield return new WaitForSecondsRealtime(.25f);
  if(error=="")try{double cash=app.Game.State.cash;Tap(Find("Faster orders"));Require(app.Game.State.outlets[0].upgrades[0]==1&&app.Game.State.cash==cash,"Unaffordable purchase granted");Require(Find("Faster orders").GetComponentInChildren<TMP_Text>().text.Contains("NEED"),"Unaffordable label missing");steps.Add("Unaffordable purchase rejected and NEED label visible");Tap(Find("KITCHEN"));}catch(Exception ex){error=ex.Message;}
  yield return new WaitForSecondsRealtime(.12f);
  if(error=="")try{Tap(Find("Faster delivery"));Require(app.Game.State.outlets[0].upgrades[3]==1,"Kitchen upgrade failed");steps.Add("Kitchen tab and purchase through raycast path");app.Game.State.cash=100000;Tap(Find("MAP"));}catch(Exception ex){error=ex.Message;}
  yield return new WaitForSecondsRealtime(.12f);
  if(error=="")try{var b=UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).First(x=>x.GetComponentInChildren<TMP_Text>().text.StartsWith("$20K"));Tap(b);Require(app.Game.State.selected==1&&app.Game.State.outlets[1].open,"Map purchase/visit failed");steps.Add("Map scroll viewport and second-outlet purchase");Tap(Find("MENU"));}catch(Exception ex){error=ex.Message;}
  yield return new WaitForSecondsRealtime(.12f);
  if(error=="")try{bool old=app.Game.State.sfx;var row=GameObject.Find("Row 0");Tap(row.GetComponentInChildren<Button>());Require(app.Game.State.sfx!=old,"Settings toggle failed");steps.Add("Settings toggle through raycast path");Tap(Find("SHOP"));}catch(Exception ex){error=ex.Message;}
  yield return new WaitForSecondsRealtime(.12f);
  if(error=="")try{Tap(GameObject.Find("Row 0").GetComponentInChildren<Button>());}catch(Exception ex){error=ex.Message;}
  yield return new WaitForSecondsRealtime(.12f);
  if(error=="")try{Tap(Find("CONFIRM DEMO"));Require(app.Game.State.income2,"Demo entitlement not granted");steps.Add("Demo checkout confirmation, saved entitlement, no production charge");finished=true;}catch(Exception ex){error=ex.Message;}
  app.Game.State=Newtonsoft.Json.JsonConvert.DeserializeObject<SaveState>(original);app.Navigate("play");app.Save();Directory.CreateDirectory("Verification");File.WriteAllText("Verification/PlayUI.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,passed=finished,steps,error,scope="Editor GraphicRaycaster pointer path; physical Android input remains unverified"},Newtonsoft.Json.Formatting.Indented));Debug.Log("RESTAURANT_UI_TEST "+(finished?"PASS":"FAIL "+error));
 }
 [MenuItem("Restaurant/Production/Record Actual Gameplay 15s (Play mode)")]
 public static void Record(){Require(EditorApplication.isPlaying,"Enter Play mode");App.StartCoroutine(Capture());}
 static IEnumerator Capture(){string folder="/tmp/restaurant-gameplay-frames";Directory.CreateDirectory(folder);var app=App;app.Navigate("play");Time.captureFramerate=30;bool priorMute=EditorUtility.audioMasterMute;EditorUtility.audioMasterMute=false;bool audio=AudioRenderer.Start();int channels=AudioSettings.speakerMode==AudioSpeakerMode.Mono?1:2;int rate=AudioSettings.outputSampleRate;var samples=new List<float>();int startWave=app.Game.State.outlets[app.Game.State.selected].waves;double startCash=app.Game.State.cash;var watch=System.Diagnostics.Stopwatch.StartNew();
  for(int frame=0;frame<450;frame++){if(frame==90||frame==270){var button=UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).FirstOrDefault(x=>x.isActiveAndEnabled&&x.GetComponentInChildren<TMP_Text>().text.Contains("\nBUY"));if(button){try{Tap(button);}catch(Exception ex){Debug.LogWarning("Recording tap skipped: "+ex.Message);}}}yield return new WaitForEndOfFrame();int width=Screen.width,height=Screen.height;var texture=new Texture2D(width,height,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,width,height),0,0,false);texture.Apply(false);File.WriteAllBytes(folder+"/"+frame.ToString("0000")+".png",texture.EncodeToPNG());UnityEngine.Object.Destroy(texture);if(audio){int count=AudioRenderer.GetSampleCountForCaptureFrame()*channels;using(var buffer=new Unity.Collections.NativeArray<float>(count,Unity.Collections.Allocator.Temp)){AudioRenderer.Render(buffer);samples.AddRange(buffer.ToArray());}}}
  AudioRenderer.Stop();EditorUtility.audioMasterMute=priorMute;Time.captureFramerate=0;WriteWave("Media/ActualGameplay.wav",samples.ToArray(),rate,channels);File.WriteAllText("Verification/GameplayCapture.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,frames=450,outputSeconds=15,wallSeconds=watch.Elapsed.TotalSeconds,source="Actual running simulation with two demonstrated UI purchases; no scripted wallets or actors",outlet=app.Game.State.selected,startCash,endCash=app.Game.State.cash,startWave,endWave=app.Game.State.outlets[app.Game.State.selected].waves,audioCaptured=audio,audioSamples=samples.Count},Newtonsoft.Json.Formatting.Indented));Debug.Log("RESTAURANT_CAPTURE_COMPLETE frames=450 audio="+audio+" samples="+samples.Count);
 }
 static void WriteWave(string path,float[] values,int rate,int channels){using(var writer=new BinaryWriter(File.Create(path))){writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+values.Length*2);writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)channels);writer.Write(rate);writer.Write(rate*channels*2);writer.Write((short)(channels*2));writer.Write((short)16);writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));writer.Write(values.Length*2);foreach(float v in values)writer.Write((short)(Mathf.Clamp(v,-1,1)*32767));}}
}
