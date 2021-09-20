﻿using MvvmCross.ViewModels;
using System;
using System.Drawing;

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
        public int companyId { get; set; }
        public Company company { get; set; }
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
        public int paymentConditionId { get; set; }
        public int discount { get; set; }
        public decimal total { get; set; }
        //public int createBy { get; set; }
        public MvxObservableCollection<ProductOrder> products { get; set; }
        public MvxObservableCollection<OpportunityProducts> Details { get; set; }

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
        }

    }
}
