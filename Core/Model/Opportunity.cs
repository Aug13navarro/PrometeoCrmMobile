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

        public OpportunityStatus opportunityStatus { get; set; }

        public DateTime closedDate { get; set; }

        public MvxObservableCollection<OpportunityProducts> opportunityProducts { get; set; } = new MvxObservableCollection<OpportunityProducts>();

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

        public decimal totalPrice { get; set; }

        //private string productsDescription;
        public string ProductsDescription => string.Join(", ", opportunityProducts.Select(x => x.product.name));
        //{
        //    get => productsDescription;
        //    private set => SetProperty(ref productsDescription, value);
        //}

        public MvxObservableCollection<OpportunityProducts> Details { get; set; } = new MvxObservableCollection<OpportunityProducts>();

        //public Opportunity()
        //{
        //    Details.CollectionChanged += (sender, args) =>
        //    {
        //        ComputeTotal();
        //        //ProductsDescription = string.Join(", ", Details.Select(d => d.product.name));
        //    };
        //}

        //public decimal ComputeTotal()
        //{
        //    totalPrice = Details.Sum(d => d.totaltemp);
        //    return totalPrice;
        //}
    }
}
