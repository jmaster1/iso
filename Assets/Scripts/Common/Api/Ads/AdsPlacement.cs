using System;
using System.Collections.Generic;
using System.Text;
using Common.Bind;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Observable;
using Common.Util.Http;

namespace Common.Api.Ads
{
    /// <summary>
    /// AdsApi proxy for named placement,
    /// contains set of configurable actions to be invoked on particular event
    /// with active placement
    /// </summary>
    public abstract class AdsPlacement : BindableBean<AdsApi>
    {
        public string Placement;
        
        /// <summary>
        /// avaliable holder, this is evaluated as RewardedVideoAvailable && Enabled
        /// </summary>
        public BoolHolder Available = new BoolHolder();
        
        /// <summary>
        /// arbitrary criterias that affect Available
        /// </summary>
        public readonly SortedDictionary<string, Func<bool>> Criterias = new SortedDictionary<string, Func<bool>>();
        
        public Action OnRewardedVideoAdRewarded;
        
        public Action OnRewardedVideoAdClosed;

        public Action OnRewardedVideoAdOpened;

        public Action OnRewardedVideoAdShowFailed;

        public Action OnRewardedVideoAdClicked;

        public Action OnRewardedVideoAdEnded;
        
        public Action OnRewardedVideoAdStarted;

        /// <summary>
        /// last error
        /// </summary>
        public AdsPlacementError Error;
        
        /// <summary>
        /// last reward
        /// </summary>
        public AdsPlacementReward Reward;

        /// <summary>
        /// context payload
        /// </summary>
        public object Payload;

        public abstract bool ShowAds(object payload = null);
        
        protected override void OnBind()
        {

        }

        [HttpInvoke]
        public void Enable()
        {
            Available.SetTrue();
        }
        
        [HttpInvoke]
        public void Disable()
        {
            Available.SetFalse();
        }

        public abstract bool UpdateAvailable();
        
        [HttpInvoke]
        public bool VideoUpdateAvailable()
        {
            bool avail = Model.RewardedVideoAvailable.Get();
            if (avail)
            {
                foreach (Func<bool> criteria in Criterias.Values)
                {
                    if(!(avail &= criteria())) break;
                }
            }
            Available.Set(avail);
            return avail;
        }
        
        [HttpInvoke]
        public bool InterstitialUpdateAvailable()
        {
            bool avail = Model.InterstitialAvailable.Get();
            if (avail)
            {
                foreach (Func<bool> criteria in Criterias.Values)
                {
                    if(!(avail &= criteria())) break;
                }
            }
            Available.Set(avail);
            return avail;
        }

        public void SafeLoadInterstitial()
        {
            if (!Model.InterstitialAvailable.Get())
            {
                Model.LoadInterstitial();
            }
        }

        public bool ShowIfAvailable()
        {
            bool avail = UpdateAvailable();
            if(avail) avail = ShowAds();
            return avail;
        }

        /// <summary>
        /// add named availability criteria 
        /// </summary>
        public void AddCriteria(string name, Func<bool> func)
        {
            Criterias[name] = func;
        }
        
        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var e in Criterias)
            {
                bool val = e.Value.Invoke();
                sb.Append(e.Key).Append('=').Append(val).Append(", ");
            }
            html.propertyTable("Placement", Placement,
                "Available", Available,
                "RewardedVideoAvailable", Model.RewardedVideoAvailable,
                "Criterias", sb);
        }

        public virtual void OnRewarded()
        {
            OnRewardedVideoAdRewarded();
        }
    }
}
