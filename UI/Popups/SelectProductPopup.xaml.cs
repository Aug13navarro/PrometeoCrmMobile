using System;
using System.Globalization;
using Core.Model;
using Core.Services.Contracts;
using MvvmCross.IoC;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectProductPopup : BasePopupPage
    {
        public event EventHandler<(decimal Price, int Quantity, int Discount)> OkTapped;

        private readonly Product product;
        private readonly IToastService toastService;

        public SelectProductPopup(Product product, bool isEditing = false)
        {
            InitializeComponent();
            toastService = MvxIoCProvider.Instance.Resolve<IToastService>();

            CloseWhenBackgroundIsClicked = true;
            this.product = product;

            descriptionLabel.Text = product.name;
            priceInput.Text = product.price.ToString(CultureInfo.InvariantCulture);
            quantityInput.Text = isEditing ? product.quantity.ToString() : "1";
            discountInput.Text = isEditing ? product.Discount.ToString() : "0";
        }

        private void OkButtonClicked(object sender, EventArgs e)
        {
            (decimal Price, int Quantity, int Discount) result =
                (Price: decimal.Parse(priceInput.Text), Quantity: int.Parse(quantityInput.Text), Discount: int.Parse(discountInput.Text));

            if (result.Discount > 100)
            {
                toastService.ShowError("El descuento no puede ser mayor a 100%.");
                return;
            }

            if (result.Quantity <= 0)
            {
                toastService.ShowError("La cantPriceInputTextChangedyor a 1.");
                return;
            }

            OkTapped?.Invoke(this, result);
        }

        private async void CancelButtonClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(false);
            NotifyDismiss();
        }

        private void PriceInputTextChanged(object sender, TextChangedEventArgs e)
        {
            subtotalInput.Text = $"${ComputeTotal():0.##}";
        }

        private void QuantityInputTextChanged(object sender, TextChangedEventArgs e)
        {
            subtotalInput.Text = $"${ComputeTotal():0.##}";
        }

        private void DiscountInputTextChanged(object sender, TextChangedEventArgs e)
        {
            subtotalInput.Text = $"${ComputeTotal():0.##}";
        }

        private decimal ComputeTotal()
        {
            try
            {
                (decimal Price, int Quantity, int Discount) result =
                    (Price: decimal.Parse(priceInput.Text), Quantity: int.Parse(quantityInput.Text), Discount: int.Parse(discountInput.Text));

                decimal tempTotal = result.Price * result.Quantity;
                if (result.Discount == 0)
                {
                    return tempTotal;
                }
                else
                {
                    return tempTotal - (tempTotal * result.Discount / 100);
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
