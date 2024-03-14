using Core.Model;
using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;
using Core.Services;
using Core.ViewModels.Model;
using AutoMapper;
using Core.Helpers;
using Xamarin.Essentials;
using Core.Model.Extern;
using Core.Data;
using Core.Data.Tables;
using Newtonsoft.Json;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Application = Xamarin.Forms.Application;
using System.Collections;
using System.IO;
using System.Text;
using System.ComponentModel.Design;
using MvvmCross.IoC;

namespace Core.ViewModels
{
    public class CreateOrderViewModel : MvxViewModel<OrderNote>
    {
        public ApplicationData data;

        // Properties
        #region PROPIEDADES
        private bool changeStatusEnable;
        public bool ChangeStatusEnable
        {
            get => changeStatusEnable;
            set => SetProperty(ref changeStatusEnable, value);
        }
        private bool stackInfo;
        public bool StackInfo
        {
            get => stackInfo;
            set => SetProperty(ref stackInfo, value);
        }

        private bool stackProductos;
        public bool StackProductos
        {
            get => stackProductos;
            set => SetProperty(ref stackProductos, value);
        }

        private bool stackAdjunto;
        public bool StackAdjunto
        {
            get => stackAdjunto;
            set => SetProperty(ref stackAdjunto, value);
        }

        private int estadoId;
        public int EstadoId
        {
            get => estadoId;
            set => SetProperty(ref estadoId, value);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private OrderNote order;
        public OrderNote Order
        {
            get => order;
            set => SetProperty(ref order, value);
        }

        private MvxObservableCollection<StatusOrderNote> selectedStatus;
        public MvxObservableCollection<StatusOrderNote> OrderStatus
        {
            get => selectedStatus;
            set => SetProperty(ref selectedStatus, value);
        }
        private StatusOrderNote status;
        public StatusOrderNote Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        //private string orderStatusOrderStr;
        //public string OrderStatusOrderStr
        //{
        //    get => orderStatusOrderStr;
        //    set => SetProperty(ref orderStatusOrderStr, value);
        //}

        public MvxObservableCollection<TypeStandard> TypeOfRemittances { get; set; } = new MvxObservableCollection<TypeStandard>();
        public MvxObservableCollection<TypeStandard> PlaceOfPayment { get; set; } = new MvxObservableCollection<TypeStandard>();

        private MvxObservableCollection<TransportCompany> freightInCharges;
        public MvxObservableCollection<TransportCompany> FreightInCharges
        {
            get => freightInCharges;
            set => SetProperty(ref freightInCharges, value);
        }

        private TransportCompany freightInCharge;
        public TransportCompany FreightInCharge
        {
            get => freightInCharge;
            set => SetProperty(ref freightInCharge, value);
        }

        private TypeStandard place;
        public TypeStandard Place
        {
            get => place;
            set => SetProperty(ref place, value);
        }

        private TypeStandard typeOfRemittance;
        public TypeStandard TypeOfRemittance
        {
            get => typeOfRemittance;
            set => SetProperty(ref typeOfRemittance, value);
        }

        private MvxObservableCollection<PaymentMethod> paymentMethods;
        public MvxObservableCollection<PaymentMethod> PaymentMethods
        {
            get => paymentMethods;
            set => SetProperty(ref paymentMethods, value);
        }

        private PaymentMethod paymentMethod;
        public PaymentMethod PaymentMethod
        {
            get => paymentMethod;
            set => SetProperty(ref paymentMethod, value);
        }

        //private OpportunityStatus status;
        //public OpportunityStatus Status
        //{
        //    get => status;
        //    set => SetProperty(ref status, value);
        //}

        private MvxObservableCollection<Company> companies;
        public MvxObservableCollection<Company> Companies
        {
            get => companies;
            set => SetProperty(ref companies, value);
        }

        private Company company;
        public Company Company
        {
            get => company;
            set
            {
                SetProperty(ref company, value);
                CargarAsistentes();
            }
        }

        private MvxObservableCollection<User> assistants;
        public MvxObservableCollection<User> Assistants
        {
            get => assistants;
            set => SetProperty(ref assistants, value);
        }

        private User assistant;
        public User Assistant
        {
            get => assistant;
            set => SetProperty(ref assistant, value);
        }

        private MvxObservableCollection<User> sellers;
        public MvxObservableCollection<User> Sellers
        {
            get => sellers;
            set => SetProperty(ref sellers, value);
        }

        private User seller;
        public User Seller
        {
            get => seller;
            set => SetProperty(ref seller, value);
        }

        private MvxObservableCollection<Deposit> deposits;
        public MvxObservableCollection<Deposit> Deposits
        {
            get => deposits;
            set => SetProperty(ref deposits, value);
        }

        private Deposit deposit;
        public Deposit Deposit
        {
            get => deposit;
            set => SetProperty(ref deposit, value);
        }

        private string selectedClosedLostStatusCause;
        public string SelectedClosedLostStatusCause
        {
            get => selectedClosedLostStatusCause;
            set => SetProperty(ref selectedClosedLostStatusCause, value);
        }

        private Customer selectedCustomer;
        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set => SetProperty(ref selectedCustomer, value);
        }

        private CustomerAddress customerAddress;
        public CustomerAddress CustomerAddress
        {
            get => customerAddress;
            set => SetProperty(ref customerAddress, value);
        }

