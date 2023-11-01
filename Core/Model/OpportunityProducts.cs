using Core.Helpers;
using MvvmCross.ViewModels;
using System.Globalization;

namespace Core.Model
{
    public class OpportunityProducts : MvxNotifyPropertyChanged
    {
        public int productId { get; set; }
        public Product product { get; set; }

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                SetProperty(ref quantity, value);
            }
        }

        private int discount;
        public int Discount
        {
            get => discount;
            set
            {
                SetProperty(ref discount, value);
            }
        }

        public double Price { get; set; }

        public string PriceStr => TransformarStr(this.Price);

        public double Total { get; set; }

        public string TotalStr => TransformarStr(this.Total);

        //para pasar company a producto
        public int CompanyId { get; set; }
        public double SubtotalProducts { get; set; }
        public double DiscountPrice { get; set; }

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
}
