using System;

namespace Core.Notification
{
    public interface INotificationManager
    {
        event EventHandler NotificationReceived;

        void Initialize();

        void SendNotification(string title, string message, int timeToClose, bool autoCancel);

        void ReceiveNotification(string title, string message);
    }
}
