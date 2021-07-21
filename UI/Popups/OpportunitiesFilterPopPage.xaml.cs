using Core.Services.Contracts;
using Core.ViewModels;
using MvvmCross.Navigation;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using UI.Pages;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpportunitiesFilterPopPage : BasePopupPage
    {

        public OpportunitiesFilterPopPage(OpportunitiesViewModel opportunitiesViewModel)
        {
            InitializeComponent();

            BindingContext = new FilterOpportunitiesViewModel(opportunitiesViewModel);
        }

        private void SelectCustomer(object sender, EventArgs e)
        {
            //var btn = (Image)sender;
            //var parameter = btn.CommandParameter as OpportunitiesViewModel;
            //var parameter = BindingContext.DataContext as OpportunitiesViewModel;

            var popup = new PopupPage();

            //PopupNavigation.Instance.PushAsync(new CustomerPopUpPage());
        }
    }
}