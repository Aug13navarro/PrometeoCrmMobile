using MvvmCross.ViewModels;
using System;
using System.Drawing;

namespace Core.Model
{
    public class OrderNote
    {
        public int  id { get; set; }
        public int opportunityId { get; set; }
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
                    estado = "Generado";
                    break;
                case 3:
                    estado = "Entregado";
                    break;
            }

            return estado;
        }

        public Color orderColor => AsignarColor(this.orderStatus);

        private Color AsignarColor(int orderStatus)
        {
            var color = new Color();

            switch (orderStatus)
            {
                case 1:
                    color = Color.Gray;
                    break;
                case 2:
                    color = Color.LightBlue;
                    break;
                case 3:
                    color = Color.Green;
                    break;
            }

            return color;
        }

        public int currencyId { get; set; }
        public Currency currency { get; set; }
        public int companyId { get; set; }
        public Company company { get; set; }
        public int tipoComprobante { get; set; }
        public int talon { get; set; }
        public int numero { get; set; }
        public DateTime fecha { get; set; }
        public int divisionCuentaId { get; set; }
        public int cuenta { get; set; }
        public int tipoCuentaId { get; set; }
        public int tipoServicioId { get; set; }
        public int customerId { get; set; }
        public Customer customer { get; set; }
        public int paymentConditionId { get; set; }
        public int discount { get; set; }
        public decimal total { get; set; }
        //public int createBy { get; set; }
        public MvxObservableCollection<ProductOrder> products { get; set; }
        public MvxObservableCollection<OpportunityProducts> Details { get; set; }

        public class ProductOrder
        {
            public int companyProductPresentationId { get; set; }
            public int quantity { get; set; }
            public decimal price { get; set; }
            public int discount { get; set; }
            public decimal subtotal { get; set; }
            public int arancel { get; set; }
            public int bonificacion { get; set; }
        }

    }
}
