using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RestaurantPreview
{
    public sealed class RestaurantFilm : MonoBehaviour
    {
        public static RestaurantFilm Instance;
        public Camera mainCamera;
        public float previewTime;
        public bool manual;
        Transform room,showcaseRoot,showBurger,showPasta,showSalad,chefBurger,servedBurger,servedPasta,saladReveal;
        Transform[] burgerLayers;
        Vector3[] burgerLayerPositions,burgerLayerScales;
        Transform grillPatty;
        RectTransform[] coins=new RectTransform[3];
        readonly List<Person> chefs=new List<Person>();
        readonly List<Person> diners=new List<Person>();
        Person host,guestA,guestB,serverA,serverB;
        readonly List<Person> queue=new List<Person>();
        readonly List<Transform> steam=new List<Transform>();
        Transform serverBurger,serverPasta;
        TMP_Text cash,wave,caption,recipeTitle,toast;
        TMP_Text[] cardNames=new TMP_Text[3],cardLevels=new TMP_Text[3],cardPrices=new TMP_Text[3];
        Image[] tabImages=new Image[3],cardImages=new Image[3];
        RectTransform tap;
        Image tapImage;
        AudioSource sound;
        float start;
        Color ink=Models.C("#3F5147"),green=Models.C("#4F8263"),muted=Models.C("#89988D");
        readonly Vector3[] tablePositions={new Vector3(-1.8f,0,.25f),new Vector3(1.8f,0,.25f),new Vector3(-1.8f,0,-2.1f),new Vector3(1.8f,0,-2.1f),new Vector3(-1.8f,0,-4.1f),new Vector3(1.8f,0,-4.1f)};
        void Awake()
        {
            Instance=this;Application.runInBackground=true;Application.targetFrameRate=60;
            if(!mainCamera)mainCamera=Camera.main;
            BuildRoom();BuildPeople();BuildFoodCamera();BuildUI();
            sound=gameObject.AddComponent<AudioSource>();sound.clip=Resources.Load<AudioClip>("RestaurantSoundtrack");sound.volume=.8f;sound.loop=true;if(sound.clip)sound.Play();
            start=Time.time;SetTime(0);
        }
        void Update(){if(!manual)SetTime((Time.time-start)%15f);}
        void BuildRoom()
        {
            room=Models.Group("Original cozy restaurant",transform,Vector3.zero);
            Models.Box(room,new Vector3(0,-.20f,0),new Vector3(10.4f,.42f,10),"#D8AA80",.35f);
            Models.Box(room,new Vector3(0,.025f,0),new Vector3(10.1f,.12f,9.7f),"#F4E5C8",.28f);
            Models.Box(room,new Vector3(-4.0f,.025f,-5.55f),new Vector3(1.65f,.12f,2.0f),"#E2D1B3",.2f);
            for(int x=0;x<10;x++)for(int z=0;z<10;z++)Models.Box(room,new Vector3(x-4.5f,.095f,z*.94f-4.25f),new Vector3(.98f,.015f,.92f),(x+z)%2==0?"#F8EBD5":"#EDE0C6",.012f);
            Models.Box(room,new Vector3(0,1.30f,4.75f),new Vector3(10.2f,2.5f,.18f),"#E8D5B3",.06f);
            Models.Box(room,new Vector3(-5.02f,1.12f,0),new Vector3(.18f,2.15f,9.6f),"#E8D5B3",.06f);
            Models.Box(room,new Vector3(0,.45f,4.60f),new Vector3(10f,.75f,.12f),"#7F9D8A",.02f);
            Models.Box(room,new Vector3(-4.88f,.45f,0),new Vector3(.12f,.75f,9.5f),"#7F9D8A",.02f);
            for(int i=0;i<3;i++)
            {
                float x=(i-1)*3.15f;
                Models.Box(room,new Vector3(x,1.79f,4.59f),new Vector3(2.55f,1.15f,.16f),"#7E9E8D",.13f);
                Models.Box(room,new Vector3(x,1.80f,4.48f),new Vector3(2.29f,.92f,.04f),"#C8DDD5",.08f);
                Models.Box(room,new Vector3(x,1.80f,4.43f),new Vector3(.06f,.93f,.04f),"#FAF0D9",.02f);
                Models.Box(room,new Vector3(x,1.80f,4.42f),new Vector3(2.28f,.055f,.04f),"#FAF0D9",.02f);
            }
            for(int i=0;i<3;i++)
            {
                float x=(i-1)*3.05f;
                var station=Models.Group(new[]{"Burger grill","Pasta kitchen","Salad prep"}[i],room,new Vector3(x,0,2.8f));
                Models.Box(station,new Vector3(0,.59f,0),new Vector3(2.82f,1.12f,1.12f),i==0?"#C58A75":i==1?"#83A391":"#D5B278",.12f);
                Models.Box(station,new Vector3(0,1.18f,-.04f),new Vector3(2.98f,.17f,1.3f),"#FFF3D8",.12f);
                for(int j=0;j<3;j++)Models.Box(station,new Vector3((j-1)*.82f,.75f,-.575f),new Vector3(.72f,.65f,.035f),i==0?"#D7A08C":i==1?"#95B5A2":"#E4C894",.035f);
                if(i==0)
                {
                    Models.Box(station,new Vector3(-.42f,1.29f,0),new Vector3(1.35f,.055f,.68f),"#596464",.07f);
                    for(int k=0;k<8;k++)Models.Box(station,new Vector3(-.96f+k*.15f,1.325f,0),new Vector3(.035f,.02f,.57f),"#8A9590",.01f);
                    grillPatty=Models.Cylinder(station,new Vector3(-.55f,1.34f,0),.22f,.065f,"#815542");
                    chefBurger=Models.Burger(station,new Vector3(.75f,1.31f,-.12f));
                    burgerLayers=new Transform[chefBurger.childCount];for(int j=0;j<burgerLayers.Length;j++)burgerLayers[j]=chefBurger.GetChild(j);
                    burgerLayerPositions=new Vector3[burgerLayers.Length];burgerLayerScales=new Vector3[burgerLayers.Length];
                    for(int j=0;j<burgerLayers.Length;j++){burgerLayerPositions[j]=burgerLayers[j].localPosition;burgerLayerScales[j]=burgerLayers[j].localScale;}
                }
                if(i==1)
                {
                    Models.Cylinder(station,new Vector3(-.6f,1.31f,.05f),.34f,.08f,"#50635F");
                    Models.Cylinder(station,new Vector3(-.6f,1.45f,.05f),.30f,.27f,"#B4C5BC");
                    Models.Cylinder(station,new Vector3(-.6f,1.59f,.05f),.26f,.025f,"#C4884D");
                    Models.Box(station,new Vector3(-1.0f,1.46f,.05f),new Vector3(.22f,.09f,.12f),"#586D66",.04f);
                    Models.Pasta(station,new Vector3(.65f,1.31f,-.1f));
                }
                if(i==2)
                {
                    Models.Box(station,new Vector3(-.6f,1.3f,.07f),new Vector3(1.15f,.045f,.67f),"#CDA575",.06f);
                    for(int j=0;j<5;j++)Models.Ball(station,new Vector3(-.95f+j*.17f,1.35f,.08f),new Vector3(.12f,.08f,.14f),j%2==0?"#8CB667":"#D5775C");
                    saladReveal=Models.Salad(station,new Vector3(.65f,1.31f,-.1f));
                }
                for(int p=0;p<4;p++)steam.Add(Models.Ball(room,new Vector3(x,1.6f,2.8f),Vector3.one*.14f,"#F8F1DE"));
                Models.Cylinder(room,new Vector3(x,3.15f,2.65f),.018f,1.0f,"#9B8669");
                Models.Ball(room,new Vector3(x,2.67f,2.65f),new Vector3(.75f,.26f,.75f),"#E0B66E");
                Models.Cylinder(room,new Vector3(x,2.58f,2.65f),.32f,.045f,"#FFF2CD");
            }
            for(int i=0;i<tablePositions.Length;i++)
            {
                var p=tablePositions[i];var table=Models.Group("Dining table "+(i+1),room,p);
                Models.Box(table,new Vector3(0,.78f,0),new Vector3(1.35f,.16f,1.23f),"#CDA479",.18f);
                Models.Box(table,new Vector3(0,.85f,0),new Vector3(1.26f,.035f,1.14f),"#EAD1AB",.14f);
                Models.Cylinder(table,new Vector3(0,.42f,0),.10f,.64f,"#C2A17B");Models.Cylinder(table,new Vector3(0,.15f,0),.38f,.08f,"#B99A75");
                Models.Cylinder(table,new Vector3(.35f,.94f,.31f),.055f,.16f,"#E8D6B8");Models.Ball(table,new Vector3(.35f,1.06f,.31f),new Vector3(.12f,.18f,.10f),"#8BAA6E");
                for(int side=-1;side<=1;side+=2)
                {
                    var chair=Models.Group("Cushioned chair",table,new Vector3(side*.94f,0,0));
                    string cushion=i%3==0?"#C88472":i%3==1?"#85A38E":"#D9BD80";
                    Models.Box(chair,new Vector3(0,.42f,0),new Vector3(.55f,.15f,.62f),cushion,.13f);
                    Models.Box(chair,new Vector3(side*.23f,.72f,0),new Vector3(.13f,.52f,.63f),cushion,.06f);
                    for(int l=0;l<4;l++)Models.Cylinder(chair,new Vector3((l%2-.5f)*.36f,.23f,(l/2-.5f)*.40f),.03f,.36f,"#9D805F");
                }
                if(i>=2) {if(i%2==0)Models.Pasta(table,new Vector3(0,.89f,0));else Models.Salad(table,new Vector3(0,.89f,0));}
            }
            servedBurger=Models.Burger(room,tablePositions[0]+new Vector3(0,.90f,0));
            servedPasta=Models.Pasta(room,tablePositions[1]+new Vector3(0,.90f,0));
            var desk=Models.Group("Order counter",room,new Vector3(-3.90f,0,-1.4f));
            Models.Box(desk,new Vector3(0,.54f,0),new Vector3(1.5f,.95f,.80f),"#73998A",.12f);Models.Box(desk,new Vector3(0,1.07f,0),new Vector3(1.7f,.16f,.98f),"#EED4AA",.1f);
            var till=Models.Box(desk,new Vector3(.28f,1.29f,.03f),new Vector3(.43f,.36f,.16f),"#5F776D",.04f);till.localRotation=Quaternion.Euler(-15,0,0);
            Models.Box(till,new Vector3(0,.01f,-.09f),new Vector3(.32f,.23f,.02f),"#D5E6CC",.025f);
            Models.Cylinder(desk,new Vector3(-.45f,1.22f,-.1f),.12f,.08f,"#D3AE66");
            Models.Plant(room,new Vector3(-4.35f,0,3.7f),1.15f);Models.Plant(room,new Vector3(4.2f,0,3.8f),1.35f);Models.Plant(room,new Vector3(-4.4f,0,-4.2f),.85f);Models.Plant(room,new Vector3(4.35f,0,-4.2f),.95f);
            Models.Box(room,new Vector3(0,.10f,-4.85f),new Vector3(1.8f,.025f,.48f),"#839A84",.1f);
        }
        void BuildPeople()
        {
            host=new Person(room,new Vector3(-3.9f,0,-.60f),"#719A86","#6A4C3F",false,true);
            for(int i=0;i<3;i++)chefs.Add(new Person(room,new Vector3((i-1)*3.05f,0,3.65f),i==0?"#A9BCA3":i==1?"#CF9C79":"#91AEA0","#6A4C3F",true));
            for(int i=0;i<3;i++)
            {
                Models.Box(chefs[i].rightArm,new Vector3(0,-.48f,0),new Vector3(.045f,.30f,.045f),"#A47B50",.01f);
                if(i==0)Models.Box(chefs[i].rightArm,new Vector3(0,-.66f,0),new Vector3(.18f,.16f,.045f),"#9AA99D",.02f);
                if(i==1)Models.Ball(chefs[i].rightArm,new Vector3(0,-.66f,0),new Vector3(.13f,.15f,.05f),"#BA9463");
                if(i==2)Models.Box(chefs[i].rightArm,new Vector3(.07f,-.63f,0),new Vector3(.18f,.15f,.025f),"#CAD3C4",.01f);
            }
            guestA=new Person(room,new Vector3(-4.0f,0,-3.2f),"#D08C78","#634B3B");guestB=new Person(room,new Vector3(-4.0f,0,-4.0f),"#98B1B7","#976C45");
            string[] shirts={"#D2B46F","#9FAD8D","#BE95A5"};
            for(int i=0;i<3;i++)queue.Add(new Person(room,new Vector3(-4,0,-4.8f-i*.7f),shirts[i],"#6F5745"));
            for(int i=0;i<4;i++)diners.Add(new Person(room,tablePositions[i+2]+new Vector3(i%2==0?-.94f:.94f,0,0),new[]{"#93B2AA","#D2AD75","#B18F9E","#809CAA"}[i],i%2==0?"#6D4D39":"#AA875A"));
            serverA=new Person(room,new Vector3(.15f,0,1.6f),"#688F7C","#775742",false,true);serverB=new Person(room,new Vector3(1.2f,0,1.6f),"#BF8E75","#6A4B37",false,true);
            serverBurger=Models.Burger(serverA.tray,new Vector3(0,.035f,0));serverPasta=Models.Pasta(serverB.tray,new Vector3(0,.035f,0));
        }
        void BuildFoodCamera()
        {
            showcaseRoot=Models.Group("Recipe showcase models",transform,new Vector3(40,0,0));
            showBurger=Models.Burger(showcaseRoot,Vector3.zero);showPasta=Models.Pasta(showcaseRoot,Vector3.zero);showSalad=Models.Salad(showcaseRoot,Vector3.zero);
            foreach(var t in showcaseRoot.GetComponentsInChildren<Transform>(true))t.gameObject.layer=8;
            var c=new GameObject("Food presentation camera",typeof(Camera)).GetComponent<Camera>();c.transform.SetParent(transform,false);c.transform.position=new Vector3(40.85f,1.05f,-1.3f);c.transform.LookAt(new Vector3(40,.19f,0));c.orthographic=true;c.orthographicSize=.50f;c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=Models.C("#F2E4CC");c.cullingMask=1<<8;c.depth=-1;
            var rt=new RenderTexture(384,384,24){name="Food recipe preview"};c.targetTexture=rt;foodTexture=rt;
            mainCamera.cullingMask&=~(1<<8);
        }
        RenderTexture foodTexture;
        static Sprite rounded;
        static Sprite RoundSprite()
        {
            if(rounded)return rounded;int n=64;var t=new Texture2D(n,n,TextureFormat.RGBA32,false);var pixels=new Color[n*n];
            for(int y=0;y<n;y++)for(int x=0;x<n;x++) {float dx=Mathf.Max(Mathf.Max(14-x,x-49),0),dy=Mathf.Max(Mathf.Max(14-y,y-49),0);float alpha=Mathf.Clamp01(14.7f-Mathf.Sqrt(dx*dx+dy*dy));pixels[y*n+x]=new Color(1,1,1,alpha);}
            t.SetPixels(pixels);t.Apply();rounded=Sprite.Create(t,new Rect(0,0,n,n),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(16,16,16,16));return rounded;
        }
        static RectTransform Place(RectTransform r,Transform p,float x,float y,float w,float h)
        {r.SetParent(p,false);r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        Image Panel(Transform p,float x,float y,float w,float h,string color)
        {
            var g=new GameObject("UI card",typeof(RectTransform),typeof(Image));var im=g.GetComponent<Image>();Place(im.rectTransform,p,x,y,w,h);im.sprite=RoundSprite();im.type=Image.Type.Sliced;im.color=Models.C(color);im.raycastTarget=false;return im;
        }
        TMP_Text Text(Transform p,string text,float x,float y,float w,float h,int size,Color color,bool bold=false)
        {
            var g=new GameObject("Label",typeof(RectTransform),typeof(TextMeshProUGUI));var r=g.GetComponent<RectTransform>();Place(r,p,x,y,w,h);var t=g.GetComponent<TextMeshProUGUI>();t.font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");t.text=text;t.fontSize=size;t.color=color;t.raycastTarget=false;if(bold)t.fontStyle=FontStyles.Bold;t.textWrappingMode=TextWrappingModes.Normal;return t;
        }
        void BuildUI()
        {
            var canvas=new GameObject("Restaurant film HUD",typeof(Canvas),typeof(CanvasScaler));canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;var s=canvas.GetComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1080,1920);s.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
            var root=new GameObject("Portrait frame",typeof(RectTransform)).GetComponent<RectTransform>();root.SetParent(canvas.transform,false);root.sizeDelta=new Vector2(1080,1920);
            var top=Panel(root,-2,-2,1084,348,"#F3E8D4");top.sprite=null;top.type=Image.Type.Simple;
            var bottom=Panel(root,-2,1435,1084,487,"#F3E8D4");bottom.sprite=null;bottom.type=Image.Type.Simple;
            Text(root,"IDLE RESTAURANT TYCOON",52,37,980,63,44,ink,true);
            Text(root,"BASIL & BUN  /  RESTAURANT 01",55,107,950,40,24,green,true);
            var money=Panel(root,52,164,465,110,"#FFFBF0");Text(money.transform,"CASH",25,13,350,30,18,muted,true);cash=Text(money.transform,"$1,240",24,42,420,60,42,ink,true);
            var earnings=Panel(root,538,164,490,110,"#E4EEDC");Text(earnings.transform,"RESTAURANT INCOME",25,13,430,30,18,green,true);Text(earnings.transform,"+$156 / min",24,43,440,58,38,green,true);
            wave=Text(root,"SERVICE WAVE   09 / 52",55,295,690,45,24,green,true);
            var recipe=Panel(root,772,350,260,300,"#FFFAEB");Text(recipe.transform,"ON THE MENU",18,14,230,32,18,green,true).alignment=TextAlignmentOptions.Center;
            var picture=new GameObject("3D food render",typeof(RectTransform),typeof(RawImage));Place(picture.GetComponent<RectTransform>(),recipe.transform,10,51,240,205);picture.GetComponent<RawImage>().texture=foodTexture;picture.GetComponent<RawImage>().raycastTarget=false;
            recipeTitle=Text(recipe.transform,"SESAME BURGER",12,256,238,34,20,ink,true);recipeTitle.alignment=TextAlignmentOptions.Center;
            var banner=Panel(root,55,1355,970,72,"#FFFAEB");caption=Text(banner.transform,"ORDERS IN. GOOD FOOD OUT.",20,15,930,48,25,ink,true);caption.alignment=TextAlignmentOptions.Center;
            string[] tabs={"ORDERS","KITCHEN","DINING"};
            for(int i=0;i<3;i++){tabImages[i]=Panel(root,55+i*326,1452,318,53,i==0?"#648D70":"#EEE2CB");Text(tabImages[i].transform,tabs[i],8,10,302,40,21,i==0?Color.white:ink,true).alignment=TextAlignmentOptions.Center;}
            for(int i=0;i<3;i++)
            {
                cardImages[i]=Panel(root,55+i*326,1522,310,220,"#FFFCF3");
                cardNames[i]=Text(cardImages[i].transform,"",18,18,274,65,23,ink,true);cardLevels[i]=Text(cardImages[i].transform,"LV 0 / 30",18,87,274,38,19,muted);
                var buy=Panel(cardImages[i].transform,15,143,280,58,i==0?"#739C6F":"#E5E8D9");cardPrices[i]=Text(buy.transform,"",8,10,264,40,24,i==0?Color.white:green,true);cardPrices[i].alignment=TextAlignmentOptions.Center;
            }
            toast=Text(root,"",55,1275,970,68,32,green,true);toast.alignment=TextAlignmentOptions.Center;
            Text(root,"MAP         MANAGERS         RESEARCH",55,1795,970,44,25,green,true).alignment=TextAlignmentOptions.Center;
            Text(root,"3D CONCEPT DEMO  /  PREVIEW TIMING ACCELERATED",50,1870,980,30,17,muted).alignment=TextAlignmentOptions.Center;
            for(int i=0;i<3;i++)
            {
                var coin=Panel(root,0,0,62,62,"#E6BB62");coins[i]=coin.rectTransform;coin.gameObject.AddComponent<CanvasGroup>();Text(coin.transform,"$",5,7,52,48,34,Color.white,true).alignment=TextAlignmentOptions.Center;
            }
            tapImage=Panel(root,0,0,56,56,"#FFFDF4");tap=tapImage.rectTransform;var shadow=tapImage.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color(.2f,.2f,.1f,.2f);shadow.effectDistance=new Vector2(0,-4);
        }
        static float Ease(float a,float b,float t)=>Mathf.SmoothStep(0,1,Mathf.InverseLerp(a,b,t));
        static Vector3 Move(Vector3 a,Vector3 b,float begin,float end,float t)=>Vector3.Lerp(a,b,Ease(begin,end,t));
        public void SetTime(float t)
        {
            previewTime=t;bool upgraded=t>=9.2f;
            float cameraPush=1-.035f*Mathf.Sin(t/15*Mathf.PI);mainCamera.orthographicSize=7.8f*cameraPush;
            float cookingTime=t<9.2f?t:9.2f+(t-9.2f)*1.15f;
            for(int i=0;i<chefs.Count;i++)chefs[i].Pose(cookingTime+i,new Vector3((i-1)*3.05f,.30f,3.65f),false,true,false,0,0);
            host.Pose(t,new Vector3(-3.9f,.15f,-.60f),false,false,false,0,0);
            var posA=t<2?Move(new Vector3(-4,0,-3.2f),new Vector3(-3.9f,0,-2.2f),0,1.8f,t):t<3.3f?new Vector3(-3.9f,0,-2.2f):t<5.8f?Move(new Vector3(-3.9f,0,-2.2f),new Vector3(-2.74f,0,.25f),3.3f,5.8f,t):t<12.5f?new Vector3(-2.74f,0,.25f):Move(new Vector3(-2.74f,0,.25f),new Vector3(-3.6f,0,-5.6f),12.5f,15,t);
            bool eatA=t>=5.8f && t<12.5f;guestA.Pose(t,posA,false,false,eatA,t>6.9f && t<7.7f?Mathf.Sin((t-6.9f)/.8f*Mathf.PI):0,eatA?-90:float.NaN);
            var posB=t<3.3f?Move(new Vector3(-4,0,-4.05f),new Vector3(-4,0,-3.0f),1.8f,3.3f,t):t<5.6f?Move(new Vector3(-4,0,-3),new Vector3(-3.9f,0,-2.2f),3.3f,4.5f,t):Move(new Vector3(-3.9f,0,-2.2f),new Vector3(2.74f,0,.25f),5.6f,9.5f,t);
            guestB.Pose(t,posB,false,false,t>=9.5f,0,t>=9.5f?90:float.NaN);
            for(int i=0;i<queue.Count;i++){var p=new Vector3(-4,0,-4.5f-i*.70f+Mathf.Min(t/15,1)*1.0f);queue[i].Pose(t+i,p,false,false,false,0,180);}
            for(int i=0;i<diners.Count;i++)diners[i].Pose(t+i,tablePositions[i+2]+new Vector3(i%2==0?-.94f:.94f,0,0),false,false,true,0,i%2==0?-90:90);
            Vector3 kitchen=new Vector3(0,0,1.65f),left=new Vector3(-.72f,0,.25f),right=new Vector3(.72f,0,.25f);
            Vector3 sa=t<4.5f?kitchen:t<6.8f?Move(kitchen,left,4.5f,6.8f,t):t<7.7f?left:Move(left,kitchen,7.7f,10,t);
            serverA.Pose(t,sa,t<7.15f,false,false,0,t>6.5f && t<7.7f?90:float.NaN);serverBurger.gameObject.SetActive(t<7.15f);
            Vector3 sb=t<8.5f?new Vector3(1,0,1.6f):t<10.5f?Move(new Vector3(1,0,1.6f),right,8.5f,10.5f,t):t<11.2f?right:Move(right,new Vector3(1,0,1.6f),11.2f,12.6f,t);
            serverB.Pose(t<9.2f?t:9.2f+(t-9.2f)*1.25f,sb,t<10.8f,false,false,0,t>10 && t<11.2f?-90:float.NaN);serverPasta.gameObject.SetActive(t<10.8f);
            servedBurger.gameObject.SetActive(t>=7.15f);servedBurger.localScale=Vector3.one*Mathf.Lerp(.4f,1,Ease(7.15f,7.4f,t));
            servedPasta.gameObject.SetActive(t>=10.8f);servedPasta.localScale=Vector3.one*Mathf.Lerp(.4f,1,Ease(10.8f,11.05f,t));
            for(int i=1;i<burgerLayers.Length;i++)
            {
                float appear=1.9f+Mathf.Min(i,15)*.065f;float p=Ease(appear,appear+.48f,t);
                burgerLayers[i].gameObject.SetActive(t>=appear);burgerLayers[i].localPosition=burgerLayerPositions[i]+Vector3.up*(1-p)*.60f;
                var scale=burgerLayerScales[i];scale.y*=1+.10f*Mathf.Sin(p*Mathf.PI);burgerLayers[i].localScale=scale;
            }
            float flip=t%2.8f;float flipT=Mathf.Clamp01((flip-.9f)/.7f);grillPatty.localPosition=new Vector3(-.55f,1.34f+Mathf.Sin(flipT*Mathf.PI)*.42f,0);grillPatty.localRotation=Quaternion.Euler(flipT*360,0,0);
            for(int i=0;i<steam.Count;i++){float p=(t*.65f+i*.29f)%1;steam[i].localPosition=new Vector3((i/4-1)*3.05f-.5f+Mathf.Sin(t*2+i)*.08f,1.65f+p*.8f,2.85f+Mathf.Cos(i)*.12f);steam[i].localScale=Vector3.one*(.08f+.12f*p)*(1-p*.65f);}
            saladReveal.localScale=Vector3.one*Mathf.Lerp(.001f,1,Ease(11.9f,12.6f,t));
            showcaseRoot.localRotation=Quaternion.Euler(0,t*16,0);showBurger.gameObject.SetActive(t<5);showPasta.gameObject.SetActive(t>=5 && t<12);showSalad.gameObject.SetActive(t>=12);
            recipeTitle.text=t<5?"SESAME BURGER":t<12?"TOMATO BASIL PASTA":"GARDEN SALAD";
            float cashValue=1240+32*Ease(7.15f,7.85f,t)-40*Ease(9.2f,9.45f,t)+36*Ease(10.8f,11.5f,t)+44*Ease(13,13.7f,t);
            cash.text="$"+Mathf.RoundToInt(cashValue).ToString("N0");wave.text="SERVICE WAVE   "+(9+(t>=7.15f?1:0)+(t>=10.8f?1:0)+(t>=13?1:0)).ToString("D2")+" / 52";
            caption.text=t<3?"WELCOME IN. YOUR TABLE IS WAITING.":t<6?"FRESHLY COOKED. BEAUTIFULLY PLATED.":t<8.5f?"HOT FOOD, STRAIGHT TO YOUR TABLE.":t<10.5f?"UPGRADE YOUR SERVICE. KEEP GUESTS SMILING.":t<12?"A LITTLE FASTER. A LOT MORE DELICIOUS.":"NEW RECIPE  /  GARDEN SALAD";
            toast.text=t>=7.15f && t<8.25f?"HAPPY GUEST  +$32":t>=9.2f && t<10.5f?"RUNNER SPEED  /  LEVEL 1":t>=12 && t<14?"A FRESH RECIPE JOINS THE MENU":"";
            bool kitchenTab=t>=8.5f;
            string[] names=kitchenTab?new[]{"RUNNER\nSPEED","COOK\nSPEED","DISH\nVALUE"}:new[]{"ORDER\nSPEED","EXPRESS\nORDERS","ORDER\nVALUE"};
            int[] prices=kitchenTab?new[]{40,50,80}:new[]{50,50,100};
            for(int i=0;i<3;i++)
            {
                cardNames[i].text=names[i];cardLevels[i].text="LV "+(i==0 && upgraded?"1":"0")+" / 30";cardPrices[i].text=i==0 && t>=9.2f && t<10.5f?"UPGRADED":"BUY $"+(i==0 && upgraded?62:prices[i]);
                bool selected=(kitchenTab?1:0)==i;tabImages[i].color=selected?Models.C("#648D70"):Models.C("#EEE2CB");tabImages[i].GetComponentInChildren<TMP_Text>().color=selected?Color.white:ink;
            }
            float pulse=t>8.8f && t<9.6f?1+.035f*Mathf.Sin((t-8.8f)/.8f*Mathf.PI):1;cardImages[0].rectTransform.localScale=Vector3.one*pulse;
            tapImage.gameObject.SetActive(t>=8.6f && t<9.65f);tap.anchoredPosition=Vector2.Lerp(new Vector2(500,-1330),new Vector2(182,-1695),Ease(8.6f,9.1f,t));tap.localScale=Vector3.one*(t>=9.1f && t<9.35f?.75f:1);
            float[] times={7.15f,10.8f,13};
            for(int i=0;i<3;i++)
            {
                float p=(t-times[i])/.8f;coins[i].gameObject.SetActive(p>=0 && p<=1);
                Vector3 wp=mainCamera.WorldToScreenPoint(tablePositions[i%2]+new Vector3(0,1.8f,0));
                Vector2 from=new Vector2(wp.x/Screen.width*1080,(1-wp.y/Screen.height)*1920),to=new Vector2(430,205);float f=Mathf.Clamp01(p);
                Vector2 at=Vector2.Lerp(from,to,f);at.y-=Mathf.Sin(f*Mathf.PI)*120;coins[i].anchoredPosition=new Vector2(at.x,-at.y);coins[i].localScale=Vector3.one*Mathf.Lerp(1,.5f,f);coins[i].GetComponent<CanvasGroup>().alpha=1-Mathf.Clamp01((f-.75f)/.25f);
            }
        }
        public IEnumerator Record(string path)
        {
            Directory.CreateDirectory(path);manual=true;Time.captureFramerate=30;if(sound)sound.Stop();
            for(int i=0;i<450;i++)
            {
                SetTime(i/30f);yield return new WaitForEndOfFrame();
                var tex=new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(path,$"frame_{i:D4}.png"),tex.EncodeToPNG());Destroy(tex);
            }
            Time.captureFramerate=0;manual=false;start=Time.time;Debug.Log("RESTAURANT_RECORD_COMPLETE "+path);
        }
    }
}
