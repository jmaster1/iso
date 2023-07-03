using System;
using System.Collections.Generic;
using Common.Api.Info;
using Common.ContextNS;
using Common.Util;

namespace Common.Api.Billing
{
    /// <summary>
    /// Api for in-app Purchasing
    /// </summary>
    public class BillingApi : AbstractApi
    {
        public event Action<Sku, bool> OnPurchaseEvent;

        public bool NeedSkipConsumed = false;

        protected readonly InfoSetIdString<SkuInfo> SkuInfoSet = Context.GetInfoSetIdString<SkuInfo>("skus");

        protected readonly List<Sku> SkuInstances = new List<Sku>();
        
        protected readonly List<PurchaseResult> PendingPurchases = new List<PurchaseResult>();
        
        private Action<PurchaseResult> _pendingProductCallback = null;

        /// <summary>
        /// sku being purchased
        /// </summary>
        protected Sku PurchasingSku = null;

        /// <summary>
        /// this should be used by billing clients to obtain sku instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onPurchaseSuccess"></param>
        /// <returns></returns>
        public Sku GetSku(string id, Action onPurchaseSuccess)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("GetSku({0})", id);
            SkuInfo info = SkuInfoSet.GetById(id);
            Sku sku = new Sku();
            sku.Manager = this;
            sku.Id = id;
            sku.OnPurchaseSuccess = onPurchaseSuccess;
            SkuInstances.Add(sku);
            return sku;
        }

        protected void HandlePendingPurchases()
        {
            if (_pendingProductCallback == null || PendingPurchases.Count == 0) return;

            foreach (var pendingPurchase in PendingPurchases)
            {
                ConfirmPendingPurchase(pendingPurchase);
            }
            AllPendingPurchasesResolvedNotify();
        }
        
        void ValidateSkuResult(Sku sku, SkuState state, object result)
        {
            LangHelper.Validate(sku != null);
            LangHelper.Validate(sku.State.Is(state));
            LangHelper.Validate(result != null);
        }
        
        internal void Fetch(Sku sku)
        {
            if (sku.IsFetched || sku.IsFetching) return;
            if (Log.IsDebugEnabled) Log.DebugFormat("Fetch({0})", sku.Id);
            sku.State.Set(SkuState.Fetching);
            FetchInternal(sku);
        }

        /// <summary>
        /// subclasses should implement info fetch for given Sku,
        /// eventually OnFetchResult must be invoked,
        /// this implementation will provide default SkuInfo from config
        /// </summary>
        protected virtual void FetchInternal(Sku sku)
        {
            FetchResult result = new FetchResult();
            result.Info = SkuInfoSet.GetById(sku.Id);
            OnFetchResult(sku, result);
        }
        
        /// <summary>
        /// Call for handling non-consumed purchases. Must call once. Callback provide sku id.
        /// </summary>
        /// <param name="pendingPurchaseResolveCallback"></param>
        public void CheckForPendingPurchases(Action<PurchaseResult> pendingPurchaseResolveCallback)
        {
            LangHelper.Validate(pendingPurchaseResolveCallback != null);
            LangHelper.Validate(_pendingProductCallback == null);

            _pendingProductCallback = pendingPurchaseResolveCallback;
            HandlePendingPurchases();
        }

        private void ConfirmPendingPurchase(PurchaseResult pendingPurchaseResult)
        {
            LangHelper.Validate(_pendingProductCallback != null);
            _pendingProductCallback.Invoke(pendingPurchaseResult);
            ConfirmPendingPurchaseInternal(pendingPurchaseResult);
        }

        /// <summary>
        /// subclasses should implement this method for confirm pending purchase  
        /// </summary>
        /// <param name="id"></param>
        protected virtual void ConfirmPendingPurchaseInternal(PurchaseResult pendingPurchaseResult) {} 

        /// <summary>
        /// subclasses should call this method when all pending purchases resolved
        /// </summary>
        protected void AllPendingPurchasesResolvedNotify()
        {
            _pendingProductCallback = null;
            PendingPurchases.Clear();
        }

