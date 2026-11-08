using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IdleRestaurant;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RestaurantPresentationVerification
{
    public static string Status = "Not started";
    static string Folder => Path.GetFullPath("Verification/Journey/OnboardingRegression");
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly List<string> Checks = new List<string>();
    static readonly List<object> Events = new List<object>();
    static string error;
    static RestaurantApp app;

    [MenuItem("Restaurant/Verification/Verify doors tutorial and upgrade presentation")]
    public static void Begin()
    {
        app = UnityEngine.Object.FindAnyObjectByType<RestaurantApp>();
        if (!Application.isPlaying || !app) throw new InvalidOperationException("Enter Restaurant Play mode first.");
        app.StartCoroutine(Run());
    }

    static bool Check(bool condition, string label)
    {
        if (!condition) { error = label; Status = "FAIL: " + label; return false; }
        Checks.Add(label); return true;
    }
    static UnityEngine.UI.Button Find(string prefix) => UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None)
        .First(b => b.isActiveAndEnabled && b.GetComponentInChildren<TMP_Text>().text.StartsWith(prefix, StringComparison.Ordinal));
    static bool Tap(string prefix)
    {
        try
        {
            var b = Find(prefix); if (!b.interactable) throw new Exception("Button is disabled: " + prefix);
            Canvas.ForceUpdateCanvases(); var r = (RectTransform)b.transform;
            var e = new PointerEventData(EventSystem.current) { position = RectTransformUtility.WorldToScreenPoint(null, r.TransformPoint(r.rect.center)), button = PointerEventData.InputButton.Left };
            var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(e, hits);
            if (hits.Count == 0 || hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>() != b) throw new Exception("Blocked tap: " + prefix);
            ExecuteEvents.Execute(b.gameObject, e, ExecuteEvents.pointerClickHandler); return true;
        }
        catch (Exception ex) { error = ex.Message; Status = "FAIL: " + error; return false; }
    }
    static void Note(string label) => Events.Add(new { label, seconds = app.Game.State.seconds, cash = app.Game.State.cash, guide = app.Game.Guide, frame = Time.frameCount });
    static IEnumerator Shot(string name)
    {
        yield return new WaitForEndOfFrame();
        var t = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        t.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0); t.Apply();
        File.WriteAllBytes(Path.Combine(Folder, name + ".png"), t.EncodeToPNG()); UnityEngine.Object.Destroy(t);
    }
    static bool Green(UnityEngine.UI.Button b) { var c = b.GetComponent<UnityEngine.UI.Image>().color; return c.g > .4f && c.g > c.r * 1.5f && c.g > c.b * 1.3f; }
    static CanvasGroup Guide => (CanvasGroup)typeof(RestaurantApp).GetField("guideGroup", Private).GetValue(app);
    static void Refresh() => typeof(RestaurantApp).GetMethod("Refresh", Private).Invoke(app, null);

    static IEnumerator Run()
    {
        Directory.CreateDirectory(Folder); Checks.Clear(); Events.Clear(); error = ""; Status = "Running";
        // Always restore both the live model and its scene. External audit also preserves the original on-disk bytes.
        string original = Newtonsoft.Json.JsonConvert.SerializeObject(app.Game.State);
        bool completed = false; double firstMeal = 0, serviceSeconds = 0; int peakQueue = 0;
        try
        {
            app.Navigate("play"); app.Game.State = new RestaurantGame(app.catalog).State; app.world.Show(0); app.Navigate("play");
            typeof(RestaurantApp).GetMethod("TutorialDialog", Private).Invoke(app, null);
            yield return new WaitForSecondsRealtime(.3f); yield return Shot("01-welcome");
            if (!Tap("OPEN MY RESTAURANT")) yield break; Note("Welcome tapped");
            if (!Check(app.world.room.Find("Guest entrance and exit/ENTER doorway") && app.world.room.Find("Guest entrance and exit/EXIT doorway"), "Visible entry and exit match shared route geometry")) yield break;
            float start = Time.realtimeSinceStartup;
            var journey = new HashSet<string>(); bool journeyShot = false;
            while (app.Game.Guide < 2 && Time.realtimeSinceStartup - start < 90) {
                var label = UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None).FirstOrDefault(t=>t.transform.parent.name=="Guest journey callout");
                if(label && label.isActiveAndEnabled) journey.Add(label.text);
                if(!journeyShot && app.Game.State.seconds>12){journeyShot=true;yield return Shot("01b-contextual-journey");}
                yield return null;
            }
            if(!Check(journey.Count>=4,"First guest journey explains at least four distinct activities beside the action")) yield break;
            if(!Check(app.gameCamera.rect.height>=.49f && app.gameCamera.rect.yMin>=.30f,"Responsive camera gives the restaurant half of the portrait screen")) yield break;
            if (!Check(app.Game.Guide == 2, "First guest completes a meal and advances onboarding")) yield break;
            firstMeal = app.Game.State.seconds; yield return new WaitForSecondsRealtime(.8f); Note("First upgrade prompt");
            var order = Find("Faster orders");
            if (!Check(Green(order) && order.interactable && order.GetComponent<UnityEngine.UI.Outline>().enabled, "Affordable upgrade is green with a yellow tutorial highlight")) yield break;
            if (!Check(order.transform.Find("Upgrade price bar") && order.transform.Find("Upgrade price bar").GetComponentInChildren<TMP_Text>().text.StartsWith("BUY"), "Price has its own darker bar and BUY label")) yield break;
            var shade=UnityEngine.GameObject.Find("Tutorial spotlight shade 0").GetComponent<UnityEngine.UI.Image>();
            if(!Check(shade.gameObject.activeSelf && shade.color.a>.2f && !shade.raycastTarget,"Spotlight focuses the upgrade without intercepting touch")) yield break;
            double cash = app.Game.State.cash; app.Game.State.cash = 0; Refresh();
            var grey = order.GetComponent<UnityEngine.UI.Image>().color;
            bool greyCorrect = !order.interactable && Math.Abs(grey.r - grey.g) < .04f && Math.Abs(grey.b - grey.g) < .04f;
            app.Game.State.cash = cash; Refresh();
            if (!Check(greyCorrect, "Insufficient cash makes the upgrade grey and non-interactable")) yield break;
            yield return Shot("02-green-upgrade-yellow-guide");
            if (!Tap("Faster orders") || !Check(app.Game.Guide == 3, "First guided upgrade buys once through the actual UI")) yield break;
            var result=UnityEngine.GameObject.Find("Upgrade result");
            if(!Check(result.GetComponentInChildren<TMP_Text>().text.Contains("Take orders faster"),"Upgrade feedback explains the affected restaurant action")) yield break;
            yield return new WaitForSecondsRealtime(.23f);
            if (!Check(Guide.alpha < .1f, "Completed tutorial prompt dismisses before the next prompt")) yield break;
            yield return Shot("03-between-prompts"); yield return new WaitForSecondsRealtime(.5f);
            if (!Check(Guide.alpha > .95f && Find("KITCHEN").GetComponent<UnityEngine.UI.Outline>().enabled, "Next prompt reappears and highlights the Kitchen tab")) yield break;
            yield return Shot("04-kitchen-tab-highlight");
            if (!Tap("KITCHEN")) yield break; yield return new WaitForSecondsRealtime(.8f);
            if (!Check(Find("Faster delivery").GetComponent<UnityEngine.UI.Outline>().enabled, "Selecting Kitchen moves the yellow highlight to Faster delivery")) yield break;
            yield return Shot("05-price-bars");
            if (!Tap("Faster delivery") || !Check(app.Game.Guide == 4, "Second guided upgrade advances to first-service completion")) yield break;
            var returned = new HashSet<int>(); start = Time.realtimeSinceStartup;
            while (app.Game.TotalWaves() == 0 && Time.realtimeSinceStartup - start < 90)
            {
                var guests = app.Game.State.outlets[0].service;
                if (guests != null) foreach (var g in guests) if (g.returnedPlate) returned.Add(g.id);
                yield return null;
            }
            serviceSeconds = app.Game.State.seconds;
            if (!Check(app.Game.TotalWaves() == 1 && returned.Count == 6 && app.Game.State.guestsServed == 6, "All six guests return their plates and exit before one service reward")) yield break;
            yield return new WaitForSecondsRealtime(.8f); yield return Shot("06-first-service-exit");
            var reward=UnityEngine.GameObject.Find("Service reward");
            if(!Check(reward.GetComponent<CanvasGroup>().alpha>.95f && !reward.GetComponent<CanvasGroup>().blocksRaycasts && reward.GetComponentsInChildren<TMP_Text>().Any(t=>t.text.Contains("+1 TICKET")),"Service reward presents credited coins and ticket without a blocking claim tap")) yield break;
            if (!Check(Find("MAP").GetComponent<UnityEngine.UI.Outline>().enabled, "Final onboarding prompt highlights Map")) yield break;
            if (!Tap("MAP")) yield break; yield return new WaitForSecondsRealtime(.4f);
            if (!Check(Find("MAP").GetComponent<UnityEngine.UI.Image>().color == new Color32(48, 77, 64, 255), "Navigation shows MAP selected instead of PLAY")) yield break;
            yield return Shot("07-themed-map");
            // A full 22-guest service exercises the bounded entrance queue and new exit route under load.
            var state = new RestaurantGame(app.catalog).State; state.introComplete = true; state.guideStep = 6; state.selected = 7;
            foreach (var outlet in state.outlets) outlet.open = false; state.outlets[7].open = true;
            app.Game.State = state; app.Navigate("play"); app.world.Show(7); app.Game.BeginService();
            returned.Clear(); start = Time.realtimeSinceStartup; bool busyShot = false;
            while (state.outlets[7].waves == 0 && Time.realtimeSinceStartup - start < 160)
            {
                var guests = state.outlets[7].service;
                if (guests != null) { peakQueue = Math.Max(peakQueue, guests.Count(g => g.phase == GuestPhase.Entering || g.phase == GuestPhase.Queued)); foreach (var g in guests) if (g.returnedPlate) returned.Add(g.id); }
                if (!busyShot && state.seconds > 18) { busyShot = true; yield return Shot("08-bounded-entrance-queue"); }
                yield return null;
            }
            if (!Check(state.outlets[7].waves == 1 && state.guestsServed == 22 && returned.Count == 22 && peakQueue <= RestaurantWorld.QueueCapacity, "Busy service completes 22 meals, 22 plate returns and exits; queue stays inside its 10-person lane")) yield break;
            Note("Busy service complete"); yield return Shot("09-busy-service-complete");
            yield return new WaitForSecondsRealtime(app.experience.rewardSeconds+.3f);
            if(!Check(UnityEngine.GameObject.Find("Service reward").GetComponent<CanvasGroup>().alpha==0,"Reward card dismisses automatically while the next service continues")) yield break;
            app.Game.State.reducedMotion=true;app.Game.State.cash=1000;Refresh();
            if(!Tap("ORDERS"))yield break;yield return new WaitForSecondsRealtime(.15f);
            if(!Tap("Faster orders"))yield break;yield return new WaitForSecondsRealtime(.3f);
            if(!Check(UnityEngine.GameObject.Find("Upgrade result").GetComponent<CanvasGroup>().alpha>.9f,"Reduced motion keeps readable upgrade feedback"))yield break;
            // Gameplay framing should fit entrance and exit on common narrow and wide portrait phones.
            foreach(var ratio in new[]{.45f,.5625f,.625f}){
                var cam=app.gameCamera;float oldAspect=cam.aspect;cam.aspect=ratio/app.experience.playfieldHeight;
                typeof(RestaurantApp).GetMethod("FrameRestaurant",Private).Invoke(app,null);
                var entry=cam.WorldToViewportPoint(RestaurantWorld.EntranceSpawn);var exit=cam.WorldToViewportPoint(RestaurantWorld.ExitDespawn);
                bool inside=entry.x>0&&entry.x<1&&entry.y>0&&entry.y<1&&exit.x>0&&exit.x<1&&exit.y>0&&exit.y<1;
                cam.aspect=oldAspect;
                if(!Check(inside,"Entrance and exit remain visible at portrait aspect "+ratio))yield break;
            }
            completed = true;
        }
        finally
        {
            app.Game.State = new RestaurantGame(app.catalog, Newtonsoft.Json.JsonConvert.DeserializeObject<SaveState>(original)).State;
            app.Navigate("play"); app.world.Show(app.Game.State.selected); app.Save();
            Status = completed && error == "" ? "PASS" : "FAIL: " + (error == "" ? "Interrupted verification; inspect Console" : error);
            File.WriteAllText(Path.Combine(Folder, "PresentationVerification.json"), Newtonsoft.Json.JsonConvert.SerializeObject(new { utc = DateTime.UtcNow, Status, Checks, Events, firstMeal, firstServiceSeconds = serviceSeconds, peakQueue, originalLiveStateRestored = true, scope = "Live Editor simulation, furniture collision and UI raycasts. Android physical touch/performance not tested." }, Newtonsoft.Json.Formatting.Indented));
            Debug.Log("PRESENTATION_VERIFY " + Status);
        }
    }
}
