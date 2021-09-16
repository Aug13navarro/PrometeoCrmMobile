using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Enums;
using Core.Services.Contracts;
using Core.Utils;
using MvvmCross.IoC;
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
        //public event EventHandler OpportunityRefresh;


        // Commands
        public Command SelectClientCommand { get; }
        public Command AddProductCommand { get; }
        public Command EditProductCommand { get; }
        public Command RemoveProductCommand { get; }
        public Command SaveOpportunityCommand { get; }
        public Command WinOpportunityCommand { get; }
        public Command LostOpportunityCommand { get; }

        // Fields
        public OpportunityProducts editingOpportunityDetail { get; set; }

        // Services
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public CreateOpportunityViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService,
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
                RemoveProductCommand = new Command<OpportunityProducts>(RemoveProduct);
                EditProductCommand = new Command<OpportunityProducts>(EditProduct);
                SaveOpportunityCommand = new Command(async () => await SaveOpportunity());
                WinOpportunityCommand = new Command<Opportunity>(async o => await WinOpportunityAsync(o));
                LostOpportunityCommand = new Command(async () => await LostOpportunity());

                CargarIconosEstados();

                CargarEmpresas();

            }
            catch (Exception e)
            {
                var s = e.Message;
            }
        }

        private async void CargarEmpresas()
        {
            try
            {
                var user = data.LoggedUser;

                Companies = new MvxObservableCollection<Company>(await prometeoApiService.GetCompaniesByUserId(user.Id, user.Token));

                Company = Companies.FirstOrDefault();
            }
            catch (Exception e)
            {
                toastService.ShowError($"{e.Message}");
            }
        }

        private async Task LostOpportunity()
        {
            try
            {
                var user = data.LoggedUser;

                //Opportunity.opportunityStatus = new OpportunityStatus { Id = 5 };

                Opportunity.customer = SelectedCustomer;

                string error = ValidateOpportunity(Opportunity);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    toastService.ShowError(error);
                    return;
                }

                var send = new OpportunityPost
                {
                    branchOfficeId = Opportunity.customer.Id,
                    closedDate = Opportunity.createDt,
                    closedReason = "",
                    customerId = Opportunity.customer.Id,
                    description = Opportunity.description,
                    opportunityProducts = new List<OpportunityPost.ProductSend>(),
                    opportunityStatusId = 5,
                    totalPrice = Convert.ToDouble(Total),
                    companyId = Company.Id
                };

                send.opportunityProducts = listaProductos(Opportunity.Details);
                
                await prometeoApiService.SaveOpportunityEdit(send,Opportunity.Id, user.Token, Opportunity);

                await navigationService.Close(this);
                NewOpportunityCreated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"{e.Message}", "Aceptar"); 
                return;
            }
        }

        private async Task WinOpportunityAsync(Opportunity o)
        {
            var order = new OrderNote
            {
                customerId = Opportunity.customer.Id,
                customer = Opportunity.customer,
                fecha = Opportunity.createDt,
                opportunityId = Opportunity.Id,
                Details = Opportunity.opportunityProducts,
                total = Opportunity.totalPrice,
                oppDescription = Opportunity.description,
                companyId = Company.Id,
            };

            //int customerId = await navigationService.Navigate<CustomersViewModel, int>();
            await navigationService.Navigate<CreateOrderViewModel, OrderNote>(order);

        }

        public void CargarIconosEstados()
        {
            var user = data.LoggedUser;

            string lang = user.Language.ToLower();

            if (lang == "es" || lang.Contains("spanish"))
            {
                IconAnalisis = "ic_tab_1_violeta.png";
                IconPropuesta = "ic_tab_2_gris.png";
                IconNegociacion = "ic_tab_3_gris.png";
                IconCerrada = "ic_tab_4_gris.png";
            }
            else
            {
                IconAnalisis = "ic_tab_1_violeta_eng.png";
                IconPropuesta = "ic_tab_2_gris_eng.png";
                IconNegociacion = "ic_tab_3_gris_eng.png";
                IconCerrada = "ic_tab_4_gris_eng.png";
            }

            //    if (CultureInfo.InstalledUICulture.EnglishName.Contains("English"))
            //{
            //    IconAnalisis = "ic_tab_1_violeta_eng.png";
            //    IconPropuesta = "ic_tab_2_gris_eng.png";
            //    IconNegociacion = "ic_tab_3_gris_eng.png";
            //    IconCerrada = "ic_tab_4_gris_eng.png";
            //}
            //else
            //{
            //    IconAnalisis = "ic_tab_1_violeta.png";
            //    IconPropuesta = "ic_tab_2_gris.png";
            //    IconNegociacion = "ic_tab_3_gris.png";
            //    IconCerrada = "ic_tab_4_gris.png";
            //}

            EstadoId = 1;
        }

        public async override void Prepare(Opportunity theOpportunity)
        {
            try
            {
                if (theOpportunity.Id > 0)
                {
                    var result = await prometeoApiService.GetOppById(theOpportunity.Id);
                    Opportunity = result;
                    Opportunity.Details.AddRange(result.opportunityProducts);

                    if(Companies == null)
                    {
                        CargarEmpresas();
                    }

                    Company = Companies.FirstOrDefault(x => x.Id == Opportunity.Company.Id);

                    if (Opportunity.opportunityStatus.Id >= 4)
                    {
                        toastService.ShowError("Si la Oportunidad se encuentra cerrada no es posible editarla.");
                    }

                    AjustarBotonesEstados(Opportunity.opportunityStatus.Id);
                }
                else
                {
                    Opportunity = theOpportunity;
                    Opportunity.createDt = DateTime.Today;
                }

                selectedClosedLostStatusCause = Opportunity.opportunityStatus.name;
                SelectedCustomer = Opportunity.customer;
                
                ActualizarTotal(Opportunity.Details);
            }
            catch( Exception e)
            {
                var s = e.Message;
            }
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
                case 5:
                    IconAnalisis = "ic_tab_1_violeta.png";
                    IconPropuesta = "ic_tab_2_violeta.png";
                    IconNegociacion = "ic_tab_3_violeta.png";
                    IconCerrada = "ic_tab_4_violeta.png";

                    EstadoId = id;
                    break;
            }
        }
        public void AjustarBotonesEstadosEng(int id)
        {
            switch (id)
            {
                case 1:
                    IconAnalisis = "ic_tab_1_violeta_eng.png";
                    IconPropuesta = "ic_tab_2_gris_eng.png";
                    IconNegociacion = "ic_tab_3_gris_eng.png";
                    IconCerrada = "ic_tab_4_gris_eng.png";

                    EstadoId = id;
                    break;
                case 2:
                    IconAnalisis = "ic_tab_1_violeta_eng.png";
                    IconPropuesta = "ic_tab_2_violeta_eng.png";
                    IconNegociacion = "ic_tab_3_gris_eng.png";
                    IconCerrada = "ic_tab_4_gris_eng.png";

                    EstadoId = id;
                    break;
                case 3:
                    IconAnalisis = "ic_tab_1_violeta_eng.png";
                    IconPropuesta = "ic_tab_2_violeta_eng.png";
                    IconNegociacion = "ic_tab_3_violeta_eng.png";
                    IconCerrada = "ic_tab_4_gris_eng.png";

                    EstadoId = id;
                    break;
                case 4:
                    IconAnalisis = "ic_tab_1_violeta_eng.png";
                    IconPropuesta = "ic_tab_2_violeta_eng.png";
                    IconNegociacion = "ic_tab_3_violeta_eng.png";
                    IconCerrada = "ic_tab_4_violeta_eng.png";

                    EstadoId = id;
                    break;
                case 5:
                    IconAnalisis = "ic_tab_1_violeta_eng.png";
                    IconPropuesta = "ic_tab_2_violeta_eng.png";
                    IconNegociacion = "ic_tab_3_violeta_eng.png";
                    IconCerrada = "ic_tab_4_violeta_eng.png";

                    EstadoId = id;
                    break;
            }
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
            var customer = await navigationService.Navigate<CustomersViewModel, Customer>();

            try
            {
                if (customer != null)
                {
                    IsLoading = true;
                    SelectedCustomer = customer;
                }
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
            //var oppCompany = new Core.Model.OpportunityProducts{ CompanyId = Company.Id};
            //var ViewModelProduct =new ProductsViewModel(prometeoApiService, navigationService, Company.Id);

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

        private double CalcularTotal(OpportunityProducts detail)
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
            Total = Convert.ToDecimal(details.Sum(x => x.Total)); 
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

        private async Task SaveOpportunity()
        {
            try
            {
                if(EstadoId == 4 || EstadoId == 5)
                {
                    toastService.ShowError("Si la Oportunidad se encuentra cerrada no es posible editarla.");
                    return;
                }

                var user = data.LoggedUser;

                Opportunity.opportunityStatus = new OpportunityStatus { Id = EstadoId };

                Opportunity.customer = SelectedCustomer;

                string error = ValidateOpportunity(Opportunity);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    toastService.ShowError(error);
                    return;
                }

                var send = new OpportunityPost
                {
                    branchOfficeId = Opportunity.customer.Id,
                    closedDate = Opportunity.createDt,
                    closedReason = "",
                    customerId = Opportunity.customer.Id,
                    description = Opportunity.description,
                    opportunityProducts = new List<OpportunityPost.ProductSend>(),
                    opportunityStatusId = Opportunity.opportunityStatus.Id,
                    totalPrice = Convert.ToDouble(Total),
                    companyId = Company.Id
                };

                send.opportunityProducts = listaProductos(Opportunity.Details);

                var id = Opportunity.Id;

                if (id == 0)
                {
                    await prometeoApiService.SaveOpportunityCommand(send, user.Token,Opportunity);

                }
                else
                {
                    await prometeoApiService.SaveOpportunityEdit(send, id, user.Token, Opportunity);
                    
                }

                await navigationService.Close(this);
                NewOpportunityCreated?.Invoke(this, EventArgs.Empty);
                
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"{ e.Message}", ""); return;
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

            return null;
        }
    }
}
