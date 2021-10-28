using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Core.Model.Extern
{
    [Serializable]
    public class CustomerExtern
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

        //public ObservableCollection<int> CompanyUserId { get; } = new ObservableCollection<int>();
        //public ObservableCollection<int> CompanyUserIds { get; } = new ObservableCollection<int>();
        //public ObservableCollection<TypeOfCustomer> CustomersTypes { get; } = new ObservableCollection<TypeOfCustomer>();

        //public ObservableCollection<CustomerContact> Contacts { get; } = new ObservableCollection<CustomerContact>();
        //public ObservableCollection<CustomerAddress> Addresses { get; } = new ObservableCollection<CustomerAddress>();
    }
}
