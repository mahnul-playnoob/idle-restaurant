using System;
using UnityEngine;
namespace IdleRestaurant {
 [Serializable] public class OutletDefinition {
  public int id,district,capacity,premium,economyRows,segments; public string title,menu,source,body;
  public double fare,unlock,currentScale,newScale; public double[] weights;
  public double check=1,stow=1,walk=1,door=1,prediction=1;
  public GameObject environment,food;
 }
 [Serializable] public class ResearchDefinition { public string key,title,evidence; public int max=100,district=-1; public double cost,income,minutes; public double effect=.01; }
 [Serializable] public class ManagerDefinition { public string id,title; public int role,rarity,affinity=-1; }
 [Serializable] public class OfferDefinition {public string id,title,type,reward;public int amount;}
 [CreateAssetMenu(menuName="Restaurant/Game Catalog")] public class RestaurantCatalog:ScriptableObject {
  public GameObject guestPrefab,chefPrefab,hostPrefab;
  public OutletDefinition[] outlets; public ResearchDefinition[] research; public ManagerDefinition[] managers;
  public OfferDefinition[] offers={new OfferDefinition{id="demo.income2",title="2x income forever",type="NonConsumable",reward="income2"},new OfferDefinition{id="demo.income3",title="3x income forever",type="NonConsumable",reward="income3"},new OfferDefinition{id="demo.offline",title="Offline hospitality",type="NonConsumable",reward="offline"},new OfferDefinition{id="demo.gems100",title="100 gems",type="Consumable",reward="gems",amount=100},new OfferDefinition{id="demo.gems255",title="255 gems",type="Consumable",reward="gems",amount=255},new OfferDefinition{id="demo.gems520",title="520 gems",type="Consumable",reward="gems",amount=520},new OfferDefinition{id="demo.gems1100",title="1100 gems",type="Consumable",reward="gems",amount=1100},new OfferDefinition{id="demo.gems2750",title="2750 gems",type="Consumable",reward="gems",amount=2750}};
  public int waveGemDailyCap=100;
  public bool useNewCosts;
  [Tooltip("Proposed starting grant. Not verified in the source package.")] public double initialCash=100;
  public int maxUpgrade=30,maxRank=0,visibleGuests=18;
  public AudioClip musicClip,coinClip,tapClip,upgradeClip;[Range(0,1)]public float musicVolume=.4f;
  public double[] upgradeBases={45,45,90,40,50,90,55,55,120};
  public double[] starterCurrent={50,50,100,40,50,80,40,40,100};
  public double[] starterNew={25,25,50,20,25,40,20,20,50};
  public double[] referenceCostPowers={1.0,1.1,1.2100000000000002,1.3310000000000004,1.4641000000000006,1.6105100000000006,1.7715610000000008,1.9487171000000012,2.1435888100000016,2.357947691000002,2.5937424601000023,2.8531167061100025,3.138428376721003,3.452271214393104,3.7974983358324144,4.177248169415656,4.594972986357222,5.054470284992945,5.55991731349224,6.115909044841464,6.727499949325611,7.400249944258173,8.14027493868399,8.95430243255239,9.84973267580763,10.834705943388395,11.918176537727234,13.10999419149996,14.420993610649957,15.863092971714952,17.44940226888645};
  public double costSlope=.4,costGrowth=1.1,payGrowth=.2,premiumValue=60,standardValue=10,completionFraction=.1;
  public double[] stageShares={.3,.3,.4};
  public double offlineFraction=.25,offlineCapSeconds=7200,premiumOfflineFraction=.75,premiumOfflineCapSeconds=86400;
  public int offlineThresholdSeconds=300;
  [Tooltip("Reconstructed timing coefficients; original runtime method was not recovered.")] public double transferSeconds=.28,turnaroundSeconds=4,rowClearSeconds=.5;
  [Tooltip("Proposed optional reward durations, pending reference verification.")] public int rewardedBoostSeconds=600,rewardedCooldownSeconds=60;
  public string sourceVersion="Traffic Flow 0.8.1";
 }
 public static class Economy {
  public static double Effect(int card,int level) { switch(card) { case 0:return .25*(1+.045*level);case 1:case 7:return 1-1/(1+.0222*level);case 3:return 9/(1+level*.041666666666666664);case 4:return 3.5/(1+.15*level);case 6:return 3/(1+.05*level);default:return 1+.2*level;} }
  public static double Cost(RestaurantCatalog c,OutletDefinition d,int card,int level,int rank,double discount=1) {
   if(level>=c.maxUpgrade)return double.PositiveInfinity;
   double baseValue=c.upgradeBases[card];
   double scale=d.id==0?(c.useNewCosts?c.starterNew[card]:c.starterCurrent[card])/baseValue:(c.useNewCosts?d.newScale:d.currentScale)*d.weights[card/3];
   double rankScale=Math.Pow(c.useNewCosts?42.875:27,rank)*scale;
   double linear=(1+c.costSlope*level)*baseValue;
   return Math.Round((c.costGrowth==1.1&&c.referenceCostPowers.Length>level?c.referenceCostPowers[level]:Math.Pow(c.costGrowth,level))*linear*discount*rankScale,MidpointRounding.ToEven);
  }
  public static double ResearchCost(ResearchDefinition d,int level) {
   if(d.district>=0)return d.cost;
   double b=d.cost*(d.key=="rsp"?2.4:d.key=="ccr"?1.75:1),t=Math.Clamp((level-20)/60.0,0,1),q=Math.Pow(1.28,.3);
   return Math.Max(1,Math.Floor(Math.Pow(q,level)*(q-1)/.28*b*Math.Pow(10000,(3-2*t)*t*t)));
  }
  public static double ResearchSeconds(ResearchDefinition d,int level,double speedMultiplier) {
   bool special=d.key=="arr"||d.key=="ccr"||d.key=="rsp"||d.key.EndsWith("ds");
   double s=d.district>=0?d.minutes*60:(special?300:180)*(1+.5*level+(special?.07:.1)*level*level);
   s/=speedMultiplier;double quantum=s>=3600?300:30;return Math.Max(30,Math.Round(s/quantum,MidpointRounding.ToEven)*quantum);
  }
  public static string Money(double x) {
   string[] suffix={"","K","M","B","T","Qa","Qi","Sx","Sp","Oc","No","Dc"};int i=0;while(Math.Abs(x)>=1000&&i<suffix.Length-1){x/=1000;i++;}return "$"+x.ToString(i==0?"N0":"0.##",System.Globalization.CultureInfo.InvariantCulture)+suffix[i];
  }
 }
}
