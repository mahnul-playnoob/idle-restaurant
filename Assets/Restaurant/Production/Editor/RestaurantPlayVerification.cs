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
 static IEnumerator Run(){RestaurantFamilyVerification.Begin();yield break;}
 [MenuItem("Restaurant/Production/Record Actual Gameplay 15s (Play mode)")]
 public static void Record(){Require(EditorApplication.isPlaying,"Enter Play mode");App.StartCoroutine(Capture());}
 static IEnumerator Capture(){string folder="/tmp/restaurant-family-gameplay-frames";Directory.CreateDirectory(folder);var app=App;app.Navigate("play");Time.captureFramerate=30;bool priorMute=EditorUtility.audioMasterMute;EditorUtility.audioMasterMute=false;bool audio=AudioRenderer.Start();int channels=AudioSettings.speakerMode==AudioSpeakerMode.Mono?1:2;int rate=AudioSettings.outputSampleRate;var samples=new List<float>();int startWave=app.Game.State.outlets[app.Game.State.selected].waves;double startCash=app.Game.State.cash;var watch=System.Diagnostics.Stopwatch.StartNew();
  for(int frame=0;frame<450;frame++){if(frame==90||frame==270){var button=UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).FirstOrDefault(x=>x.isActiveAndEnabled&&x.GetComponentInChildren<TMP_Text>().text.Contains("\nBUY"));if(button){try{Tap(button);}catch(Exception ex){Debug.LogWarning("Recording tap skipped: "+ex.Message);}}}yield return new WaitForEndOfFrame();int width=Screen.width,height=Screen.height;var texture=new Texture2D(width,height,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,width,height),0,0,false);texture.Apply(false);File.WriteAllBytes(folder+"/"+frame.ToString("0000")+".png",texture.EncodeToPNG());UnityEngine.Object.Destroy(texture);if(audio){int count=AudioRenderer.GetSampleCountForCaptureFrame()*channels;using(var buffer=new Unity.Collections.NativeArray<float>(count,Unity.Collections.Allocator.Temp)){AudioRenderer.Render(buffer);samples.AddRange(buffer.ToArray());}}}
  AudioRenderer.Stop();EditorUtility.audioMasterMute=priorMute;Time.captureFramerate=0;WriteWave("Media/FamilyGameplay.wav",samples.ToArray(),rate,channels);File.WriteAllText("Verification/FamilyGameplayCapture.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,frames=450,outputSeconds=15,wallSeconds=watch.Elapsed.TotalSeconds,source="Actual running family simulation from a configured progression state. Upgrade taps attempted if affordable; characters follow the real service simulation.",outlet=app.Game.State.selected,startCash,endCash=app.Game.State.cash,startWave,endWave=app.Game.State.outlets[app.Game.State.selected].waves,audioCaptured=audio,audioSamples=samples.Count},Newtonsoft.Json.Formatting.Indented));Debug.Log("RESTAURANT_CAPTURE_COMPLETE frames=450 audio="+audio+" samples="+samples.Count);
 }
 static void WriteWave(string path,float[] values,int rate,int channels){using(var writer=new BinaryWriter(File.Create(path))){writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+values.Length*2);writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)channels);writer.Write(rate);writer.Write(rate*channels*2);writer.Write((short)(channels*2));writer.Write((short)16);writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));writer.Write(values.Length*2);foreach(float v in values)writer.Write((short)(Mathf.Clamp(v,-1,1)*32767));}}
}
