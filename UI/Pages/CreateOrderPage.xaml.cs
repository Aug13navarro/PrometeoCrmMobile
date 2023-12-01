using Core.Model;
using Core.ViewModels;
using MvvmCross.Base;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Core;
using Core.Data;
using Core.Data.Tables;
using Core.Services;
using MvvmCross.Presenters.Hints;
using UI.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CreateOrderPage : MvxContentPage<CreateOrderViewModel>
    {
        public CreateOrderPage()
        {
            InitializeComponent();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.ShowEditProductPopup += OnShowEditProductPopup;
            ViewModel.ShowAddressPopup += OnShowCustomerAddressPopup;
            ViewModel.ShowConfirmPopup += OnShowConfirmPopup;
        }

        private async void OnShowConfirmPopup(object sender, OrderNote e)
        {
            var popup = new ConfirmOrderNotePopupPage();
            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);

                if (args.confirmed && args.notConfirmed == false)
                {
                    await ViewModel.ConfirmaOrderNote(e);
                }
            };

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void OnShowCustomerAddressPopup(object sender, List<CustomerAddress> addresses)
        {
            var popup = new CustomerAddressPopup(addresses);

            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);

                if (!string.IsNullOrWhiteSpace(args))
                {
                    editorAddress.Text = args;
                }
            };

            await PopupNavigation.Instance.PushAsync(popup);
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

        private void Entry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            try
            {
                var idioma = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                ViewModel.ResetTotal(ViewModel.Order.products);

                if (string.IsNullOrWhiteSpace(lblOrderDiscount.Text) || ViewModel.OrderDiscount == 0)
                {
                }

                if (ViewModel.OrderDiscount > 0)
                {
                    ViewModel.ValorDescuento = ViewModel.Total * ViewModel.OrderDiscount / 100;

                    ViewModel.ActualizarTotal(ViewModel.Order.products);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        private void TapInfo_Tapped(object sender, EventArgs e)
        {
            btnProductos.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblProductos.TextColor = Xamarin.Forms.Color.White;

            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblAdjuntos.TextColor = Xamarin.Forms.Color.White;

            btnInfo.BackgroundColor = Xamarin.Forms.Color.White;
            lblInfo.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = true;
            ViewModel.StackProductos = false;
            ViewModel.StackAdjunto = false;
        }

        private void TapProducts_Tapped(object sender, EventArgs e)
        {
            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblAdjuntos.TextColor = Xamarin.Forms.Color.White;

            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.White;
            lblProductos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = false;
            ViewModel.StackProductos = true;
            ViewModel.StackAdjunto = false;
        }

        private void TapAdjuntos_Tapped(object sender, EventArgs e)
        {
            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblProductos.TextColor = Xamarin.Forms.Color.White;

            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.White;
            lblAdjuntos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = false;
            ViewModel.StackProductos = false;
            ViewModel.StackAdjunto = true;
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            cmbStatusOrderNote.Focus();
        }

        private void cmbStatusOrderNote_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (Picker)sender;

            ViewModel.Order.StatusOrderNote = cmb.SelectedItem as StatusOrderNote;
            ViewModel.Order.OrderStatus = (cmb.SelectedItem as StatusOrderNote).Id;
        }

        private async void ImageButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (status == PermissionStatus.Granted)
                {
                    var files = PickAndShow(default).ContinueWith(
                        (task) => { });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        LangResources.AppResources.Attention,
                        "No se tienen los permisos para acceder a los archivos.",
                        LangResources.AppResources.Accept);
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return;
            }
        }

        async Task PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickMultipleAsync(options); //obtengo lista de imagenes 

                if (result != null && result.Any())
                {
                    ViewModel.AddFileToOrderNote(result);
                    //OnChooseMultiple(2, result);
                    //await Application.Current.MainPage.Navigation.PopPopupAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}