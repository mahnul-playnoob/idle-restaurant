using UnityEngine;
namespace IdleRestaurant {
 [CreateAssetMenu(menuName="Restaurant/Family Balance")]
 public sealed class FamilyBalance:ScriptableObject {
  public double startingCoins=0,mealCoins=12,outletIncomeGrowth=1.65,unlockBase=200,unlockGrowth=1.9;
  public double[] upgradeCoins={10,18,22,15,25,30,18,28,35};
  public double orderSpeedPerLevel=.05,cookSpeedPerLevel=.06,tableSpeedPerLevel=.04,walkSpeedPerLevel=.018,payPerLevel=.12,menuPerLevel=.06,tipChancePerLevel=.02,tipFraction=.15;
  public double[] mealShares={.25,.25,.5};public int[] extraSlotTickets={0,8,16,24,40};
  public double researchCoins=80,researchGrowth=1.2,researchSeconds=15,researchStepSeconds=10;
  public double upgradeGrowth=1.075,upgradeSlope=.12,rankIncomeGrowth=1.10;
  public float orderSeconds=1.6f,cookSeconds=2.4f,mealSeconds=3.8f,walkSpeed=1.8f;
  public int firstServiceGuests=6,normalServiceGuests=8,maxVisibleGuests=18;
  public double serviceBonus=.15,firstServiceGift=100,offlineRate=1,offlineCapSeconds=86400;
  public int starEveryServices=5,trainingTicketBase=2;public double starResearchSeconds=10,starResearchStepSeconds=5;
  public int recruitTickets=8,seniorTickets=24,masterTickets=80;
  public float enterInterval=.8f,sitSeconds=.55f,standSeconds=.5f,pickupSeconds=.4f;
 }
 public enum GuestPhase {Unspawned,Entering,Queued,ToOrder,Ordering,ToPickup,WaitingFood,Receiving,ToSeat,Sitting,Eating,Standing,Leaving,Exited}
 [System.Serializable,UnityEngine.Scripting.Preserve]
 public sealed class ServiceGuest {
  public int id,variant,seat=-1,waypoint; public GuestPhase phase;public double clock,cookClock,arrivalDelay;
  public bool ordered,foodReady,collected,mealPaid,returnedPlate;public float x,z;public double tips;
 }
}
