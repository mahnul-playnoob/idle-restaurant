using System;
namespace IdleRestaurant {
 public sealed partial class RestaurantGame {
  public bool Family=>Catalog.familyBalance;
  public FamilyBalance Balance=>Catalog.familyBalance;
  public int Guide=>State.guideStep;
  public int ServiceSize(int outlet)=>outlet==0&&State.outlets[0].waves==0?Balance.firstServiceGuests:Balance.normalServiceGuests+(outlet%8)*2;
  public int SeatCapacity=>State.guideStep<4&&!State.introComplete?4:12;
  public double FamilyResearchCost(int i)=>Math.Round(Balance.researchCoins*Math.Pow(Balance.researchGrowth,State.research[i].level)*Math.Pow(2,Math.Max(0,Catalog.research[i].district)));
  public double FamilyResearchSeconds(int i)=>(Balance.researchSeconds+Balance.researchStepSeconds*State.research[i].level)/(1+ResearchEffect("rsp"));
  public double FamilyUnlock(int i)=>i==0?0:Math.Round(Balance.unlockBase*Math.Pow(Balance.unlockGrowth,i-1));
  public double FamilyCost(int i,int card){int l=State.outlets[i].upgrades[card];return l>=Catalog.maxUpgrade?double.PositiveInfinity:Math.Max(1,Math.Round(Balance.upgradeCoins[card]*Math.Pow(Balance.outletIncomeGrowth,i)*Math.Pow(Balance.upgradeGrowth,l)*(1+Balance.upgradeSlope*l)*Discount(card)));}
  public double FamilyMealValue(int i)=>Balance.mealCoins*Math.Pow(Balance.outletIncomeGrowth,i)*(Math.Pow(Balance.rankIncomeGrowth,Math.Min(60,State.outlets[i].rank))*(1+.01*Math.Max(0,State.outlets[i].rank-60)))*(1+Balance.menuPerLevel*State.outlets[i].upgrades[1])*(1+.05*State.prestige[i/8])*(1+(i/8>0?ResearchEffect(new[]{"","cbus","ctrn","cshp","ctrk"}[i/8]):0));
  public double FamilyPay(int i,int stage)=>FamilyMealValue(i)*Balance.mealShares[stage]*(1+Balance.payPerLevel*State.outlets[i].upgrades[stage*3+2])*(1+ManagerBonus(i,stage,1))*(1+.1*State.prestige[5+stage])*(1+ResearchEffect(new[]{"gcin","gdin","gsin"}[stage]));
  public double OrderDuration(int i)=>Math.Max(.55,Balance.orderSeconds/(1+Balance.orderSpeedPerLevel*State.outlets[i].upgrades[0])/(1+ManagerBonus(i,0,0))/(1+ResearchEffect("gchk")));
  public double CookDuration(int i)=>Math.Max(.65,Balance.cookSeconds/(1+Balance.cookSpeedPerLevel*State.outlets[i].upgrades[4])/(1+ManagerBonus(i,1,0))/(1+ResearchEffect("gdor")));
  public double MealDuration(int i)=>Math.Max(1.6,Balance.mealSeconds/(1+Balance.tableSpeedPerLevel*State.outlets[i].upgrades[6])/(1+ResearchEffect("gsea")));
  public double WalkingSpeed(int i)=>Balance.walkSpeed*(1+Balance.walkSpeedPerLevel*State.outlets[i].upgrades[3])*(1+ResearchEffect("gwlk"));
  public double FamilyIncome(int i){if(!State.outlets[i].open)return 0;double gap=Math.Max(OrderDuration(i)+1.35/WalkingSpeed(i),Math.Max(CookDuration(i)/2,MealDuration(i)/6));double cycle=ServiceSize(i)*gap+36/WalkingSpeed(i)+MealDuration(i)+1;return (FamilyPay(i,0)+FamilyPay(i,1)+FamilyPay(i,2))*ServiceSize(i)*(1+Balance.serviceBonus)*60/cycle;}
  public void BeginService(){if(State.guideStep==0)State.guideStep=1;EnsureService(State.selected);}
  public void EnsureService(int i){var s=State.outlets[i];if(s.service!=null&&s.service.Length>0)return;int n=ServiceSize(i);s.service=new ServiceGuest[n];for(int j=0;j<n;j++)s.service[j]=new ServiceGuest{id=j,variant=(j+s.waves*3+i)%5,phase=GuestPhase.Unspawned,x=RestaurantWorld.EntranceSpawn.x,z=RestaurantWorld.EntranceSpawn.z,arrivalDelay=j*Balance.enterInterval};s.ordered=s.cooked=s.seated=0;s.waveIncome=0;s.spawnClock=0;}
  public void ChangePhase(ServiceGuest g,GuestPhase phase){g.phase=phase;g.clock=0;g.waypoint=0;}
  public void Arrive(ServiceGuest g){switch(g.phase){case GuestPhase.Entering:ChangePhase(g,GuestPhase.Queued);break;case GuestPhase.ToOrder:ChangePhase(g,GuestPhase.Ordering);break;case GuestPhase.ToPickup:ChangePhase(g,GuestPhase.WaitingFood);break;case GuestPhase.ToSeat:ChangePhase(g,GuestPhase.Sitting);break;case GuestPhase.Leaving:ChangePhase(g,GuestPhase.Exited);break;}}
  public int TotalWaves(){int n=0;foreach(var s in State.outlets)n+=s.waves;return n;}
  void TickFamily(double dt){
   State.seconds+=dt;double now=Now();for(int r=0;r<State.research.Length;r++){var s=State.research[r];if(s.finish>0&&s.finish<=now){s.finish=0;s.level++;Feedback?.Invoke(Catalog.research[r].title+" is ready!");}}
   for(int p=0;p<8;p++)if(State.prestigeFinish[p]>0&&State.prestigeFinish[p]<=now){State.prestigeFinish[p]=0;State.prestige[p]++;}
   for(int i=0;i<State.outlets.Length;i++){var s=State.outlets[i];if(!s.open)continue;if(i!=State.selected){double v=FamilyIncome(i)*dt/60;State.cash+=v;State.totalEarned+=v;continue;}if(State.guideStep==0&&!State.introComplete)continue;EnsureService(i);s.spawnClock+=dt;
    int cookSlots=2;foreach(var g in s.service){g.clock+=dt;if(g.ordered&&!g.foodReady&&cookSlots-->0){g.cookClock+=dt;if(g.cookClock>=CookDuration(i)){g.foodReady=true;s.cooked++;Earn(i,1,FamilyPay(i,1));}}
     if(g.phase==GuestPhase.Ordering&&g.clock>=OrderDuration(i)){g.ordered=true;s.ordered++;Earn(i,0,FamilyPay(i,0));ChangePhase(g,GuestPhase.ToPickup);}
     if(g.phase==GuestPhase.Receiving&&g.clock>=Balance.pickupSeconds){g.collected=true;ChangePhase(g,GuestPhase.ToSeat);}
     if(g.phase==GuestPhase.Sitting&&g.clock>=Balance.sitSeconds)ChangePhase(g,GuestPhase.Eating);
     if(g.phase==GuestPhase.Eating&&g.clock>=MealDuration(i)){if(!g.mealPaid){g.mealPaid=true;s.seated++;State.guestsServed++;double value=FamilyPay(i,2);uint hash=(uint)(g.id*1664525+s.waves*1013904223+i*7919);if(hash%100<Math.Min(85,s.upgrades[7]*Balance.tipChancePerLevel*100+ResearchEffect("ccr")*100)){g.tips=FamilyMealValue(i)*Balance.tipFraction;value+=g.tips;}Earn(i,2,value);if(State.guideStep==1)State.guideStep=2;}ChangePhase(g,GuestPhase.Standing);}
    }
    bool ended=true;foreach(var g in s.service)if(g.phase!=GuestPhase.Exited){ended=false;break;}if(ended){double bonus=s.waveIncome*Balance.serviceBonus*(1+ResearchEffect("arr"))+(i==0&&s.waves==0?Balance.firstServiceGift:0);State.cash+=bonus;State.totalEarned+=bonus;State.gems++;State.serviceStars++;s.waves++;Paid?.Invoke(i,3,bonus);Feedback?.Invoke("Service complete! +"+Economy.Money(bonus)+" and 1 ticket");s.service=null;if(State.guideStep==4)State.guideStep=5;}
   }
  }
  public bool Recruit(int tier){int price=tier==0?Balance.recruitTickets:tier==1?Balance.seniorTickets:Balance.masterTickets;if(State.gems<price)return false;int rarity=tier==2?4:tier==1?3:-1;for(int offset=0;offset<Catalog.managers.Length;offset++){int id=(State.recruitSequence+offset)%Catalog.managers.Length;var member=Catalog.managers[id];if(State.managerCopies[id]>0||(rarity>=0&&member.rarity!=rarity))continue;State.gems-=price;State.recruitSequence=id+1;State.managerCopies[id]=1;State.managerLevels[id]=Math.Max(1,State.managerLevels[id]);Feedback?.Invoke("Welcome, "+member.title+"!");return true;}Feedback?.Invoke("Your whole team is here. Train your favorites!");return false;}

 }
}
