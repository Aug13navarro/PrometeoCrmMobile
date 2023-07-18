using System;

namespace Core.Model.Extern
{
    [Serializable]
    public class CustomerExtern
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string BusinessName { get; set; }
        public string Abbreviature { get; set; }
        public string TypeId { get; set; }
        public string IdNumber { get; set; }
        public int? IdParentCustomer { get; set; }
        public int? TaxCondition { get; set; }
        public int AccountOwnerId { get; set; }
        public string AccountOwnerName { get; set; }
        public decimal? PesosBalance { get; set; }
        public decimal? DollarBalance { get; set; }
        public int? UnitBalance { get; set; }
        public string Descriptions { get; set; }
        public int? externalCustomerId { get; set; }
    }
}
