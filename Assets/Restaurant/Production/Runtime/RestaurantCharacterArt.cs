using UnityEngine;using RestaurantPreview;
namespace IdleRestaurant{
 public static class RestaurantCharacterArt{
  public static readonly string[] Shirts={"#D995A5","#679DAA","#DDA95E","#83A579","#987EAB","#628FA0","#C98C69","#7987A7","#BD879D","#759B8C","#BAA26E","#728B91"};
  static readonly string[] Skins={"#EEC7A5","#B97F58","#D4A078","#76503C","#E0B494","#986344","#C7946B","#593E30"};
  static readonly string[] Hairs={"#49392F","#6B4C38","#B58B52","#382F2C","#8A5E45","#D2B387","#4A3632","#75716A"};
  public static string Skin(int style)=>Skins[(style*3+style/3)%Skins.Length];
  public static Person Create(Transform parent,Vector3 pos,int style,int role){style=Mathf.Abs(style)%12;string shirt=role==1?"#FFF3DD":role==3?"#57968F":Shirts[style];var p=new Person(parent,pos,shirt,Hairs[(style*5)%Hairs.Length],role==1,role==2,style);Outfit(p,style,role,shirt);var look=p.root.gameObject.AddComponent<RestaurantCharacterAppearance>();look.style=style;look.feminine=style%2==0;look.role=role;return p;}
  public static void Hair(Transform head,int style,string color,bool chef){var hair=Models.Group("Editable hairstyle "+style,head,Vector3.zero);int type=style%12;
   if(type!=11)Models.Ball(hair,new Vector3(0,.18f,.035f),new Vector3(.50f,.22f,.46f),color);
   switch(type){
    case 0: for(int side=-1;side<=1;side+=2)Models.Ball(hair,new Vector3(side*.20f,-.015f,.06f),new Vector3(.13f,.39f,.34f),color);Models.Box(hair,new Vector3(0,-.04f,.18f),new Vector3(.39f,.34f,.13f),color,.06f);break;
    case 1: var swoop=Models.Ball(hair,new Vector3(-.07f,.20f,-.105f),new Vector3(.34f,.14f,.27f),color);swoop.localRotation=Quaternion.Euler(0,0,-18);break;
    case 2: Models.Ball(hair,new Vector3(0,.02f,.31f),new Vector3(.22f,.44f,.23f),color);Models.Ball(hair,new Vector3(0,.15f,.27f),new Vector3(.23f,.055f,.16f),"#D6B065");break;
    case 3: for(int i=0;i<9;i++){float a=i*2.4f;Models.Ball(hair,new Vector3(Mathf.Cos(a)*.18f,.19f+Mathf.Sin(i)*.04f,Mathf.Sin(a)*.15f),Vector3.one*.19f,color);}break;
    case 4: Models.Ball(hair,chef?new Vector3(0,-.02f,.28f):new Vector3(0,.31f,.06f),new Vector3(.26f,.25f,.25f),color);break;
    case 5: for(int side=-1;side<=1;side+=2)Models.Box(hair,new Vector3(side*.205f,.075f,.03f),new Vector3(.065f,.20f,.27f),color,.025f);break;
    case 6: for(int side=-1;side<=1;side+=2){for(int j=0;j<4;j++)Models.Ball(hair,new Vector3(side*.215f,.05f-j*.09f,.05f+j*.02f),Vector3.one*(.12f-j*.01f),color);Models.Ball(hair,new Vector3(side*.215f,-.24f,.11f),Vector3.one*.075f,"#D6B065");}break;
    case 7: Models.Ball(hair,new Vector3(0,.235f,.035f),new Vector3(.52f,.21f,.48f),"#D2AA69");Models.Box(hair,new Vector3(0,.18f,-.18f),new Vector3(.45f,.05f,.12f),"#D2AA69",.02f);break;
    case 8: for(int i=0;i<5;i++)Models.Ball(hair,new Vector3(-.21f+i*.105f,-.04f+Mathf.Sin(i)*.04f,.17f),new Vector3(.16f,.40f,.25f),color);for(int side=-1;side<=1;side+=2)Models.Ball(hair,new Vector3(side*.21f,.025f,.04f),new Vector3(.11f,.28f,.25f),color);break;
    case 9: Models.Ball(hair,new Vector3(0,-.07f,.29f),new Vector3(.19f,.32f,.18f),color);break;
    case 10: var fringe=Models.Ball(hair,new Vector3(.10f,.18f,-.12f),new Vector3(.30f,.12f,.23f),color);fringe.localRotation=Quaternion.Euler(0,0,22);break;
    case 11: for(int side=-1;side<=1;side+=2)Models.Ball(hair,new Vector3(side*.215f,.10f,.07f),new Vector3(.075f,.17f,.26f),color);Models.Ball(hair,new Vector3(0,-.18f,-.15f),new Vector3(.25f,.12f,.14f),color);break;
   }
   RestaurantWorld.CombineStatic(hair);
  }
  static void Outfit(Person p,int style,int role,string shirt){string pants=style%3==0?"#495961":style%3==1?"#73645B":"#495D54";foreach(var r in p.root.GetComponentsInChildren<MeshRenderer>())if(r.sharedMaterial&&r.sharedMaterial.name=="Restaurant #54646A")r.sharedMaterial=Models.Mat(pants);
   var details=Models.Group("Editable outfit and accessories",p.body,Vector3.zero);
   if(role==0&&(style==0||style==4||style==8)){Models.Box(details,new Vector3(0,.43f,0),new Vector3(.48f,.18f,.34f),shirt,.08f);Models.Box(details,new Vector3(0,.50f,-.177f),new Vector3(.43f,.045f,.025f),"#F4DBA2",.015f);}
   if(role==0&&(style==2||style==6||style==10)){for(int side=-1;side<=1;side+=2)Models.Box(details,new Vector3(side*.17f,.70f,-.17f),new Vector3(.10f,.40f,.04f),style==2?"#EEE5D4":"#D0D7BA",.02f);}
   if(role==0&&style==7){Models.Box(details,new Vector3(0,.59f,-.195f),new Vector3(.28f,.13f,.035f),"#56647E",.035f);for(int side=-1;side<=1;side+=2)Models.Box(details,new Vector3(side*.07f,.84f,-.19f),new Vector3(.018f,.14f,.02f),"#F4E7D3",.006f);}
   if(role==0&&style%3==1)for(int j=0;j<3;j++)Models.Ball(details,new Vector3(0,.83f-j*.12f,-.19f),Vector3.one*.035f,"#F7EDDA");
   if(role==3){Models.Box(details,new Vector3(0,.66f,-.19f),new Vector3(.34f,.42f,.035f),"#E3D2A5",.025f);Models.Box(details,new Vector3(0,.58f,-.215f),new Vector3(.23f,.14f,.025f),"#517C76",.02f);Models.Box(details,new Vector3(0,.51f,-.19f),new Vector3(.43f,.055f,.045f),"#74614D",.015f);}
   RestaurantWorld.CombineStatic(details);
   if(style%4==0||style==11){var glasses=Models.Group("Glasses",p.head,Vector3.zero);for(int side=-1;side<=1;side+=2){var points=new System.Collections.Generic.List<Vector3>();for(int j=0;j<=16;j++){float a=j*Mathf.PI*2/16;points.Add(new Vector3(side*.095f+Mathf.Cos(a)*.073f,.03f+Mathf.Sin(a)*.077f,-.243f));}Models.Tube(glasses,points,.010f,"#46534A");}Models.Box(glasses,new Vector3(0,.03f,-.243f),new Vector3(.05f,.018f,.015f),"#46534A",.005f);RestaurantWorld.CombineStatic(glasses);}
  }
 }
}
