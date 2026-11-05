using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using IdleRestaurant;
public static class RestaurantProgressionSimulator {
 [MenuItem("Restaurant/Production/Simulate Full Progression")]
 public static void Run(){var catalog=AssetDatabase.LoadAssetAtPath<RestaurantCatalog>(RestaurantProductionBuilder.CatalogPath);bool old=catalog.useNewCosts;var rows=new List<object>();var csv=new StringBuilder("variant,multiplier,outlet,source,unlock_hours,max_hours,network_per_minute,status\n");try{foreach(bool variant in new[]{false,true})foreach(int multiplier in new[]{1,2,6}){
   catalog.useNewCosts=variant;var game=new RestaurantGame(catalog);game.State.cash=0;game.State.income2=multiplier>=2;game.State.income3=multiplier==6;double minutes=0;
   for(int index=0;index<catalog.outlets.Length;index++){double income=game.NetworkIncome();if(index>0&&index%8==0){int research=Array.FindIndex(catalog.research,x=>x.district==index/8);var licence=catalog.research[research];if(income<licence.income){rows.Add(new{variant,multiplier,outlet=index,status="income gate not reached",income,needed=licence.income});break;}WaitSpend(game,licence.cost,income,ref minutes);double wait=Economy.ResearchSeconds(licence,0,1)/60;minutes+=wait;game.State.cash+=wait*income;game.State.research[research].level=1;}
    if(index>0)WaitSpend(game,catalog.outlets[index].unlock,income,ref minutes);game.State.outlets[index].open=true;game.State.selected=index;double unlock=minutes/60;
    for(int purchase=0;purchase<270;purchase++){income=game.NetworkIncome();double current=game.EstimatedIncome(index),best=double.PositiveInfinity,bestCost=double.PositiveInfinity;int chosen=-1;for(int card=0;card<9;card++){var state=game.State.outlets[index];if(state.upgrades[card]>=30)continue;double cost=game.Cost(index,card);state.upgrades[card]++;double delta=game.EstimatedIncome(index)-current;state.upgrades[card]--;double value=cost/Math.Max(delta,1e-30);if(value<best||(value==best&&cost<bestCost)){best=value;bestCost=cost;chosen=card;}}if(chosen<0)throw new Exception("No valid purchase");WaitSpend(game,bestCost,income,ref minutes);game.State.outlets[index].upgrades[chosen]++;}
    rows.Add(new{variant=variant?"New":"Current",multiplier,outlet=index,source=catalog.outlets[index].source,unlockHours=unlock,maxHours=minutes/60,networkPerMinute=game.NetworkIncome(),status="conditional model"});csv.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,"{0},{1},{2},{3},{4:R},{5:R},{6:R},conditional model\n",variant?"New":"Current",multiplier,index,catalog.outlets[index].source,unlock,minutes/60,game.NetworkIncome());
   }
  }}finally{catalog.useNewCosts=old;}Directory.CreateDirectory("Verification");File.WriteAllText("Verification/ProgressionSimulation.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{assumptions="Continuous online analytical estimate. Greedy cost-to-income purchases; max outlet before next; source licences; no optional research/managers/prestige/offline. Reconstructed income predictor, not actual gameplay time.",rows},Newtonsoft.Json.Formatting.Indented));File.WriteAllText("Verification/ProgressionSimulation.csv",csv.ToString());Debug.Log("RESTAURANT_PROGRESSION_SIMULATION rows="+rows.Count);}
 static void WaitSpend(RestaurantGame game,double cost,double income,ref double minutes){if(income<=0&&cost>game.State.cash)throw new Exception("Zero income gate");double wait=Math.Max(0,(cost-game.State.cash)/Math.Max(income,1e-30));minutes+=wait;game.State.cash+=wait*income-cost;}
}
