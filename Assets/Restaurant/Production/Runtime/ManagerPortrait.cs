using UnityEngine;
namespace IdleRestaurant
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ManagerPortrait:UnityEngine.UI.MaskableGraphic
    {
        public int person,role;
        UnityEngine.UI.VertexHelper mesh;
        static readonly string[] Skin={"#E6BA94","#BA8765","#85583E","#D2A27B","#F0CCAD","#A36E50"};
        static readonly string[] Hair={"#49372C","#79553C","#2E3334","#B68D56","#675E56","#C5C2B5"};
        static readonly string[] Shirt={"#427D73","#8D6359","#657B9E","#8D7950","#796586","#587B58"};
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            mesh=vh;mesh.Clear();string skin=Skin[(person*5+2)%Skin.Length],hair=Hair[(person*3+person/4)%Hair.Length],shirt=Shirt[person%Shirt.Length];
            Disc(.5f,.5f,.46f,"#DDDCC9");Disc(.5f,.23f,.32f,shirt);Quad(.23f,.08f,.77f,.29f,shirt);Quad(.43f,.28f,.57f,.44f,skin);
            Disc(.5f,.60f,.25f,hair);Disc(.28f,.54f,.055f,skin);Disc(.72f,.54f,.055f,skin);Disc(.5f,.55f,.23f,skin);
            int style=person%5;
            if(style==0){Disc(.43f,.76f,.18f,hair);Disc(.64f,.73f,.13f,hair);}
            else if(style==1){Quad(.27f,.69f,.73f,.80f,hair);Disc(.33f,.70f,.12f,hair);}
            else if(style==2){for(int i=0;i<5;i++)Disc(.29f+i*.105f,.75f,.085f,hair);}
            else if(style==3){Quad(.26f,.43f,.31f,.75f,hair);Quad(.69f,.43f,.74f,.75f,hair);Disc(.5f,.78f,.20f,hair);Quad(.34f,.67f,.66f,.77f,skin);}
            else{Quad(.29f,.68f,.71f,.78f,hair);}
            Disc(.41f,.57f,.021f,"#293D36");Disc(.59f,.57f,.021f,"#293D36");Quad(.40f,.61f,.45f,.625f,hair);Quad(.55f,.61f,.60f,.625f,hair);
            Quad(.46f,.43f,.54f,.447f,"#8F5A46");
            if(person%4==0){Ring(.40f,.56f,.067f,.052f,"#475650");Ring(.60f,.56f,.067f,.052f,"#475650");Quad(.46f,.55f,.54f,.565f,"#475650");}
            if(person%7==0){Quad(.40f,.48f,.60f,.50f,hair);}
            if(role==1){Quad(.30f,.74f,.70f,.82f,"#FFF5DF");Disc(.37f,.84f,.12f,"#FFF5DF");Disc(.51f,.88f,.13f,"#FFF5DF");Disc(.65f,.84f,.11f,"#FFF5DF");}
            else if(role==0){Quad(.30f,.20f,.43f,.29f,"#E7C45E");Quad(.32f,.22f,.41f,.27f,"#FFF4D8");}
            else{Quad(.38f,.10f,.62f,.31f,"#F5E8D0");Quad(.46f,.17f,.54f,.21f,shirt);}
        }
        Vector3 P(Vector2 v){var r=rectTransform.rect;float s=Mathf.Min(r.width,r.height);return new Vector3(r.center.x+(v.x-.5f)*s,r.center.y+(v.y-.5f)*s);}
        void Tri(Vector2 a,Vector2 b,Vector2 c,string hex){ColorUtility.TryParseHtmlString(hex,out var color);int n=mesh.currentVertCount;mesh.AddVert(P(a),color,Vector2.zero);mesh.AddVert(P(b),color,Vector2.zero);mesh.AddVert(P(c),color,Vector2.zero);mesh.AddTriangle(n,n+1,n+2);}
        void Quad(float x,float y,float xx,float yy,string c){Tri(new Vector2(x,y),new Vector2(x,yy),new Vector2(xx,yy),c);Tri(new Vector2(x,y),new Vector2(xx,yy),new Vector2(xx,y),c);}
        void Disc(float x,float y,float r,string c){for(int i=0;i<32;i++){float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;Tri(new Vector2(x,y),new Vector2(x+Mathf.Cos(a)*r,y+Mathf.Sin(a)*r),new Vector2(x+Mathf.Cos(b)*r,y+Mathf.Sin(b)*r),c);}}
        void Ring(float x,float y,float outer,float inner,string c){for(int i=0;i<32;i++){float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;var p=new Vector2(x+Mathf.Cos(a)*outer,y+Mathf.Sin(a)*outer);var q=new Vector2(x+Mathf.Cos(b)*outer,y+Mathf.Sin(b)*outer);var r=new Vector2(x+Mathf.Cos(a)*inner,y+Mathf.Sin(a)*inner);var t=new Vector2(x+Mathf.Cos(b)*inner,y+Mathf.Sin(b)*inner);Tri(p,q,t,c);Tri(p,t,r,c);}}
    }
}
