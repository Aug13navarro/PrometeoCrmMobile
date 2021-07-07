using Core.ViewModels;
using Core.ViewModels.Model;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CustomersPage : MvxContentPage<CustomersViewModel>
    {
        public CustomersPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FormsApp.RootPage.IsGestureEnabled = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            FormsApp.RootPage.IsGestureEnabled = false;
        }

        private async void OnCustomersListItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ViewModel.IsSearchInProgress ||
                ViewModel.Customers == null ||
                ViewModel.Customers.Count == 0 ||
                ViewModel.CurrentPage == ViewModel.TotalPages)
            {
                return;
            }

            var currentCustomer = (CustomerVm)e.Item;
            CustomerVm lastCustomerInList = ViewModel.Customers[ViewModel.Customers.Count - 1];

            if (currentCustomer.Customer.Id == lastCustomerInList.Customer.Id)
            {
                await ViewModel.LoadMoreCustomersCommand.ExecuteAsync();
            }
        }
    }
}
