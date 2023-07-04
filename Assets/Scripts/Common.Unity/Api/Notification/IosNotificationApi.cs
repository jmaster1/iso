using System;
using Common.Api.Notification;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace Common.Unity.Api.Notification
{
    public class IosNotificationApi : NotificationApi
    {
        #if UNITY_IOS
        public override void CancelAllNotifications()
        {
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }

        public override void SubmitNotification(Common.Api.Notification.Notification notification)
        {
            // 
            // handle restriction "time interval must be greater than 0 seconds"
            TimeSpan oneSecondTimeSpan = TimeSpan.FromSeconds(1);
            TimeSpan notificationFireTimeInterval = notification.FireTime - DateTime.Now;
            TimeSpan fireTimeInterval = notificationFireTimeInterval > oneSecondTimeSpan ?
                notificationFireTimeInterval : oneSecondTimeSpan;
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = fireTimeInterval,
                Repeats = false
            };
            var iosNotification = new iOSNotification()
            {
                Title = notification.Title,
                Body = notification.Text,
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                Trigger = timeTrigger,
            };
            iOSNotificationCenter.ScheduleNotification(iosNotification);
        }
        #endif 
    }
}
