using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Enums;
using Core.Services.Contracts;
using Core.Utils;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class CreateOpportunityViewModel : MvxViewModel<Opportunity>
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

        private string selectedStatus;
        public string SelectedStatus
        {
            get => selectedStatus;
            set => SetProperty(ref selectedStatus, value);
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

        //public List<string> StatusesDescription { get; } =
            //((OpportunityStatus[])Enum.GetValues(typeof(OpportunityStatus))).Select(v => v.GetEnumDescription()).ToList();

        public List<string> ClosedLostStatusCausesDescription { get; } =
            ((ClosedLostStatusCause[])Enum.GetValues(typeof(ClosedLostStatusCause))).Select(v => v.GetEnumDescription()).ToList();

        // Events
        public event EventHandler<Product> ShowEditProductPopup;
        public event EventHandler NewOpportunityCreated;

        // Commands
        public Command SelectClientCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }
        public Command SaveOpportunityCommand { get; }

        // Fields
        private OpportunityProducts editingOpportunityDetail;

        // Services
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public CreateOpportunityViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService,
                                          IToastService toastService)
        {
            data = new ApplicationData(); 

            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            this.toastService = toastService;

            SelectClientCommand = new Command(async () => await SelectClientAsync());
            AddProductCommand = new Command(async () => await AddProductAsync());
            RemoveProductCommand = new Command<OpportunityProducts>(RemoveProduct);
            EditProductCommand = new Command<OpportunityProducts>(EditProduct);
            SaveOpportunityCommand = new Command(async () => await SaveOpportunity());

            CargarIconosEstados();


        }

        private void CargarIconosEstados()
        {
            IconAnalisis = "ic_tab_1_violeta.png";
            IconPropuesta = "ic_tab_2_gris.png";
            IconNegociacion = "ic_tab_3_gris.png";
            IconCerrada = "ic_tab_4_gris.png";

            EstadoId = 1;
        }

        public async override void Prepare(Opportunity theOpportunity)
        {
            if (theOpportunity.Id > 0)
            {
                var result = await prometeoApiService.GetOppById(theOpportunity.Id);
                Opportunity = result;
                Opportunity.Details = new MvxObservableCollection<OpportunityProducts>( result.opportunityProducts);

                AjustarBotonesEstados(Opportunity.opportunityStatus.Id);
            }
            else
            {
                Opportunity = theOpportunity;
            }

            selectedClosedLostStatusCause = Opportunity.opportunityStatus.name;
            SelectedCustomer = Opportunity.customer;
            ActualizarTotal(Opportunity.Details);
        }

        public void AjustarBotonesEstados(int id)
        {
            switch (id)
            {
                case 1:
                    IconAnalisis = "ic_tab_1_violeta.png";
                    IconPropuesta = "ic_tab_2_gris.png";
                    IconNegociacion = "ic_tab_3_gris.png";
                    IconCerrada = "ic_tab_4_gris.png";

                    EstadoId = id;
                    break;
                case 2:
                    IconAnalisis = "ic_tab_1_violeta.png";
                    IconPropuesta = "ic_tab_2_violeta.png";
                    IconNegociacion = "ic_tab_3_gris.png";
                    IconCerrada = "ic_tab_4_gris.png";

                    EstadoId = id;
                    break;
                case 3:
                    IconAnalisis = "ic_tab_1_violeta.png";
                    IconPropuesta = "ic_tab_2_violeta.png";
                    IconNegociacion = "ic_tab_3_violeta.png";
                    IconCerrada = "ic_tab_4_gris.png";

                    EstadoId = id;
                    break;
                case 4:
                    IconAnalisis = "ic_tab_1_violeta.png";
                    IconPropuesta = "ic_tab_2_violeta.png";
                    IconNegociacion = "ic_tab_3_violeta.png";
                    IconCerrada = "ic_tab_4_violeta.png";

                    EstadoId = id;
                    break;
            }
        }

        public void FinishEditProduct((decimal Price, int Quantity, int Discount) args)
        {
            if (editingOpportunityDetail == null)
            {
                throw new InvalidOperationException("No hay ningún detalle para editar. No debería pasar esto.");
            }

            editingOpportunityDetail.product.price = args.Price;
            editingOpportunityDetail.product.quantity = args.Quantity;
            editingOpportunityDetail.product.Discount = args.Discount;

            var prodEdit = Opportunity.Details.Where(x => x.productId == editingOpportunityDetail.productId).FirstOrDefault();

            Opportunity.Details.Remove(prodEdit);
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
                detail.Total = CalcularTotal(detail);
                Opportunity.Details.Add(detail);

                ActualizarTotal(Opportunity.Details);
            }
        }

        private decimal CalcularTotal(OpportunityProducts detail)
        {
            if(detail.Discount == 0)
            {
                return detail.Quantity * detail.Price;
            }
            else
            {
               var temptotal = (detail.Quantity * detail.Price);
                return temptotal - (temptotal * detail.Discount/ 100);
            }
        }

        private void ActualizarTotal(MvxObservableCollection<OpportunityProducts> details)
        {
            Total = details.Sum(x => x.Total); 
        }

        private void RemoveProduct(OpportunityProducts detail)
        {
            Opportunity.Details.Remove(detail);
            Total = Opportunity.totalPrice;
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

        private async Task SaveOpportunity()
        {
            var user = data.LoggedUser;

            Opportunity.opportunityStatus = new OpportunityStatus{ Id = EstadoId};

            Opportunity.customer = SelectedCustomer;

            string error = ValidateOpportunity(Opportunity);
            if (!string.IsNullOrWhiteSpace(error))
            {
                toastService.ShowError(error);
                return;
            }

            var id = Opportunity.Id;

            if (id == 0)
            {
                var send = new OpportunityPost
                {
                    branchOfficeId = Opportunity.customer.Id,
                    closedDate = Opportunity.closedDate,
                    closedReason = "",
                    customerId = Opportunity.customer.Id,
                    description = Opportunity.description,
                    opportunityProducts = new List<OpportunityPost.ProductSend>(),
                    opportunityStatusId = Opportunity.opportunityStatus.Id,
                    totalPrice = Total
                };

                send.opportunityProducts = listaProductos(Opportunity.Details);

                await prometeoApiService.SaveOpportunityCommand(send, user.Token);
            }

            await navigationService.Close(this);
            NewOpportunityCreated?.Invoke(this, EventArgs.Empty);
        }

        private List<OpportunityPost.ProductSend> listaProductos(MvxObservableCollection<OpportunityProducts> details)
        {
            var lista = new List<OpportunityPost.ProductSend>();

            foreach (var item in details)
            {
                decimal tempTotal = item.Price * item.Quantity;

                if (item.Discount != 0)
                {
                    tempTotal = tempTotal - (tempTotal * item.Discount) / 100;
                }

                lista.Add(new OpportunityPost.ProductSend
                {
                    discount = item.Discount,
                    productId = item.productId,
                    quantity = item.Quantity,
                    total = tempTotal,
                });
            }

            return lista;
        }

        private ClosedLostStatusCause GetOpportunityClosedLostCauseDescriptionFromEnum(string value)
        {
            return ((ClosedLostStatusCause[])Enum.GetValues(typeof(ClosedLostStatusCause))).Single(v => v.GetEnumDescription() == value);
        }

        private string ValidateOpportunity(Opportunity theOpportunity)
        {
            if (theOpportunity.customer == null)
            {
                return "Debe seleccionar un cliente.";
            }

            if (theOpportunity.Details?.Count == 0)
            {
                return "Debe asociar algún producto.";
            }

            if (string.IsNullOrWhiteSpace(theOpportunity.description))
            {
                return "Debe ingresar una descripción.";
            }

            return null;
        }
    }
}
