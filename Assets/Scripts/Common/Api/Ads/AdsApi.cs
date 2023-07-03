using System;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Observable;
using Common.Util;
using Common.Util.Http;

namespace Common.Api.Ads
{
    /// <summary>
    /// abstraction layer for rewarded video
    /// </summary>
    public class AdsApi : AbstractApi
    {
        public Events<AdsApiEvent, AdsApi> Events = new Events<AdsApiEvent, AdsApi>();
        
        /// <summary>
        /// shows whether rewarded video available
        /// </summary>
        public readonly BoolHolder RewardedVideoAvailable = new BoolHolder(true);
        
        /// <summary>
        /// shows whether interstitial available
        /// </summary>
        public readonly BoolHolder InterstitialAvailable = new BoolHolder(true);

        /// <summary>
        /// override default video availability flag (debug purpose)
        /// </summary>
        bool? RewardedVideoAvailableOverride;

        /// <summary>
        /// current placement being served 
        /// </summary>
        public AdsPlacement Placement;

        /// <summary>
        /// last error
        /// </summary>
        public AdsPlacementError Error;

        /// <summary>
        /// last reward
        /// </summary>
        public AdsPlacementReward Reward;

        public string AppKey;
        
        public string UserId;

        /// <summary>
        /// implement to init ads provider
        /// </summary>
        /// <param name="appKey">ads provider key</param>
        /// <param name="userId">player id</param>
        public virtual void Init(string appKey, string userId, string InterstitialUnitId = null, string RewardedUnitId = null)
        {
            AppKey = appKey;
            UserId = userId;
        }

        /// <summary>
        /// check if rewarded video available
        /// </summary>
        public virtual bool IsRewardedVideoAvailable()
        {
            if (RewardedVideoAvailableOverride != null) return (bool) RewardedVideoAvailableOverride;
            return true;
        }
        public virtual bool IsInterstitialAvailable()
        {
            if (RewardedVideoAvailableOverride != null) return (bool) RewardedVideoAvailableOverride;
            return true;
        }

        [HttpInvoke]
        public void SetVideoAvailableTrue()
        {
            RewardedVideoAvailableOverride = true;
        }
        
        [HttpInvoke]
        public void SetVideoAvailableFalse()
        {
            RewardedVideoAvailableOverride = false;
        }
        
        [HttpInvoke]
        public void SetVideoAvailableDefault()
        {
            RewardedVideoAvailableOverride = null;
        }
        
        /// <summary>
        /// show rewarded video for placement
        /// </summary>
        public virtual bool ShowRewardedVideo(string placement)
        {
            if (!RewardedVideoAvailable.Get()) return false;
            DialogMessage dm = new DialogMessage()
            {
                Title = "Rewarded video",
                Message = $"Placement = {placement}",
            };
            dm.AddAction("Reward", () =>
            {
                AdsPlacementReward reward = new AdsPlacementReward(placement, "reward", 1);
                OnRewardedVideoAdRewarded(reward);
                OnRewardedVideoAdClosed();
            });
            dm.AddAction("Cancel", OnRewardedVideoAdClosed);
            dm.Show();
            return true;
        }
        
        public virtual bool ShowInterstitial(string placement)
        {
            if (!InterstitialAvailable.Get()) return false;
            DialogMessage dm = new DialogMessage()
            {
                Title = "Interstitial",
                Message = $"Placement = {placement}",
            };
            dm.AddAction("Reward", () =>
            {
                AdsPlacementReward reward = new AdsPlacementReward(placement, "reward", 1);
                OnRewardedVideoAdRewarded(reward);
                OnRewardedVideoAdClosed();
            });
            dm.AddAction("Cancel", OnRewardedVideoAdClosed);
            dm.Show();
            return true;
        }
        
        void Invoke(Action action)
        {
            if (action != null) action();
        }
        
        public bool ShowRewardedVideo(AdsPlacement placement)
        {
            Placement = placement;
            return ShowRewardedVideo(placement.Placement);
        }

        public bool ShowInterstitial(AdsPlacement placement)
        {
            Placement = placement;
            return ShowInterstitial(placement.Placement);
        }

        protected void OnRewardedVideoAdEnded()
        {
            Fire(AdsApiEvent.AdEnded);
            Invoke(Placement?.OnRewardedVideoAdEnded);
        }

        protected void OnRewardedVideoAdStarted()
        {
            Fire(AdsApiEvent.AdStarted);
            Invoke(Placement?.OnRewardedVideoAdStarted);
        }

        protected void OnRewardedVideoAvailabilityChanged(bool available)
        {
            if (RewardedVideoAvailableOverride != null) available = (bool) RewardedVideoAvailableOverride;
            if (RewardedVideoAvailable.Get() != available)
            {
                RewardedVideoAvailable.Set(available);
                Fire(AdsApiEvent.VideoAvailabilityChanged);
            }
        }
        
        protected void OnInterstitialAvailabilityChanged(bool available)
        {
            if (InterstitialAvailable.Get() != available)
            {
                InterstitialAvailable.Set(available);
                Fire(AdsApiEvent.InterstitialAvailabilityChanged);
            }
        }

        protected void OnRewardedVideoAdClosed()
        {
            Fire(AdsApiEvent.AdClosed);
            Invoke(Placement?.OnRewardedVideoAdClosed);
        }

        protected void OnRewardedVideoAdOpened()
        {
            Fire(AdsApiEvent.AdOpened);
            Invoke(Placement?.OnRewardedVideoAdOpened);
        }

        protected void OnRewardedVideoAdShowFailed(AdsPlacementError error)
        {
            Error = error;
            Fire(AdsApiEvent.AdShowFailed);
            Invoke(Placement?.OnRewardedVideoAdShowFailed);
        }
        protected void OnInterstitialAdShowFailed(AdsPlacementError error)
        {
            Error = error;
            Invoke(Placement?.OnRewardedVideoAdShowFailed);
        }

        protected void OnRewardedVideoAdRewarded(AdsPlacementReward reward)
        {
            Placement.Reward = reward;
            Placement.OnRewarded();
        }

        protected void OnRewardedVideoAdClicked(AdsPlacementReward reward)
        {
            Reward = reward;
            Fire(AdsApiEvent.AdClicked);
            Invoke(Placement?.OnRewardedVideoAdClicked);
        }
        
        void Fire(AdsApiEvent type)
        {
            Events.Fire(type, this);
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.propertyTable(
                "AppKey", AppKey,
                "UserId", UserId,
                "RewardedVideoAvailable", RewardedVideoAvailable,
                "RewardedVideoAvailableOverride", RewardedVideoAvailableOverride,
                "Placement", Placement, 
                "Reward", Reward, 
                "Error", Error);
        }

        /// <summary>
        /// called externally on application paused change
        /// </summary>
        public virtual void OnApplicationPause(bool pause)
        {
        }

        public virtual void LoadInterstitial()
        {
            
        }
    }
}
