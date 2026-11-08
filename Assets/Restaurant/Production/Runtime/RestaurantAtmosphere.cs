using System.Collections.Generic;
using UnityEngine;
using RestaurantPreview;
namespace IdleRestaurant
{
    public sealed class RestaurantAtmosphere:MonoBehaviour
    {
        RestaurantGame game;Camera camera;int outlet;Light sun;Material bulbMaterial;Transform fixtures;
        readonly List<Light> lamps=new List<Light>();readonly List<float> intensities=new List<float>();readonly List<Transform> flames=new List<Transform>();readonly List<Transform> candleRoots=new List<Transform>(),tables=new List<Transform>();
        public int Candles=>flames.Count;public int LitCandles{get{int n=0;foreach(var f in flames)if(f&&f.gameObject.activeInHierarchy)n++;return n;}}
        public float NightAmount{get;private set;}
        public int CornerFixtures{get;private set;}public int HangingFixtures{get;private set;}
        public int EnabledLights{get{int n=0;foreach(var lamp in lamps)if(lamp&&lamp.enabled)n++;return n;}}
        public void Setup(RestaurantGame g,Camera view,int index)
        {
            game=g;camera=view;outlet=index;fixtures=Models.Group("Service lighting fixtures",transform,Vector3.zero);
            foreach(var light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))if(light.type==LightType.Directional){sun=light;break;}
            bulbMaterial=new Material(g.Catalog.nightLampMaterial?g.Catalog.nightLampMaterial:Models.Mat("#FFF0C4")){name="Night-only warm lamps"};bulbMaterial.EnableKeyword("_EMISSION");
            NightAmount=g.State.outlets[index].waves%2;
            foreach(float x in new[]{-5.12f,5.12f})foreach(float z in new[]{-4.65f,5.95f}){
                var pole=Models.Group("Corner standing lamp "+CornerFixtures,fixtures,new Vector3(x,0,z));
                Models.Cylinder(pole,new Vector3(0,.08f,0),.16f,.16f,"#4C5F56");Models.Cylinder(pole,new Vector3(0,1.17f,0),.045f,2.25f,"#4C5F56");
                var block=pole.gameObject.AddComponent<BoxCollider>();block.center=new Vector3(0,1.1f,0);block.size=new Vector3(.16f,2.2f,.16f);
                Bulb(pole,new Vector3(0,2.34f,0),.17f);Models.Cylinder(pole,new Vector3(0,2.5f,0),.22f,.08f,"#4C5F56");Point(pole,new Vector3(0,2.24f,0),4.6f,2.0f);CornerFixtures++;
            }
            for(int chef=0;chef<3;chef++){
                float x=RestaurantWorld.CounterPosition(chef).x;
                Models.Box(fixtures,new Vector3(x,2.64f,4.35f),new Vector3(1.7f,.06f,.08f),"#5D6655");
                for(int i=-1;i<=1;i++){
                    var pendant=Models.Group("Counter pendant "+HangingFixtures,fixtures,new Vector3(x+i*.53f,0,4.35f));
                    Models.Cylinder(pendant,new Vector3(0,2.42f,0),.013f,.40f,"#5D6655");Models.Cylinder(pendant,new Vector3(0,2.20f,0),.12f,.08f,"#AE8F59");Bulb(pendant,new Vector3(0,2.13f,0),.07f);
                    if(i==0)Point(pendant,new Vector3(0,2.02f,0),2.65f,2.5f);HangingFixtures++;
                }
            }
            for(int row=0;row<3;row++)for(int side=0;side<2;side++){
                var candle=Models.Group("Table candle "+flames.Count,fixtures,new Vector3(-2.15f+side*4.3f,0,-2.5f+row*2));
                Models.Cylinder(candle,new Vector3(0,.84f,-.24f),.13f,.045f,"#B99154");
                Models.Cylinder(candle,new Vector3(0,.89f,-.24f),.105f,.08f,"#677F73");
                Models.Cylinder(candle,new Vector3(0,.99f,-.24f),.072f,.15f,"#FFF1CE");
                Models.Cylinder(candle,new Vector3(0,1.076f,-.24f),.009f,.035f,"#4C4439");
                var flame=Models.Group("Night candle flame",candle,new Vector3(0,1.14f,-.24f));flame.gameObject.AddComponent<RestaurantActorMarker>();
                var outer=Models.Ball(flame,Vector3.zero,new Vector3(.075f,.16f,.075f),"#FFCC75");outer.GetComponent<MeshRenderer>().sharedMaterial=bulbMaterial;
                Models.Ball(flame,new Vector3(0,-.025f,-.018f),new Vector3(.041f,.08f,.041f),"#FFF4C9");flames.Add(flame);
                Point(candle,new Vector3(0,1.16f,-.24f),1.35f,.55f);
                RestaurantWorld.CombineStatic(candle);candle.gameObject.AddComponent<RestaurantActorMarker>();candleRoots.Add(candle);tables.Add(transform.Find("Dining table "+(row*2+side)));
            }
            RestaurantWorld.CombineStatic(fixtures);UpdateLighting(0);
        }
        void Bulb(Transform parent,Vector3 pos,float radius){var bulb=Models.Ball(parent,pos,Vector3.one*radius*2,"#FFF0C4");bulb.GetComponent<MeshRenderer>().sharedMaterial=bulbMaterial;}
        void Point(Transform parent,Vector3 pos,float range,float intensity){var light=new GameObject("Warm night illumination",typeof(Light)).GetComponent<Light>();light.transform.SetParent(parent,false);light.transform.localPosition=pos;light.type=LightType.Point;light.range=range;light.intensity=intensity;light.color=new Color(1,.76f,.39f);light.shadows=LightShadows.None;lamps.Add(light);intensities.Add(intensity);}
        void Update(){if(game!=null)UpdateLighting(Time.unscaledDeltaTime);}
        void UpdateLighting(float dt)
        {
            float target=game.State.outlets[outlet].waves%2;NightAmount=Mathf.MoveTowards(NightAmount,target,dt/Mathf.Max(1,game.Journey.nightTransitionSeconds));
            RenderSettings.ambientSkyColor=Color.Lerp(new Color(.84f,.90f,.85f),new Color(.18f,.24f,.38f),NightAmount);
            RenderSettings.ambientEquatorColor=Color.Lerp(new Color(.72f,.78f,.70f),new Color(.13f,.16f,.24f),NightAmount);
            RenderSettings.ambientGroundColor=Color.Lerp(new Color(.58f,.60f,.53f),new Color(.08f,.09f,.14f),NightAmount);
            if(sun){sun.intensity=Mathf.Lerp(1.5f,.26f,NightAmount);sun.color=Color.Lerp(new Color(1,.94f,.83f),new Color(.62f,.73f,1),NightAmount);}
            float warmth=Mathf.InverseLerp(.5f,1,NightAmount);
            for(int i=0;i<candleRoots.Count;i++)candleRoots[i].gameObject.SetActive(!tables[i]||tables[i].gameObject.activeInHierarchy);
            for(int i=0;i<lamps.Count;i++){lamps[i].enabled=warmth>0&&lamps[i].gameObject.activeInHierarchy;lamps[i].intensity=intensities[i]*warmth;}
            for(int i=0;i<flames.Count;i++){var flame=flames[i];flame.gameObject.SetActive(warmth>0);float flicker=game.State.reducedMotion?1:1+.07f*Mathf.Sin(Time.unscaledTime*4.3f+i*1.7f);flame.localScale=new Vector3(1, flicker,1)*Mathf.Clamp01(warmth*2);}
            bulbMaterial.SetColor("_EmissionColor",new Color(1,.65f,.20f)*warmth*2.5f);
            bulbMaterial.SetColor("_BaseColor",Color.Lerp(new Color(.78f,.75f,.62f),new Color(1,.87f,.52f),warmth));
            if(camera)camera.backgroundColor=Color.Lerp(game.Journey.districts[outlet/8].paper*.84f,new Color(.075f,.105f,.16f),NightAmount);
        }
        void OnDestroy(){if(bulbMaterial)Destroy(bulbMaterial);}
    }
}
