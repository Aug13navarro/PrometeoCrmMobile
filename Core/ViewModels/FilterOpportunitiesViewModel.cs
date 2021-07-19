using Core.Model;
using Core.Model.Enums;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class FilterOpportunitiesViewModel : MvxViewModel
    {
        private DateTime beginDate;
        public DateTime BeginDate
        {
            get => beginDate;
            set => SetProperty(ref beginDate, value);
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get => endDate;
            set => SetProperty(ref endDate, value);
        }

        private OpportunityStatus status;
        public OpportunityStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private Customer customer;
        public Customer Customer
        {
            get => customer;
            set => SetProperty(ref customer, value);
        }

        private Product product;
        public Product Product
        {
            get => product;
            set => SetProperty(ref product, value);
        }

        private decimal total;
        public decimal Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }

        public IList<OpportunityStatus> OpportunityStatuses { get; set; }
        public IList<Customer> Customers { get; set; }
        public IList<Product> Products { get; set; }
        public IList<double> Totals { get; set; }

        public OpportunitiesViewModel OpportunitiesViewModel { get; set; }

        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }

        public FilterOpportunitiesViewModel(OpportunitiesViewModel opportunitiesViewModel)
        {
            this.OpportunitiesViewModel = opportunitiesViewModel;
            Customers = opportunitiesViewModel.Opportunities.Select(x => x.customer).Distinct().ToList();
            OpportunityStatuses = Enum.GetValues(typeof(OpportunityStatus)).OfType<OpportunityStatus>().ToList();
            var ProdIds = opportunitiesViewModel.Opportunities.Select(x => x.Details.Select(y => y.productId)).ToList();

            Totals = opportunitiesViewModel.Opportunities.Select(x => x.totalPrice).ToList();
            //ApplyFiltersCommand = new Command(() => ApplyFilters());
            //ResetFiltersCommand = new Command(() => ResetFilters());
        }

        //private void ApplyFilters()
        //{
        //    List<Opportunity> Opportunities = OpportunitiesViewModel.Opportunities.ToList();

        //    if (Customer != null)
        //    {
        //        Opportunities = Opportunities.Where(x => x.Customer?.Id == Customer.Id).ToList();
        //        //OpportunitiesViewModel = Customer;
        //    }

        //    if (Status != null)
        //    {
        //        Opportunities = Opportunities.Where(x => x.Status == Status).ToList();
        //        //  ShipmentViewModel.Destination = Destination;
        //    }

        //    //if (Ship != null)
        //    //{
        //    //    shipments = shipments.Where(x => x.Ship?.Id == Ship.Id).ToList();
        //    //    ShipmentViewModel.Ship = Ship;
        //    //}


        //    OpportunitiesViewModel.Opportunities.Clear();
        //    OpportunitiesViewModel.Opportunities.AddRange(Opportunities);

        //    MessagingCenter.Send(this, "filtered", OpportunitiesViewModel);
        //}

        //private void ResetFilters()
        //{
        //    ShipmentViewModel.Shipments.Clear();
        //    // ShipmentViewModel.Shipments.AddRange(ShipmentViewModel.ShipmentListBackUp);

        //    if (Preferences.Get("customerId", 0) > 0)
        //        ShipmentViewModel.PrepareDisplayData(ShipmentViewModel.ShipmentListBackUp);
        //    else
        //        ShipmentViewModel.Shipments.AddRange(ShipmentViewModel.ShipmentListBackUp);

        //    ShipmentViewModel.Customer = null;
        //    ShipmentViewModel.Destination = null;
        //    ShipmentViewModel.Ship = null;
        //    ShipmentViewModel.Product = null;
        //    ShipmentViewModel.Week = 0;
        //    ShipmentViewModel.ContainerNumber = null;

        //    Customer = null;
        //    Destination = null;
        //    week = 0;
        //    Ship = null;
        //    Product = null;

        //    //   MessagingCenter.Send(this, "filtered", ShipmentViewModel);
        //}

    }
}
