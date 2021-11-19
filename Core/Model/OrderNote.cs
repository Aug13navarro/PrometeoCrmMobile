using Core.Helpers;
using MvvmCross.ViewModels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Core.Model
{
    public class OrderNote 
    {
        public int  id { get; set; }
        public int? opportunityId { get; set; }
        public string oppDescription { get; set; }
        public string Description { get; set; }
        public int orderStatus { get; set; }
        public string orderStatusStr => VerificarStatus(this.orderStatus);

        private string VerificarStatus(int status)
        {
            var estado = string.Empty;

            switch (status)
            {
                case 1:
                    estado = "Pendiente";
                    break;
                case 2:
                    estado = "Aprobado";
                    break;
                case 3:
                    estado = "Rechazado";
                    break;
                case 4:
                    estado = "Remitado";
                    break;
                case 5:
                    estado = "Despachado";
                    break;
                case 6:
                    estado = "Entregado";
                    break;
            }

            return estado;
        }

        public string orderColor => AsignarColor(this.orderStatus);

        private string AsignarColor(int orderStatus)
        {
            string color = "";

            switch (orderStatus)
            {
                case 1:
                    color = "#797979";
                    break;
                case 2:
                    color = "#70B603";
                    break;
                case 3:
                    color = "#D9001B";
                    break;
                case 4:
                    color = "#FFCC00";
                    break;
                case 5:
                    color = "#00BFBF";
                    break;
                case 6:
                    color = "#006600";
                    break;
            }

            return color;
        }

        public int currencyId { get; set; }
        public Currency currency { get; set; }
        public int? companyId { get; set; }
        public Company company { get; set; }
        public int? userId { get; set; }
        public int tipoComprobante { get; set; }
        public int talon { get; set; }
        public int numero { get; set; }
        public DateTime fecha { get; set; }
        public string fechaStr => fecha.ToString("d");
        public int divisionCuentaId { get; set; }
        public int cuenta { get; set; }
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
        public int PlacePayment { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryResponsible { get; set; }
        public string DeliveryAddress { get; set; }
        public Transport Transport { get; set; }
        public int? TransportId { get; set; }
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



            public string PriceStr => TransformarStr(this.price);
            public string SubTotalStr => TransformarStr(this.subtotal);
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

    }    
}
