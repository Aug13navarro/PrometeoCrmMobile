using AutoMapper;
using Core;
using Core.Data;
using Core.Helpers;
using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Plugin.Permissions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PedidosPage : MvxContentPage<PedidosViewModel>
    {
        public PedidosPage()
        {
            InitializeComponent();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.NewOrderPopup += OnNewOrderNote;

        }

        private async void OnNewOrderNote(object sender, Company empresa)
        {
            try
            {
                if (empresa.ExportPv.HasValue)
                {
                    var pop = new NewOrderNotePopup(empresa);

                    pop.OkTapped += async (s, args) =>
                    {
                        await PopupNavigation.Instance.PopAsync(false);

                        await ViewModel.IrNuevaNotaPedido(args.Company, args.isExport);
                    };

                    await PopupNavigation.Instance.PushAsync(pop);
                }
                else
                {
                    await ViewModel.IrNuevaNotaPedido(empresa, false);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var label = (Frame)sender;
            object parameter = ((TapGestureRecognizer)label.GestureRecognizers[0]).CommandParameter;

            var cameraOK = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);
            if (cameraOK != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                Permissions.RequestAsync<Xamarin.Essentials.Permissions.Camera>();

                //ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.Camera }, 1);
            }
            var storageOK = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
            if (storageOK != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageRead>();
            }

            ViewModel.OpenOrderNoteCommand.Execute(parameter);
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var empresas = OfflineDatabase.GetCompanies();

            var company = empresas.FirstOrDefault(x => x.Id == ViewModel.CompanyId);

            if (!company.externalErpId.HasValue)
            {
                var frame = (Frame)sender;

                var popup = new ChangeStatusOrderNotePopup();

                popup.ItChanged += async (s) =>
                {
                    await PopupNavigation.Instance.PopAsync(false);
                    ViewModel.UpdateOrderNote(s, frame.AutomationId);
                };

                await PopupNavigation.Instance.PushAsync(popup);
            }
        }
        private void OnOrderListItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ViewModel.IsLoading ||
                ViewModel.OrdersNote == null ||
                ViewModel.OrdersNote.Count == 0 ||
                ViewModel.CurrentPage == ViewModel.TotalPages)
            {
                return;
            }

            var opportunity = (OrderNote)e.Item;
            OrderNote lastOpportunityInList = ViewModel.OrdersNote[ViewModel.OrdersNote.Count - 1];

            if (opportunity.id == lastOpportunityInList.id)
            {
                ViewModel.LoadMoreOrder();
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            var cameraOK = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);
            if (cameraOK != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Xamarin.Essentials.Permissions.Camera>();

                //ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.Camera }, 1);
            }
            //var storageOK = await Permissions.RequestAsync<Permissions.StorageRead>();
            var storageOK = await Permissions.RequestAsync<Permissions.StorageRead>();

            if (storageOK != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageRead>();
            }

            await ViewModel.NuevaNotaPedido();
        }
    }
}