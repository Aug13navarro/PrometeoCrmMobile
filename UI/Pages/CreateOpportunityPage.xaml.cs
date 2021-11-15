using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using UI.LangResources;
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

        private void Cerrada_Clicked(object sender, EventArgs e)
        {
            if(ViewModel.EstadoId >= 4)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Info", AppResources.NoEditOpportunity, AppResources.Accept);
                return;
            }
        }

        private void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            if (ViewModel.EstadoId >= 4)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Info", AppResources.NoEditOpportunity, AppResources.Accept);
                return;
            }

            if (lblCustomer.Text == "Cliente")
            {
                ViewModel.AjustarBotonesEstados(1);
            }
            else
            {
                ViewModel.AjustarBotonesEstadosEng(1);
            }
        }

        private void ImageButton_Clicked_2(object sender, EventArgs e)
        {
            if (ViewModel.EstadoId >= 4)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Info", AppResources.NoEditOpportunity, AppResources.Accept);
                return;
            }
            if (lblCustomer.Text == "Cliente")
            {
                ViewModel.AjustarBotonesEstados(2);
            }
            else
            {
                ViewModel.AjustarBotonesEstadosEng(2);
            }
        }

        private void ImageButton_Clicked_3(object sender, EventArgs e)
        {
            if (ViewModel.EstadoId >= 4)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Info", AppResources.NoEditOpportunity, AppResources.Accept);
                return;
            }

            if (lblCustomer.Text == "Cliente")
            {
                ViewModel.AjustarBotonesEstados(3);
            }
            else
            {
                ViewModel.AjustarBotonesEstadosEng(3);
            }
        }

        private void ImageButton_Clicked_4(object sender, EventArgs e)
        {
            if (ViewModel.EstadoId >= 4)
            {
                Application.Current.MainPage.DisplayAlert(
                    AppResources.InfoTitle, AppResources.NoEditOpportunity, AppResources.Accept);
                return;
            }

            if (ViewModel.Opportunity.Id > 0)
            {
                //ViewModel.AjustarBotonesEstados(4);

                var label = (Image)sender;
                object parameter = ((TapGestureRecognizer)label.GestureRecognizers[0]).CommandParameter;

                var popup = new CerrarOportunidadPopup();

                popup.GanadaTapped += (s, args) =>
                {
                    if (ViewModel.SelectedCustomer.ExternalId > 0)
                    {
                        ViewModel.WinOpportunityCommand.Execute(parameter);
                        PopupNavigation.Instance.PopAsync(false);
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert(
                            AppResources.InfoTitle, AppResources.WithoutExternId, AppResources.Accept);
                        return;
                    }
                };
                popup.PerdidaTapped += (s, args) =>
                {
                    if (ViewModel.SelectedCustomer.ExternalId > 0)
                    {
                        ViewModel.LostOpportunityCommand.Execute(parameter);
                        PopupNavigation.Instance.PopAsync(false);
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert(
                            AppResources.InfoTitle, AppResources.WithoutExternId, AppResources.Accept);
                        return;
                    }
                };

                PopupNavigation.Instance.PushAsync(popup);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert
                    (AppResources.InfoTitle, AppResources.InfoTextCloseOpportunity, AppResources.Accept);
                return;
            }
        }
    }
}
