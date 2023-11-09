using Core.Model;
using Core.ViewModels;
using MvvmCross.Base;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Core;
using Core.Data;
using Core.Data.Tables;
using Core.Services;
using MvvmCross.Presenters.Hints;
using UI.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CreateOrderPage : MvxContentPage<CreateOrderViewModel>
    {
        public CreateOrderPage()
        {
            InitializeComponent();

            SaveOrderBtn.Clicked += SaveOrderBtn_Clicked;
        }

        private async void SaveOrderBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                //ViewModel.IsLoading = true;

                if (ViewModel.Company == null ||
                    ViewModel.SelectedCustomer == null ||
                    ViewModel.TypeOfRemittance == null ||
                    ViewModel.PaymentMethod == null ||
                    ViewModel.Assistant == null)
                {
                    if (ViewModel.data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                        ViewModel.data.LoggedUser.Language.abbreviation.Contains("spanish"))
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

                if (ViewModel.Company.Id != 7)
                {
                    if (ViewModel.Place == null)
                    {
                        if (ViewModel.data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                            ViewModel.data.LoggedUser.Language.abbreviation.Contains("spanish"))
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

                if (ViewModel.Company.externalErpId != null)
                {
                    if (ViewModel.Condition == null)
                    {
                        if (ViewModel.data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                            ViewModel.data.LoggedUser.Language.abbreviation.Contains("spanish"))
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
                    if (ViewModel.TypeOfRemittance.Description != "En Consignación" &&
                        ViewModel.TypeOfRemittance.Description != "On Consignment")
                    {
                        if (ViewModel.Condition == null)
                        {
                            if (ViewModel.data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                                ViewModel.data.LoggedUser.Language.abbreviation.Contains("spanish"))
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

                if (ViewModel.Order.products == null || !ViewModel.Order.products.Any())
                {
                    if (ViewModel.data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                        ViewModel.data.LoggedUser.Language.abbreviation.Contains("spanish"))
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
                    companyId = ViewModel.Company.Id,
                    Description = ViewModel.Order.Description,
                    currencyId = 1,
                    customerId = ViewModel.SelectedCustomer.Id,
                    discount = ViewModel.OrderDiscount,
                    fecha = ViewModel.Order.fecha,
                    //OrderStatus = 1,
                    total = Convert.ToDecimal(ViewModel.Total),
                    //cuenta = SelectedCustomer.externalCustomerId.Value,
                    divisionCuentaId = ViewModel.Company.ExternalId.Value,
                    talon = 88, //puede ser null
                    tipoComprobante = 8, //puede ser null
                    tipoCuentaId = 1, //puede ser null
                    tipoServicioId = 50, //puede ser null
                    DeliveryAddress = ViewModel.Order.DeliveryAddress,
                    DeliveryDate = ViewModel.Order.DeliveryDate,
                    DeliveryResponsible = ViewModel.Order.DeliveryResponsible,
                    OCCustomer = ViewModel.Order.OCCustomer,
                    PlacePayment = ViewModel.Place?.Id,
                    RemittanceType = ViewModel.TypeOfRemittance.Id,
                    PaymentMethodId = ViewModel.PaymentMethod.id,
                    commercialAssistantId = ViewModel.Assistant.Id,
                    ProviderId = ViewModel.Provider?.Id,
                    //products = new MvxObservableCollection<OrderNote.ProductOrder>(Order.products),
                };

                nuevaOrder.OpportunityOrderNoteAttachFile = ViewModel.AttachFiles != null
                    ? ViewModel.AttachFiles.ToList()
                    : new List<AttachFile>();

                if (ViewModel.Condition != null)
                {
                    nuevaOrder.paymentConditionId = ViewModel.Condition.Id;
                }

                if (ViewModel.FreightInCharge != null)
                {
                    nuevaOrder.TransportCompanyId = ViewModel.FreightInCharge.Id;
                }

                if (nuevaOrder.DeliveryDate == null)
                {
                    nuevaOrder.DeliveryDate = DateTime.Now.Date;
                    nuevaOrder.ETD = DateTime.Now.Date;
                }
                else
                {
                    nuevaOrder.ETD = ViewModel.Order.DeliveryDate.Value;
                }

                if (ViewModel.Order.opportunityId == 0 || ViewModel.Order.opportunityId == null)
                {
                    nuevaOrder.opportunityId = null;
                    nuevaOrder.products = ViewModel.Order.products;
                }
                else
                {
                    nuevaOrder.opportunityId = ViewModel.Order.opportunityId;
                    nuevaOrder.products = ViewModel.Order.products;
                }

                var red = await Connection.SeeConnection();

                nuevaOrder.OrderStatus = ViewModel.Order.StatusOrderNote.Id;

                if (ViewModel.Order.id == 0 && ViewModel.Order.idOffline == 0)
                {
                    nuevaOrder.company = ViewModel.Company;
                    nuevaOrder.customer = ViewModel.SelectedCustomer;
                    var popup = new ConfirmOrderNotePopupPage(nuevaOrder, ViewModel.data.LoggedUser.Token, red);

                    popup.ItCreated += async order =>
                    {
                        await PopupNavigation.Instance.PopAsync(true);

                        Popup_ItCreated(order);
                    };
                    await PopupNavigation.Instance.PushAsync(popup);
                }
                else
                {
                    nuevaOrder.company = ViewModel.Company;
                    nuevaOrder.customer = ViewModel.SelectedCustomer;
                    nuevaOrder.idOffline = ViewModel.Order.idOffline;
                    nuevaOrder.id = ViewModel.Order.id;
                    var popup = new ConfirmOrderNotePopupPage(nuevaOrder, ViewModel.data.LoggedUser.Token, red);

                    popup.ItCreated += async order =>
                    {
                        await PopupNavigation.Instance.PopAsync(true);

                        Popup_ItCreated(order);
                    };
                    await PopupNavigation.Instance.PushAsync(popup);
                }
            }
            catch (Exception ex)
            {
                if (ViewModel.data.LoggedUser.Language.abbreviation.ToLower() == "es" ||
                    ViewModel.data.LoggedUser.Language.abbreviation.Contains("spanish"))
                {
                    await Application.Current.MainPage.DisplayAlert("Atención", $"{ex.Message}", "Aceptar");
                    return;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Attention", $"{ex.Message}", "Acept");
                    return;
                }
            }
            finally
            {
                //ViewModel.IsLoading = false;
            }
        }

        private async void Popup_ItCreated(int? order)
        {
            if (order.HasValue)
            {
                await ViewModel.navigationService.ChangePresentation(
                    new MvxPopPresentationHint(typeof(PedidosViewModel)));
                await ViewModel.navigationService.Navigate<PedidosViewModel>();
            }
            else
            {
                ViewModel.CloseAndBack();
            }
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.ShowEditProductPopup += OnShowEditProductPopup;
            ViewModel.ShowAddressPopup += OnShowCustomerAddressPopup;

        }

        private async void OnShowCustomerAddressPopup(object sender, List<CustomerAddress> addresses)
        {
            var popup = new CustomerAddressPopup(addresses);

            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);

                if (!string.IsNullOrWhiteSpace(args))
                {
                    editorAddress.Text = args;
                }
            };

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void OnShowEditProductPopup(object sender, Product product)
        {
            var popup = new SelectProductPopup(product, true);

            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);
                ViewModel.FinishEditProduct(args);
            };

            popup.Dismissed += (s, args) => ViewModel.CancelEditProduct();

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private void Entry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            try
            {
                var idioma = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                ViewModel.ResetTotal(ViewModel.Order.products);

                if (string.IsNullOrWhiteSpace(lblOrderDiscount.Text) || ViewModel.OrderDiscount == 0)
                {
                }

                if (ViewModel.OrderDiscount > 0)
                {
                    ViewModel.ValorDescuento = ViewModel.Total * ViewModel.OrderDiscount / 100;

                    ViewModel.ActualizarTotal(ViewModel.Order.products);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        private void TapInfo_Tapped(object sender, EventArgs e)
        {
            btnProductos.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblProductos.TextColor = Xamarin.Forms.Color.White;

            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblAdjuntos.TextColor = Xamarin.Forms.Color.White;

            btnInfo.BackgroundColor = Xamarin.Forms.Color.White;
            lblInfo.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = true;
            ViewModel.StackProductos = false;
            ViewModel.StackAdjunto = false;
        }

        private void TapProducts_Tapped(object sender, EventArgs e)
        {
            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblAdjuntos.TextColor = Xamarin.Forms.Color.White;

            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.White;
            lblProductos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = false;
            ViewModel.StackProductos = true;
            ViewModel.StackAdjunto = false;
        }

        private void TapAdjuntos_Tapped(object sender, EventArgs e)
        {
            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblProductos.TextColor = Xamarin.Forms.Color.White;

            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.White;
            lblAdjuntos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = false;
            ViewModel.StackProductos = false;
            ViewModel.StackAdjunto = true;
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            cmbStatusOrderNote.Focus();
        }

        private void cmbStatusOrderNote_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (Picker)sender;

            ViewModel.Order.StatusOrderNote = cmb.SelectedItem as StatusOrderNote;
            ViewModel.Order.OrderStatus = (cmb.SelectedItem as StatusOrderNote).Id;
        }

        private async void ImageButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (status == PermissionStatus.Granted)
                {
                    var files = PickAndShow(default).ContinueWith(
                        (task) => { });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        LangResources.AppResources.Attention,
                        "No se tienen los permisos para acceder a los archivos.",
                        LangResources.AppResources.Accept);
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return;
            }
        }

        async Task PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickMultipleAsync(options); //obtengo lista de imagenes 

                if (result != null && result.Any())
                {
                    ViewModel.AddFileToOrderNote(result);
                    //OnChooseMultiple(2, result);
                    //await Application.Current.MainPage.Navigation.PopPopupAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}