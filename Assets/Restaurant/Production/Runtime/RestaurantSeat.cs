using UnityEngine;
namespace IdleRestaurant {
 public sealed class RestaurantSeat:MonoBehaviour {
  public int index,owner=-1;public Vector3 centre,plate;public float yaw;
  public Vector3 Approach=>new Vector3(centre.x,0,centre.z-.65f);
  public Vector3 SeatedPosition=>centre+Vector3.up*.23f+(Quaternion.Euler(0,yaw,0)*Vector3.back)*.24f;
  public Vector3 SeatingPose(float amount){amount=Mathf.Clamp01(amount);var p=Vector3.Lerp(Approach,SeatedPosition,Mathf.SmoothStep(0,1,Mathf.Clamp01((amount-.45f)/.55f)));p.y=.23f*Mathf.SmoothStep(0,1,Mathf.Clamp01(amount/.45f));return p;}
  public bool Available=>gameObject.activeInHierarchy&&owner<0;
 }
}
