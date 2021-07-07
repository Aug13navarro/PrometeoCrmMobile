using System.Linq;
using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class NotificationsPage : MvxContentPage<NotificationsViewModel>
    {
        public NotificationsPage()
        {
            InitializeComponent();
        }

        private async void OnNotificationListItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ViewModel.IsLoadingInProgress ||
                ViewModel.Notifications == null ||
                ViewModel.Notifications.Count == 0 ||
                ViewModel.CurrentPage == ViewModel.TotalPages ||
                !(e.Item is TreatmentAlert currentNotification))
            {
                return;
            }

            TreatmentAlert lastNotificationInList = ViewModel.Notifications.SelectMany(l => l).Last();

            if (currentNotification.Id == lastNotificationInList.Id)
            {
                await ViewModel.LoadMoreNotificationsCommand.ExecuteAsync();
            }
        }
    }
}
