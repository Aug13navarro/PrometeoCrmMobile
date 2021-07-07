using System;

namespace Core.Services.Contracts
{
    public interface INotificationService
    {
        event EventHandler<bool> NotificationsUpdated;

        void StartListeningNotifications();
        void StopListeningNotifications();
    }
}
