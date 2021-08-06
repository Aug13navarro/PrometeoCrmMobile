using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class PedidosViewModel : MvxViewModel
    {
        public Command FilterOrdersCommand { get; }
        
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public PedidosViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService,
                                          IToastService toastService)
        {
            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            this.toastService = toastService;

            FilterOrdersCommand = new Command(async () => await FilterOrders());
        }

        private async Task FilterOrders()
        {
            var filtro = await navigationService.Navigate<FilterOrdersViewModel, FilterOrderJson>();
        }
    }
}
