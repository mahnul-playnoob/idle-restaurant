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
using UnityEngine.UI;
public static class RestaurantCraftVerification
{
    public static string Status="Not started";
    static RestaurantApp app;static readonly List<string> Checks=new List<string>();static readonly List<object> Rates=new List<object>();
    static string Folder=>Path.GetFullPath("Verification/Progression051/CraftRegression");
    static BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;
    [MenuItem("Restaurant/Verification/Verify progression lighting portraits and upgrades")]
    public static void Begin(){app=UnityEngine.Object.FindAnyObjectByType<RestaurantApp>();if(!Application.isPlaying||!app||app.Game==null||!app.Game.Family)throw new Exception("Enter Restaurant Play mode");app.StartCoroutine(Run());}
    static void Check(bool ok,string label){if(!ok)throw new Exception(label);Checks.Add(label);Status="Running: "+label;}
    static void Gates(RestaurantGame g,int next){var s=g.State.outlets[next-1];s.waves=g.OutletServicesRequired(next);int n=g.OutletUpgradesRequired(next);for(int c=0;c<9;c++){s.upgrades[c]=Math.Min(30,n);n-=s.upgrades[c];}}
    static void Tap(string name){var b=GameObject.Find(name).GetComponent<Button>();if(!b.interactable)throw new Exception("Disabled "+name);Canvas.ForceUpdateCanvases();var r=(RectTransform)b.transform;var evt=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center)),button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(evt,hits);if(hits.Count==0||hits[0].gameObject.GetComponentInParent<Button>()!=b)throw new Exception("Blocked "+name);ExecuteEvents.Execute(b.gameObject,evt,ExecuteEvents.pointerClickHandler);}
    static IEnumerator Shot(string name){yield return new WaitForEndOfFrame();var t=new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);t.Apply();File.WriteAllBytes(Path.Combine(Folder,name+".png"),t.EncodeToPNG());UnityEngine.Object.Destroy(t);}
    static string IconHash(int d,int v,int width=90,int height=90){var go=new GameObject("Temporary icon audit",typeof(RectTransform),typeof(CanvasRenderer),typeof(DistrictIcon));var r=go.GetComponent<RectTransform>();r.sizeDelta=new Vector2(width,height);var icon=go.GetComponent<DistrictIcon>();icon.district=d;icon.variant=v;using(var vh=new VertexHelper()){typeof(DistrictIcon).GetMethod("OnPopulateMesh",Private|BindingFlags.DeclaredOnly).Invoke(icon,new object[]{vh});var mesh=new Mesh();vh.FillMesh(mesh);var value=string.Join(";",mesh.vertices.Select(p=>p.x.ToString("F3")+","+p.y.ToString("F3")))+string.Join(";",mesh.colors32.Select(c=>c.r+","+c.g+","+c.b));UnityEngine.Object.Destroy(mesh);UnityEngine.Object.Destroy(go);return value;}}
    static void Models()
    {
        var g=new RestaurantGame(app.catalog);g.State.cash=1e30;Check(!g.Open(1),"Cash alone cannot skip expansion service and investment milestones");
        for(int next=1;next<40;next++){Gates(g,next);Check(g.Open(next),"Restaurant "+(next+1)+" unlocks after its stated milestones");}
        Check(g.State.outlets.All(o=>o.open),"All forty restaurants remain obtainable without purchases");
        var team=new RestaurantGame(app.catalog);team.State.cash=1e20;Gates(team,1);team.Open(1);team.State.gems=100000;team.State.outlets[0].waves=0;
        Check(!team.Recruit(0),"Tickets alone cannot skip the next manager's service milestone");team.State.outlets[0].waves=team.RecruitServicesRequired;int count=team.TeamCount;Check(team.Recruit(0)&&team.TeamCount==count+1,"One welcome action grants exactly one new manager");Check(!team.Recruit(0),"Next manager waits for the next restaurant milestone");
        foreach(var o in team.State.outlets){o.open=true;o.waves=5000;}while(team.NextRecruitId()>=0){team.State.outlets[0].waves+=team.Balance.recruitServiceGap;Check(team.Recruit(0),"Sequential roster welcome "+team.TeamCount);}
        Check(team.TeamCount==71&&team.State.managerCopies.All(x=>x==1),"All 71 managers can be welcomed once with earned resources");int tickets=team.State.gems;Check(!team.Recruit(0)&&team.State.gems==tickets,"A complete roster never consumes extra tickets");
        Check(app.catalog.managers.Select(m=>m.title).Distinct().Count()==71&&app.catalog.managers.Select(m=>m.title.Split(' ').Last()).Distinct().Count()==71,"All 71 fictional managers have distinct full names and surnames");
        for(int d=0;d<5;d++){Check(Enumerable.Range(0,8).Select(v=>IconHash(d,v)).Distinct().Count()==8,"District "+d+" has eight distinct outlet icon designs");Check(IconHash(d,0,180,90)==IconHash(d,0,90,90),"District "+d+" icon preserves shape in a rectangular slot");}
        var speed=new RestaurantGame(app.catalog);speed.State.outlets[1].open=true;speed.State.managerCopies[46]=1;speed.State.managerLevels[46]=15;int dining=Array.FindIndex(app.catalog.managers,m=>m.role==2);speed.State.managerCopies[dining]=1;speed.State.managerLevels[dining]=15;double before=speed.MealDuration(0);speed.Assign(dining,2);Check(speed.MealDuration(0)<before,"Assigned Dining manager speed now affects the actual meal phase");
    }
    static IEnumerator Body()
    {
        Models();app.enabled=false;var state=new RestaurantGame(app.catalog).State;state.introComplete=true;state.guideStep=6;state.cash=10000;app.Game.State=state;app.world.Show(0);typeof(RestaurantApp).GetField("mapDistrict",Private).SetValue(app,0);app.Navigate("map");yield return null;
        Check(!GameObject.Find("Map outlet action 1").GetComponent<Button>().interactable,"Map Open stays grey until both service and improvement goals are met");Gates(app.Game,1);typeof(RestaurantApp).GetMethod("Refresh",Private).Invoke(app,null);yield return Shot("01-longer-progression-map");double cash=state.cash,price=app.Game.FamilyUnlock(1);Tap("Map outlet action 1");yield return null;Check(state.cash==cash-price&&state.outlets[1].open,"Real map tap charges the new displayed unlock price exactly once");
        state.gems=100;app.Navigate("managers");yield return null;Check(UnityEngine.Object.FindObjectsByType<ManagerPortrait>(FindObjectsSortMode.None).Length==71,"Every manager row has an individual portrait component");yield return Shot("02-manager-portraits");Tap("Assign manager 0");Check(state.outlets[1].managers[0]==0,"Manager assignment works through the actual UI");Tap("Train manager 0");Check(state.managerLevels[0]==2,"Manager training works through the actual UI");int owned=app.Game.TeamCount;Tap("Welcome next manager");yield return null;Check(app.Game.TeamCount==owned+1&&!GameObject.Find("Welcome next manager").GetComponent<Button>().interactable,"Team UI welcomes one person then exposes the next progression requirement");
        state=new RestaurantGame(app.catalog).State;state.introComplete=true;state.guideStep=6;app.Game.State=state;app.Navigate("play");app.world.Show(0);app.Game.BeginService();app.enabled=true;
        var atmosphere=app.world.room.GetComponent<RestaurantAtmosphere>();Check(atmosphere.CornerFixtures==4&&atmosphere.HangingFixtures==9&&atmosphere.Candles==6&&atmosphere.LitCandles==0&&atmosphere.EnabledLights==0,"Four corners, nine counter pendants and six table candles exist with lights off by day");
        var a=app.gameCamera.WorldToViewportPoint(new Vector3(-3,2.6f,5));var b=app.gameCamera.WorldToViewportPoint(new Vector3(3,2.6f,5));Check(Math.Abs(a.y-b.y)<.0001f&&Math.Abs(app.gameCamera.transform.position.x)<.001,"Roof edge is horizontal and the camera has zero sideways rotation");
        int previous=0;float start=Time.realtimeSinceStartup;bool dayShot=false,nightShot=false,dayAgain=false;
        while(state.outlets[0].waves<4&&Time.realtimeSinceStartup-start<300){
            if(!dayShot&&state.seconds>12){dayShot=true;yield return Shot("03-day-service");}
            if(!nightShot&&state.outlets[0].waves==1&&atmosphere.NightAmount>.98f){nightShot=true;Check(atmosphere.EnabledLights==13&&atmosphere.LitCandles==6,"A completed real service fades into night and switches on warm lights");yield return Shot("04-night-service");}
            if(!dayAgain&&state.outlets[0].waves==2&&atmosphere.NightAmount<.02f){dayAgain=true;Check(atmosphere.EnabledLights==0,"The following service returns to daylight with all point lights off");}
            if(state.outlets[0].waves!=previous){previous=state.outlets[0].waves;var o=state.outlets[0];Rates.Add(new{wave=previous,o.lastServiceSeconds,o.lastServiceEarned,o.incomeCalibration,forecast=app.Game.FamilyIncome(0),measured=o.lastServiceEarned*60/o.lastServiceSeconds});}
            yield return null;
        }
        Check(state.outlets[0].waves>=4&&state.guestsServed>=30,"Four full services complete, including dishes left on tables and exits");var outlet=state.outlets[0];double actual=outlet.lastServiceEarned*60/outlet.lastServiceSeconds;Check(Math.Abs(app.Game.FamilyIncome(0)/actual-1)<.07,"Learned income forecast converges within seven percent of observed service income");
        var payments=(TMP_Text[])typeof(RestaurantApp).GetField("paymentLabels",Private).GetValue(app);Check(payments.Where(t=>!string.IsNullOrEmpty(t.text)).All(t=>t.text.StartsWith("$")&&!t.text.Contains("·")&&!t.text.Contains("+")),"Action earnings contain only currency, without action words or a plus prefix");
        app.enabled=false;state.cash=10000;state.outlets[0].upgrades[0]=4;typeof(RestaurantApp).GetField("stage",Private).SetValue(app,0);app.Navigate("play");app.world.Show(0);yield return null;var visuals=app.world.room.GetComponent<RestaurantUpgradeVisuals>();Check(visuals.Tier(0)==0,"Restaurant starts below the first visual investment milestone");Tap("Upgrade 0");yield return null;Check(visuals.Tier(0)==1,"An actual fifth Orders purchase visibly improves the counter");
        state.outlets[0].upgrades[4]=15;state.outlets[0].upgrades[6]=30;yield return null;Check(visuals.Tier(1)==2&&visuals.Tier(2)==3,"Kitchen and Dining unlock their larger persistent visual upgrades");yield return Shot("05-visible-restaurant-upgrades");
        for(int d=0;d<5;d++){state.selected=d*8;state.outlets[state.selected].open=true;app.world.Show(state.selected);app.Navigate("play");yield return new WaitForSecondsRealtime(.2f);yield return Shot("district-"+d+"-straight-view");}
        app.enabled=true;
    }
    static IEnumerator Run()
    {
        Directory.CreateDirectory(Folder);Checks.Clear();Rates.Clear();Status="Running";string original=Newtonsoft.Json.JsonConvert.SerializeObject(app.Game.State);string error="";bool done=false;var body=Body();
        try{while(true){bool next=false;try{next=body.MoveNext();}catch(Exception ex){error=ex.ToString();break;}if(!next){done=true;break;}yield return body.Current;}}
        finally{app.enabled=false;app.Game.State=new RestaurantGame(app.catalog,Newtonsoft.Json.JsonConvert.DeserializeObject<SaveState>(original)).State;app.Navigate("play");app.world.Show(app.Game.State.selected);app.Save();app.enabled=true;Status=done?"PASS":"FAIL: "+error;File.WriteAllText(Path.Combine(Folder,"CraftVerification.json"),Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,Status,Checks,Rates,originalLiveStateRestored=true,scope="Seeded milestone/roster/UI checks and four unaccelerated live services. Forecasted long-term playtime is not measured."},Newtonsoft.Json.Formatting.Indented));Debug.Log("CRAFT_VERIFY "+Status);}
    }
}
