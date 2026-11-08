using UnityEngine;
using RestaurantPreview;
namespace IdleRestaurant
{
    public sealed class RestaurantUpgradeVisuals:MonoBehaviour
    {
        RestaurantGame game;int outlet;readonly int[] tiers={-1,-1,-1};readonly Transform[] groups=new Transform[3];
        public int Tier(int stage)=>tiers[stage];
        public void Setup(RestaurantGame g,int index){game=g;outlet=index;Refresh();}
        int VisualTier(int levels){var m=game.Journey.visualUpgradeMilestones;return levels>=m[2]?3:levels>=m[1]?2:levels>=m[0]?1:0;}
        void Update(){if(game!=null)Refresh();}
        void Refresh(){for(int stage=0;stage<3;stage++){int n=0;for(int c=0;c<3;c++)n+=game.State.outlets[outlet].upgrades[stage*3+c];int tier=VisualTier(n);if(tier==tiers[stage])continue;tiers[stage]=tier;if(groups[stage]){groups[stage].gameObject.SetActive(false);Destroy(groups[stage].gameObject);}groups[stage]=Models.Group(new[]{"Orders","Kitchen","Dining"}[stage]+" improvements tier "+tier,transform,Vector3.zero);Build(stage,tier,groups[stage]);RestaurantWorld.CombineStatic(groups[stage]);groups[stage].gameObject.AddComponent<RestaurantActorMarker>();}}
        void Build(int stage,int tier,Transform r)
        {
            if(tier==0)return;
            if(stage==0){
                Models.Box(r,new Vector3(-2.8f,.70f,3.36f),new Vector3(1.75f,.17f,.035f),"#E3BF6D");
                Models.Box(r,new Vector3(-2.0f,1.42f,4.04f),new Vector3(.40f,.45f,.10f),"#426659");Models.Box(r,new Vector3(-2.0f,1.43f,3.98f),new Vector3(.29f,.32f,.02f),"#D9E8BE");
                if(tier>=2){Models.Box(r,new Vector3(-3.55f,1.45f,4.1f),new Vector3(.42f,.55f,.12f),"#A78151");for(int k=0;k<3;k++)Models.Box(r,new Vector3(-3.55f,1.58f-k*.13f,4.03f),new Vector3(.30f,.035f,.02f),"#FFF0D0");}
                if(tier>=3){Models.Plant(r,new Vector3(-1.98f,1.25f,4.36f),.35f);Models.Box(r,new Vector3(-2.8f,.40f,3.36f),new Vector3(1.75f,.30f,.04f),"#456F5B");}
            }
            else if(stage==1){
                for(int i=0;i<2;i++){float x=-.4f+i*2.4f;Models.Box(r,new Vector3(x+.46f,1.23f,4.0f),new Vector3(.52f,.05f,.40f),"#C69B62");for(int k=0;k<3;k++)Models.Ball(r,new Vector3(x+.32f+k*.12f,1.30f,4.0f),Vector3.one*.11f,k==0?"#A9B872":"#C37A55");
                    if(tier>=2){Models.Cylinder(r,new Vector3(x-.40f,1.35f,4.0f),.30f,.15f,"#B88C55");Models.Box(r,new Vector3(x,1.0f,3.36f),new Vector3(1.6f,.08f,.035f),"#E5CC8B");}
                    if(tier>=3){Models.Box(r,new Vector3(x+.65f,1.5f,4.45f),new Vector3(.46f,.55f,.35f),"#5E7E79");Models.Cylinder(r,new Vector3(x+.65f,1.86f,4.45f),.18f,.20f,"#A8C0AC");}}
            }
            else{
                for(int z=0;z<3;z++)for(int x=0;x<2;x++){
                    var centre=new Vector3(-2.15f+x*4.3f,0,-2.5f+z*2);
                    Models.Box(r,centre+new Vector3(0,.824f,0),new Vector3(.90f,.02f,.72f),outlet/8==4?"#F2CFDF":"#F5E8CF");
                    if(tier>=2){Models.Cylinder(r,centre+new Vector3(0,.94f,.20f),.075f,.20f,"#849D8C");Models.Ball(r,centre+new Vector3(0,1.09f,.20f),Vector3.one*.16f,"#E4BB76");}
                    if(tier>=3)for(int side=-1;side<=1;side+=2)Models.Box(r,centre+new Vector3(side*.35f,.838f,0),new Vector3(.38f,.008f,.44f),"#B7C5A2");
                }
            }
        }
    }
}
