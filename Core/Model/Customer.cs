using MvvmCross.ViewModels;
using SQLite;
using System;

namespace Core.Model
{
    public class Customer : MvxNotifyPropertyChanged
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string BusinessName { get; set; }
        public string Abbreviature { get; set; }
        public string TypeId { get; set; }
        public string IdNumber { get; set; }
        public int? IdParentCustomer { get; set; }
        public int? TaxCondition { get; set; }
        public int? AccountOwnerId { get; set; }
        public string AccountOwnerName { get; set; }
        public decimal? PesosBalance { get; set; }
        public decimal? DollarBalance { get; set; }
        public int? UnitBalance { get; set; }
        public string Descriptions { get; set; }
        public int? externalCustomerId { get; set; }

        [Ignore]
        public MvxObservableCollection<int> CompanyUserId { get; } = new MvxObservableCollection<int>();
        [Ignore]
        public MvxObservableCollection<int> CompanyUserIds { get; } = new MvxObservableCollection<int>();
        [Ignore]
        public MvxObservableCollection<TypeOfCustomer> CustomersTypes { get; } = new MvxObservableCollection<TypeOfCustomer>();
        [Ignore]
        public MvxObservableCollection<CustomerContact> Contacts { get; } = new MvxObservableCollection<CustomerContact>();
        [Ignore]
        public MvxObservableCollection<CustomerAddress> Addresses { get; } = new MvxObservableCollection<CustomerAddress>();
        
        private bool isContactsVisible;
        public bool IsContactsVisible
        {
            get => isContactsVisible;
            set => SetProperty(ref isContactsVisible, value);
        }
        public int? CompanyId { get; set; }
    }
}
