using Core.Model;
using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class CreateOrderViewModel : MvxViewModel<Opportunity>
    {
        private ApplicationData data;
        // Properties

        private string iconAnalisis;
        private string iconPropuesta;
        private string iconNegociacion;
        private string iconCerrada;

        public string IconAnalisis
        {
            get => iconAnalisis;
            set => SetProperty(ref iconAnalisis, value);
        }
        public string IconPropuesta
        {
            get => iconPropuesta;
            set => SetProperty(ref iconPropuesta, value);
        }
        public string IconNegociacion
        {
            get => iconNegociacion;
            set => SetProperty(ref iconNegociacion, value);
        }
        public string IconCerrada
        {
            get => iconCerrada;
            set => SetProperty(ref iconCerrada, value);
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

        private Opportunity opportunity;
        public Opportunity Opportunity
        {
            get => opportunity;
            set => SetProperty(ref opportunity, value);
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

        private decimal total;
        public decimal Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }

        // Events
        public event EventHandler<Product> ShowEditProductPopup;
        //public event EventHandler NewOpportunityCreated;

        // Commands
        public Command SelectClientCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }
        public Command CerrarOportunidad { get; }

        public OpportunityProducts editingOpportunityDetail { get; set; }

        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public CreateOrderViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService,
                                          IToastService toastService)
        {
            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            this.toastService = toastService;

            SelectClientCommand = new Command(async () => await SelectClientAsync());
            AddProductCommand = new Command(async () => await AddProductAsync());
            RemoveProductCommand = new Command<OpportunityProducts>(RemoveProduct);
            EditProductCommand = new Command<OpportunityProducts>(EditProduct);
            CerrarOportunidad = new Command(async () => await Cerrar());

            OrderStatus = new MvxObservableCollection<OpportunityStatus>();

            CargarEstados();
        }

        private void CargarEstados()
        {
            OrderStatus.Add(new OpportunityStatus { Id = 1, name = "Pendiente" });
            OrderStatus.Add(new OpportunityStatus { Id = 2, name = "Remitado" });
            OrderStatus.Add(new OpportunityStatus { Id = 3, name = "Despachado" });
            OrderStatus.Add(new OpportunityStatus { Id = 4, name = "Entregado" });
        }

        public async override void Prepare(Opportunity theOpportunity)
        {
            if(theOpportunity!=null)
            {
                Opportunity = theOpportunity;

                selectedClosedLostStatusCause = Opportunity.opportunityStatus.name;
                SelectedCustomer = Opportunity.customer;
                ActualizarTotal(Opportunity.Details);
            }

            //if (theOpportunity.Id > 0)
            //{
            //    //var result = await prometeoApiService.GetOppById(theOpportunity.Id);
            //    //Opportunity = result;
            //    //Opportunity.Details.AddRange(result.opportunityProducts);

            //    //AjustarBotonesEstados(Opportunity.opportunityStatus.Id);
            //}
            //else
            //{
            //    Opportunity = theOpportunity;
            //}

            //selectedClosedLostStatusCause = Opportunity.opportunityStatus.name;
            //SelectedCustomer = Opportunity.customer;
            //ActualizarTotal(Opportunity.Details);
        }
        public void FinishEditProduct((decimal Price, int Quantity, int Discount) args)
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

            var listaProd = new MvxObservableCollection<OpportunityProducts>(Opportunity.Details);

            var prodEdit = listaProd.Where(x => x.productId == editingOpportunityDetail.productId).FirstOrDefault();

            Opportunity.Details.Remove(prodEdit);

            //listaProd.Remove(prodEdit);
            //listaProd.Add(editingOpportunityDetail);


            Opportunity.Details.Add(editingOpportunityDetail);


            editingOpportunityDetail = null;
            ActualizarTotal(Opportunity.Details);
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
            OpportunityProducts detail = await navigationService.Navigate<ProductsViewModel, OpportunityProducts>();
            if (detail != null)
            {
                detail.product.Id = Opportunity.Details.Any() ? Opportunity.Details.Max(d => d.product.Id) + 1 : 1;
                detail.Price = detail.product.price;
                detail.Total = CalcularTotal(detail);
                Opportunity.Details.Add(detail);

                ActualizarTotal(Opportunity.Details);
            }
        }
        private decimal CalcularTotal(OpportunityProducts detail)
        {
            if (detail.Discount == 0)
            {
                return detail.Quantity * detail.Price;
            }
            else
            {
                var temptotal = (detail.Quantity * detail.Price);
                return temptotal - (temptotal * detail.Discount / 100);
            }
        }

        private void RemoveProduct(OpportunityProducts detail)
        {
            Opportunity.Details.Remove(detail);
            ActualizarTotal(Opportunity.Details);
        }

        private void EditProduct(OpportunityProducts detail)
        {
            var product = new Product()
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
        private void ActualizarTotal(MvxObservableCollection<OpportunityProducts> details)
        {
            Total = details.Sum(x => x.Total);
        }

        private async Task Cerrar()
        {
            var d = Opportunity;
        }
    }
}
