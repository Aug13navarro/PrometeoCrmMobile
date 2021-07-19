using MvvmCross.ViewModels;

namespace Core.Model
{
    public class OpportunityProducts : MvxNotifyPropertyChanged
    {
        public int productId { get; set; }
        public Product product { get; set; }
        //public string Description { get; set; }

        //private int quantity;
        //public int Quantity
        //{
        //    get => quantity;
        //    set
        //    {
        //        SetProperty(ref quantity, value);
        //        Total = ComputeTotal();
        //    }
        //}

        //private int discount;
        //public int Discount
        //{
        //    get => discount;
        //    set
        //    {
        //        SetProperty(ref discount, value);
        //        Total = ComputeTotal();
        //    }
        //}

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

        //private decimal total;
        //public decimal Total
        //{
        //    get => total;
        //    private set => SetProperty(ref total, value);
        //}

        //private decimal ComputeTotal()
        //{
        //    decimal tempTotal = Price * Quantity;
        //    if (Discount == 0)
        //    {
        //        return tempTotal;
        //    }
        //    else
        //    {
        //        return tempTotal - (tempTotal * Discount / 100);
        //    }
        //}
    }
}
