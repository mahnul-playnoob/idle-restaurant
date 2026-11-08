using System;
using System.Linq;
namespace IdleRestaurant
{
    public sealed partial class RestaurantGame
    {
        public int OutletServicesRequired(int next)=>Math.Max(0,Balance.unlockServicesBase+(int)Math.Ceiling(Math.Max(0,next-1)*Balance.unlockServicesPerOutlet));
        public int OutletUpgradesRequired(int next)=>Math.Max(0,Balance.unlockUpgradeBase+Math.Min(Math.Max(0,next-1)*Balance.unlockUpgradesPerOutlet,Balance.unlockUpgradeExtraCap));
        public int OutletUpgradeCount(int outlet)=>State.outlets[outlet].upgrades.Sum();
        public bool ExpansionReady(int next,out string reason)
        {
            reason="";if(next<=0||State.outlets[next].open)return true;
            var previous=State.outlets[next-1];
            if(previous.waves<OutletServicesRequired(next)){reason="Complete "+OutletServicesRequired(next)+" services at "+Catalog.outlets[next-1].title+" ("+previous.waves+" done)";return false;}
            if(OutletUpgradeCount(next-1)<OutletUpgradesRequired(next)){reason="Buy "+OutletUpgradesRequired(next)+" improvements at "+Catalog.outlets[next-1].title;return false;}
            return true;
        }
        public int TeamCount=>State.managerCopies.Count(x=>x>0);
        public int NextRecruitId()
        {
            int best=-1;var roles=new int[3];for(int i=0;i<Catalog.managers.Length;i++)if(State.managerCopies[i]>0)roles[Catalog.managers[i].role]++;
            for(int i=0;i<Catalog.managers.Length;i++)if(State.managerCopies[i]==0){
                if(best<0||Catalog.managers[i].rarity<Catalog.managers[best].rarity||
                  (Catalog.managers[i].rarity==Catalog.managers[best].rarity&&(roles[Catalog.managers[i].role]<roles[Catalog.managers[best].role]||
                   (roles[Catalog.managers[i].role]==roles[Catalog.managers[best].role]&&i<best))))best=i;
            }
            return best;
        }
        public void EnsureStarterManager(){if(!Family||State.managerCopies[0]>0||TotalWaves()<Balance.starterServices)return;State.managerCopies[0]=1;State.managerLevels[0]=Math.Max(1,State.managerLevels[0]);State.lastRecruitWaves=TotalWaves();Feedback?.Invoke(Catalog.managers[0].title+" joined your team! Open TEAM to assign.");}
        public int RecruitPrice=>Balance.recruitTickets+TeamCount*Balance.recruitTicketStep;
        public int RecruitServicesRequired=>Math.Max(Balance.recruitServiceBase+TeamCount*Balance.recruitServiceStep+(int)Math.Floor(TeamCount*TeamCount*Balance.recruitServiceCurve),State.lastRecruitWaves<0?0:State.lastRecruitWaves+Balance.recruitServiceGap);
        public int RecruitOutletsRequired=>Math.Min(State.outlets.Length,2+TeamCount/2);
        public bool CanRecruit(out string reason)
        {
            reason="";if(NextRecruitId()<0){reason="Your whole team is here";return false;}
            int outlets=State.outlets.Count(x=>x.open);
            if(outlets<RecruitOutletsRequired){reason="Open "+RecruitOutletsRequired+" restaurants ("+outlets+" open)";return false;}
            if(TotalWaves()<RecruitServicesRequired){reason="Complete "+RecruitServicesRequired+" services ("+TotalWaves()+" done)";return false;}
            if(State.gems<RecruitPrice){reason="Earn "+RecruitPrice+" tickets";return false;}
            return true;
        }
        void RecordServiceIncome(int index,double sustainableIncome)
        {
            var s=State.outlets[index];if(!s.serviceTimingValid)return;
            double duration=State.seconds-s.serviceStartedSeconds;if(duration<5)return;
            s.lastServiceSeconds=duration;s.lastServiceEarned=sustainableIncome;
            double raw=RawFamilyIncome(index);if(raw<=0)return;
            double ratio=Math.Clamp(sustainableIncome*60/duration/raw,.5,1.25);
            s.incomeCalibration=.5*(s.incomeCalibration>0?s.incomeCalibration:1)+.5*ratio;
        }
    }
}
