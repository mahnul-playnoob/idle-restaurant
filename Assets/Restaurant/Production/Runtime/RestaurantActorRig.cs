using UnityEngine;
namespace IdleRestaurant {
 public sealed class RestaurantActorRig:MonoBehaviour {
  public Transform body,head,leftArm,rightArm,leftLeg,rightLeg,tray;
  public bool Valid=>body&&head&&leftArm&&rightArm&&leftLeg&&rightLeg;
 }
}
