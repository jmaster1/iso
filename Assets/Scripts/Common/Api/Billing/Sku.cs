using System;
using Common.Lang;
using Common.Lang.Entity;
using Common.Lang.Observable;

namespace Common.Api.Billing
{
    /// <summary>
    /// in-app product data container/api,
    /// id matches product identifier
    /// </summary>
    public class Sku : ManagedEntity<BillingApi>
    {
        /// <summary>
        /// current state of Sku
        /// </summary>
        public readonly Holder<SkuState> State = new Holder<SkuState>();
        
        /// <summary>
        /// SkuInfo, valid once state becomes Ready
        /// </summary>
        public SkuInfo Info;

        /// <summary>
        /// this will be invoked on purchase success
        /// </summary>
        public Action OnPurchaseSuccess;

        /// <summary>
        /// last fetch result
        /// </summary>
        public FetchResult FetchResult;
            
        /// <summary>
        /// last purchase result
        /// </summary>
        public PurchaseResult PurchaseResult;

        public bool IsDraft => State.Is(SkuState.Draft);
        
        public bool IsFetching => State.Is(SkuState.Fetching);

        public bool IsFetched => Info != null;
        
        public bool IsReady => State.Is(SkuState.Ready);
        
        public bool IsPurchasing => State.Is(SkuState.Purchasing);

        /// <summary>
        /// retrieve SkuInfo from config
        /// </summary>
        public SkuInfo DefaultInfo => Manager.GetDefaultSkuInfo(Id);

        /// <summary>
        /// request to fetch sku info
        /// </summary>
        public void Fetch()
        {
            Manager.Fetch(this);
        }

        /// <summary>
        /// request to purchase
        /// </summary>
        public void Purchase()
        {
            Manager.Purchase(this);
        }

        public void PurchaseSafety()
        {
            Manager.PurchaseSafety(this);
        }
    }
}
