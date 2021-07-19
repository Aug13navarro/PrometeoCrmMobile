using Core.Model;
using Core.Model.Enums;
using Core.ViewModels.Model;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpportunitiesPerStatusPopup : BasePopupPage
    {
        public OpportunitiesPerStatusPopup(MvxObservableCollection<Opportunity> opportunities)
        {
            InitializeComponent();

            IList<OpportunitiesPerStatusVM> opps = new List<OpportunitiesPerStatusVM>();

            try
            {
                //List<OpportunityStatus> status = opportunities.Select(x => x.Status).Distinct().ToList();

                //foreach (OpportunityStatus item in status)
                //{
                //    opps.Add(
                //        new OpportunitiesPerStatusVM()
                //        {
                //            opportunityStatus = item,
                //            Amount = opportunities.Where(x => x.Status == item).Count()
                //        });
                //}

                stateList.ItemsSource = opps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}