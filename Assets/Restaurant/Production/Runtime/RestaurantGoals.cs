using System;
namespace IdleRestaurant
{
    public sealed partial class RestaurantGame
    {
        public RestaurantJourney Journey => Catalog.journey ? Catalog.journey : RestaurantJourney.Default;
        public bool LegacyGoalBoard => State.goalBoardVersion == 0 && (State.taskClaimed != 0 || TotalWaves() > 0);
        public void EnsureGoalBoard()
        {
            if(LegacyGoalBoard)return; // Honor every claimed reward and remaining target on a partially completed old board.
            if(State.goalBoardVersion==1 && State.goalKinds?.Length==12 && State.goalTargets?.Length==12 && State.goalBaselines?.Length==12)return;
            CreateGoalBoard();
        }
        void CreateGoalBoard()
        {
            State.goalBoardVersion=1;State.goalKinds=new int[12];State.goalTargets=new double[12];State.goalBaselines=new double[12];
            for(int i=0;i<12;i++){
                var def=Journey.goals[i];var kind=def.kind;
                double needed=Math.Max(1,def.firstTarget+def.extraPerBook*State.taskBook),current=GoalMetric(kind),remaining=RemainingGoalCapacity(kind);
                if(kind==RestaurantGoalKind.Coins)needed*=Math.Pow(Balance.outletIncomeGrowth,State.selected);
                if(remaining<1){kind=RestaurantGoalKind.Services;needed=2+State.taskBook;current=GoalMetric(kind);}
                else needed=Math.Min(needed,remaining);
                State.goalKinds[i]=(int)kind;State.goalTargets[i]=needed;State.goalBaselines[i]=current;
            }
        }
        double RemainingGoalCapacity(RestaurantGoalKind kind)
        {
            double max=double.PositiveInfinity;
            switch(kind){
                case RestaurantGoalKind.Outlets:max=State.outlets.Length;break;
                case RestaurantGoalKind.Teammates:max=State.managerCopies.Length;break;
                case RestaurantGoalKind.Training:max=14*State.managerCopies.Length;break;
                case RestaurantGoalKind.Research:max=0;foreach(var d in Catalog.research)max+=d.max;break;
                case RestaurantGoalKind.OrdersUpgrades:case RestaurantGoalKind.KitchenUpgrades:case RestaurantGoalKind.DiningUpgrades:max=State.outlets.Length*3*Catalog.maxUpgrade;break;
                case RestaurantGoalKind.AllUpgrades:max=State.outlets.Length*9*Catalog.maxUpgrade;break;
            }
            return Math.Max(0,max-GoalMetric(kind));
        }
        public double GoalMetric(RestaurantGoalKind kind)
        {
            double value=0;
            switch(kind){
                case RestaurantGoalKind.Guests:return State.guestsServed;
                case RestaurantGoalKind.Services:return TotalWaves();
                case RestaurantGoalKind.Coins:return State.totalEarned;
                case RestaurantGoalKind.Outlets:foreach(var s in State.outlets)if(s.open)value++;return value;
                case RestaurantGoalKind.Teammates:foreach(var n in State.managerCopies)if(n>0)value++;return value;
                case RestaurantGoalKind.Training:for(int i=0;i<State.managerLevels.Length;i++)if(State.managerCopies[i]>0)value+=Math.Max(0,State.managerLevels[i]-1);return value;
                case RestaurantGoalKind.Research:foreach(var s in State.research)value+=s.level;return value;
                case RestaurantGoalKind.Stars:foreach(var s in State.outlets)value+=s.rank;return value;
                default:
                    int first=kind==RestaurantGoalKind.OrdersUpgrades?0:kind==RestaurantGoalKind.KitchenUpgrades?3:kind==RestaurantGoalKind.DiningUpgrades?6:0;
                    int last=kind==RestaurantGoalKind.AllUpgrades?9:first+3;
                    foreach(var s in State.outlets)for(int c=first;c<last;c++)value+=s.upgrades[c];return value;
            }
        }
        public RestaurantGoalKind GoalKind(int index){EnsureGoalBoard();return LegacyGoalBoard?RestaurantGoalKind.Services:(RestaurantGoalKind)State.goalKinds[index];}
        public double GoalTarget(int index){EnsureGoalBoard();return LegacyGoalBoard?(State.taskBook*12+index+1)*5:State.goalTargets[index];}
        public double GoalProgress(int index){EnsureGoalBoard();return LegacyGoalBoard?TotalWaves():Math.Max(0,GoalMetric(GoalKind(index))-State.goalBaselines[index]);}
        public string GoalTitle(int index){return LegacyGoalBoard?"Complete service waves":GoalKind(index)==Journey.goals[index].kind?Journey.goals[index].title:"Keep welcoming guests through service";}
        public bool GoalClaimed(int index)=>(State.taskClaimed&(1<<index))!=0;
        public int GoalReward(int index)=>LegacyGoalBoard?10:Journey.goals[index].tickets;
        public bool ClaimGoal(int index)
        {
            if(index<0||index>=12)return false;
            EnsureGoalBoard();if(GoalClaimed(index)||GoalProgress(index)<GoalTarget(index))return false;
            int reward=GoalReward(index);State.taskClaimed|=1<<index;State.gems+=reward;
            Feedback?.Invoke("Goal complete! +"+reward+" tickets");
            if(State.taskClaimed==4095){State.taskBook++;State.taskClaimed=0;CreateGoalBoard();}
            return true;
        }
    }
}
