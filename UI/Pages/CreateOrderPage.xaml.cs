using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
    }
}