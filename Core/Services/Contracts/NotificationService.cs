using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Common;
using Timer = Core.Utils.Timer;

namespace Core.Services.Contracts
{
    public class NotificationService : INotificationService
    {
        // Events
        public event EventHandler<bool> NotificationsUpdated;

        // Fields
        private readonly ApplicationData appData;
        private readonly Timer timer = new Timer(3 * 60);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool isRunning;

        // Services
        private readonly IPrometeoApiService prometeoApiService;

        public NotificationService(ApplicationData appData, IPrometeoApiService prometeoApiService)
        {
            this.appData = appData;
            this.prometeoApiService = prometeoApiService;
        }

        public void StartListeningNotifications()
        {
            if (isRunning)
            {
                return;
            }

            CheckUnreadNotifications();
            
            timer.TimeElapsed += async (sender, args) => await CheckUnreadNotifications();
            timer.StartAsync(cancellationTokenSource.Token);

            isRunning = true;
        }

        public void StopListeningNotifications()
        {
            cancellationTokenSource.Cancel();
            isRunning = false;
        }

        private async Task CheckUnreadNotifications()
        {
            try
            {
                if (appData.LoggedUser == null || string.IsNullOrWhiteSpace(appData.LoggedUser.Token))
                {
                    return;
                }

                var requestData = new NotificationsPaginatedRequest()
                {
                    CurrentPage = 1,
                    PageSize = 1,
                    Viewed = false,
                };
                PaginatedList<TreatmentAlert> notifications = await prometeoApiService.GetAllNotifications(requestData);

                bool hasUnreadNotifications = notifications.TotalCount > 0;
                NotificationsUpdated?.Invoke(this, hasUnreadNotifications);
            }
            catch (Exception)
            {
                // TODO: no me importa por ahora si hubo error acá...
            }
        }
    }
}
