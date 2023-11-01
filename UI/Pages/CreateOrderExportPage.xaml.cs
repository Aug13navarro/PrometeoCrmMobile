using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using System.Linq;
using Core.Services;
using UI.Popups;
using Xamarin.Forms;
using MvvmCross.Presenters.Hints;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CreateOrderExportPage : MvxContentPage<CreateOrderExportViewModel>
    {
        public CreateOrderExportPage()
        {
            InitializeComponent();
        }
        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.ShowEditProductPopup += OnShowEditProductPopup;

            SaveOrderBtn.Clicked += SaveOrderBtn_Clicked;
        }

        private async void SaveOrderBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var lang = ViewModel.data.LoggedUser.Language.abbreviation.ToLower();

                if (ViewModel.SelectedCustomer == null
                    || ViewModel.Condition == null
                    || ViewModel.Incoterm == null
                    || ViewModel.FreightInCharge == null
                    || ViewModel.Assistant == null)
                {
                    if (lang == "es")
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención",
                            "Falta completar datos Obligatorios.", "Aceptar");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "Required data to be entered.",
                            "Acept");
                        return;
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
                    discount = ViewModel.OrderDiscount,
                    total = Convert.ToDecimal(ViewModel.Total),
                    //cuenta = SelectedCustomer.externalCustomerId,
                    divisionCuentaId = ViewModel.Company.ExternalId.Value,
                    talon = 88, //puede ser null
                    tipoComprobante = 8, //puede ser null
                    tipoCuentaId = 1, //puede ser null
                    tipoServicioId = 50, //puede ser null
                    currencyId = 1,
                    companyId = ViewModel.Company.Id,
                    Description = string.IsNullOrEmpty(ViewModel.Order.Description)
                        ? string.Empty
                        : ViewModel.Order.Description,
                    paymentConditionId = ViewModel.Condition.Id,
                    ImporterCustomerId = ViewModel.SelectedCustomer.Id,
                    IsExport = true,
                    IsFinalClient = ViewModel.IsChecked,
                    IncotermId = ViewModel.Incoterm.Id,
                    FreightId = ViewModel.FreightInCharge.id,
                    fecha = ViewModel.ETD,
                    ETD = ViewModel.ETD,
                    customerId = ViewModel.SelectedCustomer.Id,
                    commercialAssistantId = ViewModel.Assistant.IdUser
                };

                if (ViewModel.FreightInCharge != null)
                {
                    nuevaOrder.FreightId = ViewModel.FreightInCharge.id;
                }

                nuevaOrder.products = ViewModel.Order.products;

                if (ViewModel.Order.id == 0)
                {
                    nuevaOrder.OrderStatus = ViewModel.Status.Id;

                    var red = await Connection.SeeConnection();

                    if (red)
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

                        //var respuesta = await ViewModel.prometeoApiService.CreateOrderNote(nuevaOrder);

                        //if (respuesta != null)
                        //{
                        //    //if (respuesta.opportunityId > 0)
                        //    //{
                        //    //    var send = new OpportunityPost
                        //    //    {
                        //    //        branchOfficeId = Order.customer.Id,
                        //    //        closedDate = DateTime.Now,
                        //    //        closedReason = "",
                        //    //        customerId = Order.customer.Id,
                        //    //        description = Order.oppDescription,
                        //    //        opportunityProducts = new List<OpportunityPost.ProductSend>(),
                        //    //        opportunityStatusId = 4,
                        //    //        totalPrice = Total
                        //    //    };

                        //    //    send.opportunityProducts = listaProductos(Order.Details);

                        //    //    var opp = new Opportunity();

                        //    //    await prometeoApiService.SaveOpportunityEdit(send, Order.id, data.LoggedUser.Token, opp);
                        //    //}

                        //    await navigationService.Close(this);
                        //    NewOrderCreatedd(true);
                        //}

                        //await navigationService.ChangePresentation(new MvxPopPresentationHint(typeof(PedidosViewModel)));
                        //await navigationService.Navigate<PedidosViewModel>();
                    }
                    else
                    {
                        nuevaOrder.company = ViewModel.Company;
                        nuevaOrder.customer = ViewModel.SelectedCustomer;

                        ViewModel.CloseAndBack();
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atención",
                        "Por Ahora no se puede modificar un Pedido de Venta.", "Aceptar");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
                return;
            }
            finally
            {
                //IsLoading = false;
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
                if (lblDiscount.Text == "Discount")
                {
                    ViewModel.ResetTotal(ViewModel.Order.products);

                    if (string.IsNullOrWhiteSpace(lblOrderDiscount.Text) || ViewModel.OrderDiscount == 0)
                    {
                        ViewModel.OrderDiscount = 0;
                        lblDiscountResult.Text = 0.ToString();
                    }

                    if (ViewModel.OrderDiscount > 0)
                    {
                        ViewModel.ValorDescuento = ViewModel.Total * ViewModel.OrderDiscount / 100;
                        lblDiscountResult.Text = ViewModel.ValorDescuento.ToString("N2", new CultureInfo("es-US"));

                        ViewModel.ActualizarTotal(ViewModel.Order.products);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(lblOrderDiscount.Text))
                    {
                        ViewModel.ResetTotal(ViewModel.Order.products);

                        if (string.IsNullOrWhiteSpace(lblOrderDiscount.Text) || ViewModel.OrderDiscount == 0)
                        {
                            ViewModel.OrderDiscount = 0;
                            lblDiscountResult.Text = 0.ToString();
                        }

                        if (Convert.ToDecimal(lblOrderDiscount.Text) > 0)
                        {
                            var o = Convert.ToDecimal(lblOrderDiscount.Text) / 100;

                            var descuento = Convert.ToDouble($"0.{o}");

                            ViewModel.ValorDescuento = ViewModel.Total * descuento;
                            lblDiscountResult.Text = ViewModel.ValorDescuento.ToString("N2", new CultureInfo("es-ES"));

                            ViewModel.ActualizarTotal(ViewModel.Order.products);
                        }
                    }
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

            btnInfo.BackgroundColor = Xamarin.Forms.Color.White;
            lblInfo.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackDetail = true;
            ViewModel.StackProductos = false;
        }

        private void TapProducts_Tapped(object sender, EventArgs e)
        {
            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.White;
            lblProductos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackDetail = false;
            ViewModel.StackProductos = true;
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
    }
}