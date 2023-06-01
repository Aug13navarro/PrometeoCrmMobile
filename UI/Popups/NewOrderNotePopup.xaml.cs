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

        public Company Company { get; set; }
        public bool Export { get; set; }

        public NewOrderNotePopup(Company companie)
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = true;
            
            Company = companie;

            CheckExportación.IsEnabled = false;
            CheckExportación.Color = Color.Gray;

            if (Company != null)
            {
                CompanyName.Text = Company.BusinessName;
                if (Company.ExportPv.HasValue && Company.ExportPv.Value) 
                { CheckExportación.IsEnabled = true; CheckExportación.Color = Color.FromHex("#FF4081"); }
            }

            descriptionLabel.Text = LangResources.AppResources.NewOrder;
        }
        private async void CancelButtonClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(false);
            NotifyDismiss();
        }

        //private void cmbCompanies_SelectedIndexChanged (object sender, EventArgs args)
        //{
        //    var index = cmbCompanies.SelectedIndex;

        //    var comp = Companies.ToList()[index];

        //    Company = comp;

        //    //sCheckExportación.IsEnabled = true;
        //    //CheckExportación.Color = Color.Gray;

        //    if (Company.ExportPv.HasValue && Company.ExportPv.Value)
        //    {
        //        CheckExportación.IsEnabled = true;
        //        CheckExportación.Color = Color.FromHex("#FF4081");

        //    }
        //}

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