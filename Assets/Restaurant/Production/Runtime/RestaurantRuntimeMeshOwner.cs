using System;
using UnityEngine;
namespace IdleRestaurant {
 // Only newly generated runtime meshes are registered here. Imported prefab meshes stay shared.
 public sealed class RestaurantRuntimeMeshOwner:MonoBehaviour {
  [NonSerialized] public Mesh mesh;
  void OnDestroy(){if(mesh)Destroy(mesh);}
 }
}
