namespace Common.Api.Billing
{
    /// <summary>
    /// represents sku purchase flow result
    /// </summary>
    public class PurchaseResult
    {
        /// <summary>
        /// Status of purchase, required
        /// </summary>
        public PurchaseStatus Status;

        /// <summary>
        /// Error message, optional
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// SkuInfo payload, optional
        /// </summary>
        public SkuInfo SkuInfo;
        
        public bool IsSuccess => Status == PurchaseStatus.Success;
    }
}