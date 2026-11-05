using UnityEngine;
using RestaurantPreview;
namespace IdleRestaurant {
 public static class RestaurantFoodArt {
  public static Transform Create(Transform parent,Vector3 position,int outlet){
   if(outlet==0||outlet==2||outlet==7)return Models.Burger(parent,position);
   if(outlet==10||outlet==18||outlet==26||outlet==31)return Models.Pasta(parent,position);
   if(outlet==14||outlet==24||outlet==29||outlet==30)return Models.Salad(parent,position);
   if(outlet==4||outlet==16||outlet==23)return RestaurantWorld.Food(parent,position,2,0);
   var r=Models.Group("Signature dish "+outlet,parent,position);Models.Plate(r,Vector3.zero);
   if(outlet==1||outlet==11){Models.Box(r,new Vector3(0,.09f,0),new Vector3(.43f,.13f,.42f),"#CF9052",.06f);Models.Box(r,new Vector3(0,.16f,0),new Vector3(.34f,.025f,.33f),"#F5DA91",.05f);Models.Ball(r,new Vector3(0,.2f,0),new Vector3(.29f,.04f,.27f),"#FFF4D4");Models.Ball(r,new Vector3(0,.23f,0),new Vector3(.11f,.06f,.11f),"#F7BA36");}
   else if(outlet==3){for(int i=0;i<2;i++){var t=Models.Group("Taco",r,new Vector3(-.12f+i*.24f,.13f,0));Models.Ball(t,Vector3.zero,new Vector3(.18f,.27f,.45f),"#E6B857");Models.Ball(t,new Vector3(0,.09f,0),new Vector3(.13f,.06f,.33f),"#74A54E");for(int j=0;j<3;j++)Models.Ball(t,new Vector3(0,.13f,-.1f+j*.1f),Vector3.one*.065f,"#D56843");}}
   else if(outlet==5){Models.Cylinder(r,new Vector3(0,.08f,0),.28f,.07f,"#E0A45E");Models.Cylinder(r,new Vector3(0,.125f,0),.245f,.03f,"#D16B48");Models.Cylinder(r,new Vector3(0,.146f,0),.23f,.02f,"#F1CD72");for(int i=0;i<6;i++){float a=i*Mathf.PI/3;Models.Cylinder(r,new Vector3(Mathf.Cos(a)*.15f,.165f,Mathf.Sin(a)*.15f),.045f,.015f,"#BA5C43");}}
   else if(outlet==8||outlet==9){for(int i=0;i<4;i++)Models.Cylinder(r,new Vector3(0,.08f+i*.06f,0),.235f,.055f,i%2==0?"#D4A360":"#E8BF78");Models.Box(r,new Vector3(0,.31f,0),new Vector3(.1f,.04f,.09f),"#FFE5A0",.01f);Models.Ball(r,new Vector3(.13f,.3f,.09f),new Vector3(.11f,.07f,.1f),"#C85759");}
   else if(outlet==12||outlet==25){Models.Ball(r,new Vector3(0,.17f,0),new Vector3(.36f,.23f,.43f),"#BE8254");for(int i=0;i<2;i++)Models.Ball(r,new Vector3(i==0?-.19f:.19f,.13f,-.1f),new Vector3(.13f,.12f,.25f),"#D29A65");Vegetables(r);}
   else if(outlet==13||outlet==28){var fish=Models.Ball(r,new Vector3(0,.13f,0),new Vector3(.2f,.14f,.47f),"#E6B188");fish.localRotation=Quaternion.Euler(0,40,0);for(int i=0;i<4;i++)Models.Box(r,new Vector3(-.19f+i*.09f,.09f,-.23f),new Vector3(.055f,.05f,.20f),"#EFC56E",.015f);Vegetables(r);}
   else if(outlet==17||outlet==22){for(int i=0;i<5;i++){float a=i*1.25f;var p=new Vector3(Mathf.Cos(a)*.16f,.12f,Mathf.Sin(a)*.16f);Models.Ball(r,p,new Vector3(.18f,.16f,.15f),"#F5E7C5");Models.Tube(r,new[]{p+new Vector3(-.06f,.065f,0),p+new Vector3(0,.09f,0),p+new Vector3(.06f,.065f,0)},.013f,"#D8C69F");}}
   else if(outlet==19){for(int i=0;i<4;i++){var p=new Vector3(i%2==0?-.12f:.12f,.1f,i/2==0?-.12f:.12f);Models.Cylinder(r,p,.1f,.13f,"#3E5745");Models.Cylinder(r,p+new Vector3(0,.07f,0),.083f,.02f,"#F8ECD5");Models.Cylinder(r,p+new Vector3(0,.087f,0),.04f,.015f,"#E7A078");}}
   else if(outlet<32){Models.Ball(r,new Vector3(0,.12f,0),new Vector3(.5f,.2f,.5f),"#F0DCA8");Models.Cylinder(r,new Vector3(0,.2f,0),.22f,.035f,outlet==20?"#D89842":"#CF9A59");Vegetables(r);}
   else if(outlet==32){for(int i=0;i<4;i++){var p=new Vector3(i%2==0?-.12f:.12f,.08f+i*.012f,i/2==0?-.11f:.11f);Models.Cylinder(r,p,.12f,.055f,"#D5A574");for(int j=0;j<3;j++)Models.Ball(r,p+new Vector3(Mathf.Sin(j*2)*.065f,.034f,Mathf.Cos(j*2)*.065f),Vector3.one*.035f,"#745043");}}
   else if(outlet==36){for(int i=0;i<2;i++){var p=new Vector3(i==0?-.12f:.12f,.12f,0);Models.Cylinder(r,p,.1f,.2f,"#D7AA76");Models.Ball(r,p+new Vector3(0,.18f,0),Vector3.one*.23f,i==0?"#C4D29F":"#ECA7B7");}}
   else if(outlet==37){for(int i=0;i<3;i++){float a=i*2.1f;Models.Ball(r,new Vector3(Mathf.Cos(a)*.14f,.14f,Mathf.Sin(a)*.14f),new Vector3(.21f,.18f,.21f),i==0?"#E5B7BC":i==1?"#BED09B":"#F2E4CC");}}
   else if(outlet==38){for(int i=0;i<7;i++){float a=(i/6f)*Mathf.PI;Models.Ball(r,new Vector3(Mathf.Cos(a)*.2f,.12f,Mathf.Sin(a)*.17f),new Vector3(.12f+(1-Mathf.Abs(i-3)/3f)*.06f,.16f,.16f),i%2==0?"#D89E5C":"#EABD78");}}
   else if(outlet==34){for(int i=0;i<3;i++){var crepe=Models.Box(r,new Vector3(0,.08f+i*.035f,0),new Vector3(.40f,.035f,.27f),"#E8BE81",.08f);crepe.localRotation=Quaternion.Euler(0,i*25,0);}Berries(r,.21f);}
   else{Models.Cylinder(r,new Vector3(0,.13f,0),.23f,.18f,outlet==35?"#805443":"#E7BC88");Models.Cylinder(r,new Vector3(0,.25f,0),.235f,.07f,outlet==35?"#B47D61":"#F2D4D0");Berries(r,.32f);}
   return r;
  }
  static void Berries(Transform r,float y){for(int i=0;i<5;i++){float a=i*1.25f;Models.Ball(r,new Vector3(Mathf.Cos(a)*.13f,y,Mathf.Sin(a)*.13f),Vector3.one*.10f,i%2==0?"#D66675":"#7C7EAD");}}
  static void Vegetables(Transform r){for(int i=0;i<4;i++){float a=i*1.5f;Models.Ball(r,new Vector3(Mathf.Cos(a)*.23f,.12f,Mathf.Sin(a)*.23f),new Vector3(.09f,.08f,.1f),i%2==0?"#82AB5B":"#E6B35C");}}
 }
}
