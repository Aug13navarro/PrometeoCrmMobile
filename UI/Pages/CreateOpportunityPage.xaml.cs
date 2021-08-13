using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using UI.Popups;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CreateOpportunityPage : MvxContentPage<CreateOpportunityViewModel>
    {
        public CreateOpportunityPage()
        {
            InitializeComponent();

        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.ShowEditProductPopup += OnShowEditProductPopup;
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

        private async void OnShowEditProductPopup(object sender, Product product)
        {
            var popup = new SelectProductPopup(product, true);

            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);
                ViewModel.FinishEditProduct(args);
            };

            popup.Dismissed += (s, args) => ViewModel.CancelEditProduct();

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private void ImageButton_Clicked_1(object sender, EventArgs e)
        {

            ViewModel.AjustarBotonesEstados(1);
        }

        private void ImageButton_Clicked_2(object sender, EventArgs e)
        {

            ViewModel.AjustarBotonesEstados(2);
        }

        private void ImageButton_Clicked_3(object sender, EventArgs e)
        {

            ViewModel.AjustarBotonesEstados(3);
        }

        private void ImageButton_Clicked_4(object sender, EventArgs e)
        {

            //ViewModel.AjustarBotonesEstados(4);

            var label = (Image)sender;
            object parameter = ((TapGestureRecognizer)label.GestureRecognizers[0]).CommandParameter;

            var popup = new CerrarOportunidadPopup();

            popup.GanadaTapped += (s, args) =>
            {
                ViewModel.WinOpportunityCommand.Execute(parameter);
                PopupNavigation.Instance.PopAsync(false);
            };
            popup.PerdidaTapped += (s, args) =>
            {
                ViewModel.LostOpportunityCommand.Execute(parameter);
                PopupNavigation.Instance.PopAsync(false);
            };

            PopupNavigation.Instance.PushAsync(popup);
        }
    }
}
