using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System.Linq;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class SelectProductoPage : MvxContentPage<SelectProductViewModel>
    {
        public SelectProductoPage()
        {
            InitializeComponent();

        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            //ViewModel.ShowSelectProductPopup += OnShowSelectProductPopup;
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

        private void productsList_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
            if (ViewModel.IsSearchInProgress ||
                ViewModel.Products == null ||
                ViewModel.Products.Count == 0 ||
                ViewModel.CurrentPage == ViewModel.TotalPages)
            {
                return;
            }

            var currentProducts = (Product)e.Item;
            Product LastProduct = ViewModel.Products[ViewModel.Products.Count - 1];

            if (currentProducts.Id == LastProduct.Id)
            {
                ViewModel.LoadMorePruductsCommand.Execute(null);
            }
        }
    }
}