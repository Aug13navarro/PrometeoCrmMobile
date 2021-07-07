using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class ContactsPage : MvxContentPage<ContactsViewModel>
    {
        public ContactsPage()
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

        private void OnContactsListItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ViewModel.IsSearchInProgress ||
                ViewModel.Contacts == null ||
                ViewModel.Contacts.Count == 0 ||
                ViewModel.CurrentPage == ViewModel.TotalPages)
            {
                return;
            }

            var currentContact = (CustomerContact)e.Item;
            CustomerContact lastContactInList = ViewModel.Contacts[ViewModel.Contacts.Count - 1];

            if (currentContact.Id == lastContactInList.Id)
            {
                ViewModel.LoadMoreContactsCommand.Execute(null);
            }
        }
    }
}
