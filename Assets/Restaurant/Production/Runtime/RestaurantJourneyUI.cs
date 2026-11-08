using System;
using TMPro;
using UnityEngine;
namespace IdleRestaurant
{
    public sealed partial class RestaurantApp
    {
        int mapDistrict=-1;
        RectTransform expansionBadge,teamBadge;
        RectTransform goalRewardBadge;TMP_Text goalRewardCount;
        readonly RestaurantSymbol[] navigationIcons=new RestaurantSymbol[5];
        RestaurantDistrict Theme=>Game.Journey.districts[Game.State.selected/8];
        RestaurantSymbol Symbol(Transform parent,RestaurantSymbolKind kind,Vector2 min,Vector2 max,Color color)
        {
            var icon=Rect(kind+" icon",parent,min,max).gameObject.AddComponent<RestaurantSymbol>();icon.kind=kind;icon.color=color;icon.raycastTarget=false;return icon;
        }
        DistrictIcon FoodIcon(Transform parent,int district,Vector2 min,Vector2 max,int outlet=-1)
        {
            var icon=Rect("District food emblem",parent,min,max).gameObject.AddComponent<DistrictIcon>();icon.district=district;icon.variant=outlet<0?-1:outlet%8;icon.color=Game.Journey.districts[district].ink;icon.raycastTarget=false;return icon;
        }
        void StyleNavigation(UnityEngine.UI.Button button,int index)
        {
            var text=button.GetComponentInChildren<TMP_Text>();text.rectTransform.anchorMin=new Vector2(0,.02f);text.rectTransform.anchorMax=new Vector2(1,.36f);text.fontSize=18;
            navigationIcons[index]=Symbol(button.transform,(RestaurantSymbolKind)index,new Vector2(.28f,.39f),new Vector2(.72f,.94f),index==2?Cream:Green);
            if(index==0||index==1){var badge=Panel(index==0?"Map readiness":"Team readiness",button.transform,new Vector2(.71f,.65f),new Vector2(.97f,.97f),GuideYellow);badge.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;Text(badge,"!",Vector2.zero,Vector2.one,16,Ink,TextAlignmentOptions.Center).raycastTarget=false;badge.gameObject.SetActive(false);if(index==0)expansionBadge=badge;else teamBadge=badge;}
            if(index==4){goalRewardBadge=Panel("Goal reward badge",button.transform,new Vector2(.71f,.65f),new Vector2(.97f,.97f),GuideYellow);goalRewardBadge.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;goalRewardCount=Text(goalRewardBadge,"",Vector2.zero,Vector2.one,16,Ink,TextAlignmentOptions.Center);goalRewardCount.raycastTarget=false;goalRewardBadge.gameObject.SetActive(false);}
        }
        void RefreshGoalBadge()
        {
            if(!goalRewardBadge||Game==null)return;
            int next=Array.FindIndex(Game.State.outlets,o=>!o.open);if(expansionBadge)expansionBadge.gameObject.SetActive(Game.State.introComplete&&next>0&&Game.CanOpen(next,out _));
            bool useful=false;for(int id=0;id<catalog.managers.Length;id++)if(Game.State.managerCopies[id]>0&&Game.State.outlets[Game.State.selected].managers[catalog.managers[id].role]<0){useful=true;break;}
            if(teamBadge)teamBadge.gameObject.SetActive(Game.State.introComplete&&(useful||Game.CanRecruit(out _)));
            int ready=0;if(Game.State.introComplete)for(int i=0;i<12;i++)if(!Game.GoalClaimed(i)&&Game.GoalProgress(i)>=Game.GoalTarget(i))ready++;
            goalRewardBadge.gameObject.SetActive(ready>0);goalRewardCount.text=ready.ToString();
        }
        void BuildGrowthHint()
        {
            var background=Panel("Next restaurant progress background",body,new Vector2(.245f,.845f),new Vector2(.745f,.905f),Cream);background.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;var hint=Text(background,"",new Vector2(.025f,.06f),new Vector2(.975f,.94f),14,Theme.ink,TextAlignmentOptions.Center);hint.name="Next restaurant progress";hint.raycastTarget=false;
            refresh.Add(()=>{background.gameObject.SetActive(Game.State.introComplete);int next=Array.FindIndex(Game.State.outlets,o=>!o.open);if(next<1){hint.text="Your restaurants keep growing";return;}int prev=next-1;
                hint.text=Game.CanOpen(next,out _)?"NEXT RESTAURANT READY\nTap MAP to open":"NEXT RESTAURANT: "+Economy.Money(Game.FamilyUnlock(next))+"\n"+Math.Min(Game.State.outlets[prev].waves,Game.OutletServicesRequired(next))+"/"+Game.OutletServicesRequired(next)+" services · "+Math.Min(Game.OutletUpgradeCount(prev),Game.OutletUpgradesRequired(next))+"/"+Game.OutletUpgradesRequired(next)+" upgrades";});
        }
        void BuildDistrictMap()
        {
            if(mapDistrict<0)mapDistrict=Game.State.selected/8;
            var theme=Game.Journey.districts[mapDistrict];
            var p=Panel("District journey",body,Vector2.zero,Vector2.one,theme.paper);
            Text(p,"YOUR RESTAURANT JOURNEY",new Vector2(.04f,.945f),new Vector2(.96f,.99f),26,theme.ink).fontStyle=FontStyles.Bold;
            for(int district=0;district<5;district++){
                int id=district;var d=Game.Journey.districts[id];
                var tab=Button(p,d.shortName,new Vector2(.02f+id*.196f,.838f),new Vector2(.207f+id*.196f,.936f),()=>{mapDistrict=id;Navigate("map");},id==mapDistrict?d.ink:Cream,15);
                tab.name="District tab "+id;
                var label=tab.GetComponentInChildren<TMP_Text>();label.rectTransform.anchorMax=new Vector2(1,.34f);label.color=id==mapDistrict?Cream:d.ink;
                FoodIcon(tab.transform,id,new Vector2(.29f,.36f),new Vector2(.71f,.98f));
            }
            var banner=Panel("District landmark",p,new Vector2(.025f,.66f),new Vector2(.975f,.825f),theme.ink);
            FoodIcon(banner,mapDistrict,new Vector2(.025f,.13f),new Vector2(.19f,.93f));
            var name=Text(banner,theme.name,new Vector2(.22f,.65f),new Vector2(.96f,.97f),29,Cream);name.fontStyle=FontStyles.Bold;
            var desc=Text(banner,theme.description,new Vector2(.22f,.25f),new Vector2(.97f,.66f),19,Cream);desc.enableAutoSizing=true;desc.fontSizeMin=17;desc.fontSizeMax=19;
            var count=Text(banner,"",new Vector2(.22f,.03f),new Vector2(.96f,.26f),18,Cream);
            refresh.Add(()=>{int n=0;for(int k=mapDistrict*8;k<mapDistrict*8+8;k++)if(Game.State.outlets[k].open)n++;count.text=theme.landmark+"  ·  "+n+" / 8 OPEN";});
            // Keep the existing fixed-height RectMask2D scroll architecture; its explicit size is authoritative.
            var viewport=Panel("Map viewport",p,new Vector2(.015f,.02f),new Vector2(.985f,.645f),theme.paper);
            viewport.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
            var scroll=viewport.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();scroll.horizontal=false;scroll.movementType=UnityEngine.UI.ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=40;
            var content=Rect("Journey path content",viewport,new Vector2(0,1),Vector2.one);content.pivot=new Vector2(.5f,1);content.sizeDelta=new Vector2(0,8*174+30);
            scroll.content=content;scroll.viewport=viewport;
            for(int k=0;k<8;k++){
                int id=mapDistrict*8+k;bool left=k%2==0;
                var row=Rect("Outlet stop "+id,content,new Vector2(0,1),Vector2.one);row.pivot=new Vector2(.5f,1);row.sizeDelta=new Vector2(0,174);row.anchoredPosition=new Vector2(0,-k*174);
                float x=left?.12f:.88f;
                Panel("Connected route",row,new Vector2(x-.005f,0),new Vector2(x+.005f,1),theme.accent).GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
                Panel("Route turn",row,new Vector2(.12f,.0f),new Vector2(.88f,.025f),theme.accent).GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
                var node=Panel("Route node",row,new Vector2(x-.055f,.40f),new Vector2(x+.055f,.78f),theme.ink);
                Text(node,(id+1).ToString("00"),Vector2.zero,Vector2.one,23,Cream,TextAlignmentOptions.Center).fontStyle=FontStyles.Bold;
                var card=Panel("Outlet card",row,new Vector2(left?.23f:.025f,.08f),new Vector2(left?.975f:.77f,.94f),Cream);
                FoodIcon(card,mapDistrict,new Vector2(.025f,.42f),new Vector2(.17f,.92f),id);
                var title=Text(card,catalog.outlets[id].title,new Vector2(.19f,.64f),new Vector2(.98f,.95f),24,theme.ink);title.enableAutoSizing=true;title.fontSizeMin=19;title.fontSizeMax=24;
                var status=Text(card,"",new Vector2(.19f,.42f),new Vector2(.98f,.66f),17,Muted);status.enableAutoSizing=true;status.fontSizeMin=15;status.fontSizeMax=17;status.textWrappingMode=TextWrappingModes.NoWrap;
                var info=Text(card,"",new Vector2(.035f,.08f),new Vector2(.60f,.40f),17,theme.ink);info.enableAutoSizing=true;info.fontSizeMin=15;info.fontSizeMax=17;
                var visit=Button(card,"",new Vector2(.63f,.07f),new Vector2(.98f,.40f),()=>{if(Game.Open(id)){world.Show(id);Navigate("play");}},Green,18);visit.name="Map outlet action "+id;
                var label=visit.GetComponentInChildren<TMP_Text>();
                refresh.Add(()=>{
                    bool opened=Game.State.outlets[id].open,previous=id==0||Game.State.outlets[id-1].open,can=Game.CanOpen(id,out _),current=Game.State.selected==id;
                    status.text=current?"YOU ARE HERE":opened?"OPEN · STAR "+Game.State.outlets[id].rank:previous?"NEXT RESTAURANT":"ON YOUR JOURNEY";
                    double price=Game.Family?Game.FamilyUnlock(id):catalog.outlets[id].unlock;
                    info.text=opened?catalog.outlets[id].menu:previous?Economy.Money(price)+" · "+Game.State.outlets[id-1].waves+"/"+Game.OutletServicesRequired(id)+" services\n"+Game.OutletUpgradeCount(id-1)+"/"+Game.OutletUpgradesRequired(id)+" improvements":"First open "+catalog.outlets[id-1].title;
                    label.text=opened?"VISIT":previous?"OPEN":"LOCKED";
                    visit.interactable=can;visit.GetComponent<UnityEngine.UI.Image>().color=can?UpgradeGreen:Unavailable;label.color=can?Cream:Ink;
                    node.GetComponent<UnityEngine.UI.Image>().color=opened?theme.ink:theme.accent;
                    var outline=card.GetComponent<UnityEngine.UI.Outline>();if(!outline)outline=card.gameObject.AddComponent<UnityEngine.UI.Outline>();outline.enabled=current;outline.effectColor=theme.accent;outline.effectDistance=new Vector2(3,3);
                });
            }
            Canvas.ForceUpdateCanvases();int focus=Math.Clamp(Game.State.selected-mapDistrict*8,0,7);
            content.anchoredPosition=new Vector2(0,Mathf.Clamp(focus*174-50,0,Mathf.Max(0,content.rect.height-viewport.rect.height)));
        }
        void BuildJourneyGoals()
        {
            Game.EnsureGoalBoard();var p=PageTitle("Your goal book · "+(Game.State.taskBook+1),Game.LegacyGoalBoard?"Finish your existing book; varied goals follow next.":"Small achievements across your restaurant journey.");
            var content=Scroll(p,12,180);
            for(int i=0;i<12;i++){
                int id=i;var row=ListRow(content,i,180);row.name="Goal row "+id;
                var kind=Game.GoalKind(id);var icon=kind==RestaurantGoalKind.Outlets?RestaurantSymbolKind.Map:kind==RestaurantGoalKind.Teammates||kind==RestaurantGoalKind.Training?RestaurantSymbolKind.Team:kind==RestaurantGoalKind.Research?RestaurantSymbolKind.Research:kind==RestaurantGoalKind.Services?RestaurantSymbolKind.Restaurant:kind==RestaurantGoalKind.OrdersUpgrades?RestaurantSymbolKind.Orders:kind==RestaurantGoalKind.KitchenUpgrades?RestaurantSymbolKind.Kitchen:RestaurantSymbolKind.Dining;
                Symbol(row,icon,new Vector2(.025f,.57f),new Vector2(.12f,.94f),Theme.ink);
                var title=Text(row,Game.GoalTitle(id),new Vector2(.15f,.57f),new Vector2(.97f,.96f),25,Theme.ink);title.enableAutoSizing=true;title.fontSizeMin=21;title.fontSizeMax=25;
                var status=Text(row,"",new Vector2(.035f,.28f),new Vector2(.65f,.55f),21,Ink);
                var track=Panel("Goal progress track",row,new Vector2(.035f,.14f),new Vector2(.62f,.20f),Hex("#D2D8CA"));
                var fill=Panel("Goal progress fill",track,Vector2.zero,Vector2.one,UpgradeGreen);fill.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
                var reward=Text(row,"+"+Game.GoalReward(id)+" tickets",new Vector2(.66f,.35f),new Vector2(.97f,.58f),20,Green,TextAlignmentOptions.Center);
                var claim=Button(row,"",new Vector2(.67f,.07f),new Vector2(.98f,.34f),()=>{int book=Game.State.taskBook;if(Game.ClaimGoal(id)){Sound(upgrade);Save();if(Game.State.taskBook!=book)Navigate("tasks");else Refresh();}},Green,18);claim.name="Goal claim "+id;
                var label=claim.GetComponentInChildren<TMP_Text>();
                refresh.Add(()=>{double value=Game.GoalProgress(id),target=Game.GoalTarget(id);bool claimed=Game.GoalClaimed(id),ready=value>=target;
                    status.text=(kind==RestaurantGoalKind.Coins?Economy.Money(Math.Min(value,target))+" / "+Economy.Money(target):Math.Min(value,target).ToString("0")+" / "+target.ToString("0"))+(claimed?"  · DONE":"");
                    fill.anchorMax=new Vector2((float)Math.Min(1,value/target),1);label.text=claimed?"CLAIMED":ready?"CLAIM":"IN PROGRESS";claim.interactable=!claimed&&ready;
                    claim.GetComponent<UnityEngine.UI.Image>().color=!claimed&&ready?UpgradeGreen:Unavailable;label.color=!claimed&&ready?Cream:Ink;
                });
            }
        }
    }
}
