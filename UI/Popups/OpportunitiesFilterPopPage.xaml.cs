using Core.Services.Contracts;
using Core.ViewModels;
using MvvmCross.Navigation;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpportunitiesFilterPopPage : BasePopupPage
    {

        public OpportunitiesFilterPopPage(OpportunitiesViewModel opportunitiesViewModel)
        {
            InitializeComponent();

            BindingContext = new FilterOpportunitiesViewModel(opportunitiesViewModel);
        }
    }
}