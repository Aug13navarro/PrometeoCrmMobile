using MvvmCross.ViewModels;

namespace Core.Model
{
    public class Customer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string BusinessName { get; set; }
        public string Abbreviature { get; set; }
        public int? TypeId { get; set; }
        public string IdNumber { get; set; }
        public int? IdParentCustomer { get; set; }
        public int? TaxCondition { get; set; }
        public int AccountOwnerId { get; set; }
        public string AccountOwnerName { get; set; }
        public decimal? PesosBalance { get; set; }
        public decimal? DollarBalance { get; set; }
        public int? UnitBalance { get; set; }
        public string Descriptions { get; set; }
        public int ExternalId { get; set; }

        public MvxObservableCollection<int> CompanyUserId { get; } = new MvxObservableCollection<int>();
        public MvxObservableCollection<int> CompanyUserIds { get; } = new MvxObservableCollection<int>();
        public MvxObservableCollection<TypeOfCustomer> CustomersTypes { get; } = new MvxObservableCollection<TypeOfCustomer>();

        public MvxObservableCollection<CustomerContact> Contacts { get; } = new MvxObservableCollection<CustomerContact>();
        public MvxObservableCollection<CustomerAddress> Addresses { get; } = new MvxObservableCollection<CustomerAddress>();
    }
}
