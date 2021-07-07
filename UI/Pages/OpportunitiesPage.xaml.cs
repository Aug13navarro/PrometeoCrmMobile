using System;
using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using UI.Popups;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class OpportunitiesPage : MvxContentPage<OpportunitiesViewModel>
    {
        public OpportunitiesPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FormsApp.RootPage.IsGestureEnabled = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            FormsApp.RootPage.IsGestureEnabled = false;
        }

        private void OnOpportunitiesListItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ViewModel.IsLoading ||
                ViewModel.Opportunities == null ||
                ViewModel.Opportunities.Count == 0 ||
                ViewModel.CurrentPage == ViewModel.TotalPages)
            {
                return;
            }

            var opportunity = (Opportunity)e.Item;
            Opportunity lastOpportunityInList = ViewModel.Opportunities[ViewModel.Opportunities.Count - 1];

            if (opportunity.Id == lastOpportunityInList.Id)
            {
                ViewModel.LoadMoreOpportunitiesCommand.Execute(null);
            }
        }

        private void OptionsIconTapped(object sender, EventArgs e)
        {
            var label = (Image)sender;
            object parameter = ((TapGestureRecognizer)label.GestureRecognizers[0]).CommandParameter;

            var popup = new OpportunityOptionsPopup();

            popup.EditTapped += (s, args) =>
            {
                ViewModel.EditOpportunityCommand.Execute(parameter);
                PopupNavigation.Instance.PopAsync(false);
            };

            PopupNavigation.Instance.PushAsync(popup);
        }

        private void OpportunietesPerStatus(object sender, EventArgs e)
        {
            var stackLayout = (StackLayout)sender;
            var parameter = ((TapGestureRecognizer)stackLayout.GestureRecognizers[0]).CommandParameter as OpportunitiesViewModel;

            var popup = new OpportunitiesPerStatusPopup(parameter.Opportunities);

            PopupNavigation.Instance.PushAsync(popup);
        }

        private void FilterOpportunities(object sender, EventArgs e)
        {
            var btn = (Image)sender;
            //var parameter = btn.CommandParameter as OpportunitiesViewModel;
            var parameter = BindingContext.DataContext as OpportunitiesViewModel;

            var popup = new OpportunitiesFilterPopPage(parameter);

            PopupNavigation.Instance.PushAsync(popup);
        }
    }
}
