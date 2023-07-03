namespace Common.Api.Ads
{
    public class AdsPlacementError
    {
        public int code;
        
        public string description;

        public AdsPlacementError(int code, string description)
        {
            this.code = code;
            this.description = description;
        }
    }
}