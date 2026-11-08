using UnityEngine;
using RestaurantPreview;
namespace IdleRestaurant
{
    public sealed partial class RestaurantWorld
    {
        void BuildDistrictDetails(int district,int variation)
        {
            var definition=game.Journey.districts[district];
            var root=Models.Group("District identity: "+definition.name,room,Vector3.zero);
            if(definition.decoration){Instantiate(definition.decoration,root);return;}
            // Decorations stay behind the counters, outside every customer route and chair collider.
            string ink="#"+ColorUtility.ToHtmlStringRGB(definition.ink),accent="#"+ColorUtility.ToHtmlStringRGB(definition.accent);
            if(district==0){
                var sign=Models.Group("Street market sign",root,new Vector3(-3.3f,3.0f,3.42f));
                Models.Box(sign,Vector3.zero,new Vector3(2.2f,.48f,.16f),ink);AccessLabel(sign,"MARKET",new Vector3(0,.03f,-.12f));for(int side=-1;side<=1;side+=2)Models.Cylinder(sign,new Vector3(side*.8f,-.30f,.06f),.035f,.4f,ink);
                for(int j=0;j<7;j++)Models.Box(root,new Vector3(-4.2f+j*1.4f,2.83f,5.72f),new Vector3(.44f,.30f,.045f),j%2==0?accent:"#FFF4D8",.02f);
            }
            else if(district==1){
                for(int i=0;i<3;i++){
                    float x=-3.2f+i*3.2f;Models.Box(root,new Vector3(x,2.15f,5.98f),new Vector3(2.15f,1.2f,.12f),"#28575D");
                    for(int j=0;j<3;j++)Models.Box(root,new Vector3(x,2.38f-j*.22f,5.88f),new Vector3(1.48f-j*.16f,.05f,.03f),"#FFF1D5",.01f);
                    Models.Cylinder(root,new Vector3(x,3.12f,4.9f),.32f,.13f,accent);Models.Cylinder(root,new Vector3(x,3.34f,4.9f),.035f,.4f,ink);
                }
            }
            else if(district==2){
                for(int i=0;i<4;i++){
                    float x=-3.7f+i*2.45f;Models.Ball(root,new Vector3(x,2.8f,5.3f),new Vector3(.54f,.74f,.54f),accent);
                    Models.Cylinder(root,new Vector3(x,3.2f,5.3f),.055f,.20f,"#66463E");
                    for(int j=0;j<3;j++)Models.Cylinder(root,new Vector3(x,2.57f+j*.20f,5.3f),.28f,.025f,"#A56D3F");
                }
                Models.Box(root,new Vector3(0,1.55f,5.97f),new Vector3(8.8f,.22f,.24f),"#9C7650");
            }
            else if(district==3){
                for(int x=0;x<12;x++)for(int y=0;y<2;y++)Models.Box(root,new Vector3(-4.4f+x*.8f,1.42f+y*.48f,6.0f),new Vector3(.70f,.39f,.045f),(x+y)%2==0?"#397AA0":"#F9F2DE",.03f);
                for(int side=-1;side<=1;side+=2){
                    Models.Cylinder(root,new Vector3(side*4.6f,.36f,5.65f),.36f,.72f,"#B7855D");Models.Cylinder(root,new Vector3(side*4.6f,1.3f,5.65f),.075f,1.6f,"#816345");
                    for(int n=0;n<3;n++)Models.Ball(root,new Vector3(side*4.6f+(n-1)*.3f,2.06f+(.16f*(n%2)),5.65f),new Vector3(.8f,.9f,.7f),"#829A61");
                }
            }
            else{
                Models.Box(root,new Vector3(0,2.35f,6.0f),new Vector3(8.9f,1.45f,.08f),"#F5E0EC");
                for(int i=0;i<12;i++)Models.Box(root,new Vector3(-4.2f+i*.76f,2.35f,5.93f),new Vector3(.21f,1.4f,.04f),"#D7A4BD",.01f);
                for(int i=0;i<3;i++){
                    float x=-3.1f+i*3.1f;Models.Cylinder(root,new Vector3(x,1.6f,5.6f),.17f,.38f,"#765369");Models.Cylinder(root,new Vector3(x,1.82f,5.6f),.48f,.08f,"#FFF0D9");
                    Food(root,new Vector3(x,1.89f,5.6f),4,variation+i);
                }
            }
        }
    }
}
