using System;
using System.Linq;
using Core.Model.Enums;
using MvvmCross.ViewModels;

namespace Core.Model
{
    public class Opportunity : MvxNotifyPropertyChanged
    {
        public int Id { get; set; }

        private Customer customer;
        public Customer Customer
        {
            get => customer;
            set => SetProperty(ref customer, value);
        }

        private OpportunityStatus status;
        public OpportunityStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        private string description;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private ClosedLostStatusCause closedLostStatusCause;
        public ClosedLostStatusCause ClosedLostStatusCause
        {
            get => closedLostStatusCause;
            set => SetProperty(ref closedLostStatusCause, value);
        }

        private decimal total;
        public decimal Total
        {
            get => total;
            private set => SetProperty(ref total, value);
        }

        private string productsDescription;
        public string ProductsDescription
        {
            get => productsDescription;
            private set => SetProperty(ref productsDescription, value);
        }

        public MvxObservableCollection<OpportunityDetail> Details { get; } = new MvxObservableCollection<OpportunityDetail>();

        public Opportunity()
        {
            Details.CollectionChanged += (sender, args) =>
            {
                ComputeTotal();
                ProductsDescription = string.Join(", ", Details.Select(d => d.Description));
            };
        }

        public decimal ComputeTotal()
        {
            Total = Details.Sum(d => d.Total);
            return total;
        }
    }
}
