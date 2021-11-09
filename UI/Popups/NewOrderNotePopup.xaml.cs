using Core.Model;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewOrderNotePopup : BasePopupPage
    {
        public event EventHandler<(Company Company, bool isExport)> OkTapped;

        public IEnumerable<Company> Companies { get; set; }
        public Company Company { get; set; }

        public NewOrderNotePopup(List<Company> companies)
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = true;
            
            Companies = companies;

            if(Companies.Count() > 0 )
            {
                cmbCompanies.ItemsSource = Companies.ToList();
            }

            descriptionLabel.Text = LangResources.AppResources.NewOrder;
        }
        private async void CancelButtonClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(false);
            NotifyDismiss();
        }

        private void cmbCompanies_SelectedIndexChanged (object sender, EventArgs args)
        {
            var index = cmbCompanies.SelectedIndex;

            var comp = Companies.ToList()[index];

            Company = comp;
        }

        private void AceptButtonClicked(object sender , EventArgs e)
        {
            if(Company != null)
            {
                var result = (Company, true);

                OkTapped?.Invoke(this, result);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("", "", "aceptar"); return;
            }
        }
    }
}