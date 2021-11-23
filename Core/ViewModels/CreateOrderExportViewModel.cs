using AutoMapper;
using Core.Helpers;
using Core.Model;
using Core.Services;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class CreateOrderExportViewModel : MvxViewModel<OrderNote>
    {
        private ApplicationData data;

        #region PROPIEDADES
        private bool stackDetail;
        public bool StackDetail
        {
            get => stackDetail;
            set => SetProperty(ref stackDetail, value);
        }

        private bool stackProductos;
        public bool StackProductos
        {
            get => stackProductos;
            set => SetProperty(ref stackProductos, value);
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

        private Company company;
        public Company Company
        {
            get => company;
            set
            {
                SetProperty(ref company, value);
                CargarCondiciones();
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

        private Customer selectedCustomer;
        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set => SetProperty(ref selectedCustomer, value);
        }

        private MvxObservableCollection<Incoterm> incoterms;
        public MvxObservableCollection<Incoterm> Incoterms
        {
            get => incoterms;
            set => SetProperty(ref incoterms, value);
        }

        private Incoterm incoterm;
        public Incoterm Incoterm
        {
            get => incoterm;
            set => SetProperty(ref incoterm, value);
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

        private MvxObservableCollection<FreightInCharge> freightInCharges;
        public MvxObservableCollection<FreightInCharge> FreightInCharges
        {
            get => freightInCharges;
            set => SetProperty(ref freightInCharges, value);
        }

        private FreightInCharge freightInCharge;
        public FreightInCharge FreightInCharge
        {
            get => freightInCharge;
            set => SetProperty(ref freightInCharge, value);
        }
        
        private bool enableForEdit;
        public bool EnableForEdit
        {
            get => enableForEdit;
            set => SetProperty(ref enableForEdit, value);
        }

        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                SetProperty(ref isChecked, value);
                HabilitarClienteFinal();
            }
        }

        private bool enableFinalClient;
        public bool EnableFinalClient
        {
            get => enableFinalClient;
            set => SetProperty(ref enableFinalClient, value);
        }

        private DateTime etd;
        public DateTime ETD
        {
            get => etd;
            set => SetProperty(ref etd, value);
        }

        private DateTime minimunDate;
        public DateTime MinimunDate
        {
            get => minimunDate;
            set => SetProperty(ref minimunDate, value);
        }
        #endregion

        //EVENTS
        public event EventHandler<Product> ShowEditProductPopup;
        public event EventHandler NewOrderCreated;

        //COMMAND
        public Command SavePedidoCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }
        public Command SelectClientCommand { get; }

        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IOfflineDataService offlineDataService;

        public OpportunityProducts editingOpportunityDetail { get; set; }

        public CreateOrderExportViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService, IOfflineDataService offlineDataService)
        {
            try
            {
                stackDetail = true;
                StackProductos = false;

                data = new ApplicationData();

                this.navigationService = navigationService;
                this.prometeoApiService = prometeoApiService;
                this.offlineDataService = offlineDataService;

                SelectClientCommand = new Command(async () => await SelectClientAsync());
                AddProductCommand = new Command(async () => await AddProductAsync());
                RemoveProductCommand = new Command<OrderNote.ProductOrder>(RemoveProduct);
                EditProductCommand = new Command<OrderNote.ProductOrder>(EditProduct);

                SavePedidoCommand = new Command(async () => await SaveOrder());

                ETD = DateTime.Now.AddDays(15);
                MinimunDate = DateTime.Now;

                CargarIncoterms();
                CargarFlete();

            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }

        }

        private async void CargarIncoterms()
        {
            try
            {
                var red = await Connection.SeeConnection();

                if (red)
                {
                    var incoterms = await prometeoApiService.GetIncoterms(data.LoggedUser.Token);

                    if (incoterms != null)
                    {
                        Incoterms = new MvxObservableCollection<Incoterm>(incoterms);
                    }
                }
                else
                {
                    var mapperConfig = new MapperConfiguration(m =>
                    {
                        m.AddProfile(new MappingProfile());
                    });

                    IMapper mapper = mapperConfig.CreateMapper();

                    if (!offlineDataService.IsDataLoadedIncoterms)
                    {
                        await offlineDataService.LoadIncoterms();
                    }

                    var data = await offlineDataService.SearchIncoterms();

                    if(data != null)
                    {
                        var inc = mapper.Map<List<Incoterm>>(data);
                        Incoterms = new MvxObservableCollection<Incoterm>(inc);
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

        private async void CargarFlete()
        {
            try
            {
                var user = data.LoggedUser;

                string lang = user.Language.ToLower();

                var red = await Connection.SeeConnection();

                if (red)
                {
                    var fletes = await prometeoApiService.GetFreight(lang, user.Token);

                    if (fletes != null)
                    {
                        FreightInCharges = new MvxObservableCollection<FreightInCharge>(fletes);
                    }
                }
                else
                {
                    var mapperConfig = new MapperConfiguration(m =>
                    {
                        m.AddProfile(new MappingProfile());
                    });

                    IMapper mapper = mapperConfig.CreateMapper();

                    if(!offlineDataService.IsDataLoadedFreigths)
                    {
                        await offlineDataService.LoadFreights();
                    }

                    var data = await offlineDataService.SearchFreights();

                    if( data != null)
                    {
                        var f = mapper.Map<List<FreightInCharge>>(data);
                        FreightInCharges = new MvxObservableCollection<FreightInCharge>(f);
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

        private async Task SaveOrder()
        {
            try
            {
                IsLoading = true;

                var lang = data.LoggedUser.Language.ToLower();

                if(SelectedCustomer == null
                    || Condition == null
                    || Incoterm == null
                    || FreightInCharge == null
                    || Assistant == null)
                {
                    if (lang == "es")
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "Falta completar datos Obligatorios.", "Aceptar");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "Required data to be entered.", "Acept");
                        return;

                    }
                }

                if (Order.products == null || Order.products.Count() == 0)
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
                    discount = OrderDiscount,
                    total = Convert.ToDecimal(Total),
                    cuenta = SelectedCustomer.externalCustomerId.Value,
                    divisionCuentaId = Company.externalId.Value,
                    talon = 88,                          //puede ser null
                    tipoComprobante = 8,                 //puede ser null
                    tipoCuentaId = 1,                    //puede ser null
                    tipoServicioId = 50,                  //puede ser null

                    companyId = Company.Id,
                    Description = Order.Description,
                    paymentConditionId = Condition.id,
                    ImporterCustomerId = SelectedCustomer.Id,
                    IsExport = true,
                    IsFinalClient = IsChecked,
                    IncotermId = Incoterm.Id,
                    FreightId = FreightInCharge.id,
                };

                if (FreightInCharge != null)
                {
                    nuevaOrder.FreightId = FreightInCharge.id;
                }

                nuevaOrder.products = DefinirProductos(Order.Details.ToList());

                if (Order.id == 0)
                {
                    var red = await Connection.SeeConnection();

                    if (red)
                    {
                        var respuesta = await prometeoApiService.CreateOrderNote(nuevaOrder);

                        if (respuesta != null)
                        {
                            //if (respuesta.opportunityId > 0)
                            //{
                            //    var send = new OpportunityPost
                            //    {
                            //        branchOfficeId = Order.customer.Id,
                            //        closedDate = DateTime.Now,
                            //        closedReason = "",
                            //        customerId = Order.customer.Id,
                            //        description = Order.oppDescription,
                            //        opportunityProducts = new List<OpportunityPost.ProductSend>(),
                            //        opportunityStatusId = 4,
                            //        totalPrice = Total
                            //    };

                            //    send.opportunityProducts = listaProductos(Order.Details);

                            //    var opp = new Opportunity();

                            //    await prometeoApiService.SaveOpportunityEdit(send, Order.id, data.LoggedUser.Token, opp);
                            //}
                        }

                        //await navigationService.ChangePresentation(new MvxPopPresentationHint(typeof(PedidosViewModel)));
                        //await navigationService.Navigate<PedidosViewModel>();
                    }
                    else
                    {
                        nuevaOrder.company = Company;
                        nuevaOrder.customer = SelectedCustomer;

                        offlineDataService.SaveOrderNotes(nuevaOrder);
                        await offlineDataService.SynchronizeToDisk();


                        //await navigationService.ChangePresentation(new MvxPopPresentationHint(typeof(PedidosViewModel)));
                        //await navigationService.Navigate<PedidosViewModel>();
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
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar"); return;
            }
            finally
            {
                IsLoading = false;
            }
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
            catch (Exception e)
            {
                //await Application.Current.MainPage.DisplayAlert("", e.Message, "Aceptar");
                var s = e.Message;
                return;
            }
        }

        public async override void Prepare(OrderNote orderNote)
        {
            try
            {
                var user = data.LoggedUser;

                if (orderNote.id > 0)
                {
                    EnableForEdit = false;
                    EnableFinalClient = false;

                    Order = await prometeoApiService.GetOrdersById(orderNote.id, user.Token);

                    //AjustarEstado(user.Language, Order.orderStatus);

                    SelectedCustomer = Order.customer;
                    //Company = Companies.FirstOrDefault(x => x.Id == Order.company.Id);
                    //TypeOfRemittance = TypeOfRemittances.FirstOrDefault(x => x.Id == Order.RemittanceType);
                    //Place = PlaceOfPayment.FirstOrDefault(x => x.Id == Order.PlacePayment);
                    //PaymentMethod = PaymentMethods.FirstOrDefault(x => x.id == Order.PaymentMethodId);

                    SelectedCustomer.Addresses.Add(new CustomerAddress { Address = Order.DeliveryAddress });

                    //CustomerAddress = SelectedCustomer.Addresses.FirstOrDefault();

                    CargarAsistentes();
                    CargarCondiciones();

                    //Condition = PaymentConditions.FirstOrDefault(x => x.id == Order.paymentConditionId);
                    Total = Convert.ToDouble(Order.total);
                    OrderDiscount = Order.discount;

                    ActualizarTotal(Order.products);

                }
                else
                {
                    EnableForEdit = true;

                    Order = orderNote;
                    Order.orderStatus = 1;

                    //AjustarEstado(user.Language, Order.orderStatus);

                    if (Order.customer != null)
                    {
                        SelectedCustomer = Order.customer;
                    }

                    if (Order.company != null)
                    {
                        Company = Order.company;
                    }

                    if (Order.paymentConditionId > 0)
                    {
                        Condition = PaymentConditions.FirstOrDefault(x => x.id == Order.paymentConditionId);
                    }

                    if (orderNote.Details == null)
                    {
                        if (orderNote.products != null)
                        {
                            Order.products = orderNote.products;
                        }
                        else
                        {
                            Order.products = new MvxObservableCollection<OrderNote.ProductOrder>();
                        }
                    }
                    else
                    {
                        Order.products = AsignarProductos(orderNote.Details);
                        ActualizarTotal(Order.products);
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", e.Message, "Aceptar");
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
                    var asistentes = await prometeoApiService.GetUsersByRol(Company.Id, "Asistente Comercial");

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
                    var mapperConfig = new MapperConfiguration(m =>
                    {
                        m.AddProfile(new MappingProfile());
                    });

                    IMapper mapper = mapperConfig.CreateMapper();

                    if (!offlineDataService.IsDataLoadedAssistant)
                    {
                        await offlineDataService.LoadAssistant();
                    }

                    var data = await offlineDataService.SearchAssistant();

                    if (data != null || data.Count() > 0)
                    {
                        var d = mapper.Map<List<User>>(data);
                        Assistants = new MvxObservableCollection<User>(d);

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
                await Application.Current.MainPage.DisplayAlert("", $"{e.Message}", "Aceptar");
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

        private async Task AddProductAsync()
        {
            try
            {
                var dExport = new DataExport()
                {
                    CompanyId = Company.Id
                };

                var detail = await navigationService.Navigate<ProductsViewModel,DataExport,OpportunityProducts>(dExport);

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
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

        private async Task SelectClientAsync()
        {
            var dExport = new DataExport()
            {
                CompanyId = Company.Id,
                CustomerTypeId = 26,
            };

            var customer = await navigationService.Navigate<CustomersViewModel, DataExport, Customer>(dExport);

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

        private void HabilitarClienteFinal()
        {
            if (IsChecked)
            {
                EnableFinalClient = true;
            }
            else
            {
                EnableFinalClient = false;
            }

        }

        public void ResetTotal(MvxObservableCollection<OrderNote.ProductOrder> details)
        {
            Total = details.Sum(x => x.subtotal);
        }
    }
}
