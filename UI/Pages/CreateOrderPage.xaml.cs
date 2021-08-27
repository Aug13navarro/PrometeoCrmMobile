﻿using Core.Model;
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
            if(lblDiscount.Text == "Discount")
            {
                if (ViewModel.OrderDiscount > 0)
                {
                    ViewModel.ActualizarTotal(ViewModel.Order.products);
                    ViewModel.ValorDescuento = ViewModel.Total * ViewModel.OrderDiscount / 100;
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

                        lblDiscountResult.Text = (ViewModel.Total * descuento).ToString();
                        ViewModel.ActualizarTotal(ViewModel.Order.products);
                    }
                }
            }
        }
    }
}