        private MvxObservableCollection<PaymentCondition> paymentConditions;
        public MvxObservableCollection<PaymentCondition> PaymentConditions
        {
            get => paymentConditions;
            set => SetProperty(ref paymentConditions, value);
        }
        private MvxObservableCollection<Provider> providers;
        public MvxObservableCollection<Provider> Providers
        {
            get => providers;
            set => SetProperty(ref providers, value);
        }
        private Provider provider;
        public Provider Provider
        {
            get => provider;
            set => SetProperty(ref provider, value);
        }

        private PaymentCondition condition;
        public PaymentCondition Condition
        {
            get => condition;
            set => SetProperty(ref condition, value);
        }


        private double valorDescuento;
        public double ValorDescuento
        {
            get => valorDescuento;
            set
            {
                SetProperty(ref valorDescuento, value);
                ConvertirDescuentoStr(this.valorDescuento);
            }
        }

        private void ConvertirDescuentoStr(double valorDescuento)
        {
            if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                LblDiscountResult = valorDescuento.ToString("N2", new CultureInfo("es-ES"));
            }
            else
            {
                LblDiscountResult = valorDescuento.ToString("N2", new CultureInfo("en-US"));
            }
        }

        private string lblDiscountResult;
        public string LblDiscountResult
        {
            get => lblDiscountResult;
            set => SetProperty(ref lblDiscountResult, value);
        }

        private int orderDiscount;
        public int OrderDiscount
        {
            get => orderDiscount;
            set
            {
                SetProperty(ref orderDiscount, value);
                //CalcularDescuento(this.OrderDiscount);
            }
        }


        private double total;
        public double Total
        {
            get => total;
            set
            {
                SetProperty(ref total, value);
                ConvertirTotalStr(Convert.ToDecimal(total));
            }
        }
        private string totalOfOrderStr;
        public string TotalOfOrderStr
        {
            get => totalOfOrderStr;
            set
            {
                SetProperty(ref totalOfOrderStr, value);
            }
        }
        private bool fleteChecked;
        public bool FleteChecked
        {
            get => fleteChecked;
            set
            {
                SetProperty(ref fleteChecked, value);
                if(!fleteChecked)
                {
                    FreightInCharge = null;
                }
            }
        }

        private void ConvertirTotalStr(decimal total)
        {
            if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                TotalOfOrderStr = Total.ToString("N2", new CultureInfo("es-ES"));
            }
            else
            {
                TotalOfOrderStr = Total.ToString("N2", new CultureInfo("en-US"));
            }

