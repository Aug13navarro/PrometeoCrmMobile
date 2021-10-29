using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Extern
{
    [Serializable]
    public class OrderNoteExtern
    {
        public int id { get; set; }
        public int? opportunityId { get; set; }
        public string oppDescription { get; set; }
        public string Description { get; set; }
        public int orderStatus { get; set; }

        public int currencyId { get; set; }
        //public Currency currency { get; set; }
        public int companyId { get; set; }
        public CompanyExtern company { get; set; }
        public int tipoComprobante { get; set; }
        public int talon { get; set; }
        public int numero { get; set; }
        public DateTime fecha { get; set; }
        //public string fechaStr => fecha.ToString("d");
        public int divisionCuentaId { get; set; }
        public int cuenta { get; set; }
        public int tipoCuentaId { get; set; }
        public int tipoServicioId { get; set; }
        public int customerId { get; set; }
        public CustomerExtern customer { get; set; }
        public int paymentConditionId { get; set; }
        public int discount { get; set; }
        public decimal total { get; set; }

        //public string totalStr => CanvertirTotal(this.total);

        //public int createBy { get; set; }
        public List<ProductOrderExtern> products { get; set; }
        //public MvxObservableCollection<OpportunityProducts> Details { get; set; }
        public string OCCustomer { get; set; }
        public int RemittanceType { get; set; }
        public int PlacePayment { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryResponsible { get; set; }
        public string DeliveryAddress { get; set; }

        [Serializable]
        public class ProductOrderExtern
        {
            public ProductExtern companyProductPresentation { get; set; }
            public int companyProductPresentationId { get; set; }
            public int quantity { get; set; }
            public double price { get; set; }
            public int discount { get; set; }
            public double subtotal { get; set; }
            public int arancel { get; set; }
            public int bonificacion { get; set; }


        }

        public string error { get; set; }
    }
}
