using UnityEngine;
namespace IdleRestaurant
{
    public enum RestaurantSymbolKind { Map, Team, Restaurant, Research, Goals, Orders, Kitchen, Dining, Ticket, Star, Lock }
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class RestaurantSymbol : UnityEngine.UI.MaskableGraphic
    {
        public RestaurantSymbolKind kind;
        UnityEngine.UI.VertexHelper mesh;
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();mesh=vh;Color c=color;
            switch(kind)
            {
                case RestaurantSymbolKind.Map:
                    Ring(.50f,.60f,.29f,.14f,c);Tri(new Vector2(.27f,.45f),new Vector2(.73f,.45f),new Vector2(.5f,.05f),c);break;
                case RestaurantSymbolKind.Team:
                    Disc(.5f,.72f,.14f,c);Disc(.19f,.65f,.105f,c);Disc(.81f,.65f,.105f,c);Quad(.30f,.14f,.70f,.49f,c);Quad(.04f,.20f,.24f,.46f,c);Quad(.76f,.20f,.96f,.46f,c);break;
                case RestaurantSymbolKind.Restaurant:
                    Quad(.16f,.15f,.39f,.65f,c);Quad(.59f,.15f,.84f,.65f,c);Quad(.39f,.49f,.59f,.65f,c);Tri(new Vector2(.08f,.67f),new Vector2(.92f,.67f),new Vector2(.5f,.94f),c);break;
                case RestaurantSymbolKind.Research:
                    Line(.38f,.90f,.62f,.90f,.08f,c);Line(.42f,.88f,.42f,.62f,.07f,c);Line(.58f,.88f,.58f,.62f,.07f,c);Line(.42f,.62f,.18f,.15f,.07f,c);Line(.58f,.62f,.82f,.15f,.07f,c);Line(.18f,.15f,.82f,.15f,.07f,c);Quad(.31f,.22f,.69f,.32f,c);break;
                case RestaurantSymbolKind.Goals:
                    Line(.21f,.15f,.21f,.86f,.07f,c);Line(.21f,.86f,.81f,.86f,.07f,c);Line(.81f,.86f,.81f,.15f,.07f,c);Line(.21f,.15f,.81f,.15f,.07f,c);Line(.33f,.51f,.46f,.37f,.08f,c);Line(.46f,.37f,.69f,.67f,.08f,c);break;
                case RestaurantSymbolKind.Orders:
                    Quad(.17f,.16f,.83f,.38f,c);Quad(.28f,.42f,.71f,.82f,c);Quad(.35f,.49f,.64f,.74f,new Color(1,.97f,.9f,c.a));break;
                case RestaurantSymbolKind.Kitchen:
                    Disc(.28f,.63f,.19f,c);Disc(.5f,.77f,.22f,c);Disc(.73f,.63f,.19f,c);Quad(.27f,.20f,.74f,.63f,c);Line(.29f,.18f,.71f,.18f,.06f,c);break;
                case RestaurantSymbolKind.Dining:
                    Ring(.5f,.5f,.31f,.23f,c);Line(.07f,.15f,.07f,.83f,.04f,c);Line(.94f,.15f,.94f,.83f,.05f,c);break;
                case RestaurantSymbolKind.Ticket:
                    Quad(.12f,.24f,.88f,.76f,c);for(int i=0;i<4;i++)Quad(.62f,.28f+i*.115f,.65f,.34f+i*.115f,new Color(1,.97f,.9f,c.a));break;
                case RestaurantSymbolKind.Star:
                    for(int i=0;i<10;i++){float a=i*Mathf.PI/5+Mathf.PI*.5f,b=(i+1)*Mathf.PI/5+Mathf.PI*.5f;float r=i%2==0?.44f:.20f,s=i%2==0?.20f:.44f;Tri(new Vector2(.5f,.5f),new Vector2(.5f+Mathf.Cos(a)*r,.5f+Mathf.Sin(a)*r),new Vector2(.5f+Mathf.Cos(b)*s,.5f+Mathf.Sin(b)*s),c);}break;
                default:
                    Quad(.22f,.15f,.78f,.57f,c);Line(.32f,.55f,.32f,.76f,.08f,c);Line(.68f,.55f,.68f,.76f,.08f,c);Line(.32f,.76f,.68f,.76f,.08f,c);break;
            }
        }
        Vector3 P(Vector2 v){var r=rectTransform.rect;float side=Mathf.Min(r.width,r.height);return new Vector3(r.center.x+(v.x-.5f)*side,r.center.y+(v.y-.5f)*side);}
        void Tri(Vector2 a,Vector2 b,Vector2 c,Color col){int n=mesh.currentVertCount;mesh.AddVert(P(a),col,Vector2.zero);mesh.AddVert(P(b),col,Vector2.zero);mesh.AddVert(P(c),col,Vector2.zero);mesh.AddTriangle(n,n+1,n+2);}
        void Quad(float x,float y,float xx,float yy,Color col){Tri(new Vector2(x,y),new Vector2(x,yy),new Vector2(xx,yy),col);Tri(new Vector2(x,y),new Vector2(xx,yy),new Vector2(xx,y),col);}
        void Disc(float x,float y,float radius,Color col){for(int i=0;i<24;i++){float a=i*Mathf.PI/12,b=(i+1)*Mathf.PI/12;Tri(new Vector2(x,y),new Vector2(x+Mathf.Cos(a)*radius,y+Mathf.Sin(a)*radius),new Vector2(x+Mathf.Cos(b)*radius,y+Mathf.Sin(b)*radius),col);}}
        void Ring(float x,float y,float outer,float inner,Color col){for(int i=0;i<24;i++){float a=i*Mathf.PI/12,b=(i+1)*Mathf.PI/12;var p=new Vector2(x+Mathf.Cos(a)*outer,y+Mathf.Sin(a)*outer);var q=new Vector2(x+Mathf.Cos(b)*outer,y+Mathf.Sin(b)*outer);var r=new Vector2(x+Mathf.Cos(a)*inner,y+Mathf.Sin(a)*inner);var t=new Vector2(x+Mathf.Cos(b)*inner,y+Mathf.Sin(b)*inner);Tri(p,q,t,col);Tri(p,t,r,col);}}
        void Line(float x,float y,float xx,float yy,float w,Color col){var a=new Vector2(x,y);var b=new Vector2(xx,yy);var p=new Vector2(-(b-a).y,(b-a).x).normalized*w*.5f;Tri(a-p,a+p,b+p,col);Tri(a-p,b+p,b-p,col);}
    }
}
