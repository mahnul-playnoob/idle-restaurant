using System;
using UnityEngine;
namespace IdleRestaurant
{
    public enum RestaurantGoalKind { Guests, Services, OrdersUpgrades, KitchenUpgrades, DiningUpgrades, AllUpgrades, Outlets, Teammates, Training, Research, Coins, Stars }
    [Serializable] public sealed class RestaurantDistrict
    {
        public string name, shortName, description, landmark;
        public Color ink, paper, accent;
        public GameObject decoration;
    }
    [Serializable] public sealed class RestaurantGoalDefinition
    {
        public string title;
        public RestaurantGoalKind kind;
        public double firstTarget = 1, extraPerBook = 1;
        public int tickets = 10;
    }
    [CreateAssetMenu(menuName="Restaurant/Journey and Goals")]
    public sealed class RestaurantJourney : ScriptableObject
    {
        [Range(1,10)]public float nightTransitionSeconds=4;
        public int[] visualUpgradeMilestones={5,15,30};
        public RestaurantDistrict[] districts;
        public RestaurantGoalDefinition[] goals;
        static RestaurantJourney fallback;
        public static RestaurantJourney Default { get { if(!fallback){fallback=CreateInstance<RestaurantJourney>();fallback.hideFlags=HideFlags.HideAndDontSave;fallback.SetDefaults();}return fallback;} }
        public void SetDefaults()
        {
            districts = new[] {
                District("Street Kitchens","STREET","Open-air counters, sunny awnings and lively street food.","MARKET SQUARE","#4D5737","#F5E7C8","#E6AC3C"),
                District("Comfort Diners","DINER","Checkerboard floors, warm booths and comforting favourites.","DINER AVENUE","#28575D","#DCEDE8","#DF9A63"),
                District("Asian Kitchens","ASIAN","Lantern-lit kitchens, bamboo screens and steaming bowls.","LANTERN COURT","#66463E","#F4E1CA","#CF9750"),
                District("Mediterranean Bistros","BISTRO","Blue tiles, olive trees and relaxed terrace dining.","OLIVE PROMENADE","#285C74","#DEEAF0","#9CAC62"),
                District("Dessert Cafes","CAFE","Berry colours, cake displays and a sweet garden atmosphere.","BERRY GARDENS","#724C6B","#F3E0EB","#C793AF")
            };
            goals = new[] {
                Goal("Welcome happy guests",RestaurantGoalKind.Guests,1,10), Goal("Finish a service",RestaurantGoalKind.Services,1,2),
                Goal("Improve the order counter",RestaurantGoalKind.OrdersUpgrades,1,2),Goal("Improve your kitchen",RestaurantGoalKind.KitchenUpgrades,1,2),
                Goal("Improve the dining room",RestaurantGoalKind.DiningUpgrades,1,2),Goal("Make restaurant improvements",RestaurantGoalKind.AllUpgrades,5,5),
                Goal("Open another restaurant",RestaurantGoalKind.Outlets,1,0),Goal("Welcome a teammate",RestaurantGoalKind.Teammates,1,0),
                Goal("Train your team",RestaurantGoalKind.Training,1,1),Goal("Learn a kitchen skill",RestaurantGoalKind.Research,1,1),
                Goal("Earn coins by serving",RestaurantGoalKind.Coins,150,100),Goal("Keep the service flowing",RestaurantGoalKind.Services,5,3)
            };
        }
        static RestaurantDistrict District(string name,string shortName,string desc,string landmark,string ink,string paper,string accent) => new RestaurantDistrict{name=name,shortName=shortName,description=desc,landmark=landmark,ink=ColorOf(ink),paper=ColorOf(paper),accent=ColorOf(accent)};
        static Color ColorOf(string value){ColorUtility.TryParseHtmlString(value,out var c);return c;}
        static RestaurantGoalDefinition Goal(string title,RestaurantGoalKind kind,double target,double growth)=>new RestaurantGoalDefinition{title=title,kind=kind,firstTarget=target,extraPerBook=growth,tickets=10};
    }
}
