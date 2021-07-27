using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model.Enums;
using MvvmCross.ViewModels;

namespace Core.Model
{
    public class Opportunity : MvxNotifyPropertyChanged
    {
        public int Id { get; set; }

        //private Customer customer;
        //public Customer Customer
        //{
        //    get => customer;
        //    set => SetProperty(ref customer, value);
        //}

        public Customer  customer { get; set; }

        //private OpportunityStatus status;
        //public OpportunityStatus Status
        //{
        //    get => status;
        //    set => SetProperty(ref status, value);
        //}

        public OpportunityStatus opportunityStatus { get; set; }

        //private DateTime date;
        //public DateTime Date
        //{
        //    get => date;
        //    set => SetProperty(ref date, value);
        //}

        public DateTime closedDate { get; set; }

        public List<OpportunityProducts> opportunityProducts { get; set; }

        //private string description;
        //public string Description
        //{
        //    get => description;
        //    set => SetProperty(ref description, value);
        //}

        public string description { get; set; }

        private ClosedLostStatusCause closedLostStatusCause;
        public ClosedLostStatusCause ClosedLostStatusCause
        {
            get => closedLostStatusCause;
            set => SetProperty(ref closedLostStatusCause, value);
        }

        //private decimal total;
        //public decimal Total
        //{
        //    get => total;
        //    private set => SetProperty(ref total, value);
        //}

        public double totalPrice { get; set; }

        //private string productsDescription;
        public string ProductsDescription => string.Join(", ", opportunityProducts.Select(x => x.product.name));
        //{
        //    get => productsDescription;
        //    private set => SetProperty(ref productsDescription, value);
        //}

        public MvxObservableCollection<OpportunityProducts> Details { get; set; } = new MvxObservableCollection<OpportunityProducts>();

        public Opportunity()
        {
            Details.CollectionChanged += (sender, args) =>
            {
                ComputeTotal();
                //ProductsDescription = string.Join(", ", Details.Select(d => d.product.name));
            };
        }

        public double ComputeTotal()
        {
            totalPrice = (double)Details.Sum(d => d.Total);
            return totalPrice;
        }
    }
}
