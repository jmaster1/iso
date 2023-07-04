using Common.Api.Notification;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace Common.Unity.Api.Notification
{
    public class AndroidNotificationApi : NotificationApi
    {
        #if UNITY_ANDROID
        private const string ANDROID_DEFAULT_CHANNEL_ID = "default_channel_id";

        public AndroidNotificationApi() : base()
        {
            AndroidNotificationCenter.Initialize();
            var notificationChannel = new AndroidNotificationChannel()
            {
                Id = ANDROID_DEFAULT_CHANNEL_ID,
                Name = "Default Channel",
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
        }

        public override void CancelAllNotifications()
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public override void SubmitNotification(Common.Api.Notification.Notification notification)
        {
            var androidNotification = new AndroidNotification()
            {
                Title = notification.Title,
                Text = notification.Text,
                FireTime = notification.FireTime,
                RepeatInterval = notification.RepeatInterval,
                Color = notification.Color,
                LargeIcon = notification.LargeIcon,
                SmallIcon = notification.SmallIcon,
                Style = NotificationStyle.BigTextStyle,
            };
            AndroidNotificationCenter.SendNotification(androidNotification, ANDROID_DEFAULT_CHANNEL_ID);
        }
        #endif
    }
}
