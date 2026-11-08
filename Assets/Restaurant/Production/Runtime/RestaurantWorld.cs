using System;
using System.Collections.Generic;
using UnityEngine;
using RestaurantPreview;
namespace IdleRestaurant {
 public sealed partial class RestaurantWorld:MonoBehaviour {
  public RestaurantCatalog catalog;public RestaurantGame game;public Camera viewCamera;Camera ViewCamera=>viewCamera?viewCamera:Camera.main;public Transform room;public int shown=-1;
  readonly List<Person> guests=new List<Person>();readonly List<Person> staff=new List<Person>();readonly List<RestaurantSeat> seats=new List<RestaurantSeat>();RestaurantServiceView serviceView;
  public int OccupiedSeats=>seats.FindAll(x=>x&&x.owner>=0).Count;public int AvailableSeats=>seats.FindAll(x=>x&&x.gameObject.activeInHierarchy).Count;
  readonly List<Transform> dishes=new List<Transform>();
  static readonly string[] floors={"#E9D8AF","#DDD8CA","#D9D9BF","#EFE4D0","#EEDCE1"};
  static readonly string[] walls={"#C5D6A3","#86B9B0","#547E70","#F7EEDC","#BCE0D8"};
  static readonly string[] accents={"#E99B53","#C86552","#B85848","#4D90AF","#D98FB0"};
  void Box(Transform p,Vector3 pos,Vector3 size,string c)=>Models.Box(p,pos,size,c);
  public void Show(int index){
   shown=index;if(room){room.gameObject.SetActive(false);Destroy(room.gameObject);}guests.Clear();staff.Clear();seats.Clear();dishes.Clear();room=Models.Group("Playable outlet: "+catalog.outlets[index].title,transform,Vector3.zero);
   int d=index/8,variant=index%8;string accent=accents[d];
   if(catalog.outlets[index].environment){Instantiate(catalog.outlets[index].environment,room);}
   else BuildEnvironment(room,d,variant);
   var floorCollider=new GameObject("Floor collider",typeof(BoxCollider));floorCollider.transform.SetParent(room,false);floorCollider.GetComponent<BoxCollider>().center=new Vector3(0,-.06f,.4f);floorCollider.GetComponent<BoxCollider>().size=new Vector3(10.8f,.08f,13);
   for(int z=0;z<3;z++)for(int x=0;x<2;x++){
    Vector3 t=new Vector3(-2.15f+x*4.3f,0,-2.5f+z*2);var table=Models.Group("Dining table "+(z*2+x),room,Vector3.zero);Box(table,t+new Vector3(0,.75f,0),new Vector3(1.3f,.12f,.95f),"#D8B788");Models.Cylinder(table,t+new Vector3(0,.35f,0),.1f,.7f,"#697975");var tableBlock=table.gameObject.AddComponent<BoxCollider>();tableBlock.center=t+new Vector3(0,.4f,0);tableBlock.size=new Vector3(1.35f,.8f,1);
    for(int side=0;side<2;side++){Vector3 seat=t+new Vector3((side==0?-1:1)*1.02f,0,0);var chair=Models.Group("Chair "+seats.Count,table,seat);var slot=chair.gameObject.AddComponent<RestaurantSeat>();slot.index=seats.Count;slot.centre=seat;slot.plate=t+new Vector3(side==0?-.35f:.35f,.84f,0);slot.yaw=side==0?-90:90;seats.Add(slot);Box(chair,new Vector3(0,.42f,0),new Vector3(.65f,.14f,.65f),accent);Box(chair,new Vector3((side==0?-.28f:.28f),.75f,0),new Vector3(.14f,.65f,.65f),accent);for(int j=0;j<2;j++)Box(chair,new Vector3((j==0?-.19f:.19f),.2f,0),new Vector3(.065f,.4f,.5f),"#6D7770");var collider=chair.gameObject.AddComponent<BoxCollider>();collider.center=new Vector3(0,.48f,0);collider.size=new Vector3(.68f,.94f,.68f);}
    CombineStatic(table);table.gameObject.AddComponent<RestaurantActorMarker>();
   }

   for(int i=0;i<3;i++){
    Vector3 p=new Vector3(-2.8f+i*2.4f,0,4.0f);Box(room,p+new Vector3(0,.55f,0),new Vector3(2.1f,1.1f,1.2f),i==0?accent:walls[d]);Box(room,p+new Vector3(0,1.13f,0),new Vector3(2.2f,.13f,1.25f),"#F6EADB");
    if(i==0){Box(room,p+new Vector3(0,1.34f,0),new Vector3(.55f,.35f,.4f),"#344B49");Box(room,p+new Vector3(0,1.37f,-.22f),new Vector3(.42f,.20f,.035f),"#A7D3B1");}
    else{Models.Cylinder(room,p+new Vector3(-.4f,1.25f,0),.27f,.12f,"#435550");}
    var counter=new GameObject("Counter collider",typeof(BoxCollider));counter.transform.SetParent(room,false);counter.GetComponent<BoxCollider>().center=p+new Vector3(0,.6f,0);counter.GetComponent<BoxCollider>().size=new Vector3(2.2f,1.2f,1.3f);
    var actorPrefab=i==0?catalog.hostPrefab:catalog.chefPrefab;var chef=actorPrefab?new Person(room,p+new Vector3(0,0,1.08f),actorPrefab):new Person(room,p+new Vector3(0,0,1.08f),i==0?accent:"#F6F0DA","#614B3C",i>0,i==0);staff.Add(chef);
   }
   int visible=Math.Min(catalog.visibleGuests,18);string[] shirts={"#7AAEA6","#E9AE62","#A7B98C","#CE8D83","#9B9FCA","#E5BD95"};
   for(int i=0;i<visible;i++){var person=catalog.guestPrefab?new Person(room,new Vector3(-4,0,-4),catalog.guestPrefab):new Person(room,new Vector3(-4,0,-4),shirts[i%6],i%2==0?"#554539":"#9A6D44");person.root.localScale=Vector3.one*.85f;var block=new MaterialPropertyBlock();block.SetColor("_BaseColor",Models.C(i<Math.Min(4,catalog.outlets[index].premium)?"#E5AC65":shirts[i%6]));foreach(var renderer in person.root.GetComponentsInChildren<Renderer>())if(renderer.sharedMaterial.name.Contains("#75AAA1"))renderer.SetPropertyBlock(block);guests.Add(person);}
   BuildGuestAccess();BuildDistrictDetails(d,variant);
   var returnTray=Models.Group("Tray return",room,TrayReturnPosition);Box(returnTray,new Vector3(0,.45f,0),new Vector3(.8f,.9f,.7f),walls[d]);var trayCollider=returnTray.gameObject.AddComponent<BoxCollider>();trayCollider.center=new Vector3(0,.45f,0);trayCollider.size=new Vector3(.7f,.9f,.6f);AccessLabel(returnTray,"PLATES",new Vector3(0,1.25f,0));for(int plate=0;plate<3;plate++)Models.Cylinder(returnTray,new Vector3(0,.94f+plate*.04f,0),.28f,.03f,"#F8F3E7");CombineStatic(room);ImproveMaterialSeparation();if(ViewCamera)ViewCamera.backgroundColor=Models.C(floors[d]);if(!serviceView)serviceView=gameObject.AddComponent<RestaurantServiceView>();serviceView.Setup(game,this,guests,staff,seats);var upgrades=room.gameObject.AddComponent<RestaurantUpgradeVisuals>();upgrades.Setup(game,index);var atmosphere=room.gameObject.AddComponent<RestaurantAtmosphere>();atmosphere.Setup(game,ViewCamera,index);
  }
  void ImproveMaterialSeparation(){
   // Property blocks preserve replaceable shared assets and add no material instances.
   var colors=new Dictionary<string,string>{{"#E9D8AF","#D4C5A4"},{"#C5D6A3","#75A68B"},{"#E99B53","#E58B37"},{"#D8B788","#BD925D"},{"#F6EADB","#FFF4E1"}};
   foreach(var renderer in room.GetComponentsInChildren<MeshRenderer>())foreach(var pair in colors)if(renderer.sharedMaterial&&renderer.sharedMaterial.name=="Restaurant "+pair.Key){var block=new MaterialPropertyBlock();renderer.GetPropertyBlock(block);block.SetColor("_BaseColor",Models.C(pair.Value));renderer.SetPropertyBlock(block);break;}
  }
  public static Transform Food(Transform p,Vector3 pos,int district,int variation){
   if(district==0)return variation%3==0?Models.Burger(p,pos):variation%3==1?Models.Pasta(p,pos):Models.Salad(p,pos);
   if(district==1)return variation%2==0?Models.Burger(p,pos):Models.Pasta(p,pos);
   if(district==3)return variation%2==0?Models.Salad(p,pos):Models.Pasta(p,pos);
   var r=Models.Group(district==2?"Ramen bowl":"Berry dessert",p,pos);
   if(district==2){Models.Ball(r,new Vector3(0,.1f,0),new Vector3(.58f,.23f,.58f),"#EFF0D9");Models.Cylinder(r,new Vector3(0,.21f,0),.235f,.025f,"#C8914D");for(int i=0;i<8;i++){float a=i*.8f;Models.Box(r,new Vector3(Mathf.Sin(a)*.14f,.24f,Mathf.Cos(a)*.14f),new Vector3(.1f,.035f,.13f),i%2==0?"#7EAD66":"#F0CD79",.02f);}for(int i=0;i<2;i++){var stick=Models.Box(r,new Vector3(.08f+i*.06f,.28f,0),new Vector3(.025f,.025f,.65f),"#AA7350",.01f);stick.localRotation=Quaternion.Euler(0,25,0);}}
   else{Models.Plate(r,Vector3.zero);Models.Cylinder(r,new Vector3(0,.12f,0),.22f,.22f,"#E4B58A");Models.Cylinder(r,new Vector3(0,.25f,0),.23f,.09f,"#F5D9DC");Models.Ball(r,new Vector3(0,.34f,0),new Vector3(.26f,.18f,.26f),"#FFF2DF");Models.Ball(r,new Vector3(0,.45f,0),Vector3.one*.12f,"#D56672");}
   return r;
  }
  public static void BuildEnvironment(Transform r,int d,int variant){
   string floor=floors[d],wall=walls[d],accent=accents[d];
   Models.Box(r,new Vector3(0,-.22f,.4f),new Vector3(10.8f,.4f,13),floor,.3f);
   Models.Box(r,new Vector3(0,.02f,-5.6f),new Vector3(11.2f,.12f,1),d==0?"#BDBFA5":floor);
   if(d!=0){Models.Box(r,new Vector3(0,1.6f,6.4f),new Vector3(10.8f,3.2f,.25f),wall);Models.Box(r,new Vector3(-5.3f,.7f,.4f),new Vector3(.22f,1.4f,12.2f),wall);}
   else{for(int j=0;j<3;j++){Models.Box(r,new Vector3(-3.3f+j*3.3f,2.6f,5),new Vector3(3.1f,.18f,2.8f),j%2==0?accent:"#FFF0D3");for(int k=0;k<2;k++)Models.Cylinder(r,new Vector3(-4.6f+j*3.3f+k*2.6f,1.3f,5.6f),.05f,2.6f,"#697D60");}for(int i=0;i<7;i++){Models.Ball(r,new Vector3(-4.2f+i*1.4f,3.3f,3.4f),Vector3.one*.18f,"#FFE4A0");}}
   for(int j=0;j<3;j++){
    float x=-3.3f+j*3.3f;
    if(d==1){Models.Box(r,new Vector3(x,2.1f,6.2f),new Vector3(2.5f,1.7f,.12f),"#FFF0D7");Models.Box(r,new Vector3(x,2.1f,6.1f),new Vector3(2.2f,1.4f,.08f),"#A6CFCC");Models.Box(r,new Vector3(x,2.1f,6.02f),new Vector3(.08f,1.4f,.08f),accent);}
    if(d==2){Models.Box(r,new Vector3(x,1.8f,6.15f),new Vector3(2.9f,2.5f,.1f),"#DBCCAA");for(int k=0;k<6;k++)Models.Cylinder(r,new Vector3(x-1.2f+k*.48f,1.8f,6.04f),.025f,2.5f,"#8D7951");Models.Ball(r,new Vector3(x,2.8f,3.9f),new Vector3(.5f,.7f,.5f),accent);Models.Cylinder(r,new Vector3(x,3.2f,3.9f),.07f,.15f,"#453C35");}
    if(d==3){Models.Box(r,new Vector3(x,1.5f,6.15f),new Vector3(2.4f,2.4f,.1f),"#6CA9BF");var pts=new List<Vector3>();for(int k=0;k<=16;k++){float a=k*Mathf.PI/16;pts.Add(new Vector3(x+1.27f*Mathf.Cos(a),1.7f+1.27f*Mathf.Sin(a),5.98f));}Models.Tube(r,pts,.15f,"#FFFBEC",10);Models.Box(r,new Vector3(x-1.27f,.85f,5.98f),new Vector3(.28f,1.7f,.28f),"#FFFBEC");Models.Box(r,new Vector3(x+1.27f,.85f,5.98f),new Vector3(.28f,1.7f,.28f),"#FFFBEC");}
    if(d==4){Models.Box(r,new Vector3(x,1.19f,5.5f),new Vector3(2.8f,.1f,.85f),"#F5E5D0");Models.Box(r,new Vector3(x,1.9f,6.16f),new Vector3(2.8f,1.8f,.12f),"#F6ECDD");Models.Box(r,new Vector3(x,1.9f,6.05f),new Vector3(2.5f,1.5f,.08f),accent);for(int k=0;k<3;k++)Food(r,new Vector3(x-.8f+k*.8f,1.25f,5.5f),4,k);}
   }
   // Outlet-specific landscaping and patterned floor, independent of simulation capacity.
   for(int i=0;i<4;i++){Models.Plant(r,new Vector3(i%2==0?-4.6f:4.6f,0,-3.8f+(i/2)*7.8f),.9f+(variant%3)*.08f);}
   if(d==1)for(int z=0;z<8;z++)for(int x=0;x<7;x++)if((x+z)%2==0)Models.Box(r,new Vector3(-4.2f+x*1.4f,-.005f,-4.2f+z*1.4f),new Vector3(1.35f,.02f,1.35f),"#F6EFDA",.01f);
   if(d==3)for(int j=0;j<5;j++)Models.Box(r,new Vector3(-4+j*2,3.35f,4.8f),new Vector3(.18f,.18f,3.3f),"#BBA47C");
   if(d==3){for(int pole=0;pole<4;pole++)Models.Box(r,new Vector3(pole%2==0?-4.4f:4.4f,1.65f,pole/2==0?3.25f:6.25f),new Vector3(.16f,3.3f,.16f),"#BBA47C");Models.Box(r,new Vector3(0,3.3f,3.25f),new Vector3(9,.16f,.16f),"#BBA47C");}
   for(int k=0;k<variant%4;k++){Models.Box(r,new Vector3(4.7f,.5f,-2+k*1.7f),new Vector3(.45f,1,.9f),accent);}
  }
  // Static material batches reduce the original concept's hundreds of draw calls.
  public static void CombineStatic(Transform root){
   var groups=new Dictionary<Material,List<CombineInstance>>();var toDisable=new List<MeshRenderer>();
   foreach(var filter in root.GetComponentsInChildren<MeshFilter>()){if(filter.transform.GetComponentInParent<RestaurantActorMarker>())continue;bool actor=false;for(var p=filter.transform;p&&p!=root;p=p.parent)if(p.name=="Guest"||p.name=="Chef"||p.name=="Server"){actor=true;break;}if(actor||filter.GetComponentInParent<RestaurantActorRig>())continue;var renderer=filter.GetComponent<MeshRenderer>();if(!renderer||!renderer.enabled||!filter.sharedMesh)continue;var mat=renderer.sharedMaterial;if(!groups.ContainsKey(mat))groups[mat]=new List<CombineInstance>();groups[mat].Add(new CombineInstance{mesh=filter.sharedMesh,transform=root.worldToLocalMatrix*filter.transform.localToWorldMatrix});toDisable.Add(renderer);}
   foreach(var pair in groups){var go=new GameObject("Static batch "+pair.Key.name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(root,false);var mesh=new Mesh{indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.CombineMeshes(pair.Value.ToArray());if(Application.isPlaying)go.AddComponent<RestaurantRuntimeMeshOwner>().mesh=mesh;go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=pair.Key;}
   foreach(var renderer in toDisable)renderer.enabled=false;
  }
  void Update(){if(game!=null&&shown!=game.State.selected)Show(game.State.selected);}

 }
 // Replacement characters can place this marker above their renderers.
 public sealed class RestaurantActorMarker:MonoBehaviour{}
}
