using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using UI.Popups;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class ProductsPage : MvxContentPage<ProductsViewModel>
    {
        public ProductsPage()
        {
            InitializeComponent();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.ShowSelectProductPopup += OnShowSelectProductPopup;
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

        private async void OnShowSelectProductPopup(object sender, Product product)
        {
            var popup = new SelectProductPopup(product);

            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);

                (decimal price, int quantity, int discount) = args;
                var detail = new OpportunityDetail()
                {
                    ProductId = product.Id,
                    Description = product.Description,
                    Price = price,
                    Discount = discount,
                    Quantity = quantity,
                };

                await ViewModel.Close(detail);
            };

            await PopupNavigation.Instance.PushAsync(popup);
        }
    }
}
