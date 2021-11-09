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
using Xamarin.Essentials;
using System.Globalization;
using Core.Services;
using AutoMapper;
using Core.Helpers;

namespace Core.ViewModels
{
    public class CreateOrderViewModel : MvxViewModel<OrderNote>
    {
        private ApplicationData data;
        // Properties

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

        private MvxObservableCollection<OpportunityStatus> selectedStatus;
        public MvxObservableCollection<OpportunityStatus> OrderStatus
        {
            get => selectedStatus;
            set => SetProperty(ref selectedStatus, value);
        }

        private string orderStatusOrderStr;
        public string OrderStatusOrderStr
        {
            get => orderStatusOrderStr;
            set => SetProperty(ref orderStatusOrderStr, value);
        }

        public MvxObservableCollection<TypeStandard> TypeOfRemittances { get; set; } = new MvxObservableCollection<TypeStandard>();
        public MvxObservableCollection<PaymentMethod> PaymentMethods { get; set; } = new MvxObservableCollection<PaymentMethod>();
        public MvxObservableCollection<TypeStandard> PlaceOfPayment { get; set; } = new MvxObservableCollection<TypeStandard>();
        public MvxObservableCollection<FreightInCharge> FreightInCharges { get; set; } = new MvxObservableCollection<FreightInCharge>();

        private FreightInCharge freightInCharge;
        public FreightInCharge FreightInCharge
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

        private PaymentMethod paymentMethod;
        public PaymentMethod PaymentMethod
        {
            get => paymentMethod;
            set => SetProperty(ref paymentMethod, value);
        }

        private OpportunityStatus status;
        public OpportunityStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

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
                //CargarCondiciones();
                //CargarMedioPago();
            }
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
            set => SetProperty(ref valorDescuento, value);
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

