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
            Identity.LanguageUser = LoggedUser.Language.abbreviation;

            var red = await Connection.SeeConnection();

            if (red)
            {
                //if(offlineDataService.IsDataLoaded)
                //{
                    if (DateTime.Now.ToString("dddd", CultureInfo.CreateSpecificCulture("es")) == "lunes")
                    {
                        CargarEnCache(LoggedUser.Language.abbreviation.ToLower());
                    }
                //}
                //else
                //{
                //    CargarEnCache(LoggedUser.Language.ToLower());
                //}
            }
            else
            {

            }

        }

        public async void CargarEnCache (string lang)
        {
            //INIO AUTOMAPER PARA IGUALAR LOS MODELS
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            //OBTENGO TODAS LAS EMPRESAS POR USUARIO
            var empresas = await prometeoApiService.GetCompaniesByUserId(LoggedUser.Id, LoggedUser.Token);


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

            var status = await prometeoApiService.GetOpportunityStatus(LoggedUser.Language.abbreviation.ToLower(), LoggedUser.Token);

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

            //OBTENGO LOS USUARIOS ASISTENTES
            var asistentes = new List<User>();

            foreach (var item in empresas)
            {
                var asistant = await prometeoApiService.GetUsersByRol(item.Id, "Asistente Comercial");

                asistentes.AddRange(asistant);
            }

            //GUARDO LOS ASISTENTES COMERCIALES
            var ass = mapper.Map<List<UserExtern>>(asistentes);
            offlineDataService.SaveAssitant(ass);

            //OBTENGO TODOS LOS MEDIOS DE PAGO
            var medios = new List<PaymentMethod>();

            foreach (var item in empresas)
            {
                var mediosPago = await prometeoApiService.GetPaymentMethod(item.Id, LoggedUser.Language.abbreviation.ToLower(), LoggedUser.Token);

                medios.AddRange(mediosPago);
            }

            //GUARDO LOS MEDIOS DE PAGOS 
            var mp = mapper.Map<List<PaymentMethodExtern>>(medios);
            offlineDataService.SavePaymentMethod(mp);

            //OBTENER LOS INCOTERMS
            var incoterms = await prometeoApiService.GetIncoterms(LoggedUser.Token);

            //GUARDO LOS INCOTERMS
            var inc = mapper.Map<List<IncotermExtern>>(incoterms);
            offlineDataService.SaveIncoterms(inc);

            //OBTENER FLETES PARA PEDIDO EXPORTACIÓN 
            var fletes = await prometeoApiService.GetFreight(LoggedUser.Language.abbreviation.ToLower(), LoggedUser.Token);

            //GUARDO LOS FLETES
            var freigths = mapper.Map<List<FreightInChargeExtern>>(fletes);
            offlineDataService.SaveFreights(freigths);

            //ONTENGO TODOS LOS TRANSPORTES
            var transportes = await prometeoApiService.GetTransport(LoggedUser.Language.abbreviation.ToLower(), LoggedUser.Token);

            //GUARDO LOS TRANSPORTES
            var tra = mapper.Map<List<TransportExtern>>(transportes);
            offlineDataService.SaveTransports(tra);

            //SINCRONIZO LA DATA CON LOS ARCHIVOS EN LA CACHE
            await offlineDataService.SynchronizeToDisk();
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
