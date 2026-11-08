using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IdleRestaurant;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
public static class RestaurantJourneyVerification
{
    public static string Status="Not started";
    static RestaurantApp app;
    static bool uiOnly;
    static readonly List<string> Checks=new List<string>();
    static readonly List<object> Services=new List<object>();
    static string Folder=>Path.GetFullPath("Verification/Journey");
    [MenuItem("Restaurant/Verification/Verify district journey and varied goals")]
    public static void Begin(){Start(false);}
    public static void BeginUIOnly(){Start(true);}
    static void Start(bool onlyUI){uiOnly=onlyUI;app=UnityEngine.Object.FindAnyObjectByType<RestaurantApp>();if(!Application.isPlaying||!app)throw new Exception("Enter Play mode first");app.StartCoroutine(Run());}
    static void Check(bool value,string message){if(!value)throw new Exception(message);Checks.Add(message);Status="Running: "+message;}
    static void Tap(string name)
    {
        var button=GameObject.Find(name).GetComponent<UnityEngine.UI.Button>();if(!button.interactable)throw new Exception("Disabled button "+name);
        Canvas.ForceUpdateCanvases();var rect=(RectTransform)button.transform;
        var evt=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center)),button=PointerEventData.InputButton.Left};
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(evt,hits);
        if(hits.Count==0||hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>()!=button)throw new Exception("Blocked tap "+name);
        ExecuteEvents.Execute(button.gameObject,evt,ExecuteEvents.pointerClickHandler);
    }
    static IEnumerator Shot(string name){yield return new WaitForEndOfFrame();var t=new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);t.Apply();File.WriteAllBytes(Path.Combine(Folder,name+".png"),t.EncodeToPNG());UnityEngine.Object.Destroy(t);}
    static void ModelChecks()
    {
        var g=new RestaurantGame(app.catalog);Check(g.State.goalKinds.Distinct().Count()>=10,"New goal board contains ten different activity types");
        Check(!g.ClaimGoal(0),"Unfinished goals cannot be claimed");
        g.State.guestsServed=1000;g.State.outlets[0].waves=100;g.State.totalEarned=100000;
        foreach(var s in g.State.outlets){s.open=true;for(int c=0;c<9;c++)s.upgrades[c]=30;}
        for(int i=0;i<g.State.managerCopies.Length;i++){g.State.managerCopies[i]=1;g.State.managerLevels[i]=15;}
        for(int i=0;i<g.State.research.Length;i++)g.State.research[i].level=app.catalog.research[i].max;
        int before=g.State.gems;
        for(int i=0;i<12;i++){Check(g.ClaimGoal(i),"Varied goal "+i+" pays when complete");Check(!g.ClaimGoal(i),"Varied goal "+i+" rejects duplicate claim");}
        Check(g.State.gems-before==120&&g.State.taskBook==1&&g.State.taskClaimed==0,"Whole board grants 120 tickets once and advances to a new book");
        Check(g.State.goalKinds[6]==(int)RestaurantGoalKind.Services&&g.State.goalKinds[7]==(int)RestaurantGoalKind.Services&&g.State.goalKinds[8]==(int)RestaurantGoalKind.Services,"Maxed expansion, recruitment and training become repeatable service goals");
        Check(g.State.goalTargets.All(x=>x>0)&&Enumerable.Range(0,12).All(i=>g.GoalProgress(i)==0),"New book starts from fresh baselines with attainable positive targets");
        var unclaimed=new RestaurantGame(app.catalog).State;unclaimed.goalBoardVersion=0;unclaimed.outlets[0].waves=3;var pending=new RestaurantGame(app.catalog,unclaimed);Check(pending.LegacyGoalBoard&&pending.GoalProgress(0)==3&&pending.GoalTarget(0)==5,"Unclaimed legacy goal progress is preserved before the first reward");
        var legacy=new RestaurantGame(app.catalog).State;legacy.goalBoardVersion=0;legacy.taskClaimed=1;legacy.taskBook=0;legacy.outlets[0].waves=60;
        var old=new RestaurantGame(app.catalog,legacy);Check(old.LegacyGoalBoard&&old.GoalTarget(11)==60&&old.GoalClaimed(0),"Partially claimed legacy board preserves targets and paid claim bits");
        int tickets=old.State.gems;for(int i=1;i<12;i++)Check(old.ClaimGoal(i),"Legacy remaining claim "+i+" succeeds");
        Check(old.State.gems-tickets==110&&!old.LegacyGoalBoard&&old.State.taskBook==1,"Finishing old book pays only remaining rewards then migrates to varied goals");
        g.State.guestsServed+=(int)g.GoalTarget(0);Check(g.ClaimGoal(0),"A new-book goal can be earned after its baseline");
        string path=Path.Combine(Path.GetTempPath(),"restaurant-journey-check-"+Guid.NewGuid()+".json");
        try{var store=new SaveStore(path);store.Write(g.State);var loaded=store.Read(out var recovered);var restored=new RestaurantGame(app.catalog,loaded);Check(!recovered&&restored.State.taskBook==1&&restored.State.goalTargets.SequenceEqual(g.State.goalTargets)&&restored.State.goalBaselines.SequenceEqual(g.State.goalBaselines)&&restored.State.taskClaimed==1&&!restored.ClaimGoal(0),"Goal baselines, targets, book and paid claim bit survive disk save/load without duplicate reward");}
        finally{if(File.Exists(path))File.Delete(path);if(File.Exists(path+".bak"))File.Delete(path+".bak");}
    }
    static IEnumerator Body()
    {
        ModelChecks();
        app.enabled=false;var state=new RestaurantGame(app.catalog).State;state.introComplete=true;state.guideStep=6;state.cash=0;app.Game.State=state;app.world.Show(0);app.Navigate("map");
        var field=typeof(RestaurantApp).GetField("mapDistrict",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);field.SetValue(app,0);app.Navigate("map");yield return null;
        Check(!GameObject.Find("Map outlet action 1").GetComponent<UnityEngine.UI.Button>().interactable,"Next outlet is disabled with insufficient cash");
        state.cash=10000;typeof(RestaurantApp).GetMethod("Refresh",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(app,null);yield return null;
        Check(GameObject.Find("Map outlet action 1").GetComponent<UnityEngine.UI.Button>().interactable&&!GameObject.Find("Map outlet action 2").GetComponent<UnityEngine.UI.Button>().interactable,"Cash enables next outlet but cannot bypass previous-outlet requirement");
        yield return Shot("01-street-map");double cost=app.Game.FamilyUnlock(1),cash=state.cash;Tap("Map outlet action 1");yield return null;
        Check(state.selected==1&&state.outlets[1].open&&state.cash==cash-cost,"Actual map tap opens next outlet and charges exact unchanged unlock price");
        app.Navigate("map");yield return null;
        for(int d=0;d<5;d++){
            Tap("District tab "+d);yield return null;
            var content=GameObject.Find("Journey path content").transform;
            Check(content.childCount==8&&GameObject.Find("District landmark").GetComponentsInChildren<TMP_Text>().Any(t=>t.text==app.Game.Journey.districts[d].name),"District "+d+" shows its chapter identity and eight connected outlets");
            if(d>0)yield return Shot("0"+(d+1)+"-district-map");
            var scroll=GameObject.Find("Map viewport").GetComponent<UnityEngine.UI.ScrollRect>();
            float beforeScroll=scroll.content.anchoredPosition.y;var evt=new PointerEventData(EventSystem.current){scrollDelta=new Vector2(0,-10)};scroll.OnScroll(evt);yield return null;
            Check(scroll.content.anchoredPosition.y>beforeScroll,"District "+d+" path responds to scroll input");
        }
        Check(UnityEngine.Object.FindObjectsByType<RestaurantSymbol>(FindObjectsSortMode.None).Count(x=>x.transform.parent.parent.name=="Navigation")==5,"All five navigation destinations use the shared vector icon family");
        app.Navigate("tasks");yield return null;Check(GameObject.Find("Goal reward badge")&&GameObject.Find("Goal reward badge").GetComponentInChildren<TMP_Text>().text=="2","Goals badge identifies the two earned expansion and teammate rewards");yield return Shot("06-varied-goals");
        if(uiOnly){
            var final=new RestaurantGame(app.catalog).State;final.introComplete=true;final.guideStep=6;app.Game.State=final;app.world.Show(0);app.Navigate("play");app.Game.BeginService();app.enabled=true;bool shot=false;float start=Time.realtimeSinceStartup;
            while(final.outlets[0].waves==0&&Time.realtimeSinceStartup-start<120){if(!shot&&final.seconds>12){shot=true;yield return Shot("10-district-gameplay");}yield return null;}
            Check(final.outlets[0].waves==1&&final.guestsServed==6,"Final street scenery completes a six-guest service without blocking routes");yield break;
        }
        // Each district is exercised by its real, unaccelerated guest simulation, including furniture collision.
        app.enabled=true;
        for(int d=0;d<5;d++){
            int index=d*8;state=new RestaurantGame(app.catalog).State;state.introComplete=true;state.guideStep=6;state.selected=index;
            foreach(var s in state.outlets)s.open=false;state.outlets[index].open=true;app.Game.State=state;app.Navigate("play");app.world.Show(index);app.Game.BeginService();
            var returns=new HashSet<int>();float started=Time.realtimeSinceStartup;bool captured=false;int expected=app.Game.ServiceSize(index);
            while(state.outlets[index].waves==0&&Time.realtimeSinceStartup-started<150){
                var guests=state.outlets[index].service;if(guests!=null)foreach(var g in guests)if(g.returnedPlate)returns.Add(g.id);
                if(!captured&&state.seconds>12){captured=true;yield return Shot("1"+d+"-district-gameplay");}
                yield return null;
            }
            Check(state.outlets[index].waves==1&&state.guestsServed==expected&&returns.Count==expected,"District "+d+" completes all "+expected+" meals, plate returns and exits");
            Check(app.world.room.Find("District identity: "+app.Game.Journey.districts[d].name),"District "+d+" has its own playable environment details");
            Services.Add(new{district=d,guests=expected,seconds=state.seconds,returned=returns.Count});
            if(d==0){app.enabled=false;app.Navigate("tasks");yield return null;int gems=state.gems;Tap("Goal claim 0");Check(state.gems==gems+10&&app.Game.GoalClaimed(0),"Guest goal becomes claimable from actual gameplay and UI pays exactly ten tickets");yield return Shot("07-earned-goal");app.enabled=true;}
        }
    }
    static IEnumerator Run()
    {
        Directory.CreateDirectory(Folder);Checks.Clear();Services.Clear();Status="Running";string saved=Newtonsoft.Json.JsonConvert.SerializeObject(app.Game.State);string error="";bool completed=false;var body=Body();
        try{
            while(true){bool next=false;try{next=body.MoveNext();}catch(Exception ex){error=ex.ToString();break;}if(!next){completed=true;break;}yield return body.Current;}
        }
        finally{
            app.enabled=false;app.Game.State=new RestaurantGame(app.catalog,Newtonsoft.Json.JsonConvert.DeserializeObject<SaveState>(saved)).State;app.Navigate("play");app.world.Show(app.Game.State.selected);app.Save();app.enabled=true;
            Status=completed?"PASS":"FAIL: "+error;
            File.WriteAllText(Path.Combine(Folder,uiOnly?"JourneyUIVerification.json":"JourneyVerification.json"),Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,Status,Checks,Services,originalLiveStateRestored=true,scope=uiOnly?"Seeded UI/economy/migration checks plus one live unaccelerated Street Kitchen service. No physical Android device testing.":"Seeded UI/economy/migration checks plus live unaccelerated gameplay in all five districts. No physical Android device testing."},Newtonsoft.Json.Formatting.Indented));Debug.Log("JOURNEY_VERIFY "+Status);
        }
    }
}
