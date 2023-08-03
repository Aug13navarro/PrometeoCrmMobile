using SQLite;
using System;

namespace Core.Data.Tables
{
    [Table("OrderNote")]
    public class OrderNoteTable
    {
        public int id { get; set; }
        public int? opportunityId { get; set; }
        public string oppDescription { get; set; }
        public string Description { get; set; }
        public int orderStatus { get; set; }
        public int currencyId { get; set; }
        //public Currency currency { get; set; }
        public int? companyId { get; set; }
        public string companyJson { get; set; }
        public int tipoComprobante { get; set; }
        public int talon { get; set; }
        public int numero { get; set; }
        public DateTime fecha { get; set; }
        public int divisionCuentaId { get; set; }
        public int? cuenta { get; set; }
        public int tipoCuentaId { get; set; }
        public int tipoServicioId { get; set; }
        public int customerId { get; set; }
        public string customerJson { get; set; }
        public int? paymentConditionId { get; set; }
        public int discount { get; set; }
        public decimal total { get; set; }

        //public int createBy { get; set; }
        public string productsJson { get; set; }
        //public MvxObservableCollection<OpportunityProducts> Details { get; set; }
        public string OCCustomer { get; set; }
        public int RemittanceType { get; set; }
        public int? PlacePayment { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryResponsible { get; set; }
        public string DeliveryAddress { get; set; }
        //public TransportCompany TransportCompany { get; set; }
        public int? TransportCompanyId { get; set; }
        //public PaymentMethod paymentMethod { get; set; }
        public int? PaymentMethodId { get; set; }
        //public FreightInCharge FreightInCharge { get; set; }
        public int FreightInChargeId { get; set; }
        public bool IsExport { get; set; }
        public int? ImporterCustomerId { get; set; }
        public DateTime ETD { get; set; }
        public bool IsFinalClient { get; set; }
        public string FinalClient { get; set; }
        public int? FreightId { get; set; }
        public int? IncotermId { get; set; }
        public int? commercialAssistantId { get; set; }
        public bool? sentToErp { get; set; }
        public int? ProviderId { get; set; }

        //public class ProductOrder
        //{
        //    //public Product companyProductPresentation { get; set; }
        //    public int companyProductPresentationId { get; set; }
        //    public int quantity { get; set; }
        //    public double price { get; set; }
        //    public int discount { get; set; }
        //    public double subtotal { get; set; }
        //    public int arancel { get; set; }
        //    public int bonificacion { get; set; }
        //}

        public string error { get; set; }
        [PrimaryKey,AutoIncrement]
        public int idOffline { get; set; }
    }
}
