using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}