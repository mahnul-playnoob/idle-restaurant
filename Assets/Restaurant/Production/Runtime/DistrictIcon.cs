using UnityEngine;
using UnityEngine.UI;
namespace IdleRestaurant {
 [RequireComponent(typeof(CanvasRenderer))]
 public sealed class DistrictIcon:MaskableGraphic {
  public int district;VertexHelper helper;
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();helper=vh;
   if(district==0){Disc(.5f,.64f,.27f,new Color(1,.72f,.32f));Quad(.19f,.54f,.81f,.64f,new Color(1,.72f,.32f));Quad(.19f,.43f,.81f,.51f,new Color(.34f,.62f,.32f));Quad(.2f,.32f,.8f,.42f,new Color(.5f,.29f,.2f));Quad(.2f,.22f,.8f,.31f,new Color(1,.72f,.32f));}
   else if(district==1){Disc(.5f,.5f,.34f,new Color(.96f,.9f,.74f));Disc(.5f,.5f,.24f,new Color(.7f,.83f,.76f));Line(.1f,.18f,.1f,.8f,.04f,color);Line(.06f,.62f,.18f,.62f,.035f,color);Line(.87f,.18f,.87f,.8f,.045f,color);}
   else if(district==2){Disc(.5f,.4f,.3f,new Color(.84f,.39f,.3f));Quad(.12f,.44f,.88f,.57f,new Color(.98f,.87f,.62f));Quad(.15f,.61f,.85f,.72f,new Color(.47f,.67f,.4f));Line(.22f,.72f,.8f,.91f,.035f,color);Line(.26f,.65f,.84f,.84f,.035f,color);}
   else if(district==3){Line(.25f,.12f,.7f,.86f,.04f,color);for(int i=0;i<4;i++){float y=.25f+i*.15f,x=.32f+i*.09f;Disc(x-.10f,y+.07f,.09f,new Color(.4f,.61f,.37f));Disc(x+.12f,y+.07f,.09f,new Color(.55f,.7f,.4f));}}
   else{Quad(.28f,.2f,.72f,.48f,new Color(.81f,.51f,.62f));Disc(.5f,.58f,.24f,new Color(.99f,.87f,.74f));Disc(.44f,.77f,.14f,new Color(.98f,.73f,.78f));Disc(.53f,.9f,.06f,new Color(.8f,.29f,.38f));}
  }
  Vector3 P(float x,float y)=>new Vector3(rectTransform.rect.xMin+x*rectTransform.rect.width,rectTransform.rect.yMin+y*rectTransform.rect.height);
  void Quad(float x0,float y0,float x1,float y1,Color c){int n=helper.currentVertCount;helper.AddVert(P(x0,y0),c,Vector2.zero);helper.AddVert(P(x0,y1),c,Vector2.zero);helper.AddVert(P(x1,y1),c,Vector2.zero);helper.AddVert(P(x1,y0),c,Vector2.zero);helper.AddTriangle(n,n+1,n+2);helper.AddTriangle(n,n+2,n+3);}
  void Disc(float x,float y,float r,Color c){int n=helper.currentVertCount;helper.AddVert(P(x,y),c,Vector2.zero);for(int i=0;i<=24;i++){float a=i*Mathf.PI*2/24;helper.AddVert(P(x+r*Mathf.Cos(a),y+r*Mathf.Sin(a)),c,Vector2.zero);if(i>0)helper.AddTriangle(n,n+i,n+i+1);}}
  void Line(float x0,float y0,float x1,float y1,float width,Color c){Vector2 a=new Vector2(x0,y0),b=new Vector2(x1,y1),p=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;int n=helper.currentVertCount;foreach(var v in new[]{a-p,a+p,b+p,b-p})helper.AddVert(P(v.x,v.y),c,Vector2.zero);helper.AddTriangle(n,n+1,n+2);helper.AddTriangle(n,n+2,n+3);}
 }
}
