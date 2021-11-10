using Core.Model;
using Core.Services;
using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
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
            }
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

        #endregion

        //EVENTS
        public event EventHandler<Product> ShowEditProductPopup;
        public event EventHandler NewOrderCreated;

        //COMMAND
        public Command SavePedidoCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }

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
                //this.toastService = toastService;
                this.offlineDataService = offlineDataService;

                //SelectClientCommand = new Command(async () => await SelectClientAsync());
                AddProductCommand = new Command(async () => await AddProductAsync());
                RemoveProductCommand = new Command<OrderNote.ProductOrder>(RemoveProduct);
                EditProductCommand = new Command<OrderNote.ProductOrder>(EditProduct);
                //CustomerAddressCommand = new Command(async () => await CustomerAddressMethod());

                SavePedidoCommand = new Command(async () => await SaveOrder());

                //OrderStatus = new MvxObservableCollection<OpportunityStatus>();

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
                    await Application.Current.MainPage.DisplayAlert("Atención", "Incoterms - No disponible Offline", "Aceptar");
                    return;
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
                    await Application.Current.MainPage.DisplayAlert("Atención", "Flete - No disponible Offline", "Aceptar");
                    return;
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

            }
            catch (Exception)
            {

                throw;
            }
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

                    Order = await prometeoApiService.GetOrdersById(orderNote.id, user.Token);

                    //AjustarEstado(user.Language, Order.orderStatus);

                    SelectedCustomer = Order.customer;
                    //Company = Companies.FirstOrDefault(x => x.Id == Order.company.Id);
                    //TypeOfRemittance = TypeOfRemittances.FirstOrDefault(x => x.Id == Order.RemittanceType);
                    //Place = PlaceOfPayment.FirstOrDefault(x => x.Id == Order.PlacePayment);
                    //PaymentMethod = PaymentMethods.FirstOrDefault(x => x.id == Order.PaymentMethodId);

                    SelectedCustomer.Addresses.Add(new CustomerAddress { Address = Order.DeliveryAddress });

                    //CustomerAddress = SelectedCustomer.Addresses.FirstOrDefault();

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
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
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
    }
}
