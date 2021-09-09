using MvvmCross.ViewModels;

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

        public double Total { get; set; }
        
        //para pasar company a producto
        public int CompanyId { get; set; }
    }
}
