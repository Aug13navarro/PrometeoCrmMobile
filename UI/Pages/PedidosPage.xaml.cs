using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System;

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

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var label = (Frame)sender;
            object parameter = ((TapGestureRecognizer)label.GestureRecognizers[0]).CommandParameter;
            
            ViewModel.OpenOrderNoteCommand.Execute(parameter);
        }
    }
}