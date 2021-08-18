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

namespace Core.ViewModels
{
    public class CreateOrderViewModel : MvxViewModel<OrderNote>
    {
        private ApplicationData data;
        // Properties

        //private string iconAnalisis;
        //private string iconPropuesta;
        //private string iconNegociacion;
        //private string iconCerrada;

        //public string IconAnalisis
        //{
        //    get => iconAnalisis;
        //    set => SetProperty(ref iconAnalisis, value);
        //}
        //public string IconPropuesta
        //{
        //    get => iconPropuesta;
        //    set => SetProperty(ref iconPropuesta, value);
        //}
        //public string IconNegociacion
        //{
        //    get => iconNegociacion;
        //    set => SetProperty(ref iconNegociacion, value);
        //}
        //public string IconCerrada
        //{
        //    get => iconCerrada;
        //    set => SetProperty(ref iconCerrada, value);
        //}

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
            set => SetProperty(ref company, value);
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
            set => SetProperty(ref total, value);
        }

        // Events
        public event EventHandler<Product> ShowEditProductPopup;
        public event EventHandler NewOrderCreated;

        // Commands
        public Command SelectClientCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }
        public Command CerrarOportunidad { get; }

        public Command SavePedidoCommand { get; }

        public OpportunityProducts editingOpportunityDetail { get; set; }

        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public CreateOrderViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService,
                                          IToastService toastService)
        {
            try
            {
                data = new ApplicationData();

                this.navigationService = navigationService;
                this.prometeoApiService = prometeoApiService;
                this.toastService = toastService;

                SelectClientCommand = new Command(async () => await SelectClientAsync());
                AddProductCommand = new Command(async () => await AddProductAsync());
                RemoveProductCommand = new Command<OrderNote.ProductOrder>(RemoveProduct);
                EditProductCommand = new Command<OpportunityProducts>(EditProduct);

                SavePedidoCommand = new Command(async () => await SaveOrder());

                OrderStatus = new MvxObservableCollection<OpportunityStatus>();

                CargarEstados();
                CargarEmpresas();
                CargarCondiciones();
            }
            catch(Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e",$"{e.Message}","aceptar"); return;
            }
        }

        private async Task SaveOrder()
        {
            try
            {
                //if (Status == null ||
                //    Company == null ||
                //    SelectedCustomer == null ||
                //    Condition == null)
                //{
                //    await Application.Current.MainPage.DisplayAlert("Atención", "Todos los campos son requeridos.", "Aceptar");
                //    return;
                //}

                //var nuevaOrder = new OrderNote
                //{
                //    companyId = Company.Id,
                //    currencyId = 1,
                //    customerId = SelectedCustomer.Id,
                //    discount = OrderDiscount,
                //    fecha = Opportunity.createDt,
                //    opportunityId = Opportunity.Id,
                //    orderStatus = Status.Id,
                //    paymentConditionId = Condition.id,
                //    total = Total,
                //    products = new List<OrderNote.ProductOrder>(DefinirProductos(Opportunity.Details)), // hacer que las empresas que no tengan un external id que no se muestren en la lista
                //    cuenta = SelectedCustomer.ExternalId,
                //    divisionCuentaId = Company.externalId.Value,
                //    talon = 28,
                //    tipoComprobante = 8,
                //    tipoCuentaId = 1,
                //    tipoServicioId = 50
                //};

                //var respuesta = await prometeoApiService.CreateOrderNote(nuevaOrder);

                //if (respuesta != null)
                //{
                //    var send = new OpportunityPost
                //    {
                //        branchOfficeId = Opportunity.customer.Id,
                //        closedDate = Opportunity.createDt,
                //        closedReason = "",
                //        customerId = Opportunity.customer.Id,
                //        description = Opportunity.description,
                //        opportunityProducts = new List<OpportunityPost.ProductSend>(),
                //        opportunityStatusId = 4,
                //        totalPrice = Total
                //    };

                //    send.opportunityProducts = listaProductos(Opportunity.Details);

                //    await prometeoApiService.SaveOpportunityEdit(send, Opportunity.Id);

                //}

                await navigationService.ChangePresentation(new MvxPopPresentationHint(typeof(PedidosViewModel)));
                await navigationService.Navigate<PedidosViewModel>();

            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"{e.Message}", "Aceptar"); 
                return;
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

        private IEnumerable<OrderNote.ProductOrder> DefinirProductos(MvxObservableCollection<OpportunityProducts> details)
        {
            var lista = new List<OrderNote.ProductOrder>();

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

                PaymentConditions = new MvxObservableCollection<PaymentCondition>(await prometeoApiService.GetPaymentConditions(user.Token));
            }
            catch(Exception e)
            {
                toastService.ShowError($"{e.Message}");
            }
        }

        private async void CargarEmpresas()
        {
            try
            {
                var user = data.LoggedUser;

                Companies = new MvxObservableCollection<Company>(await prometeoApiService.GetCompaniesByUserId(user.Id, user.Token));
            }
            catch ( Exception e)
            {
                toastService.ShowError($"{e.Message}");
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
                if (theOrder.id > 0)
                {
                    var user = data.LoggedUser;
                    
                    Order = await prometeoApiService.GetOrdersById(theOrder.id, user.Token);

                    ActualizarTotal(Order.products);
                }
                else
                {
                    Order = theOrder;
                    if (theOrder.Details == null)
                    {
                        Order.products = new MvxObservableCollection<OrderNote.ProductOrder>();
                    }
                    else
                    {
                        Order.products = AsignarProductos(theOrder.Details);
                    }
                }
            }
            catch(Exception e)
            {
                toastService.ShowError($"{e.Message}");
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
                    productPresentationName = item.product.name,
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

            Order.products.Remove(prodEdit);

            //listaProd.Remove(prodEdit);
            //listaProd.Add(editingOpportunityDetail);

            var product = new OrderNote.ProductOrder
            {
                discount = editingOpportunityDetail.Discount,
                price = editingOpportunityDetail.Price,
                quantity = editingOpportunityDetail.Quantity,
                subtotal = editingOpportunityDetail.Total,
                companyProductPresentationId = editingOpportunityDetail.productId
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
            int customerId = await navigationService.Navigate<CustomersViewModel, int>();

            try
            {
                IsLoading = true;
                SelectedCustomer = await prometeoApiService.GetCustomer(customerId);
            }
            catch (Exception ex)
            {
                toastService.ShowError($"Ocurrió un error al obtener el cliente. Compruebe su conexión a internet.");
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

                var product = new OrderNote.ProductOrder
                {
                    discount = detail.Discount,
                    price = detail.product.price,
                    quantity = detail.Quantity,
                    subtotal = detail.Total,
                    companyProductPresentationId = detail.productId,
                    productPresentationName = detail.product.name,
                };

                //if (detail != null)
                //{
                //    detail.product.Id = Order.products.Any() ? Order.products.Max(d => d.companyProductPresentationId) + 1 : 1;
                //    detail.Price = detail.product.price;
                //    detail.Total = CalcularTotal(detail);
                    
                //}

                Order.products.Add(product);

                ActualizarTotal(Order.products);

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

        private void EditProduct(OpportunityProducts detail)
        {
            try
            { 
            var product = new Product
            {
                name = detail.product.name,
                price = detail.product.price,
                stock = detail.product.stock,
                Discount = detail.Discount,
                quantity = detail.Quantity
            };

            editingOpportunityDetail = detail;
            ShowEditProductPopup?.Invoke(this, product);

            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }
        private void ActualizarTotal(MvxObservableCollection<OrderNote.ProductOrder> details)
        {
            try
            {
                if (OrderDiscount > 0)
                {
                    Total = details.Sum(x => x.subtotal) - ValorDescuento;
                }
                else
                {
                    Total = details.Sum(x => x.subtotal);
                }

            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

    }
}
