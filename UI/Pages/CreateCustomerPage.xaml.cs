using System.Linq;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CreateCustomerPage : MvxContentPage<CreateCustomerViewModel>
    {
        public CreateCustomerPage()
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

        private void OnCompanyNameTextChanged(object sender, TextChangedEventArgs e)
        {
            abbreavitureInput.Text = new string(e.NewTextValue.Take(3).ToArray()).ToUpper();
        }
    }
}
