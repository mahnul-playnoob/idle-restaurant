using System.Linq;
using TMPro;
using UnityEngine;
namespace IdleRestaurant
{
    public sealed partial class RestaurantApp
    {
        void BuildProgressiveTeam()
        {
            if(!Game.State.outlets[1].open){var locked=PageTitle("Your hospitality team","Your first teammate joins when your second restaurant opens.");Button(locked,"OPEN MAP",new Vector2(.15f,.4f),new Vector2(.85f,.55f),()=>Navigate("map"),Green);return;}
            var p=PageTitle("Your hospitality team","Welcome one person at a time. Each brings their own skills.");
            var content=Scroll(p,catalog.managers.Length+1,210);var pack=ListRow(content,0,210);pack.name="Next teammate";
            var nextTitle=Text(pack,"",new Vector2(.03f,.62f),new Vector2(.97f,.96f),27,Ink);nextTitle.fontStyle=FontStyles.Bold;
            var detail=Text(pack,"",new Vector2(.03f,.05f),new Vector2(.71f,.61f),19,Muted);
            var recruit=Button(pack,"WELCOME",new Vector2(.74f,.11f),new Vector2(.98f,.53f),()=>{if(Game.Recruit(0)){Sound(upgrade);Navigate("managers");}else if(!Game.CanRecruit(out var reason))Notify(reason);},Green,17);recruit.name="Welcome next manager";
            refresh.Add(()=>{int id=Game.NextRecruitId();bool ready=Game.CanRecruit(out var reason);nextTitle.text=id<0?"Your team is complete":"Next: "+catalog.managers[id].title;detail.text=id<0?"Train your favourites with earned tickets.":Game.RecruitPrice+" tickets · "+Game.TotalWaves()+" / "+Game.RecruitServicesRequired+" services\n"+Game.State.outlets.Count(x=>x.open)+" / "+Game.RecruitOutletsRequired+" restaurants · "+(ready?"Ready to welcome":reason);recruit.interactable=ready;recruit.GetComponent<UnityEngine.UI.Image>().color=ready?UpgradeGreen:Unavailable;recruit.GetComponentInChildren<TMP_Text>().color=ready?Cream:Ink;});
            for(int i=0;i<catalog.managers.Length;i++){
                int id=i;var d=catalog.managers[id];var row=ListRow(content,i+1,210);row.name="Manager row "+id;
                var portrait=Rect("Manager portrait "+id,row,new Vector2(.02f,.18f),new Vector2(.20f,.89f)).gameObject.AddComponent<ManagerPortrait>();portrait.person=id;portrait.role=d.role;portrait.raycastTarget=false;
                var name=Text(row,d.title,new Vector2(.23f,.64f),new Vector2(.72f,.96f),25,Ink);name.enableAutoSizing=true;name.fontSizeMin=20;name.fontSizeMax=25;
                var status=Text(row,"",new Vector2(.23f,.05f),new Vector2(.71f,.65f),18,Muted);
                var assign=Button(row,"ASSIGN",new Vector2(.75f,.52f),new Vector2(.98f,.92f),()=>{if(Game.Assign(id,d.role)){Sound(upgrade);Refresh();}},Green,16);assign.name="Assign manager "+id;
                var train=Button(row,"TRAIN",new Vector2(.75f,.08f),new Vector2(.98f,.47f),()=>{if(Game.LevelManager(id)){Sound(upgrade);Refresh();}},Peach,16);train.name="Train manager "+id;
                refresh.Add(()=>{bool owned=Game.State.managerCopies[id]>0;bool assigned=Game.State.outlets[Game.State.selected].managers[d.role]==id;assign.interactable=owned;assign.GetComponentInChildren<TMP_Text>().text=assigned?"ASSIGNED":"ASSIGN";train.interactable=owned&&Game.State.managerLevels[id]<15&&Game.State.gems>=Game.ManagerNeed(id);string role=new[]{"Orders","Kitchen","Dining"}[d.role];status.text=role+" · "+new[]{"Common","Rare","Epic","Legendary","Mythic"}[d.rarity]+"\n"+(owned?"Lv. "+Game.State.managerLevels[id]+(Game.State.managerLevels[id]>=15?" · MAX":" · Train: "+Game.ManagerNeed(id)+" tickets"):id==Game.NextRecruitId()?"NEXT TEAMMATE":"Available later in your journey");});
            }
        }
    }
}
