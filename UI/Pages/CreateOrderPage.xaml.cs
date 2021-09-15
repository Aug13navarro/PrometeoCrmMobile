using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
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
                        lblDiscountResult.Text = ViewModel.ValorDescuento.ToString();

                        ViewModel.ActualizarTotal(ViewModel.Order.products);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(lblOrderDiscount.Text))
                    {
                        if (Convert.ToDecimal(lblOrderDiscount.Text) > 0)
                        {
                            var o = Convert.ToDecimal(lblOrderDiscount.Text) / 100;

                            var descuento = Convert.ToDouble($"0.{o}");

                            ViewModel.ValorDescuento = ViewModel.Total * descuento;
                            lblDiscountResult.Text = ViewModel.ValorDescuento.ToString();

                            ViewModel.ActualizarTotal(ViewModel.Order.products);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                var s = ex.Message;
            }
        }
    }
}