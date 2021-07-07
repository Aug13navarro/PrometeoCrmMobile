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
        // Properties
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

        public List<string> StatusesDescription { get; } =
            ((OpportunityStatus[])Enum.GetValues(typeof(OpportunityStatus))).Select(v => v.GetEnumDescription()).ToList();

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
        private OpportunityDetail editingOpportunityDetail;

        // Services
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public CreateOpportunityViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService,
                                          IToastService toastService)
        {
            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            this.toastService = toastService;

            SelectClientCommand = new Command(async () => await SelectClientAsync());
            AddProductCommand = new Command(async () => await AddProductAsync());
            RemoveProductCommand = new Command<OpportunityDetail>(RemoveProduct);
            EditProductCommand = new Command<OpportunityDetail>(EditProduct);
            SaveOpportunityCommand = new Command(async () => await SaveOpportunity());
        }

        public override void Prepare(Opportunity theOpportunity)
        {
            Opportunity = theOpportunity;

            SelectedStatus = Opportunity.Status.GetEnumDescription();
            selectedClosedLostStatusCause = Opportunity.ClosedLostStatusCause.GetEnumDescription();
            SelectedCustomer = Opportunity.Customer;
            Total = Opportunity.ComputeTotal();
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

            editingOpportunityDetail = null;
            Total = Opportunity.ComputeTotal();
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
                toastService.ShowError("Ocurrió un error al obtener el cliente. Compruebe su conexión a internet.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddProductAsync()
        {
            OpportunityDetail detail = await navigationService.Navigate<ProductsViewModel, OpportunityDetail>();
            if (detail != null)
            {
                detail.Id = Opportunity.Details.Any() ? Opportunity.Details.Max(d => d.Id) + 1 : 1;
                Opportunity.Details.Add(detail);

                Total = Opportunity.ComputeTotal();
            }
        }

        private void RemoveProduct(OpportunityDetail detail)
        {
            Opportunity.Details.Remove(detail);
            Total = Opportunity.ComputeTotal();
        }

        private void EditProduct(OpportunityDetail detail)
        {
            var product = new Product()
            {
                Description = detail.Description,
                Price = detail.Price,
                Stock = detail.Quantity,
                Discount = detail.Discount,
            };

            editingOpportunityDetail = detail;
            ShowEditProductPopup?.Invoke(this, product);
        }

        private async Task SaveOpportunity()
        {
            Opportunity.Status = GetOpportunityStatusDescriptionFromEnum(SelectedStatus);
            Opportunity.ClosedLostStatusCause = GetOpportunityClosedLostCauseDescriptionFromEnum(selectedClosedLostStatusCause);
            Opportunity.Customer = SelectedCustomer;

            string error = ValidateOpportunity(Opportunity);
            if (!string.IsNullOrWhiteSpace(error))
            {
                toastService.ShowError(error);
                return;
            }

            if (Opportunity.Id == 0)
            {
                await prometeoApiService.SaveOpportunityCommand(Opportunity);
            }

            await navigationService.Close(this);
            NewOpportunityCreated?.Invoke(this, EventArgs.Empty);
        }

        private OpportunityStatus GetOpportunityStatusDescriptionFromEnum(string value)
        {
            return ((OpportunityStatus[])Enum.GetValues(typeof(OpportunityStatus))).Single(v => v.GetEnumDescription() == value);
        }

        private ClosedLostStatusCause GetOpportunityClosedLostCauseDescriptionFromEnum(string value)
        {
            return ((ClosedLostStatusCause[])Enum.GetValues(typeof(ClosedLostStatusCause))).Single(v => v.GetEnumDescription() == value);
        }

        private string ValidateOpportunity(Opportunity theOpportunity)
        {
            if (theOpportunity.Customer == null)
            {
                return "Debe seleccionar un cliente.";
            }

            if (theOpportunity.Details?.Count == 0)
            {
                return "Debe asociar algún producto.";
            }

            if (string.IsNullOrWhiteSpace(theOpportunity.Description))
            {
                return "Debe ingresar una descripción.";
            }

            return null;
        }
    }
}
