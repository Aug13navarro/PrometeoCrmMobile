using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Model;
using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class ProductsViewModel : MvxViewModelResult<OpportunityDetail>
    {
        // Properties
        private List<Product> products;
        public List<Product> Products
        {
            get => products;
            set => SetProperty(ref products, value);
        }

        // Events
        public event EventHandler<Product> ShowSelectProductPopup;

        // Commands
        public Command SelectProductCommand { get; }

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;

        public ProductsViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService)
        {
            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;
            SelectProductCommand = new Command<Product>(SelectProduct);
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            var listProducts = await prometeoApiService.GetAvailableProducts(1,7);
            Products = listProducts.results;
        }

        public async Task Close(OpportunityDetail opportunityDetail)
        {
            await navigationService.Close(this, opportunityDetail);
        }

        private void SelectProduct(Product product)
        {
            ShowSelectProductPopup?.Invoke(this, product);
        }
    }
}