            var s = TotalOfOrderStr;
        }

        private MvxObservableCollection<AttachFile> attachFiles;
        public MvxObservableCollection<AttachFile> AttachFiles
        {
            get => attachFiles;
            set => SetProperty(ref attachFiles, value);
        }

        private bool enableForEdit;
        public bool EnableForEdit
        {
            get => enableForEdit;
            set => SetProperty(ref enableForEdit, value);
        }
        #endregion

        // Events
        public event EventHandler<Product> ShowEditProductPopup;
        public delegate void EventHandlerOrder(bool created);
        public event EventHandlerOrder NewOrderCreated;
        public event EventHandler<List<CustomerAddress>> ShowAddressPopup;
        public event EventHandler<OrderNote> ShowConfirmPopup;

        // Commands
        public Command SelectClientCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }
        public Command RemoveFileCommand { get; }
        public Command CerrarOportunidad { get; }
        public Command CustomerAddressCommand { get; }

        public Command SavePedidoCommand { get; }

        public OpportunityProducts editingOpportunityDetail { get; set; }

        public readonly IMvxNavigationService navigationService;

        public readonly IPrometeoApiService prometeoApiService;

        //private readonly IOfflineDataService offlineDataService;
        IMapper mapper;
        public CreateOrderViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService)//, IOfflineDataService offlineDataService
        {
            try
            {
                var mapperConfig = new MapperConfiguration(m =>
                {
                    m.AddProfile(new MappingProfile());
                });

                mapper = mapperConfig.CreateMapper();

                StackInfo = true;
                StackProductos = false;
                StackAdjunto = false;

                data = new ApplicationData();

                this.navigationService = navigationService;
                this.prometeoApiService = prometeoApiService;
                //this.offlineDataService = offlineDataService;

                SelectClientCommand = new Command(async () => await SelectClientAsync());
                AddProductCommand = new Command(async () => await AddProductAsync());
                RemoveProductCommand = new Command<OrderNote.ProductOrder>(RemoveProduct);
                RemoveFileCommand = new Command<AttachFile>(RemoveFile);
                EditProductCommand = new Command<OrderNote.ProductOrder>(EditProduct);
                CustomerAddressCommand = new Command(async () => await CustomerAddressMethod());

                SavePedidoCommand = new Command(async () => await SaveOrder());

                CargarTipoRemito();
                CargarLugarPago();
                CargarFleteCargo();

            }
            catch(Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e",$"{e.Message}","aceptar"); return;
            }
        }

        private async void CargarSellers()
        {
            try
            {
                if (Company == null) return;

                var red = await Connection.SeeConnection();

                if (red)
                {
                    var sellers = await prometeoApiService.GetUsersByRolUserVending(data.LoggedUser.Token, "Vendedor");

                    if (sellers != null)
                    {
                        Sellers = new MvxObservableCollection<User>(sellers.OrderBy(x => x.CodeFullName));

                        if (Order.SellerId.HasValue)
                        {
                            Seller = Sellers.FirstOrDefault(x => x.Id == Order.SellerId.Value);
                        }
                    }
                }
                else
                {

                    //var providersTable = OfflineDatabase.GetProviders();

                    //if (providersTable != null)
                    //{
                    //    Providers = new MvxObservableCollection<Provider>(mapper.Map<List<Provider>>(providersTable
                    //        .Where(x => x.IdCompany == data.LoggedUser.CompanyId)).OrderBy(x => x.Name));
                    //}

                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private async void CargarDepositos()
        {
            try
            {
                var red = await Connection.SeeConnection();

                if (red)
                {
                    var deposits = await prometeoApiService.GetDeposits($"Deposit/Mobile/GetByCompanyId", data.LoggedUser.Token);

                    if (deposits != null)
                    {
                        Deposits = new MvxObservableCollection<Deposit>(deposits.OrderBy(x => x.Name));

                        if (Order.DepositId.HasValue)
                        {
                            Deposit = Deposits.FirstOrDefault(x => x.Id == Order.DepositId.Value);
                        }
                    }
                }
                else
                {

                    //var providersTable = OfflineDatabase.GetProviders();

                    //if (providersTable != null)
                    //{
                    //    Providers = new MvxObservableCollection<Provider>(mapper.Map<List<Provider>>(providersTable
                    //        .Where(x => x.IdCompany == data.LoggedUser.CompanyId)).OrderBy(x => x.Name));
                    //}

                }

            }
            catch (Exception)
            {
                return;
            }
        }


        private async void GetStatusToOrderNote()
        {
            try
            {

                var red = await Connection.SeeConnection();

                if (red)
                {
                    var status = await prometeoApiService.GetStatusOrderNote(data.LoggedUser.Token);
                    OrderStatus = new MvxObservableCollection<StatusOrderNote>(status);

                    Order.StatusOrderNote = Order.OrderStatus != 0
                        ? OrderStatus.FirstOrDefault(x => x.Id == Order.OrderStatus)
                        : OrderStatus.FirstOrDefault(x => x.Name.Contains("Pend"));
                }
                else
                {
                }
            }
            catch (Exception e)
            {
                var m = e.Message;
                return;
            }
        }

        public async void CargarProviders()
        {
            try
            {
                var red = await Connection.SeeConnection();

                if (red)
                {
                    var providers = await prometeoApiService.GetProvidersByType(8, data.LoggedUser.Token);

                    if (providers != null)
                    {
                        Providers = new MvxObservableCollection<Provider>(providers.OrderBy(x => x.Name));

                        if(Order.ProviderId.HasValue)
                        {
                            Provider = Providers.FirstOrDefault(x => x.Id == Order.ProviderId.Value);
                        }
                    }
                }
                else
                {

                    var providersTable = OfflineDatabase.GetProviders();

                    if (providersTable != null)
                    {
                        Providers = new MvxObservableCollection<Provider>(mapper.Map<List<Provider>>(providersTable
                            .Where(x => x.IdCompany == data.LoggedUser.CompanyId)).OrderBy(x => x.Name));
                    }

                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", $"{e.Message} - Transporte", "Aceptar"); return;
            }
        }

        private async void CargarMedioPago()
        {
            try
            {
                var red = await Connection.SeeConnection();

                if(red)
                {
                    var mediosPago = await prometeoApiService.GetPaymentMethod(Company.Id, data.LoggedUser.Language.abbreviation.ToLower(), data.LoggedUser.Token);

                    if(mediosPago != null)
                    {
                        PaymentMethods = new MvxObservableCollection<PaymentMethod>(mediosPago.OrderBy(x => x.name));

                        if(Order.PaymentMethodId != null)
                        {
                            PaymentMethod = PaymentMethods.FirstOrDefault(x => x.id == Order.PaymentMethodId);
                        }
                    }
                }
                else
                {
                    var paymentMethod = OfflineDatabase.GetPaymentMethod();

                    if(paymentMethod != null)
                    {
                        PaymentMethods = new MvxObservableCollection<PaymentMethod>(mapper.Map<List<PaymentMethod>>(paymentMethod
                            .Where(x => x.CompanyId == data.LoggedUser.CompanyId)).OrderBy(x => x.name));

                        if(Order?.PaymentMethodId != null)
                        {
                            PaymentMethod= PaymentMethods.FirstOrDefault(x => x.id == Order.PaymentMethodId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", $"{e.Message} - Metodo de Pago", "Aceptar"); return;
            }
        }

        private async void CargarFleteCargo()
        {
            try
            {
                var user = data.LoggedUser;

                string lang = user.Language.abbreviation.ToLower();

                var red = await Connection.SeeConnection();

                if (red)
                {
                    if (Company != null)
                    {
                        var fletes = await prometeoApiService.GetTransport(lang, Company.Id, user.Token);

                        if (fletes != null)
                        {
                            FreightInCharges = new MvxObservableCollection<TransportCompany>(fletes);

                            if (Order.TransportCompanyId != null)
                            {
                                FreightInCharge = FreightInCharges.FirstOrDefault(x => x.Id == Order.TransportCompanyId);
                            }
                        }
                    }
                }
                else
                {
                    var trasnport = OfflineDatabase.GetTransportCompany();

                    if(trasnport!= null)
                    {
                        FreightInCharges = new MvxObservableCollection<TransportCompany>(mapper.Map<List<TransportCompany>>(trasnport
                            .Where(x => x.CompanyId == data.LoggedUser.CompanyId)));
                    }
                }
            }
            catch(Exception e)
            {
                var m = e.Message;
                return;
            }
        }

        private async void CargarAsistentes()
        {
            try
            {
                var user = data.LoggedUser;

                var red = await Connection.SeeConnection();

                if (red)
                {
                    var asistentes = await prometeoApiService.GetUsersByRolUserVending(data.LoggedUser.Token, "Asistente Comercial");

                    Assistants = new MvxObservableCollection<User>(asistentes);

                    if (Order != null)
                    {
                        if (Order.commercialAssistantId != null)
                        {
                            Assistant = Assistants.FirstOrDefault(x => x.Id == Order.commercialAssistantId);
                        }
                    }

                }
                else
                {
                    var assistantsDb = OfflineDatabase.GetAssistantComercial();

                    if(assistantsDb != null)
                    {
                        Assistants = new MvxObservableCollection<User>(mapper.Map<List<User>>(assistantsDb
                            .Where(x => x.CompanyId == data.LoggedUser.CompanyId)));

                        if (Order != null)
                        {
                            if (Order.commercialAssistantId != null)
                            {
                                Assistant = Assistants.FirstOrDefault(x => x.Id == Order.commercialAssistantId);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", $"{e.Message} - Asistente Comercial", "Aceptar");
            }
        }

        private async Task CustomerAddressMethod()
        {
            if (SelectedCustomer != null)
            {
                ShowAddressPopup?.Invoke(this, SelectedCustomer.Addresses.ToList());
            }
            else
            {
                if(data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
                {
                    await Application.Current.MainPage.DisplayAlert("Atención","Seleccione un Cliente","Aceptar");
                    return;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atention", "Select a Customer.", "Acept");
                    return;
                }
            }
        }

        private void CargarLugarPago()
        {
            var user = data.LoggedUser;

            string lang = user.Language.abbreviation.ToLower();

            if (lang == "es" || lang.Contains("spanish"))
            {
                PlaceOfPayment.Add(new TypeStandard { Id = 1, Description = "En Origen" });
                PlaceOfPayment.Add(new TypeStandard { Id = 2, Description = "En Destino" });
            }
            else
            {
                PlaceOfPayment.Add(new TypeStandard { Id = 1, Description = "In Origin" });
                PlaceOfPayment.Add(new TypeStandard { Id = 2, Description = "At Destination" });
            }
        }

        private void CargarTipoRemito()
        {
            var user = data.LoggedUser;

            string lang = user.Language.abbreviation.ToLower();

            if (lang == "es" || lang.Contains("spanish"))
            {
                TypeOfRemittances.Add(new TypeStandard { Id = 1, Description = "Normal/Estándar" });
                TypeOfRemittances.Add(new TypeStandard { Id = 2, Description = "En Consignación" });
                TypeOfRemittances.Add(new TypeStandard { Id = 3, Description = "FC Anticipada" });
            }
            else
            {
                TypeOfRemittances.Add(new TypeStandard { Id = 1, Description = "Normal/Stándard" });
                TypeOfRemittances.Add(new TypeStandard { Id = 2, Description = "On Consignment" });
                TypeOfRemittances.Add(new TypeStandard { Id = 3, Description = "FC Early" });
            }
        }

        private async Task SaveOrder()
        {
            try
            {
                IsLoading = true;

                if (Company == null ||
                    SelectedCustomer == null ||
                    TypeOfRemittance == null ||
                    PaymentMethod == null ||
                    Assistant == null ||
                    Seller == null)
                {
                    IsLoading = false;

                    if (data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                        data.LoggedUser.Language.abbreviation.Contains("spanish"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención",
                            "Faltan ingresar datos obligatorios.", "Aceptar");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "Required data to be entered.",
                            "Acept");
                        return;
                    }
                }

                if (Company.Id == 7 && Seller == null)
                {
                    if (data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                        data.LoggedUser.Language.abbreviation.Contains("spanish"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención",
                            "Faltan ingresar datos obligatorios.", "Aceptar");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "Required data to be entered.",
                            "Acept");
                        return;
                    }
                }

                if (Company.Id != 7)
                {
                    if (Place == null)
                    {
                        IsLoading = false;

                        if (data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                            data.LoggedUser.Language.abbreviation.Contains("spanish"))
                        {
                            await Application.Current.MainPage.DisplayAlert("Atención",
                                "Faltan ingresar datos obligatorios.", "Aceptar");
                            return;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Attention", "Required data to be entered.",
                                "Acept");
                            return;
                        }
                    }
                }

                if (Company.externalErpId != null)
                {
                    if (Condition == null)
                    {
                        IsLoading = false;

                        if (data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                            data.LoggedUser.Language.abbreviation.Contains("spanish"))
                        {
                            await Application.Current.MainPage.DisplayAlert("Atención",
                                "Seleccione una condición de pago.", "Aceptar");
                            return;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Attention", "Select a payment term.",
                                "Acept");
                            return;
                        }
                    }
                }
                else
                {
                    if (TypeOfRemittance.Description != "En Consignación" &&
                        TypeOfRemittance.Description != "On Consignment")
                    {
                        if (Condition == null)
                        {
                            IsLoading = false;

                            if (data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                                data.LoggedUser.Language.abbreviation.Contains("spanish"))
                            {
                                await Application.Current.MainPage.DisplayAlert("Atención",
                                    "Seleccione una condición de pago.", "Aceptar");
                                return;
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Attention", "Select a payment term.",
                                    "Acept");
                                return;
                            }
                        }
                    }
                }

                if (Order.products == null || Order.products.Count() == 0)
                {
                    IsLoading = false;

                    if (data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                        data.LoggedUser.Language.abbreviation.Contains("spanish"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "Necesita asociar productos",
                            "Aceptar");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "You need to associate products.",
                            "Acept");
                        return;
                    }
                }

                var nuevaOrder = new OrderNote
                {
                    companyId = Company.Id,
                    Description = Order.Description,
                    currencyId = 2,
                    customerId = SelectedCustomer.Id,
                    discount = OrderDiscount,
                    fecha = Order.fecha,
                    total = Convert.ToDecimal(Total),
                    divisionCuentaId = Company.ExternalId.Value,
                    talon = 88, //puede ser null
                    tipoComprobante = 8, //puede ser null
                    tipoCuentaId = 1, //puede ser null
                    tipoServicioId = 50, //puede ser null
                    DeliveryAddress = Order.DeliveryAddress,
                    DeliveryDate = Order.DeliveryDate,
                    DeliveryResponsible = Order.DeliveryResponsible,
                    OCCustomer = Order.OCCustomer,
                    PlacePayment = Place?.Id,
                    RemittanceType = typeOfRemittance.Id,
                    PaymentMethodId = PaymentMethod.id,
                    commercialAssistantId = Assistant.Id,
                    ProviderId = Provider?.Id,
                    OpportunityOrderNoteAttachFile = new List<AttachFile>(),
                    SellerId = Seller?.Id,
                    DepositId = Deposit?.Id,
                };

                nuevaOrder.OpportunityOrderNoteAttachFile = AttachFiles != null
                    ? AttachFiles.ToList()
                    : new List<AttachFile>();

                if (Condition != null)
                {
                    nuevaOrder.paymentConditionId = Condition.Id;
                }

                if (FreightInCharge != null)
                {
                    nuevaOrder.TransportCompanyId = FreightInCharge.Id;
                }

                if (nuevaOrder.DeliveryDate == null)
                {
                    nuevaOrder.DeliveryDate = DateTime.Now.Date;
                    nuevaOrder.ETD = DateTime.Now.Date;
                }
                else
                {
                    nuevaOrder.ETD = Order.DeliveryDate.Value;
                }

                if (Order.opportunityId == 0 || Order.opportunityId == null)
                {
                    nuevaOrder.opportunityId = null;
                    nuevaOrder.products = Order.products;
                }
                else
                {
                    nuevaOrder.opportunityId = Order.opportunityId;
                    nuevaOrder.products = Order.products;
                }

                ShowConfirmPopup?.Invoke(this, nuevaOrder);

            }
            catch (Exception e)
            {
                return;
            }
        }

        public async Task ConfirmaOrderNote(OrderNote nuevaOrder)
        {
            try
            {
                var red = await Connection.SeeConnection();

                if (Order.id == 0 && Order.idOffline == 0)
                {
                    nuevaOrder.OrderStatus = Order.StatusOrderNote.Id;

                    if (red)
                    {
                        var respuesta = await prometeoApiService.CreateOrderNote(nuevaOrder);

                        if (respuesta != null)
                        {
                            if (respuesta.opportunityId > 0)
                            {
                                var send = new OpportunityPost
                                {
                                    branchOfficeId = Order.customer.Id,
                                    closedDate = DateTime.Now,
                                    closedReason = "",
                                    customerId = Order.customer.Id,
                                    description = Order.oppDescription,
                                    opportunityProducts = new List<OpportunityPost.ProductSend>(),
                                    opportunityStatusId = 4,
                                    totalPrice = Total,
                                    companyId = respuesta.companyId.Value,
                                };

                                send.opportunityProducts = listaProductos(Order.Details);

                                var opp = new Opportunity();

                                await prometeoApiService.SaveOpportunityEdit(send, respuesta.opportunityId.Value,
                                    data.LoggedUser.Token,
                                    opp);

                                await navigationService.ChangePresentation(
                                    new MvxPopPresentationHint(typeof(PedidosViewModel)));
                                await navigationService.Navigate<PedidosViewModel>();
                            }
                            else
                            {
                                await navigationService.Close(this);

                                NewOrderCreated(true);
                            }
                        }
                    }
                    else
                    {
                        nuevaOrder.company = Company;
                        nuevaOrder.customer = SelectedCustomer;
                        var saved = await OfflineDatabase.SaveOrderNote(mapper.Map<OrderNoteTable>(nuevaOrder));

                        await navigationService.Close(this);
                        NewOrderCreated(true);
                    }
                }
                else
                {
                    if (red)
                    {
                        nuevaOrder.OrderStatus = Order.StatusOrderNote.Id;
                        nuevaOrder.id = Order.id;
                        await prometeoApiService.UpdateOrderNote(nuevaOrder, data.LoggedUser.Token);

                        NewOrderCreated(true);
                        await navigationService.Close(this);
                    }
                    else
                    {
                        nuevaOrder.OrderStatus = Order.StatusOrderNote.Id;
                        nuevaOrder.company = Company;
                        nuevaOrder.customer = SelectedCustomer;
                        nuevaOrder.idOffline = Order.idOffline;

                        var ok = await OfflineDatabase.UpdateOrderNote(mapper.Map<OrderNoteTable>(nuevaOrder));

                        if (ok)
                        {
                            NewOrderCreated(true);
                            await navigationService.Close(this);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                    data.LoggedUser.Language.abbreviation.Contains("spanish"))
                {
                    await Application.Current.MainPage.DisplayAlert("Atención",
                        $"No se pudo guardar el PV, intente nuevamente mas tarde o comuniquese con soporte.",
                        "Aceptar");
                    return;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Attention",
                        "No se pudo guardar el PV, intente nuevamente mas tarde o comuniquese con soporte.", "Acept");
                    return;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public List<OpportunityPost.ProductSend> listaProductos(MvxObservableCollection<OpportunityProducts> details)
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

        private MvxObservableCollection<OrderNote.ProductOrder> DefinirProductos(List<OpportunityProducts> details)
        {
            var lista = new MvxObservableCollection<OrderNote.ProductOrder>();

            foreach (var item in details)
            {
                var prod = new OrderNote.ProductOrder
                {
                    arancel = 0,
                    bonificacion = 0,
                    companyProductPresentationId = item.productId,
                    discount = item.Discount,
                    price = item.Price,
                    quantity = item.Quantity,
                    subtotal = item.Total
                };

                lista.Add(prod);
            }

            return lista;
        }

        private async void CargarCondiciones()
        {
            try
            {
                var user = data.LoggedUser;

                var red = await Connection.SeeConnection();

                if (red)
                {

                    var condiciones = new MvxObservableCollection<PaymentCondition>(await prometeoApiService.GetPaymentConditions(user.Token, Company.Id));

                    if (Company.externalErpId == null)
                    {
                        PaymentConditions = new MvxObservableCollection<PaymentCondition>(condiciones.OrderBy(x => x.Description));

                        if (Order != null)
                        {
                            if (Order.paymentConditionId > 0)
                            {
                                Condition = PaymentConditions.FirstOrDefault(x => x.Id == Order.paymentConditionId);
                            }
                        }
                    }
                    else
                    {
                        var conExternal = condiciones.Where(x => x.Code > 0).ToList();

                        PaymentConditions = new MvxObservableCollection<PaymentCondition>(conExternal.OrderBy(x => x.Description));

                        if (Order != null)
                        {
                            if (Order.paymentConditionId > 0)
                            {
                                Condition = PaymentConditions.FirstOrDefault(x => x.Id == Order.paymentConditionId);
                            }
                        }
                    }

                }
                else
                {
                    var conditionsDb = OfflineDatabase.GetPaymentCondition();

                    if (conditionsDb != null)
                    {
                        conditionsDb = conditionsDb.OrderBy(x => x.Description).ToList();
                        PaymentConditions = new MvxObservableCollection<PaymentCondition>(mapper.Map<List<PaymentCondition>>(conditionsDb));

                        if (Order != null)
                        {
                            if (Order.paymentConditionId > 0)
                            {
                                Condition = PaymentConditions.FirstOrDefault(x => x.Id == Order.paymentConditionId);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", $"{e.Message} - Condiciones de pago", "Aceptar");
                return;
            }
        }

        public async override void Prepare(OrderNote theOrder)
        {
            try
            {
                var user = data.LoggedUser;

                if (theOrder.id > 0)
                {
                    if (theOrder.sentToErp.HasValue)
                    {
                        EnableForEdit = false;
                    }
                    else
                    {
                        EnableForEdit = true;
                    }

                    Order = await prometeoApiService.GetOrdersById(theOrder.id, user.Token);

                    AttachFiles = new MvxObservableCollection<AttachFile>(Order.OpportunityOrderNoteAttachFile);

                    SelectedCustomer = Order.customer;
                    Company = Order.company;
                    ChangeStatusEnable = Company.externalErpId.HasValue ? false : true;


                    GetStatusToOrderNote();
                    CargarAsistentes();
                    CargarSellers();
                    CargarProviders();
                    CargarDepositos();

                    TypeOfRemittance = TypeOfRemittances.FirstOrDefault(x => x.Id == Order.RemittanceType);
                    Place = PlaceOfPayment.FirstOrDefault(x => x.Id == Order.PlacePayment);

                    if (Order.TransportCompanyId.HasValue)
                    {
                        FleteChecked = true;

                    }
                    CargarFleteCargo();

                    CargarMedioPago();
                    SelectedCustomer.Addresses.Add(new CustomerAddress { Address = Order.DeliveryAddress });

                    CustomerAddress = SelectedCustomer.Addresses.FirstOrDefault();

                    CargarCondiciones();

                    Total = Convert.ToDouble(Order.total);
                    OrderDiscount = Order.discount;

                    ActualizarTotal(Order.products);
                }
                else
                {
                    EnableForEdit = true;

                    Order = theOrder;
                    //Order.OrderStatus = 1;

                    //AjustarEstado(user.Language.abbreviation, Order.OrderStatus);

                    if (Order.customer != null)
                    {
                        SelectedCustomer = Order.customer;
                    }
                    if (Order.company != null)
                    {
                        Company = Order.company;
                        ChangeStatusEnable = Company.externalErpId.HasValue ? false : true;
                        CargarSellers();
                        CargarCondiciones();
                        CargarMedioPago();
                    }

                    GetStatusToOrderNote();
                    CargarProviders();
                    CargarDepositos();

                    if (Order.PlacePayment > 0)
                    {
                        Place = PlaceOfPayment.FirstOrDefault(x => x.Id == Order.PlacePayment);
                    }
                    if(Order.RemittanceType > 0)
                    {
                        TypeOfRemittance = TypeOfRemittances.FirstOrDefault(x => x.Id == Order.RemittanceType);
                    }
                    //if (Order.paymentConditionId > 0)
                    //{
                    //    Condition = PaymentConditions.FirstOrDefault(x => x.Id == Order.paymentConditionId);
                    //}
                    //if (Order.PaymentMethodId > 0)
                    //{
                    //    PaymentMethod = PaymentMethods.FirstOrDefault(x => x.id == Order.PaymentMethodId);
                    //}

                    if (theOrder.Details == null)
                    {
                        if (theOrder.products != null)
                        {
                            Order.products = theOrder.products;
                        }
                        else
                        {
                            Order.products = new MvxObservableCollection<OrderNote.ProductOrder>();
                        }
                    }
                    else
                    {
                        Order.products = AsignarProductos(theOrder.Details);
                        ActualizarTotal(Order.products);
                    }
                }
            }
            catch(Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", e.Message, "Aceptar");
                return;
            }
        }

        private MvxObservableCollection<OrderNote.ProductOrder> AsignarProductos(MvxObservableCollection<OpportunityProducts> details)
        {
            var listaProductos = new MvxObservableCollection<OrderNote.ProductOrder>();

            foreach (var item in details)
            {
                listaProductos.Add(new OrderNote.ProductOrder
                {
                    companyProductPresentationId = item.productId,
                    companyProductPresentation = item.product,
                    discount = item.Discount,
                    price = item.Price,
                    quantity = item.Quantity,
                    subtotal = item.Total,
                    subtotalProduct = item.Price * item.Quantity,
                    discountPrice = CalcularDescuento(item.Quantity, item.Price, item.Discount)
                });
            }

            return listaProductos;
        }

        private double CalcularDescuento(int quantity, double price, int discount)
        {
            var totalProduct = quantity * price;
            var descuento = Convert.ToDecimal(discount) / 100;

            return totalProduct * Convert.ToDouble(descuento);
        }

        public void FinishEditProduct((double Price, int Quantity, int Discount) args)
        {
            if (editingOpportunityDetail == null)
            {
                throw new InvalidOperationException("No hay ningún detalle para editar. No debería pasar esto.");
            }

            editingOpportunityDetail.Price = args.Price;
            editingOpportunityDetail.Quantity = args.Quantity;
            editingOpportunityDetail.Discount = args.Discount;

            var temp = args.Price * args.Quantity;

            if (args.Discount == 0)
            {
                editingOpportunityDetail.Total = temp;
            }
            else
            {

                editingOpportunityDetail.Total = temp - (temp * args.Discount / 100);
            }

            var listaProd = new MvxObservableCollection<OrderNote.ProductOrder>(Order.products);

            var prodEdit = listaProd.Where(x => x.companyProductPresentationId == editingOpportunityDetail.productId).FirstOrDefault();
            var name = prodEdit.companyProductPresentation.name;

            Order.products.Remove(prodEdit);

            var product = new OrderNote.ProductOrder
            {
                discount = editingOpportunityDetail.Discount,
                price = editingOpportunityDetail.Price,
                quantity = editingOpportunityDetail.Quantity,
                subtotal = editingOpportunityDetail.Total,
                companyProductPresentationId = editingOpportunityDetail.productId,
                companyProductPresentation = editingOpportunityDetail.product,
            };

            Order.products.Add(product);


            editingOpportunityDetail = null;
            ActualizarTotal(Order.products);
        }

        public void CancelEditProduct()
        {
            editingOpportunityDetail = null;
        }

        private async Task SelectClientAsync()
        {
            var dExport = new DataExport()
            {
                CompanyId = Company.Id,
            };

            if(Company.externalErpId != null)
            {
                dExport.WhitExternal = true;
            }

            var customer = await navigationService.Navigate<CustomersViewModel, DataExport, Customer>(dExport);

            try
            {
                IsLoading = true;
                SelectedCustomer = customer;
                if(SelectedCustomer != null && SelectedCustomer.AccountOwnerId.HasValue && Sellers != null) Seller = Sellers.FirstOrDefault(x => x.Id == SelectedCustomer.AccountOwnerId.Value);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", "Ocurrió un error al obtener el cliente.Compruebe su conexión a internet.", "Aceptar");
                return;
                
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddProductAsync()
        {
            try
            {
                var dExport = new DataExport()
                {
                    CompanyId = Company.Id
                };

                var detail = await navigationService.Navigate<ProductsViewModel, DataExport, OpportunityProducts>(dExport);

                if (detail != null)
                {
                    var product = new OrderNote.ProductOrder
                    {
                        discount = detail.Discount,
                        price = detail.product.price,
                        quantity = detail.Quantity,
                        subtotal = detail.Total,
                        companyProductPresentationId = detail.productId,
                        companyProductPresentation = detail.product,
                        discountPrice = detail.DiscountPrice,
                        subtotalProduct = detail.SubtotalProducts,
                    };

                    if (Order.products == null)
                    {
                        Order.products = new MvxObservableCollection<OrderNote.ProductOrder>();
                    }

                    Order.products.Add(product);

                    ActualizarDescuento();
                    ActualizarTotal(Order.products);
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", $"{e.Message}", "aceptar"); return;
            }
        }

        private void ActualizarDescuento()
        {
            try
            {
                var idioma = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                var totalPro = Order.products.Sum(x => x.subtotal);
                var str = (totalPro * OrderDiscount / 100);
                if (idioma.ToLower().Contains("es"))
                {
                    var r = Convert.ToDouble(str.ToString().Replace(",", "."));
                    ValorDescuento = r;
                }
                else
                {
                    ValorDescuento = str;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void RemoveProduct(OrderNote.ProductOrder detail)
        {
            try
            { 
                Order.products.Remove(detail);
                ActualizarDescuento();
                ActualizarTotal(Order.products);

            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Atención", $"{e.Message}", "aceptar"); return;
            }
        }

        private void RemoveFile(AttachFile file)
        {
            try
            {
                AttachFiles.Remove(file);
                //Order.OpportunityOrderNoteAttachFile.Remove(file);
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar");
                return;
            }
        }

        private void EditProduct(OrderNote.ProductOrder detail)
        {
            try
            {
                var product = new Product()
                {

                    name = detail.companyProductPresentation.name,
                    price = detail.price,
                    Id = detail.companyProductPresentationId,
                    //stock = detail.quantity,
                    Discount = detail.discount,
                    quantity = detail.quantity,
                };

                editingOpportunityDetail = ConvertProduct(detail);
                ShowEditProductPopup?.Invoke(this, product);

            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Atención", $"{e.Message}", "aceptar"); return;
            }
        }

        private OpportunityProducts ConvertProduct(OrderNote.ProductOrder detail)
        {
            return new OpportunityProducts
            {
                Discount = detail.discount,
                Price = detail.price,
                productId = detail.companyProductPresentationId,
                //product = detail.
                Quantity = detail.quantity,
                Total = detail.subtotal,
            };
        }

        public void ActualizarTotal(MvxObservableCollection<OrderNote.ProductOrder> details)
        {
            try
            {
                var idioma = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                if (ValorDescuento != null)
                {
                    if (idioma.Contains("es"))
                    {
                        Total = details.Sum(x => x.subtotal) - ValorDescuento;
                    }
                    else
                    {
                        var d1 = Convert.ToDouble(ValorDescuento);
                        var t1 = details.Sum(x => x.subtotal);
                        var r1 = t1 - d1;

                        Total = r1;
                    }
                }
                else
                {
                }
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

        public void ResetTotal(MvxObservableCollection<OrderNote.ProductOrder> details)
        {
            Total = details.Sum(x => x.subtotal);
        }

        public async void CloseAndBack()
        {
            await navigationService.Close(this);

            NewOrderCreated(true);
        }


        public async void AddFileToOrderNote(IEnumerable<FileResult> result)
        {
            try
            {
                if (AttachFiles == null) AttachFiles = new MvxObservableCollection<AttachFile>();

                foreach (FileResult fileResult in result)
                {
                    if (fileResult.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        fileResult.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                        fileResult.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        Stream stream1 = fileResult.OpenReadAsync().Result;

                        var streamTask = fileResult.OpenReadAsync().ContinueWith(
                            (task) =>
                            {
                                stream1 = task.Result;
                                var Image1 = ImageSource.FromStream(() => stream1);
                                var resource = Convert.ToBase64String(ImageSourceToByteArray(Image1));

                                AttachFiles.Add(new AttachFile
                                {
                                    //AssignmentId = AssignmentId,
                                    OpportunityOrderNoteId = Order.id,
                                    FilePath = $"data:image/jpg;base64," + resource,
                                    FileName = fileResult.FileName,
                                    MineType = ".jpeg",
                                    UploadDate = DateTime.Now
                                    //FileTypeId = number,
                                });
                            });

                        streamTask.Wait();
                    }
                    else
                    {
                        Stream stream1 = await fileResult.OpenReadAsync();

                        var fileName = fileResult.FileName;
                        var base64 = string.Empty;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            await stream1.CopyToAsync(ms);
                            byte[] bytes = ms.ToArray();
                            base64 = Convert.ToBase64String(bytes);
                        }

                        if (fileResult.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            AttachFiles.Add(new AttachFile()
                            {
                                OpportunityOrderNoteId = Order.id,
                                FilePath = $"data:application/pdf;base64," + base64,
                                FileName = fileResult.FileName,
                                MineType = ".pdf",
                                UploadDate = DateTime.Now
                            });
                        }

                        if (fileResult.FileName.EndsWith("docx", StringComparison.OrdinalIgnoreCase) ||
                        fileResult.FileName.EndsWith("doc", StringComparison.OrdinalIgnoreCase))
                        {
                            AttachFiles.Add(new AttachFile()
                            {
                                OpportunityOrderNoteId = Order.id,
                                FilePath = $"application/msword;base64," + base64,
                                FileName = fileResult.FileName,
                                MineType = ".docx",
                                UploadDate = DateTime.Now
                            });
                        }

                        if (fileResult.FileName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase) ||
                        fileResult.FileName.EndsWith("xls", StringComparison.OrdinalIgnoreCase))
                        {
                            AttachFiles.Add(new AttachFile()
                            {
                                OpportunityOrderNoteId = Order.id,
                                FilePath = $"application/vnd.ms-excel;base64," + base64,
                                FileName = fileResult.FileName,
                                MineType = ".xlsx",
                                UploadDate = DateTime.Now
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Información", $"No se pudo agregar el archivo.", "aceptar");
                return;
            }
        }

        public byte[] ImageSourceToByteArray(ImageSource source)
        {
            StreamImageSource streamImageSource = (StreamImageSource)source;
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Task<Stream> task = streamImageSource.Stream(cancellationToken);
            Stream stream = task.Result;

            byte[] b;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                b = ms.ToArray();
            }

            return b;
        }
        public void AddPictureToOrderNote(string filePath, string fileResource)
        {
            try
            {
                if (AttachFiles == null) AttachFiles = new MvxObservableCollection<AttachFile>();

                if (filePath.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    filePath.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                    filePath.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    AttachFiles.Add(new AttachFile
                    {
                        //AssignmentId = AssignmentId,
                        OpportunityOrderNoteId = Order.id,
                        FilePath = $"data:image/jpg;base64," + fileResource,
                        FileName = filePath,
                        MineType = ".jpeg",
                        UploadDate = DateTime.Now
                        //FileTypeId = number,
                    });
                }
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Información", $"No se pudo agregar el archivo.", "aceptar");
                return;
            }
        }

        public void Cancel()
        {
            try
            {
                IsLoading = false;
            }
            catch (Exception)
            {
                IsLoading = false;
            }
        }
    }
}
