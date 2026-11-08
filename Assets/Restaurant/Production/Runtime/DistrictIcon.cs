using UnityEngine;
namespace IdleRestaurant
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class DistrictIcon:UnityEngine.UI.MaskableGraphic
    {
        public int district,variant=-1;
        UnityEngine.UI.VertexHelper mesh;
        static Color C(string hex){ColorUtility.TryParseHtmlString(hex,out var c);return c;}
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();mesh=vh;
            if(variant<0){Chapter();return;}
            int type=district*8+Mathf.Clamp(variant,0,7);
            // Each outlet has a menu-specific silhouette; all coordinates share a square design space.
            switch(type){
                case 0:Burger();break;case 1:Toast();break;case 2:Salad();break;case 3:Taco();break;case 4:Noodles();break;case 5:Pizza();break;case 6:Rice();break;case 7:Market();break;
                case 8:Pancakes();break;case 9:Toast();Cup(.69f,.40f,.16f);break;case 10:Pan();break;case 11:Plate();Egg(.5f,.5f);break;case 12:Roast();break;case 13:Bowl("#C67846");Steam();break;case 14:Egg(.38f,.48f);Toast(.58f,.50f,.30f);break;case 15:Cloche();break;
                case 16:Noodles();Chopsticks();break;case 17:Plate();for(int i=0;i<3;i++)Dumpling(.28f+i*.23f,.48f);break;case 18:Pan();Quad(.28f,.44f,.67f,.55f,"#75A14D");break;case 19:Sushi();break;case 20:Bowl("#D99837");Disc(.35f,.57f,.09f,"#78A554");break;case 21:Bowl("#B75F45");Chopsticks();break;case 22:Rice();Chopsticks();break;case 23:Plate();Dumpling(.30f,.56f);SushiRoll(.63f,.52f);break;
                case 24:Salad();break;case 25:Plate();Disc(.5f,.52f,.25f,"#ECC64C");Disc(.5f,.52f,.19f,"#FFF0A0");Line(.5f,.33f,.5f,.71f,.025f,"#F8D96B");break;case 26:Plate();for(int i=0;i<3;i++){Disc(.32f+i*.2f,.5f,.13f,"#C65C45");Disc(.34f+i*.2f,.54f,.08f,"#F8EBD8");}break;case 27:Bowl("#E2AD48");Steam();break;case 28:Fish();break;case 29:Salad();Olive();break;case 30:Pan();Quad(.27f,.42f,.69f,.56f,"#A56B46");for(int i=0;i<4;i++)Line(.3f+i*.1f,.42f,.36f+i*.1f,.56f,.02f,"#633E2D");break;case 31:Plate();Disc(.35f,.56f,.12f,"#C5B27A");Disc(.62f,.55f,.12f,"#8AA65B");Disc(.50f,.33f,.09f,"#C98357");break;
                case 32:Cookies();break;case 33:Cake();break;case 34:Cupcake();break;case 35:Cup(.5f,.46f,.3f);Steam();break;case 36:Gelato();break;case 37:Plate();Disc(.33f,.5f,.17f,"#D7A4BC");Disc(.68f,.5f,.17f,"#A8BC8F");break;case 38:Pastry();break;default:Cake();Quad(.32f,.54f,.68f,.78f,"#F2D1A5");Quad(.30f,.76f,.70f,.83f,"#FFF6E8");Disc(.5f,.87f,.055f,"#C36683");break;
            }
        }
        void Chapter(){if(district==0)Market();else if(district==1){Plate();Line(.12f,.18f,.12f,.82f,.045f,"#537E78");Line(.88f,.18f,.88f,.82f,.045f,"#537E78");}else if(district==2){Noodles();Chopsticks();}else if(district==3)Olive();else Cupcake();}
        void Plate(){Disc(.5f,.42f,.37f,"#C9C2AB");Disc(.5f,.46f,.36f,"#FFF7E6");Disc(.5f,.46f,.28f,"#EBE0C8");}
        void Burger(){Disc(.5f,.64f,.29f,"#EAB354");Quad(.20f,.56f,.80f,.66f,"#EAB354");for(int i=0;i<4;i++)Disc(.32f+i*.12f,.72f+(i%2)*.05f,.018f,"#FFF0BB");Quad(.18f,.47f,.82f,.55f,"#78A34E");Quad(.22f,.39f,.78f,.47f,"#A45037");Quad(.21f,.30f,.79f,.39f,"#66402B");Quad(.20f,.20f,.80f,.30f,"#DB9B47");}
        void Toast(){Toast(.50f,.50f,.60f);}void Toast(float x,float y,float size){Quad(x-size*.42f,y-size*.47f,x+size*.42f,y+size*.30f,"#AF7541");Disc(x-size*.20f,y+size*.24f,size*.24f,"#AF7541");Disc(x+size*.20f,y+size*.24f,size*.24f,"#AF7541");Quad(x-size*.34f,y-size*.38f,x+size*.34f,y+size*.31f,"#F0CD8F");Quad(x-.10f,y-.03f,x+.10f,y+.10f,"#F7E5A5");}
        void Bowl(string fill){Disc(.5f,.40f,.30f,"#B5664B");Quad(.18f,.40f,.82f,.55f,"#EBDFBC");Disc(.5f,.55f,.29f,fill);Quad(.20f,.45f,.80f,.52f,"#F6E9CD");}
        void Noodles(){Bowl("#D8AB5D");for(int i=0;i<4;i++)Line(.30f,.54f+i*.025f,.67f,.57f+i*.025f,.028f,"#F8D58A");Disc(.68f,.60f,.08f,"#79A258");Disc(.36f,.64f,.06f,"#F5E6BC");}
        void Chopsticks(){Line(.28f,.73f,.79f,.91f,.032f,"#674B37");Line(.31f,.66f,.83f,.84f,.032f,"#8B6545");}
        void Salad(){Bowl("#89AA5D");for(int i=0;i<5;i++)Disc(.30f+i*.10f,.59f+(i%2)*.09f,.10f,i%2==0?"#77A355":"#A6BB70");Disc(.38f,.66f,.055f,"#C76B4D");Disc(.64f,.57f,.055f,"#C76B4D");}
        void Taco(){Disc(.5f,.52f,.32f,"#E2B65C");Quad(.19f,.48f,.81f,.66f,"#E2B65C");for(int i=0;i<5;i++)Disc(.25f+i*.12f,.64f,.075f,i%2==0?"#79A056":"#B76647");Quad(.20f,.27f,.80f,.50f,"#F0C675");}
        void Pizza(){Tri(new Vector2(.16f,.78f),new Vector2(.85f,.78f),new Vector2(.50f,.14f),"#D89D50");Tri(new Vector2(.23f,.69f),new Vector2(.78f,.69f),new Vector2(.50f,.23f),"#F4D27E");Disc(.40f,.59f,.065f,"#B65B43");Disc(.62f,.58f,.065f,"#B65B43");Disc(.50f,.39f,.05f,"#B65B43");}
        void Rice(){Bowl("#F6EDD4");for(int i=0;i<4;i++)Disc(.31f+i*.12f,.59f,.065f,"#FFF5DD");Quad(.44f,.53f,.58f,.71f,"#567552");}
        void Market(){Quad(.18f,.18f,.82f,.51f,"#BF8C54");Quad(.12f,.49f,.88f,.57f,"#F2D49F");Line(.20f,.50f,.20f,.85f,.04f,"#526A4D");Line(.80f,.50f,.80f,.85f,.04f,"#526A4D");for(int i=0;i<5;i++)Quad(.12f+i*.153f,.72f,.273f+i*.153f,.87f,i%2==0?"#DDA747":"#FFF0CC");Quad(.34f,.29f,.66f,.42f,"#6B8858");}
        void Pancakes(){Plate();for(int i=0;i<3;i++){Quad(.22f,.32f+i*.13f,.78f,.42f+i*.13f,"#CC9652");Quad(.25f,.39f+i*.13f,.75f,.44f+i*.13f,"#EBC68B");}Quad(.43f,.68f,.59f,.77f,"#F4DE8E");}
        void Cup(float x,float y,float r){Disc(x+r*.80f,y+.02f,r*.44f,"#9D7958");Disc(x+r*.80f,y+.02f,r*.25f,"#FFF3D7");Quad(x-r*.75f,y-r*.6f,x+r*.65f,y+r*.65f,"#EEE2C9");Quad(x-r*.65f,y+r*.42f,x+r*.55f,y+r*.62f,"#6E4A37");}
        void Egg(float x,float y){Disc(x,y,.20f,"#FFF7E2");Disc(x+.03f,y+.03f,.09f,"#E7B649");}
        void Pan(){Disc(.46f,.48f,.30f,"#49615D");Disc(.46f,.50f,.24f,"#6C8076");Line(.68f,.30f,.90f,.11f,.10f,"#704F39");Egg(.45f,.52f);}
        void Roast(){Plate();Disc(.5f,.52f,.24f,"#AE7548");Disc(.30f,.40f,.12f,"#C18B55");Disc(.72f,.40f,.12f,"#C18B55");Line(.28f,.35f,.22f,.26f,.04f,"#F5E4BD");Line(.73f,.35f,.80f,.27f,.04f,"#F5E4BD");}
        void Cloche(){Plate();Disc(.5f,.45f,.31f,"#91ACA2");Quad(.18f,.30f,.82f,.47f,"#91ACA2");Disc(.5f,.78f,.05f,"#526E65");Quad(.14f,.27f,.86f,.34f,"#52786C");}
        void Steam(){for(int i=0;i<3;i++)Line(.35f+i*.14f,.76f,.32f+i*.14f,.91f,.027f,"#B3B4A0");}
        void Dumpling(float x,float y){Disc(x,y,.14f,"#E6D2A9");Quad(x-.14f,y-.08f,x+.14f,y,"#E6D2A9");for(int i=0;i<3;i++)Line(x-.06f+i*.06f,y+.02f,x-.04f+i*.04f,y+.12f,.014f,"#BD9A6F");}
        void SushiRoll(float x,float y){Disc(x,y,.14f,"#425C4A");Disc(x,y,.105f,"#FFF3D9");Disc(x,y,.05f,"#D18D66");}
        void Sushi(){Plate();SushiRoll(.29f,.5f);SushiRoll(.53f,.55f);SushiRoll(.72f,.38f);}
        void Fish(){Plate();Disc(.47f,.49f,.23f,"#7C9B9E");Tri(new Vector2(.64f,.49f),new Vector2(.88f,.68f),new Vector2(.88f,.30f),"#7C9B9E");Disc(.36f,.54f,.025f,"#2F494B");Line(.40f,.42f,.56f,.55f,.02f,"#DFE5D3");}
        void Olive(){Line(.25f,.14f,.72f,.88f,.04f,"#6B7950");for(int i=0;i<4;i++){float y=.28f+i*.15f,x=.33f+i*.09f;Disc(x-.1f,y+.03f,.09f,"#789256");Disc(x+.1f,y+.06f,.09f,"#9CAC6B");}}
        void Cookies(){Plate();for(int i=0;i<3;i++){float x=.30f+i*.2f,y=.45f+(i%2)*.16f;Disc(x,y,.17f,"#D3A56B");Disc(x-.04f,y+.04f,.022f,"#70503A");Disc(x+.05f,y-.04f,.025f,"#70503A");}}
        void Cake(){Plate();Quad(.24f,.27f,.76f,.65f,"#E2BA87");Quad(.24f,.39f,.76f,.47f,"#B7778E");Quad(.20f,.61f,.80f,.72f,"#FFF1DD");for(int i=0;i<3;i++)Disc(.34f+i*.16f,.75f,.045f,"#BF6A83");}
        void Cupcake(){Quad(.28f,.20f,.72f,.46f,"#B87E98");Disc(.5f,.53f,.25f,"#F5DEC5");Disc(.43f,.71f,.16f,"#E9BACB");Disc(.54f,.83f,.065f,"#BD647F");}
        void Gelato(){Tri(new Vector2(.28f,.49f),new Vector2(.72f,.49f),new Vector2(.5f,.08f),"#D6AC75");Disc(.40f,.61f,.19f,"#B4C59E");Disc(.65f,.61f,.19f,"#D6A1BC");Disc(.51f,.80f,.16f,"#F6E3CA");}
        void Pastry(){Plate();for(int i=0;i<5;i++){float x=.26f+i*.12f;Disc(x,.48f+Mathf.Sin(i*Mathf.PI/4)*.09f,.12f,i%2==0?"#D3A367":"#E4BC80");}}
        Vector3 P(float x,float y){var r=rectTransform.rect;float side=Mathf.Min(r.width,r.height);return new Vector3(r.center.x+(x-.5f)*side,r.center.y+(y-.5f)*side);}
        void Tri(Vector2 a,Vector2 b,Vector2 c,string hex){int n=mesh.currentVertCount;Color col=C(hex);mesh.AddVert(P(a.x,a.y),col,Vector2.zero);mesh.AddVert(P(b.x,b.y),col,Vector2.zero);mesh.AddVert(P(c.x,c.y),col,Vector2.zero);mesh.AddTriangle(n,n+1,n+2);}
        void Quad(float x,float y,float xx,float yy,string c){Tri(new Vector2(x,y),new Vector2(x,yy),new Vector2(xx,yy),c);Tri(new Vector2(x,y),new Vector2(xx,yy),new Vector2(xx,y),c);}
        void Disc(float x,float y,float radius,string c){for(int i=0;i<32;i++){float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;Tri(new Vector2(x,y),new Vector2(x+Mathf.Cos(a)*radius,y+Mathf.Sin(a)*radius),new Vector2(x+Mathf.Cos(b)*radius,y+Mathf.Sin(b)*radius),c);}}
        void Line(float x,float y,float xx,float yy,float width,string c){var a=new Vector2(x,y);var b=new Vector2(xx,yy);var p=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;Tri(a-p,a+p,b+p,c);Tri(a-p,b+p,b-p,c);}
    }
}
