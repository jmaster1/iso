namespace Common.Api.Billing
{
    /// <summary>
    /// known purchase statuses
    /// </summary>
    public enum PurchaseStatus
    {
        PurchasingUnavailable,
        ExistingPurchasePending,
        ProductUnavailable,
        SignatureInvalid,
        UserCancelled,
        PaymentDeclined,
        DuplicateTransaction,
        Unknown,
        IapSystemIsNotInitialized,
        SkuAlreadyInProgress,
        Success,
        Error
    }
}
