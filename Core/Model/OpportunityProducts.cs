using MvvmCross.ViewModels;

namespace Core.Model
{
    public class OpportunityProducts : MvxNotifyPropertyChanged
    {
        public int productId { get; set; }
        public Product product { get; set; }

        //private string descrption;
        //public string Description
        //{
        //    get => descrption;
        //    set
        //    {
        //        SetProperty(ref descrption, value);
        //    }
        //}

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                SetProperty(ref quantity, value);
                Total = ComputeTotal();
            }
        }

        private int discount;
        public int Discount
        {
            get => discount;
            set
            {
                SetProperty(ref discount, value);
                Total = ComputeTotal();
            }
        }

        public decimal Price => product.price;

        //private decimal price;
        //public decimal Price
        //{
        //    get => price;
        //    set
        //    {
        //        SetProperty(ref price, value);
        //        Total = ComputeTotal();
        //    }
        //}

        private decimal total;
        public decimal Total
        {
            get => total;
            private set => SetProperty(ref total, value);
        }

        private decimal ComputeTotal()
        {
            decimal tempTotal = Price * Quantity;
            if (Discount == 0)
            {
                return tempTotal;
            }
            else
            {
                return tempTotal - (tempTotal * Discount / 100);
            }
        }
    }
}
