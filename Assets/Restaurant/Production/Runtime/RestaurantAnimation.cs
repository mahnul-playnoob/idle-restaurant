using UnityEngine;
using RestaurantPreview;
namespace IdleRestaurant {
 public enum HumanMotion {Idle,Waiting,Ordering,Walking,Carrying,Receiving,Sitting,Eating,Standing,Leaving,Cooking,Serving}
 public static class RestaurantAnimation {
  public static void Pose(Person p,HumanMotion mode,float time,Vector3 position,float yaw,int variant,float progress=0,bool reduced=false){
   float t=reduced?0:time*(.88f+.07f*(variant%4));bool seated=mode==HumanMotion.Eating;bool carrying=mode==HumanMotion.Carrying||mode==HumanMotion.Receiving||mode==HumanMotion.Leaving;
   p.Pose(t,position,carrying,mode==HumanMotion.Cooking,seated,0,yaw);p.body.localRotation=Quaternion.identity;
   if(carrying){p.leftArm.localRotation=Quaternion.Euler(95,0,12);p.rightArm.localRotation=Quaternion.Euler(95,0,-12);}
   if(mode==HumanMotion.Waiting||mode==HumanMotion.Idle){p.body.localPosition=new Vector3(reduced?0:Mathf.Sin(t*1.4f+variant)*.014f,0,0);p.head.localRotation=Quaternion.Euler(0,reduced?0:Mathf.Sin(t*.8f+variant)*12,0);}
   if(mode==HumanMotion.Ordering){p.rightArm.localRotation=Quaternion.Euler(58+Mathf.Sin(t*4)*8,0,-12);p.head.localRotation=Quaternion.Euler(4+Mathf.Sin(t*5)*5,-8,0);}
   if(mode==HumanMotion.Cooking){int style=variant%3;float work=Mathf.Sin(t*(style==0?8:style==1?4:5));p.body.localRotation=Quaternion.Euler(3,0,0);p.rightArm.localRotation=Quaternion.Euler(style==0?88+work*16:100,style==1?work*25:0,style==2?work*16:0);p.leftArm.localRotation=Quaternion.Euler(78,0,8);p.head.localRotation=Quaternion.Euler(10,work*3,0);}
   if(mode==HumanMotion.Receiving||mode==HumanMotion.Serving){float reach=Mathf.SmoothStep(0,1,Mathf.Clamp01(progress));p.leftArm.localRotation=Quaternion.Euler(40+55*reach,0,10);p.rightArm.localRotation=Quaternion.Euler(40+55*reach,0,-10);p.head.localRotation=Quaternion.Euler(10,0,0);}
   if(mode==HumanMotion.Sitting||mode==HumanMotion.Standing){float v=mode==HumanMotion.Sitting?progress:1-progress;p.leftLeg.localRotation=Quaternion.Euler(v*65,0,0);p.rightLeg.localRotation=Quaternion.Euler(v*65,0,0);p.body.localPosition=new Vector3(0,-Mathf.Sin(v*Mathf.PI)*.04f,0);p.leftArm.localRotation=p.rightArm.localRotation=Quaternion.Euler(65,0,0);}
   if(mode==HumanMotion.Eating){float cycle=reduced?.45f:(progress*3+variant*.08f)%1;float bite=Mathf.SmoothStep(0,1,Mathf.Sin(cycle*Mathf.PI));bool left=variant%3==2;var hand=left?p.leftArm:p.rightArm;var resting=left?p.rightArm:p.leftArm;hand.localRotation=Quaternion.Euler(82+44*bite,left?-35:35,left?5:-5);resting.localRotation=Quaternion.Euler(80,0,left?-8:8);p.head.localRotation=Quaternion.Euler(3+3*bite,0,0);}

  }
 }
}
