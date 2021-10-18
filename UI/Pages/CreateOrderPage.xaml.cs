using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using UI.Popups;

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
                if (lblDiscount.Text == "Discount")
                {
                    ViewModel.ResetTotal(ViewModel.Order.products);

                    if (string.IsNullOrWhiteSpace(lblOrderDiscount.Text) || ViewModel.OrderDiscount == 0)
                    {
                        ViewModel.OrderDiscount = 0;
                        lblDiscountResult.Text = 0.ToString();
                    }

                    if (ViewModel.OrderDiscount > 0)
                    {
                        ViewModel.ValorDescuento = ViewModel.Total * ViewModel.OrderDiscount / 100;
                        lblDiscountResult.Text = ViewModel.ValorDescuento.ToString("N2");

                        ViewModel.ActualizarTotal(ViewModel.Order.products);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(lblOrderDiscount.Text))
                    {
                        ViewModel.ResetTotal(ViewModel.Order.products);

                        if (string.IsNullOrWhiteSpace(lblOrderDiscount.Text) || ViewModel.OrderDiscount == 0)
                        {
                            ViewModel.OrderDiscount = 0;
                            lblDiscountResult.Text = 0.ToString();
                        }

                        if (Convert.ToDecimal(lblOrderDiscount.Text) > 0)
                        {
                            var o = Convert.ToDecimal(lblOrderDiscount.Text) / 100;

                            var descuento = Convert.ToDouble($"0.{o}");

                            ViewModel.ValorDescuento = ViewModel.Total * descuento;
                            lblDiscountResult.Text = ViewModel.ValorDescuento.ToString("N2");

                            ViewModel.ActualizarTotal(ViewModel.Order.products);
                        }
                    }
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
    }
}