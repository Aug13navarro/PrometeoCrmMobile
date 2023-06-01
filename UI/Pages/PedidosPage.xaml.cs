using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using UI.Popups;
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
                var pop = new NewOrderNotePopup(empresa);

                pop.OkTapped += async (s, args) =>
                {
                    await PopupNavigation.Instance.PopAsync(false);

                    //(Company comp, bool isExport) = args;

                    await ViewModel.IrNuevaNotaPedido(args.Company, args.isExport);
                };

                await PopupNavigation.Instance.PushAsync(pop);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var label = (Frame)sender;
            object parameter = ((TapGestureRecognizer)label.GestureRecognizers[0]).CommandParameter;
            
            ViewModel.OpenOrderNoteCommand.Execute(parameter);
        }
    }
}