        private void ConvertirTotalStr(decimal total)
        {
            if (data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
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

        // Events
        public event EventHandler<Product> ShowEditProductPopup;
        public event EventHandler NewOrderCreated;
        public event EventHandler<List<CustomerAddress>> ShowAddressPopup;

        // Commands
        public Command SelectClientCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }
        public Command CerrarOportunidad { get; }
        public Command CustomerAddressCommand { get; }

        public Command SavePedidoCommand { get; }

        public OpportunityProducts editingOpportunityDetail { get; set; }

        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IOfflineDataService offlineDataService;
        //private readonly IToastService toastService;

        public CreateOrderViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService, IOfflineDataService offlineDataService
                                          )//IToastService toastService
        {
            try
            {
                StackInfo = true;
                StackProductos = false;
                StackAdjunto = false;

                data = new ApplicationData();

                this.navigationService = navigationService;
                this.prometeoApiService = prometeoApiService;
                //this.toastService = toastService;
                this.offlineDataService = offlineDataService;

                SelectClientCommand = new Command(async () => await SelectClientAsync());
                AddProductCommand = new Command(async () => await AddProductAsync());
                RemoveProductCommand = new Command<OrderNote.ProductOrder>(RemoveProduct);
                EditProductCommand = new Command<OrderNote.ProductOrder>(EditProduct);
                CustomerAddressCommand = new Command(async () => await CustomerAddressMethod());

                SavePedidoCommand = new Command(async () => await SaveOrder());

                OrderStatus = new MvxObservableCollection<OpportunityStatus>();

                CargarEstados();
                //CargarEmpresas();
                CargarTipoRemito();
                CargarLugarPago();
                CargarFleteCargo();

            }
            catch(Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e",$"{e.Message}","aceptar"); return;
            }
        }
        private async void CargarMedioPago()
        {
            try
            {
                var red = await Connection.SeeConnection();

                if(red)
                {
                    var mediosPago = await prometeoApiService.GetPaymentMethod(Company.Id, data.LoggedUser.Language.ToLower(), data.LoggedUser.Token);

                    if(mediosPago != null)
                    {
                        PaymentMethods.Clear();
                        PaymentMethods.AddRange(mediosPago);

                        if(Order.PaymentMethodId != null)
                        {
                            PaymentMethod = PaymentMethods.FirstOrDefault(x => x.id == Order.PaymentMethodId);
                        }
                    }
                }
                else
                {
                    //obtener de Cache
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void CargarFleteCargo()
        {
            var user = data.LoggedUser;

            string lang = user.Language.ToLower();

            var red = await Connection.SeeConnection();

            if (red)
            {
                var fletes = await prometeoApiService.GetFreight(lang, user.Token, "Transport");

                if(fletes != null)
                {
                    FreightInCharges = new MvxObservableCollection<FreightInCharge>(fletes);
                }
            }
            else
            {

            }


            //if (lang == "es" || lang.Contains("spanish"))
            //{
            //    FreightInCharges.Add(new FreightInCharge { id = 1, name = "Empresa" });
            //    FreightInCharges.Add(new FreightInCharge { id = 2, name = "Cliente" });
            //}
            //else
            //{
            //    FreightInCharges.Add(new FreightInCharge { id = 1, name = "Company" });
            //    FreightInCharges.Add(new FreightInCharge { id = 2, name = "Customer" });
            //}
        }

        private async Task CustomerAddressMethod()
        {
            if (SelectedCustomer != null)
            {
                ShowAddressPopup?.Invoke(this, SelectedCustomer.Addresses.ToList());
            }
            else
            {
                if(data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
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

            string lang = user.Language.ToLower();

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

            string lang = user.Language.ToLower();

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
                if (Company == null ||
                    SelectedCustomer == null ||
                    TypeOfRemittance == null ||
                    Place == null ||
                    PaymentMethod == null)
                {
                    if (data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "Faltan ingresar datos obligatorios.", "Aceptar");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "Required data to be entered.", "Acept");
                        return;
                    }
                }
                
                if (TypeOfRemittance.Description != "En Consignación" && TypeOfRemittance.Description != "On Consignment")
                {
                    if(Condition == null)
                    {
                        if (data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
                        {
                            await Application.Current.MainPage.DisplayAlert("Atención", "Seleccione una condición de pago.", "Aceptar");
                            return;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Attention", "Select a payment term.", "Acept");
                            return;
                        }
                    }
                }

                if(Order.products == null || Order.products.Count() == 0)
                {
                    if (data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "Necesita asociar productos", "Aceptar");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "You need to associate products.", "Acept");
                        return;
                    }
                }

                var nuevaOrder = new OrderNote
                {
                    companyId = Company.Id,
                    Description = Order.Description,
                    currencyId = 1,
                    customerId = SelectedCustomer.Id,
                    discount = OrderDiscount,
                    fecha = Order.fecha,
                    orderStatus = 1,
                    paymentConditionId = Condition.id,
                    total = Convert.ToDecimal(Total),
                    cuenta = SelectedCustomer.ExternalId,
                    divisionCuentaId = Company.externalId.Value,
                    talon = 88,                          //puede ser null
                    tipoComprobante = 8,                 //puede ser null
                    tipoCuentaId = 1,                    //puede ser null
                    tipoServicioId = 50,                  //puede ser null
                    DeliveryAddress = Order.DeliveryAddress,
                    DeliveryDate = Order.DeliveryDate,
                    DeliveryResponsible = Order.DeliveryResponsible,
                    OCCustomer = Order.OCCustomer,
                    PlacePayment = Place.Id,
                    RemittanceType = typeOfRemittance.Id,
                    PaymentMethodId = PaymentMethod.id,
                };

                if (nuevaOrder.DeliveryDate == null)
                {
                    nuevaOrder.DeliveryDate = DateTime.Now.Date;
                }

                if (Order.opportunityId == 0 || Order.opportunityId == null)
                {
                    nuevaOrder.opportunityId = null;
                    nuevaOrder.products = Order.products;
                }
                else
                {
                    nuevaOrder.opportunityId = Order.opportunityId;
                    nuevaOrder.products = DefinirProductos(Order.Details.ToList());
                }

                if (Order.id == 0)
                {
                    var red = await Connection.SeeConnection();

                    if (!red)
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
                                    totalPrice = Total
                                };

                                send.opportunityProducts = listaProductos(Order.Details);

                                var opp = new Opportunity();

                                await prometeoApiService.SaveOpportunityEdit(send, Order.id, data.LoggedUser.Token, opp);
                            }
                        }

                        await navigationService.ChangePresentation(new MvxPopPresentationHint(typeof(PedidosViewModel)));
                        await navigationService.Navigate<PedidosViewModel>();
                    }
                    else
                    {
                        nuevaOrder.company = Company;
                        nuevaOrder.customer = SelectedCustomer;

                        offlineDataService.SaveOrderNotes(nuevaOrder);
                        await offlineDataService.SynchronizeToDisk();


                        await navigationService.ChangePresentation(new MvxPopPresentationHint(typeof(PedidosViewModel)));
                        await navigationService.Navigate<PedidosViewModel>();
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atención", "Por Ahora no se puede modificar un Pedido de Venta.", "Aceptar");
                    return;
                }
            }
            catch (Exception e)
            {
                if (data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
                {
                    await Application.Current.MainPage.DisplayAlert("Atención", $"{e.Message}", "Aceptar");
                    return;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Attention", $"{e.Message}", "Acept");
                    return;
                }
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

                if (!red)
                {

                    PaymentConditions = new MvxObservableCollection<PaymentCondition>(await prometeoApiService.GetPaymentConditions(user.Token, Company.Id));

                    if (Order != null)
                    {
                        if (Order.paymentConditionId > 0)
                        {
                            Condition = PaymentConditions.FirstOrDefault(x => x.id == Order.paymentConditionId);
                        }
                        else
                        {
                            //Condition = PaymentConditions.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    if (!offlineDataService.IsDataLoadedPaymentConditions)
                    {
                        await offlineDataService.LoadDataPayment();
                    }
                    var data = await offlineDataService.SearchPaymentConditions();

                    var d = data.Where(x => x.companyId == Company.Id).ToList();

                    if (d != null || d.Count() > 0)
                    {
                        PaymentConditions = new MvxObservableCollection<PaymentCondition>(d);
                    }

                    if (Order != null)
                    {
                        if (Order.paymentConditionId > 0)
                        {
                            Condition = PaymentConditions.FirstOrDefault(x => x.id == Order.paymentConditionId);
                        }
                        else
                        {
                            //Condition = PaymentConditions.FirstOrDefault();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                //await Application.Current.MainPage.DisplayAlert("", e.Message, "Aceptar");
                var s = e.Message;
                return;
            }
        }

        private async void CargarEmpresas()
        {
            try
            {
                var user = data.LoggedUser;

                var red = await Connection.SeeConnection();

                if (!red)
                {

                    Companies = new MvxObservableCollection<Company>(await prometeoApiService.GetCompaniesByUserId(user.Id, user.Token));

                    if (Order != null)
                    {
                        if (order.companyId != null)
                        {
                            if (Order.companyId > 0)
                            {
                                Company = Companies.FirstOrDefault(x => x.Id == Order.companyId);
                            }
                        }
                        else
                        {
                            Company = Companies.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    var mapperConfig = new MapperConfiguration(m =>
                    {
                        m.AddProfile(new MappingProfile());
                    });

                    IMapper mapper = mapperConfig.CreateMapper();

                    if (!offlineDataService.IsDataLoadedCompanies)
                    {
                        await offlineDataService.LoadCompanies();
                    }
                    var empresas = await offlineDataService.SearchCompanies();

                    var e = mapper.Map<List<Company>>(empresas);

                    Companies = new MvxObservableCollection<Company>(e);

                    if (Order != null)
                    {
                        if (order.companyId != null)
                        {
                            if (Order.companyId > 0)
                            {
                                Company = Companies.FirstOrDefault(x => x.Id == Order.companyId);
                                if (PaymentConditions.Count <= 0)
                                {
                                    CargarCondiciones();
                                }
                            }                            
                        }
                        else
                        {
                            Company = Companies.FirstOrDefault();
                            //if (PaymentConditions.Count <= 0)
                            //{
                            //    //CargarCondiciones();
                            //}
                        }
                    }
                }

            }
            catch ( Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", e.Message, "Aceptar");
                return;
            }
        }

        private void CargarEstados()
        {
            OrderStatus.Add(new OpportunityStatus { Id = 1, name = "Pendiente" });
            OrderStatus.Add(new OpportunityStatus { Id = 2, name = "Remitado" });
            OrderStatus.Add(new OpportunityStatus { Id = 3, name = "Despachado" });
            OrderStatus.Add(new OpportunityStatus { Id = 4, name = "Entregado" });
        }

        public async override void Prepare(OrderNote theOrder)
        {
            try
            {
                var user = data.LoggedUser;

                if (theOrder.id > 0)
                {
                    EnableForEdit = false;

                    Order = await prometeoApiService.GetOrdersById(theOrder.id, user.Token);

                    AjustarEstado(user.Language, Order.orderStatus);

                    SelectedCustomer = Order.customer;
                    //Company = Companies.FirstOrDefault(x => x.Id == Order.company.Id);
                    TypeOfRemittance = TypeOfRemittances.FirstOrDefault(x => x.Id == Order.RemittanceType);
                    Place = PlaceOfPayment.FirstOrDefault(x => x.Id == Order.PlacePayment);
                    //PaymentMethod = PaymentMethods.FirstOrDefault(x => x.id == Order.PaymentMethodId);

                    SelectedCustomer.Addresses.Add(new CustomerAddress { Address = Order.DeliveryAddress });

                    CustomerAddress = SelectedCustomer.Addresses.FirstOrDefault();

                    CargarCondiciones();

                    //Condition = PaymentConditions.FirstOrDefault(x => x.id == Order.paymentConditionId);
                    Total = Convert.ToDouble(Order.total);
                    OrderDiscount = Order.discount;

                    ActualizarTotal(Order.products);

                }
                else
                {
                    EnableForEdit = true;

                    Order = theOrder;
                    Order.orderStatus = 1;

                    AjustarEstado(user.Language, Order.orderStatus);

                    if (Order.customer != null)
                    {
                        SelectedCustomer = Order.customer;
                    }
                    if (Order.company != null)
                    {
                        Company = Order.company;
                        CargarCondiciones();
                        CargarMedioPago();
                    }

                    if(Order.PlacePayment > 0)
                    {
                        Place = PlaceOfPayment.FirstOrDefault(x => x.Id == Order.PlacePayment);
                    }
                    if(Order.RemittanceType > 0)
                    {
                        TypeOfRemittance = TypeOfRemittances.FirstOrDefault(x => x.Id == Order.RemittanceType);
                    }
                    if (Order.paymentConditionId > 0)
                    {
                        Condition = PaymentConditions.FirstOrDefault(x => x.id == Order.paymentConditionId);
                    }

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

        private void AjustarEstado(string language, int orderStatus)
        {
            if (language.ToLower() == "es" || language.Contains("spanish"))
            {
                switch (orderStatus)
                {
                    case 1:
                        OrderStatusOrderStr = "Pendiente";
                        break;
                    case 2:
                        OrderStatusOrderStr = "Aprobado";
                        break;
                    case 3:
                        OrderStatusOrderStr = "Rechazado";
                        break;
                    case 4:
                        OrderStatusOrderStr = "Remitado";
                        break;
                    case 5:
                        OrderStatusOrderStr = "Despachado";
                        break;
                    case 6:
                        OrderStatusOrderStr = "Entregado";
                        break;
                }
            }
            else
            {
                switch (orderStatus)
                {
                    case 1:
                        OrderStatusOrderStr = "Pending";
                        break;
                    case 2:
                        OrderStatusOrderStr = "Approved";
                        break;
                    case 3:
                        OrderStatusOrderStr = "Rejected";
                        break;
                    case 4:
                        OrderStatusOrderStr = "Forwarded";
                        break;
                    case 5:
                        OrderStatusOrderStr = "Dispatched";
                        break;
                    case 6:
                        OrderStatusOrderStr = "Delivered";
                        break;
                }
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

                });
            }

            return listaProductos;
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

            //listaProd.Remove(prodEdit);
            //listaProd.Add(editingOpportunityDetail);

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
            var customer = await navigationService.Navigate<CustomersViewModel, Customer>();

            try
            {
                IsLoading = true;
                SelectedCustomer = customer;
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
                OpportunityProducts detail = await navigationService.Navigate<ProductsViewModel, OpportunityProducts>();

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
                    };

                    if (Order.products == null)
                    {
                        Order.products = new MvxObservableCollection<OrderNote.ProductOrder>();
                    }

                    Order.products.Add(product);

                    ActualizarTotal(Order.products);
                }
                //if (detail != null)
                //{
                //    detail.product.Id = Order.products.Any() ? Order.products.Max(d => d.companyProductPresentationId) + 1 : 1;
                //    detail.Price = detail.product.price;
                //    detail.Total = CalcularTotal(detail);
                    
                //}
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

        //private void CalcularDescuento(int orderDiscount)
        //{
        //    try
        //    {
        //        var totalTemp = Order.products.Sum(x => x.subtotal);

        //        var descuento = totalTemp * orderDiscount / 100;

        //        var desStr = descuento.ToString();

        //        if(desStr.Contains(","))
        //        {
        //            ValorDescuento = double.Parse(desStr.Replace(",", "."));
        //        }

        //        ValorDescuento = descuento;

        //        ActualizarTotal(Order.products);

        //    }
        //    catch (Exception e)
        //    {
        //        Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
        //    }
        //}

        private double CalcularTotal(OpportunityProducts detail)
        {
            try
            { 
            if (detail.Discount == 0)
            {
                if (ValorDescuento > 0)
                {
                    return (detail.Quantity * detail.Price) - ValorDescuento;
                }
                else
                {
                    return detail.Quantity * detail.Price;
                }
            }
            else
            {
                var temptotal = detail.Quantity * detail.Price;

                if (ValorDescuento > 0)
                {
                    var result = temptotal - (temptotal * detail.Discount / 100);
                    return result - ValorDescuento;
                }
                else
                {
                    return temptotal - (temptotal * detail.Discount / 100);
                }
            }

            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return 0;
            }
        }

        private void RemoveProduct(OrderNote.ProductOrder detail)
        {
            try
            { 
            Order.products.Remove(detail);
            ActualizarTotal(Order.products);

            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
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
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
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
                Total = details.Sum(x => x.subtotal) - ValorDescuento;
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

        //#region SELECCIONAR ARCHIVO
        //async Task<FileResult> PickAndShow(PickOptions options)
        //{
        //    try
        //    {
        //        var result = await FilePicker.PickAsync(options);
        //        if (result != null)
        //        {
        //            Text = $"File Name: {result.FileName}";
        //            if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
        //                result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
        //            {
        //                var stream = await result.OpenReadAsync();
        //                Image = ImageSource.FromStream(() => stream);
        //            }
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        // The user canceled or something went wrong
        //    }

        //    return null;
        //}
        //public class PickOptions
        //{
        //    public string PickerTitle { get; set; }
        //    public Filepickerfiletype FileType { get; set; }
        //}
        //#endregion
    }
}
