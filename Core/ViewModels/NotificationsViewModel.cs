using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Common;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class NotificationsViewModel : MvxViewModel
    {
        // Properties
        private bool isLoadingInProgress;
        public bool IsLoadingInProgress
        {
            get => isLoadingInProgress;
            private set => SetProperty(ref isLoadingInProgress, value);
        }

        private bool error;
        public bool Error
        {
            get => error;
            private set => SetProperty(ref error, value);
        }

        public MvxObservableCollection<NotificationList> Notifications { get; } = new MvxObservableCollection<NotificationList>();

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public IMvxAsyncCommand LoadMoreNotificationsCommand { get; }
        public IMvxAsyncCommand<TreatmentAlert> MarkNotificacionAsReadCommand { get; }

        // Constants
        private const int PageSize = 25;

        // Services
        private readonly IPrometeoApiService prometeoApiService;

        public NotificationsViewModel(IPrometeoApiService prometeoApiService)
        {
            this.prometeoApiService = prometeoApiService;

            LoadMoreNotificationsCommand = new MvxAsyncCommand(LoadMoreNotificationsAsync);
            MarkNotificacionAsReadCommand = new MvxAsyncCommand<TreatmentAlert>(MarkNotificacionAsReadAsync);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var requestData = new NotificationsPaginatedRequest()
            {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                Viewed = true,
            };

            await SearchNotificationsAsync(requestData);
        }

        private async Task SearchNotificationsAsync(NotificationsPaginatedRequest requestData)
        {
            try
            {
                IsLoadingInProgress = true;
                Error = false;

                PaginatedList<TreatmentAlert> allNotifications = await prometeoApiService.GetAllNotifications(requestData);
                DateTime today = DateTime.Now;

                // Recent notifications group
                List<TreatmentAlert> recentNotifications = allNotifications.Results.Where(n => n.InsertDate.Date == today.Date).ToList();
                if (recentNotifications.Any())
                {
                    NotificationList recentNotificationList = Notifications.SingleOrDefault(l => l.Heading == "Recientes");

                    if (recentNotificationList == null)
                    {
                        recentNotificationList = new NotificationList() {Heading = "Recientes"};
                        Notifications.Add(recentNotificationList);
                    }

                    recentNotificationList.AddRange(recentNotifications);
                }

                // Older notifications group
                List<TreatmentAlert> olderNotifications = allNotifications.Results.Where(n => n.InsertDate.Date != today.Date).ToList();
                if (olderNotifications.Any())
                {
                    NotificationList olderNotificationList = Notifications.SingleOrDefault(l => l.Heading == "Anterior");

                    if (olderNotificationList == null)
                    {
                        olderNotificationList = new NotificationList() {Heading = "Anterior"};
                        Notifications.Add(olderNotificationList);
                    }

                    olderNotificationList.AddRange(olderNotifications);
                }

                CurrentPage = allNotifications.CurrentPage;
                TotalPages = allNotifications.TotalPages;
            }
            catch (Exception)
            {
                Error = true;
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }

        private async Task LoadMoreNotificationsAsync()
        {
            if (CurrentPage == 4)
            {
                return;
            }

            var requestData = new NotificationsPaginatedRequest()
            {
                CurrentPage = CurrentPage + 1,
                PageSize = PageSize,
                Viewed = true,
            };

            await SearchNotificationsAsync(requestData);
        }

        private async Task MarkNotificacionAsReadAsync(TreatmentAlert notification)
        {
            try
            {
                IsLoadingInProgress = true;

                await prometeoApiService.SetViewedNotification(notification.Id);

                notification.Viewed = true;

                NotificationList recentNotifications = Notifications[0];
                if (recentNotifications != null && recentNotifications.Count > 0)
                {
                    int index = recentNotifications.IndexOf(notification);
                    if (index != -1)
                    {
                        recentNotifications.Remove(notification);
                        recentNotifications.Insert(index, notification);
                    }
                }

                if (Notifications.Count > 1)
                {
                    NotificationList olderNotifications = Notifications[1];
                    if (olderNotifications != null && olderNotifications.Count > 0)
                    {
                        int index = olderNotifications.IndexOf(notification);
                        if (index != -1)
                        {
                            olderNotifications.Remove(notification);
                            olderNotifications.Insert(index, notification);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // TODO: mostrar mensaje de error de alguna manera...
            }
            finally
            {
                IsLoadingInProgress = false;
            }
        }
    }
}
