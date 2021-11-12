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
        public bool Export { get; set; }

        public NewOrderNotePopup(List<Company> companies)
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = true;
            
            Companies = companies;

            CheckExportación.IsEnabled = false;
            CheckExportación.Color = Color.Gray;

            if (Companies.Count() > 0 )
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

            CheckExportación.IsEnabled = false;
            CheckExportación.Color = Color.Gray;

            if (Company.CompanyOrderTypes.Count > 0)
            {
                foreach (var item in Company.CompanyOrderTypes)
                {
                    if(item.OrderType.Name == "Exportación")
                    {
                        CheckExportación.IsEnabled = true;
                        CheckExportación.Color = Color.FromHex("#FF4081");
                    }
                }
            }
        }

        private void AceptButtonClicked(object sender , EventArgs e)
        {
            if(Company != null)
            {
                var result = (Company, Export);

                OkTapped?.Invoke(this, result);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(LangResources.AppResources.Attention, LangResources.AppResources.SelectCompany, LangResources.AppResources.Accept); return;
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Export = e.Value;
        }

    }
}