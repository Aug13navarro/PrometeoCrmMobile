using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Helpers;
using Core.Model;
using Core.Model.Extern;
using Core.Services.Contracts;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
    {
        // Properties
        public User LoggedUser { get; }

        // Commands
        public IMvxAsyncCommand GoToOpportunitiesCommand { get; }
        public IMvxAsyncCommand GoToOrderCommand { get; }
        public IMvxAsyncCommand GoToCustomersCommand { get; }
        public IMvxAsyncCommand GoToContactsCommand { get; }

        // Fields
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IOfflineDataService offlineDataService;

        public HomeViewModel(IMvxNavigationService navigationService, ApplicationData appData, IPrometeoApiService _prometeoApiService, IOfflineDataService offlineData)
        {
            this.navigationService = navigationService;
            this.prometeoApiService = _prometeoApiService;
            offlineDataService = offlineData;

            LoggedUser = appData.LoggedUser;

            GoToOpportunitiesCommand = new MvxAsyncCommand(GoToOpportunitiesAsync);
            GoToOrderCommand = new MvxAsyncCommand(GoToOrderAsync);
            GoToCustomersCommand = new MvxAsyncCommand(GoToCustomersAsync);
            GoToContactsCommand = new MvxAsyncCommand(GoToContactsAsync);

            CargarObtenerDatos();
        }

        private async void CargarObtenerDatos()
        {
            if (offlineDataService.IsWifiConection)
            {
                //REVISAR SI EXISTEN OPORTUNIDADES Y PEDIDODOS EN LA MEMORIA PARA PUBLICAR

                

                //if (DateTime.Now.Date.Day.ToString("dddd", CultureInfo.CreateSpecificCulture("es")) == "lunes")
                //{

                //INIO AUTOMAPER PARA IGUALAR LOS MODELS
                var mapperConfig = new MapperConfiguration(m =>
                {
                    m.AddProfile(new MappingProfile());
                });

                IMapper mapper = mapperConfig.CreateMapper();

                //OBTENGO TODAS LAS EMPRESAS POR USUARIO
                var empresas = await prometeoApiService.GetCompaniesByUserId(LoggedUser.Id, LoggedUser.Token);

                    //offlineDataService.UnloadAllData("Company");
                    //offlineDataService.UnloadAllData("Customer");
                    //offlineDataService.UnloadAllData("Payment");
                    //offlineDataService.UnloadAllData("Presentation");

                //ELIMINO CACHE VIEJO
                await offlineDataService.DeleteAllData();

                //GUARDO EMPRESAS EM CACHE
                offlineDataService.SaveCompanySearch(empresas);

                var clientes = new List<Customer>();
                //foreach (var item in empresas)
                //{
                //    var allCustomer = await prometeoApiService.GetAllCustomer(LoggedUser.Id, true, 3, LoggedUser.Token, item.Id);

                //    clientes.AddRange(allCustomer);
                //}

                //OBTENGO TODOS LOS CLIENTES
                var cliente = await prometeoApiService.GetAllCustomer(LoggedUser.Id, true, 3, LoggedUser.Token, empresas.FirstOrDefault().Id);

                clientes.AddRange(cliente);

                //GUARDO LOS CLIENTES EN CACHE
                var r = mapper.Map<List<CustomerExtern>>(clientes);
                offlineDataService.SaveCustomerSearch(r);

                var condiciones = new List<PaymentCondition>();

                //OBTENGO TODAS LAS CONDICIONES DE PAGOS POR LAS EMPRESAS
                foreach (var item in empresas)
                {
                    var condic = await prometeoApiService.GetPaymentConditions(LoggedUser.Token, item.Id);

                    condiciones.AddRange(condic);
                }

                //GUARDO LAS CONDICIONES DE PAGO
                offlineDataService.SavePaymentConditions(condiciones);

                //SINCRONIZO LA DATA CON LOS ARCHIVOS EN LA CACHE
                    await offlineDataService.SynchronizeToDisk();
                //}
            }
            else
            {
                //await offlineDataService.LoadCompanies();
                //await offlineDataService.LoadAllData();
            }

        }

        private async Task GoToOrderAsync()
        {
            await navigationService.Navigate<PedidosViewModel>();
        }

        private async Task GoToCustomersAsync()
        {
            await navigationService.Navigate<CustomersViewModel>();
        }

        private async Task GoToContactsAsync()
        {
            await navigationService.Navigate<ContactsViewModel>();
        }

        private async Task GoToOpportunitiesAsync()
        {
            await navigationService.Navigate<OpportunitiesViewModel>();
        }
    }
}
