namespace Common.Api.Ads
{
    public class AdsPlacementReward
    {
        public string placement;
        
        public string name;
        
        public int amount;

        public AdsPlacementReward(string placement, string name, int amount)
        {
            this.placement = placement;
            this.name = name;
            this.amount = amount;
        }
    }
}