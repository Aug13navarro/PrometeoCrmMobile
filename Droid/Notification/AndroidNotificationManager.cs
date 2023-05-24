using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Core.Notification;
using PrometeoCrmMobile.Droid.Notification;
using System;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(AndroidNotificationManager))]
namespace PrometeoCrmMobile.Droid.Notification
{
    public class AndroidNotificationManager : INotificationManager
        {

            const string channelId = "default";
            const string channelName = "Default";
            const string channelDescription = "the default channel for notification";

            public const string TitleKey = "title";
            public const string MessageKey = "message";

            bool channelInitialized = false;
            //int messageId = 0;
            int pendingIntentId = 0;

            NotificationManager manager;

            //==================================

            public event EventHandler NotificationReceived;

            public static AndroidNotificationManager Instace { get; private set; }

            public AndroidNotificationManager() => Initialize();


            public void Initialize()
            {
                if (Instace == null)
                {
                    CreateNotificationChannel();
                    Instace = this;
                }
            }

            public void ReceiveNotification(string title, string message)
            {
                var args = new NotificationEventArgs()
                {
                    Title = title,
                    Message = message,
                };
                NotificationReceived?.Invoke(null, args);
            }

            public void SendNotification(string title, string message, int timeToClose, bool autoCancel)
            {
                if (!channelInitialized)
                {
                    CreateNotificationChannel();
                }

                //if (notifyTime != null)
                //{
                //    Intent intent = new Intent(AndroidApp.Context, typeof(AlarmHandler));
                //    intent.PutExtra(TitleKey, title);
                //    intent.PutExtra(MessageKey, message);

                //    PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.CancelCurrent);
                //    long triggerTime = GetNotifyTime(notifyTime.Value);
                //    AlarmManager alarmManager = AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;
                //    alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
                //}
                //else
                //{
                Show(title, message, timeToClose, autoCancel);
                //}
            }

            public void Show(string title, string message, int timeToClose, bool autoCancel)
            {
                Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
                intent.PutExtra(TitleKey, title);
                intent.PutExtra(MessageKey, message);

                PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                    .SetContentIntent(pendingIntent)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    //.SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.icon))
                    .SetSmallIcon(Resource.Drawable.add)
                    .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                    .SetAutoCancel(autoCancel)
                    .SetTimeoutAfter(timeToClose);


                //Android.App.Notification notification = builder.Build();

                //manager.Notify(messageId++, builder.Build()); //== messageId es el valor de identificacion que toma la notificacion
                manager.Notify(1, builder.Build());
            }
            void CreateNotificationChannel()
            {
                manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelNameJava = new Java.Lang.String(channelName);
                    var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                    {
                        Description = channelDescription
                    };
                    manager.CreateNotificationChannel(channel);
                }

                channelInitialized = true;
            }

            long GetNotifyTime(DateTime notifyTime)
            {
                DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
                double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
                long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
                return utcAlarmTime; // milliseconds
            }
        }
    }