using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms.Xaml;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class FilterOportinitiesPage : MvxContentPage<FilterOpportunitiesViewModel>
    {
        public FilterOportinitiesPage()
        {
            InitializeComponent();
        }
    }
}