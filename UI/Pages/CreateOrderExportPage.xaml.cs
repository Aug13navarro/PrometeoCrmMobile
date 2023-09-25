using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using UI.Popups;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CreateOrderExportPage : MvxContentPage<CreateOrderExportViewModel>
    {
        public CreateOrderExportPage()
        {
            InitializeComponent();
        }
        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.ShowEditProductPopup += OnShowEditProductPopup;
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
                        lblDiscountResult.Text = ViewModel.ValorDescuento.ToString("N2", new CultureInfo("es-US"));

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
                            lblDiscountResult.Text = ViewModel.ValorDescuento.ToString("N2", new CultureInfo("es-ES"));

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

            btnInfo.BackgroundColor = Xamarin.Forms.Color.White;
            lblInfo.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackDetail = true;
            ViewModel.StackProductos = false;
        }

        private void TapProducts_Tapped(object sender, EventArgs e)
        {
            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.White;
            lblProductos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackDetail = false;
            ViewModel.StackProductos = true;
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
    }
}