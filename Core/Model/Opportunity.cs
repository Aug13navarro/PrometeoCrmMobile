using System;
using System.Globalization;
using System.Linq;
using Core.Helpers;
using MvvmCross.ViewModels;

namespace Core.Model
{
    public class Opportunity : MvxNotifyPropertyChanged
    {
        public int Id { get; set; }

        public Customer  customer { get; set; }
        public Company Company { get; set; }

        public OpportunityStatus opportunityStatus { get; set; }

        public DateTime createDt { get; set; }
        public DateTime closedDate { get; set; }

        public MvxObservableCollection<OpportunityProducts> opportunityProducts { get; set; } = new MvxObservableCollection<OpportunityProducts>();

        public string description { get; set; }

        public decimal totalPrice { get; set; }

        public string ProductsDescription => string.Join(", ", opportunityProducts.Select(x => x.product.name));

        public MvxObservableCollection<OpportunityProducts> Details { get; set; } = new MvxObservableCollection<OpportunityProducts>();

        public string totalPriceStr => TransformarStr(this.totalPrice);

        private string TransformarStr(decimal value)
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
