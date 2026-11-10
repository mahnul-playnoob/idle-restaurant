using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RestaurantPreview
{
    public static class Models
    {
        public static Color C(string hex){ColorUtility.TryParseHtmlString(hex,out var c);return c;}
        static Dictionary<string,Material> mats=new Dictionary<string,Material>();
        public static Material Mat(string hex)
        {
            if(mats.TryGetValue(hex,out var m) && m)return m;
            var shader=Shader.Find("Universal Render Pipeline/Lit");if(!shader)throw new Exception("URP Lit missing");
            m=new Material(shader){name="Restaurant "+hex,enableInstancing=true};m.SetColor("_BaseColor",C(hex));m.SetFloat("_Smoothness",.22f);mats[hex]=m;return m;
        }
        public static Transform Group(string name,Transform parent,Vector3 pos)
        {
            var t=new GameObject(name).transform;t.SetParent(parent,false);t.localPosition=pos;return t;
        }
        public static Transform Shape(PrimitiveType kind,Transform p,Vector3 pos,Vector3 size,string color)
        {
            var g=GameObject.CreatePrimitive(kind);g.name=kind.ToString();g.transform.SetParent(p,false);g.transform.localPosition=pos;g.transform.localScale=size;
            var col=g.GetComponent<Collider>();if(col){col.enabled=false;if(Application.isPlaying)UnityEngine.Object.Destroy(col);else UnityEngine.Object.DestroyImmediate(col);}
            g.GetComponent<Renderer>().sharedMaterial=Mat(color);return g.transform;
        }
        public static Transform Ball(Transform p,Vector3 pos,Vector3 size,string c)=>Shape(PrimitiveType.Sphere,p,pos,size,c);
        public static Transform Cylinder(Transform p,Vector3 pos,float radius,float height,string c)=>Shape(PrimitiveType.Cylinder,p,pos,new Vector3(radius*2,height*.5f,radius*2),c);
        public static Transform Box(Transform p,Vector3 pos,Vector3 size,string color,float radius=.06f)
        {
            var g=new GameObject("Rounded block",typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(p,false);g.transform.localPosition=pos;
            var generated=RoundedMesh(size.x,size.y,size.z,Mathf.Min(radius,Mathf.Min(size.x,size.z)*.48f));g.GetComponent<MeshFilter>().sharedMesh=generated;if(Application.isPlaying)g.AddComponent<IdleRestaurant.RestaurantRuntimeMeshOwner>().mesh=generated;g.GetComponent<MeshRenderer>().sharedMaterial=Mat(color);return g.transform;
        }
        static Mesh RoundedMesh(float w,float h,float d,float r)
        {
            float bevel=Mathf.Min(.04f,Mathf.Min(h*.23f,r*.5f));int n=32;var vs=new List<Vector3>();var ts=new List<int>();
            for(int ring=0;ring<4;ring++)
            {
                float inset=(ring==0 || ring==3)?bevel:0;float y=ring==0?-h/2:ring==1?-h/2+bevel:ring==2?h/2-bevel:h/2;
                float rr=Mathf.Max(.001f,r-inset),ww=w-2*inset,dd=d-2*inset;
                for(int corner=0;corner<4;corner++)for(int j=0;j<8;j++)
                {
                    float angle=(corner*90+j*90f/7)*Mathf.Deg2Rad;
                    float cx=(corner==0 || corner==3?1:-1)*(ww/2-rr),cz=(corner<2?1:-1)*(dd/2-rr);
                    vs.Add(new Vector3(cx+Mathf.Cos(angle)*rr,y,cz+Mathf.Sin(angle)*rr));
                }
            }
            for(int ring=0;ring<3;ring++)for(int i=0;i<n;i++)
            {int next=(i+1)%n,a=ring*n+i,b=(ring+1)*n+i,c=(ring+1)*n+next,e=ring*n+next;ts.AddRange(new[]{a,b,c,a,c,e});}
            int bottom=vs.Count;vs.Add(new Vector3(0,-h/2,0));int top=vs.Count;vs.Add(new Vector3(0,h/2,0));
            for(int i=0;i<n;i++){int next=(i+1)%n;ts.AddRange(new[]{bottom,i,next,top,3*n+next,3*n+i});}
            var mesh=new Mesh{name="Soft bevel block"};mesh.SetVertices(vs);mesh.SetTriangles(ts,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        public static Transform Tube(Transform p,IList<Vector3> points,float radius,string color,int sides=6)
        {
            var g=new GameObject("Curved detail",typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(p,false);
            var v=new List<Vector3>();var t=new List<int>();
            for(int i=0;i<points.Count;i++)
            {
                Vector3 tangent=(points[Mathf.Min(i+1,points.Count-1)]-points[Mathf.Max(0,i-1)]).normalized;
                Vector3 right=Vector3.Cross(tangent,Mathf.Abs(tangent.y)>.9f?Vector3.right:Vector3.up).normalized;Vector3 up=Vector3.Cross(right,tangent).normalized;
                for(int j=0;j<sides;j++){float a=j*Mathf.PI*2/sides;v.Add(points[i]+radius*(Mathf.Cos(a)*right+Mathf.Sin(a)*up));}
            }
            for(int i=0;i<points.Count-1;i++)for(int j=0;j<sides;j++){int a=i*sides+j,b=i*sides+(j+1)%sides,c=(i+1)*sides+j,d=(i+1)*sides+(j+1)%sides;t.AddRange(new[]{a,c,b,b,c,d});}
            var mesh=new Mesh{name="Food and face curve"};mesh.SetVertices(v);mesh.SetTriangles(t,0);mesh.RecalculateNormals();g.GetComponent<MeshFilter>().sharedMesh=mesh;if(Application.isPlaying)g.AddComponent<IdleRestaurant.RestaurantRuntimeMeshOwner>().mesh=mesh;g.GetComponent<MeshRenderer>().sharedMaterial=Mat(color);return g.transform;
        }
        public static Transform Plate(Transform p,Vector3 pos)
        {
            var root=Group("Porcelain plate",p,pos);Cylinder(root,Vector3.zero,.35f,.035f,"#F8F3E7");Cylinder(root,new Vector3(0,.023f,0),.30f,.022f,"#FFFFFF");return root;
        }
        public static Transform Burger(Transform p,Vector3 pos)
        {
            var root=Group("Sesame burger",p,pos);Plate(root,Vector3.zero);
            Ball(root,new Vector3(0,.10f,0),new Vector3(.48f,.15f,.46f),"#DDA45F");
            Cylinder(root,new Vector3(0,.19f,0),.22f,.08f,"#754432");
            var cheese=Box(root,new Vector3(0,.239f,0),new Vector3(.47f,.025f,.42f),"#FFCB55",.025f);cheese.localRotation=Quaternion.Euler(0,25,0);
            Cylinder(root,new Vector3(0,.273f,0),.205f,.035f,"#D85D49");
            for(int i=0;i<9;i++){float a=i*2*Mathf.PI/9;var l=Ball(root,new Vector3(Mathf.Cos(a)*.19f,.30f,Mathf.Sin(a)*.18f),new Vector3(.19f,.05f,.14f),i%2==0?"#86B95A":"#A8CC6C");l.localRotation=Quaternion.Euler(0,i*40,12);}
            Ball(root,new Vector3(0,.366f,0),new Vector3(.49f,.22f,.46f),"#EAB66E");
            for(int i=0;i<13;i++){float a=i*2.4f,r=.15f*Mathf.Sqrt((i+.5f)/13);var seed=Ball(root,new Vector3(Mathf.Cos(a)*r,.454f+(.15f-r)*.1f,Mathf.Sin(a)*r),new Vector3(.018f,.009f,.041f),"#FFF1C6");seed.localRotation=Quaternion.Euler(0,i*29,0);}
            return root;
        }
        public static Transform Pasta(Transform p,Vector3 pos)
        {
            var root=Group("Tomato basil pasta",p,pos);Plate(root,Vector3.zero);Ball(root,new Vector3(0,.075f,0),new Vector3(.53f,.11f,.50f),"#C96E47");
            for(int k=0;k<22;k++)
            {
                var points=new List<Vector3>();float phase=k*1.73f;
                for(int i=0;i<13;i++){float a=i*.48f+phase,r=.08f+.13f*(.5f+.5f*Mathf.Sin(k*2.1f+i*.3f));points.Add(new Vector3(Mathf.Cos(a)*r,.105f+k*.002f+Mathf.Sin(i*.9f)*.018f,Mathf.Sin(a)*r));}
                Tube(root,points,.014f,k%3==0?"#F0B655":"#F7CE77");
            }
            for(int i=0;i<5;i++){float a=i*2.3f;Ball(root,new Vector3(Mathf.Cos(a)*.14f,.16f,Mathf.Sin(a)*.14f),new Vector3(.09f,.075f,.09f),"#B9563E");}
            for(int i=0;i<3;i++){var leaf=Ball(root,new Vector3((i-1)*.05f,.21f,0),new Vector3(.09f,.025f,.16f),"#608D49");leaf.localRotation=Quaternion.Euler(8,i*48-30,0);}
            return root;
        }
        public static Transform Salad(Transform p,Vector3 pos)
        {
            var root=Group("Garden salad",p,pos);Plate(root,Vector3.zero);
            for(int i=0;i<13;i++){float a=i*2.4f,r=.21f*Mathf.Sqrt((i+.5f)/13);var l=Ball(root,new Vector3(Mathf.Cos(a)*r,.09f+(.22f-r)*.25f,Mathf.Sin(a)*r),new Vector3(.19f,.055f,.16f),i%2==0?"#81B75A":"#AACB70");l.localRotation=Quaternion.Euler(0,i*41,15);}
            for(int i=0;i<6;i++){float a=i*1.1f;Ball(root,new Vector3(Mathf.Cos(a)*.16f,.17f,Mathf.Sin(a)*.16f),new Vector3(.09f,.08f,.09f),"#DB6955");}
            for(int i=0;i<5;i++)Box(root,new Vector3(Mathf.Sin(i*2)*.13f,.18f,Mathf.Cos(i*2)*.13f),new Vector3(.065f,.055f,.065f),"#F9E8B8",.01f);
            return root;
        }
        public static Transform Plant(Transform p,Vector3 pos,float size=1)
        {
            var root=Group("Potted plant",p,pos);root.localScale=Vector3.one*size;Cylinder(root,new Vector3(0,.18f,0),.22f,.36f,"#C58A70");Cylinder(root,new Vector3(0,.37f,0),.235f,.06f,"#DFA88B");
            for(int i=0;i<7;i++){float a=i*2.4f;var leaf=Ball(root,new Vector3(Mathf.Cos(a)*.17f,.65f+Mathf.Sin(i)*.1f,Mathf.Sin(a)*.17f),new Vector3(.17f,.58f,.20f),i%2==0?"#70986B":"#8BA76E");leaf.localRotation=Quaternion.Euler(Mathf.Cos(a)*30,a*Mathf.Rad2Deg,Mathf.Sin(a)*30);}
            return root;
        }
    }
    public sealed class Person
    {
        public Transform root,body,head,leftArm,rightArm,leftLeg,rightLeg,leftKnee,rightKnee,tray;
        public Vector3 last;
        public Person(Transform parent,Vector3 position,GameObject prefab)
        {
            var instance=UnityEngine.Object.Instantiate(prefab,parent);root=instance.transform;root.localPosition=position;last=position;
            var rig=instance.GetComponent<IdleRestaurant.RestaurantActorRig>();if(!rig||!rig.Valid)throw new Exception("Replacement character needs a complete RestaurantActorRig");
            body=rig.body;head=rig.head;leftArm=rig.leftArm;rightArm=rig.rightArm;leftLeg=rig.leftLeg;rightLeg=rig.rightLeg;tray=rig.tray;leftKnee=rig.leftKnee?rig.leftKnee:Articulate(leftLeg);rightKnee=rig.rightKnee?rig.rightKnee:Articulate(rightLeg);rig.leftKnee=leftKnee;rig.rightKnee=rightKnee;
        }
        public Person(Transform parent,Vector3 position,string shirt,string hair,bool chef=false,bool server=false,int style=-1)
        {
            root=Models.Group(chef?"Chef":server?"Server":"Guest",parent,position);last=position;
            body=Models.Group("Body bounce",root,Vector3.zero);
            Models.Shape(PrimitiveType.Capsule,body,new Vector3(0,.69f,0),new Vector3(.51f,.36f,.36f),shirt);
            Models.Box(body,new Vector3(0,.47f,0),new Vector3(.38f,.23f,.30f),"#54646A",.07f);
            string skin=style<0?"#E8BC96":IdleRestaurant.RestaurantCharacterArt.Skin(style);head=Models.Group("Head",body,new Vector3(0,1.12f,0));Models.Ball(head,Vector3.zero,new Vector3(.49f,.54f,.46f),skin);
            Models.Ball(head,new Vector3(-.09f,.03f,-.217f),new Vector3(.042f,.065f,.025f),"#3E3C36");Models.Ball(head,new Vector3(.09f,.03f,-.217f),new Vector3(.042f,.065f,.025f),"#3E3C36");
            Models.Ball(head,new Vector3(0,-.04f,-.246f),new Vector3(.065f,.06f,.05f),skin);
            Models.Tube(head,new[]{new Vector3(-.055f,-.095f,-.218f),new Vector3(0,-.12f,-.229f),new Vector3(.055f,-.095f,-.218f)},.012f,"#81594A");
            if(style>=0)IdleRestaurant.RestaurantCharacterArt.Hair(head,style,hair,chef);
            if(chef)
            {
                Models.Cylinder(head,new Vector3(0,.25f,0),.25f,.14f,"#FFFCF0");
                for(int i=0;i<4;i++)Models.Ball(head,new Vector3(Mathf.Sin(i*1.57f)*.13f,.36f,Mathf.Cos(i*1.57f)*.1f),new Vector3(.29f,.29f,.28f),"#FFFCF0");
            }
            else if(style<0){Models.Ball(head,new Vector3(0,.19f,.025f),new Vector3(.51f,.20f,.47f),hair);Models.Ball(head,new Vector3(.18f,.10f,.015f),new Vector3(.12f,.22f,.36f),hair);}
            if(chef || server){Models.Box(body,new Vector3(0,.65f,-.19f),new Vector3(.35f,.43f,.045f),"#FFF5DC",.02f);Models.Box(body,new Vector3(0,.55f,-.219f),new Vector3(.20f,.13f,.02f),shirt,.01f);}
            leftArm=Arm(body,-1,shirt,skin);rightArm=Arm(body,1,shirt,skin);leftLeg=Leg(body,-1);rightLeg=Leg(body,1);leftKnee=leftLeg.Find("Knee");rightKnee=rightLeg.Find("Knee");
            if(server){tray=Models.Group("Serving tray",body,new Vector3(0,.93f,-.48f));Models.Cylinder(tray,Vector3.zero,.36f,.035f,"#D8B586");}
        }
        Transform Arm(Transform parent,int side,string shirt,string skin)
        {
            var t=Models.Group("Arm",parent,new Vector3(side*.29f,.88f,0));Models.Shape(PrimitiveType.Capsule,t,new Vector3(0,-.17f,0),new Vector3(.15f,.20f,.15f),shirt);Models.Ball(t,new Vector3(0,-.36f,0),new Vector3(.15f,.17f,.15f),skin);return t;
        }
        Transform Leg(Transform parent,int side)
        {
            var t=Models.Group("Leg",parent,new Vector3(side*.13f,.39f,0));
            Models.Shape(PrimitiveType.Capsule,t,new Vector3(0,-.10f,0),new Vector3(.15f,.11f,.16f),"#54646A");
            var knee=Models.Group("Knee",t,new Vector3(0,-.20f,0));Models.Shape(PrimitiveType.Capsule,knee,new Vector3(0,-.04f,0),new Vector3(.145f,.075f,.15f),"#54646A");Models.Box(knee,new Vector3(0,-.10f,-.06f),new Vector3(.19f,.11f,.30f),"#5C514B",.055f);return t;
        }
        static Transform Articulate(Transform leg)
        {
            // Upgrade the shipped rigid-leg prefabs; artist replacement rigs can supply explicit knees.
            var upper=leg.Find("Capsule");var shoe=leg.Find("Rounded block");if(!upper||!shoe)return null;
            upper.localPosition=new Vector3(0,-.10f,0);upper.localScale=new Vector3(.15f,.11f,.16f);
            var knee=Models.Group("Knee",leg,new Vector3(0,-.20f,0));var shin=Models.Shape(PrimitiveType.Capsule,knee,new Vector3(0,-.04f,0),new Vector3(.145f,.075f,.15f),"#54646A");var col=shin.GetComponent<Collider>();if(col)col.enabled=false;
            shoe.SetParent(knee,false);shoe.localPosition=new Vector3(0,-.10f,-.06f);return knee;
        }
        public void SeatLegs(float amount)
        {
            amount=Mathf.Clamp01(amount);leftLeg.localRotation=rightLeg.localRotation=Quaternion.Euler(90*amount,0,0);
            if(leftKnee)leftKnee.localRotation=Quaternion.Euler(-90*amount,0,0);if(rightKnee)rightKnee.localRotation=Quaternion.Euler(-90*amount,0,0);
        }
        public void Pose(float time,Vector3 pos,bool carry=false,bool cook=false,bool eat=false,float cheer=0,float yaw=float.NaN)
        {
            Vector3 delta=pos-last;float moving=Mathf.Clamp01(delta.magnitude*30*1.5f);root.localPosition=pos;
            if(!float.IsNaN(yaw))root.localRotation=Quaternion.Euler(0,yaw,0);
            else if(delta.sqrMagnitude>.00001f)root.localRotation=Quaternion.Slerp(root.localRotation,Quaternion.LookRotation(-delta),.22f);
            float stride=Mathf.Sin(time*11)*moving;
            body.localPosition=new Vector3(0,Mathf.Abs(stride)*.045f,0);
            leftLeg.localRotation=Quaternion.Euler(stride*31,0,0);rightLeg.localRotation=Quaternion.Euler(-stride*31,0,0);if(leftKnee)leftKnee.localRotation=Quaternion.Euler(-Mathf.Max(0,-stride)*18,0,0);if(rightKnee)rightKnee.localRotation=Quaternion.Euler(-Mathf.Max(0,stride)*18,0,0);if(eat)SeatLegs(1);
            float work=cook?110+Mathf.Sin(time*13)*15:carry?100:stride*-27;
            leftArm.localRotation=Quaternion.Euler(work,0,cheer*135);
            rightArm.localRotation=Quaternion.Euler(eat?60+Mathf.Sin(time*8)*25:work,0,-cheer*135);
            head.localRotation=Quaternion.Euler(eat?Mathf.Sin(time*6)*7:cook?8:Mathf.Sin(time*2)*2,Mathf.Sin(time*1.8f)*4,0);
            last=pos;
        }
    }
}
