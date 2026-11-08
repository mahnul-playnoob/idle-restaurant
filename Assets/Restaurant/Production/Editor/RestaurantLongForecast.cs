using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IdleRestaurant;
using UnityEditor;
using UnityEngine;
public static class RestaurantLongForecast
{
    public static void Run()
    {
        var original=AssetDatabase.LoadAssetAtPath<RestaurantCatalog>(RestaurantProductionBuilder.CatalogPath);
        var reports=new List<object>();foreach(int window in new[]{180,600,1200})reports.Add(Simulate(original,false,window));reports.Add(Simulate(original,true,180));
        Directory.CreateDirectory("Verification/Progression050");File.WriteAllText("Verification/Progression050/ProgressionForecast.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{utc=DateTime.UtcNow,classification="Analytical estimates, not measured playtime. Continuous online income; no assigned managers, research, claimed goal rewards, stars or offline earnings. Service durations use the reconstructed raw cycle predictor. Greedy upgrades plus mandatory expansion investments. Includes the earlier observed onboarding seed; calibration remains 1 until actual gameplay measures it.",reports},Newtonsoft.Json.Formatting.Indented));
    }
    static object Simulate(RestaurantCatalog source,bool old,int window)
    {
        var catalog=UnityEngine.Object.Instantiate(source);var balance=UnityEngine.Object.Instantiate(source.familyBalance);catalog.familyBalance=balance;
        if(old){balance.unlockBase=200;balance.unlockGrowth=1.9;balance.unlockServicesBase=0;balance.unlockServicesPerOutlet=0;balance.unlockUpgradeBase=0;balance.unlockUpgradesPerOutlet=0;}
        var g=new RestaurantGame(catalog);g.State.introComplete=true;g.State.guideStep=6;g.State.outlets[0].waves=1;g.State.outlets[0].upgrades[0]=g.State.outlets[0].upgrades[3]=1;g.State.cash=157.8;double seconds=42.8;int cur=0,purchases=2,iterations=0;double remaining=Cycle(g,cur);var milestones=new List<object>();
        while(cur<39&&seconds<2592000&&iterations++<100000){
            int next=cur+1;if(g.CanOpen(next,out _)){g.Open(next);cur=next;remaining=Cycle(g,cur);milestones.Add(new{outlet=cur+1,minutes=seconds/60,services=g.TotalWaves(),purchases});continue;}
            double income=Math.Max(1e-9,g.NetworkIncome()/60),best=0,price=0;int oi=-1,ci=-1;bool invest=g.OutletUpgradeCount(cur)<g.OutletUpgradesRequired(next);
            for(int i=0;i<=cur;i++){if(invest&&i!=cur)continue;double before=g.FamilyIncome(i);for(int card=0;card<9;card++){double cost=g.Cost(i,card);if(!double.IsFinite(cost))continue;g.State.outlets[i].upgrades[card]++;double gain=(g.FamilyIncome(i)-before)/60;g.State.outlets[i].upgrades[card]--;double roi=gain/cost;if(roi>best&&(roi>1.0/window||invest)){best=roi;oi=i;ci=card;price=cost;}}}
            if(oi>=0&&g.State.cash>=price){double cycle=Cycle(g,cur);g.State.selected=oi;if(g.Buy(ci))purchases++;g.State.selected=cur;remaining*=Cycle(g,cur)/cycle;continue;}
            double wait=remaining;if(oi>=0)wait=Math.Min(wait,Math.Max(.001,(price-g.State.cash)/income));if(g.ExpansionReady(next,out _))wait=Math.Min(wait,Math.Max(.001,(g.FamilyUnlock(next)-g.State.cash)/income));
            g.State.cash+=income*wait;seconds+=wait;remaining-=wait;if(remaining<.001){g.State.outlets[cur].waves++;remaining=Cycle(g,cur);}
        }
        var result=new{variant=old?"Previous 0.4 balance":"Longer 0.5 balance",paybackWindowSeconds=window,allOutletsOpened=cur==39,totalHours=seconds/3600,totalServices=g.TotalWaves(),purchases,iterations,milestones};UnityEngine.Object.DestroyImmediate(catalog);UnityEngine.Object.DestroyImmediate(balance);return result;
    }
    static double Cycle(RestaurantGame g,int i){double pay=0;for(int s=0;s<3;s++)pay+=g.FamilyPay(i,s);return Math.Max(1,pay*g.ServiceSize(i)*(1+g.Balance.serviceBonus)*60/Math.Max(1e-9,g.RawFamilyIncome(i)));}
}
