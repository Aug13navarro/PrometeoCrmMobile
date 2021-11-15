using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Helpers;
using Core.Model;
using Core.Model.Extern;
using Core.Services;
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
            Identity.LanguageUser = LoggedUser.Language;

            var red = await Connection.SeeConnection();

            if (red)
            {
                //REVISAR SI EXISTEN OPORTUNIDADES Y PEDIDODOS EN LA MEMORIA PARA PUBLICAR

                if (DateTime.Now.ToString("dddd", CultureInfo.CreateSpecificCulture("es")) == "viernes")
                {

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
                    offlineDataService.UnloadAllData();

                    //GUARDO EMPRESAS EM CACHE
                    var e = mapper.Map<List<CompanyExtern>>(empresas);
                    offlineDataService.SaveCompanySearch(e);

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

                    //OBTENGOS LOS ESTADOS DE OPORTUNIDADES

                    var status = await prometeoApiService.GetOpportunityStatus(LoggedUser.Language.ToLower(), LoggedUser.Token);

                    //GUARDO LOS ESTADOS 
                    var s = mapper.Map<List<OpportunityStatusExtern>>(status);
                    offlineDataService.SaveOpportunityStatus(s);

                    //OBTENGO TODAS LAS CONDICIONES DE PAGOS POR LAS EMPRESAS
                    var condiciones = new List<PaymentCondition>();

                    foreach (var item in empresas)
                    {
                        var condic = await prometeoApiService.GetPaymentConditions(LoggedUser.Token, item.Id);

                        condiciones.AddRange(condic);
                    }

                    //GUARDO LAS CONDICIONES DE PAGO
                    offlineDataService.SavePaymentConditions(condiciones);

                    //SINCRONIZO LA DATA CON LOS ARCHIVOS EN LA CACHE
                    await offlineDataService.SynchronizeToDisk();
                }
            }
            else
            {

            }

        }
        private List<OpportunityPost.ProductSend> listaProductos(MvxObservableCollection<OpportunityProducts> details)
        {
            var lista = new List<OpportunityPost.ProductSend>();

            foreach (var item in details)
            {
                double tempTotal = item.Price * item.Quantity;

                if (item.Discount != 0)
                {
                    tempTotal = tempTotal - ((tempTotal * item.Discount) / 100);
                }

                lista.Add(new OpportunityPost.ProductSend
                {
                    discount = item.Discount,
                    productId = item.productId,
                    quantity = item.Quantity,
                    total = tempTotal,
                    price = item.Price
                });
            }

            return lista;
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