        /// <summary>
        /// should be called as result of FetchInternal() call
        /// </summary>
        protected void OnFetchResult(Sku sku, FetchResult result)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("OnFetchResult({0}, {1})", sku.Id, result);
            ValidateSkuResult(sku, SkuState.Fetching, result);
            if (result.Info != null)
            {
                sku.Info = result.Info;
                sku.State.Set(SkuState.Ready);
            }
            else
            {
                sku.State.Set(SkuState.Draft);
            }
        }

        internal void Purchase(Sku sku)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("Purchase({0})", sku.Id);
            LangHelper.Validate(PurchasingSku == null);
            LangHelper.Validate(sku.IsReady);
            sku.State.Set(SkuState.Purchasing);
            PurchasingSku = sku;
            PurchaseInternal();
        }

        internal void PurchaseSafety(Sku sku)
        {
            if (!sku.IsReady || PurchasingSku != null)
            {
                PurchaseResult purchaseResult = new PurchaseResult();
                purchaseResult.Status = PurchaseStatus.PaymentDeclined;
                sku.PurchaseResult = purchaseResult;
                OnPurchaseEvent?.Invoke(sku, false);
                return;
            }
            Purchase(sku);
        }

        /// <summary>
        /// subclasses should implement purchase flow,
        /// eventually OnPurchaseResult must be invoked,
        /// this implementation will show DialogMessage with ok/cancel/error actions
        /// </summary>
        protected virtual void PurchaseInternal()
        {
            var sku = PurchasingSku.Info;
            new DialogMessage()
            {
                Title = sku.Id,
                Message = String.Format("{0}\n{1}\n{2} ({3} {4})", 
                    sku.title,
                    sku.description, 
                    sku.priceFormatted, 
                    sku.price,
                    sku.priceCurrencyCode)
            }
                .AddAction("Ok", () => OnPurchaseResult(PurchaseStatus.Success))
                .AddAction("Cancel", () => OnPurchaseResult(PurchaseStatus.UserCancelled))
                .AddAction("Error", () => OnPurchaseResult(PurchaseStatus.Error))
                .Show();
        }

        /// <summary>
        /// subclasses should implement initialize flow,
        /// this implementation intently do nothing
        /// </summary>
        public virtual void Initialize() {}

        /// <summary>
        /// subclasses should implement initialize flow,
        /// this implementation intently do nothing
        /// </summary>
        /// <returns>ask the question "Is initialized?"</returns>
        public virtual bool IsInitialized()
        {
            return true;
        }

        /// <summary>
        /// should be called as result of PurchaseInternal() call
        /// </summary>
        protected void OnPurchaseResult(PurchaseResult result)
        {
            Sku sku = PurchasingSku;
            try
            {
                if (Log.IsDebugEnabled) Log.DebugFormat("OnPurchaseResult({0}, {1})", sku.Id, result);
                LangHelper.Validate(sku != null);
                ValidateSkuResult(sku, SkuState.Purchasing, result);
                sku.PurchaseResult = result;
                if (result.IsSuccess && !NeedSkipConsumed)
                {
                    sku.OnPurchaseSuccess();
                }
                OnPurchaseEvent?.Invoke(sku, NeedSkipConsumed);

                sku.State.Set(SkuState.Ready);
            }
            finally
            {
                PurchasingSku = null;
            }
        }

        protected void OnPurchaseResult(PurchaseStatus status)
        {
            PurchaseResult result = new PurchaseResult();
            result.Status = status;
            OnPurchaseResult(result);
        }

        public SkuInfo GetDefaultSkuInfo(string id)
        {
            return SkuInfoSet.GetById(id);
        }

        /// <summary>
        /// utility method for filtering "SkuInstances" field
        /// </summary>
        protected List<Sku> FindSkuInstancesWithState(SkuState state)
        {
            return SkuInstances.FindAll(sku => sku.State.Get() == state);
        }
    }
}
