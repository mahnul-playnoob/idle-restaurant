using UnityEngine;
namespace IdleRestaurant {
 public sealed class RestaurantSeat:MonoBehaviour {
  public int index,owner=-1;public Vector3 centre,plate;public float yaw;
  public Vector3 Approach=>new Vector3(centre.x,0,centre.z-.65f);
  public bool Available=>gameObject.activeInHierarchy&&owner<0;
 }
}
