using Core.Model;
using Core.ViewModels;
using MvvmCross.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.LangResources;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerAddressPopup : BasePopupPage
    {
        public event EventHandler<string> OkTapped;

        public IEnumerable<CustomerAddress> CustomerAddresses { get; set; }

        public CustomerAddressPopup(IEnumerable<CustomerAddress> customerAddresses)
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = true;

            CustomerAddresses = customerAddresses;

            if(CustomerAddresses.Count() > 0)
            {
                listAddress.ItemsSource = CustomerAddresses;
            }

            descriptionLabel.Text = AppResources.SelectAddressTitle;
        }
        private async void CancelButtonClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(false);
            NotifyDismiss();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var lbl = (Xamarin.Forms.Label)sender;

            OkTapped?.Invoke(this, lbl.Text);
        }
    }
}