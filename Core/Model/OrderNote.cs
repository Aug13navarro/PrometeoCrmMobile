using Core.Helpers;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core.Model
{
    public class OrderNote : MvxNotifyPropertyChanged
    {
        public int  id { get; set; }
        private string consecutive;
        public string Consecutive
        {
            get => consecutive;
            set => SetProperty(ref consecutive, value);
        }
        public int? opportunityId { get; set; }
        public string oppDescription { get; set; }
        public string Description { get; set; }
        private int orderStatus;
        public int OrderStatus
        {
            get => orderStatus;
            set => SetProperty(ref orderStatus, value);
        }
        private StatusOrderNote statusOrderNote;
        public StatusOrderNote StatusOrderNote
        {
            get => statusOrderNote;
            set => SetProperty(ref statusOrderNote, value);
        }
        public int currencyId { get; set; }
        public Currency currency { get; set; }
        public int? companyId { get; set; }
        public Company company { get; set; }
        public int tipoComprobante { get; set; }
        public int talon { get; set; }
        public int numero { get; set; }
        public DateTime fecha { get; set; }
        public string fechaStr => fecha.ToString("d");
        public int divisionCuentaId { get; set; }
        public int? cuenta { get; set; }
        public int tipoCuentaId { get; set; }
        public int tipoServicioId { get; set; }
        public int customerId { get; set; }
        public Customer customer { get; set; }
        public int? paymentConditionId { get; set; }
        public int discount { get; set; }
        public decimal total { get; set; }

        public string totalStr => CanvertirTotal(this.total);

        private string CanvertirTotal(decimal total)
        {
            if (!string.IsNullOrWhiteSpace(Identity.LanguageUser))
            {
                if (Identity.LanguageUser.ToLower() == "es" || Identity.LanguageUser.Contains("spanish"))
                {
                    return total.ToString("N2", new CultureInfo("es-ES"));
                }
                else
                {
                    return total.ToString("N2", new CultureInfo("es-US"));
                }
            }
            else
            {
                return total.ToString("N2", new CultureInfo("es-ES"));
            }
        }

        //public int createBy { get; set; }
        public MvxObservableCollection<ProductOrder> products { get; set; }
        public MvxObservableCollection<OpportunityProducts> Details { get; set; }
        public string OCCustomer { get; set; }
        public int RemittanceType { get; set; }
        public int? PlacePayment { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryResponsible { get; set; }
        public string DeliveryAddress { get; set; }
        public TransportCompany TransportCompany { get; set; }
        public int? TransportCompanyId { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public int? PaymentMethodId { get; set; }
        public FreightInCharge FreightInCharge { get; set; }
        public int FreightInChargeId { get; set; }
        public bool IsExport { get; set;}
        public int? ImporterCustomerId { get; set; }
        public DateTime ETD { get; set; }
        public bool IsFinalClient { get; set; }
        public string FinalClient { get; set; }
        public int? FreightId { get; set; }
        public int? IncotermId { get; set; }
        public int? commercialAssistantId { get; set; }
        public bool? sentToErp { get; set; }
        public int? ProviderId { get; set; }
        public User Seller { get; set; }

        public class ProductOrder
        {
            public Product companyProductPresentation { get; set; }
            public int companyProductPresentationId { get; set; }
            public int quantity { get; set; }
            public double price { get; set; }
            public int discount { get; set; }
            public double subtotal { get; set; }
            public int arancel { get; set; }
            public int bonificacion { get; set; }
            public double discountPrice { get; set; }
            public double subtotalProduct { get; set; }


            public string PriceStr => TransformarStr(this.price);
            public string DiscountPriceStr => TransformarStr(this.discountPrice);
            public string SubTotalStr => TransformarStr(this.subtotal);
            public string SubTotalProductsStr => TransformarStr(this.subtotalProduct);

            private string TransformarStr(double value)
            {
                string str = string.Empty;

                if (Identity.LanguageUser.ToLower() == "es" || Identity.LanguageUser.Contains("spanish"))
                {
                    str = value.ToString("N2", new CultureInfo("es-ES"));
                }
                else
                {
                    str = value.ToString("N2", new CultureInfo("en-US"));
                }

                return str;
            }
        }

        public string error { get; set; }
        public int idOffline { get; set; }
        public List<AttachFile> OpportunityOrderNoteAttachFile { get; set; }
        public bool IsBusy { get; set; }
        public int? SellerId { get; set; }
        public int? DepositId { get; set; }
    }    
}
