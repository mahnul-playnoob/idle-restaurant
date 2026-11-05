using UnityEngine;
using RestaurantPreview;
namespace IdleRestaurant {
 public enum HumanMotion {Idle,Waiting,Ordering,Walking,Carrying,Receiving,Sitting,Eating,Standing,Leaving,Cooking}
 public static class RestaurantAnimation {
  public static void Pose(Person p,HumanMotion mode,float time,Vector3 position,float yaw,int variant,float progress=0,bool reduced=false){
   float t=reduced?0:time*(.88f+.07f*(variant%4));bool seated=mode==HumanMotion.Eating;bool carrying=mode==HumanMotion.Carrying||mode==HumanMotion.Receiving||mode==HumanMotion.Leaving;
   p.Pose(t,position,carrying,mode==HumanMotion.Cooking,seated,0,yaw);
   if(mode==HumanMotion.Waiting||mode==HumanMotion.Idle){p.body.localPosition=new Vector3(reduced?0:Mathf.Sin(t*1.4f+variant)*.014f,0,0);p.head.localRotation=Quaternion.Euler(0,reduced?0:Mathf.Sin(t*.8f+variant)*12,0);}
   if(mode==HumanMotion.Ordering){p.rightArm.localRotation=Quaternion.Euler(58+Mathf.Sin(t*4)*8,0,-12);p.head.localRotation=Quaternion.Euler(4+Mathf.Sin(t*5)*5,-8,0);}
   if(mode==HumanMotion.Cooking){int style=variant%3;float work=Mathf.Sin(t*(style==0?9:style==1?4:6));p.rightArm.localRotation=Quaternion.Euler(style==0?94+work*19:104,style==1?work*24:0,style==2?work*18:0);p.leftArm.localRotation=Quaternion.Euler(85,0,style==1?-15:0);}
   if(mode==HumanMotion.Sitting||mode==HumanMotion.Standing){float v=mode==HumanMotion.Sitting?progress:1-progress;p.leftLeg.localRotation=Quaternion.Euler(v*65,0,0);p.rightLeg.localRotation=Quaternion.Euler(v*65,0,0);p.body.localPosition=new Vector3(0,-Mathf.Sin(v*Mathf.PI)*.04f,0);p.leftArm.localRotation=p.rightArm.localRotation=Quaternion.Euler(65,0,0);}
   if(mode==HumanMotion.Eating){float bite=Mathf.Sin(t*(variant%2==0?5:3.8f));p.rightArm.localRotation=Quaternion.Euler(65+bite*24,variant%3==1?10:0,0);p.leftArm.localRotation=Quaternion.Euler(variant%3==2?70-bite*16:35,0,0);p.head.localRotation=Quaternion.Euler(4+Mathf.Max(0,bite)*6,0,0);}
   if(mode==HumanMotion.Leaving&&progress<.7f)p.rightArm.localRotation=Quaternion.Euler(20,0,-90-Mathf.Sin(t*8)*15);
  }
 }
}
