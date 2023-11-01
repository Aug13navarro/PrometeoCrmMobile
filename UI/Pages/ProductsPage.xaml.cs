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

                (double price, int quantity, int discount) = args;
                var detail = new OpportunityProducts()
                {
                    productId = product.Id,
                    product = new Product
                    {
                        name = product.name,
                        price = price,
                        //Discount = discount,
                        //quantity = quantity,
                    },
                    //Price = price,
                    Discount = discount,
                    Quantity = quantity,
                    Total = CalcularTotal(price, quantity, discount),
                    SubtotalProducts = quantity * price,
                    DiscountPrice = CalcularDescuento(quantity, price, discount)
                };

                await ViewModel.Close(detail);
            };
            
            await PopupNavigation.Instance.PushAsync(popup);
        }

        private double CalcularDescuento(int quantity, double price, int discount)
        {
            var totalProduct = quantity * price;
            var descuento = Convert.ToDecimal(discount) / 100;

            return totalProduct * Convert.ToDouble(descuento);
        }

        private double CalcularTotal(double price, int quantity, int discount)
        {
            var totalTemp = price * quantity;

            if (discount == 0)
            {
                return totalTemp;
            }
            else
            {
                return totalTemp - (totalTemp * discount / 100);
            }
        }

        private void productsList_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
            if(ViewModel.IsSearchInProgress ||
                ViewModel.Products == null ||
                ViewModel.Products.Count == 0 ||
                ViewModel.CurrentPage >= ViewModel.TotalPages)
            {
                return;
            }

            var currentProducts = (Product)e.Item;
            Product LastProduct = ViewModel.Products[ViewModel.Products.Count - 1];

            if (ViewModel.Products.Count >= 1 && ViewModel.CurrentPage != ViewModel.TotalPages)
            {
                if (currentProducts.Id == LastProduct.Id)
                {
                    ViewModel.LoadMorePruductsCommand.Execute(null);
                }
            }
        }
    }
}
