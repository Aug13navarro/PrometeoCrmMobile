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

        public Customer  customer { get; set; }
        public Company Company { get; set; }

        public OpportunityStatus opportunityStatus { get; set; }

        public DateTime createDt { get; set; }
        public DateTime closedDate { get; set; }

        public MvxObservableCollection<OpportunityProducts> opportunityProducts { get; set; } = new MvxObservableCollection<OpportunityProducts>();

        public string description { get; set; }

        private ClosedLostStatusCause closedLostStatusCause;
        public ClosedLostStatusCause ClosedLostStatusCause
        {
            get => closedLostStatusCause;
            set => SetProperty(ref closedLostStatusCause, value);
        }


        public decimal totalPrice { get; set; }

        public string ProductsDescription => string.Join(", ", opportunityProducts.Select(x => x.product.name));

        public MvxObservableCollection<OpportunityProducts> Details { get; set; } = new MvxObservableCollection<OpportunityProducts>();
    }
}
