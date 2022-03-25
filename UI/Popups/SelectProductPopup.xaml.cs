using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core;
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
        public event EventHandler<(double Price, int Quantity, int Discount)> OkTapped;

        private readonly Product product;
        private readonly IToastService toastService;

        private ApplicationData data;

        public SelectProductPopup(Product product, bool isEditing = false)
        {
            InitializeComponent();
            toastService = MvxIoCProvider.Instance.Resolve<IToastService>();

            data = new ApplicationData();

            CloseWhenBackgroundIsClicked = true;
            this.product = product;

            if (product.priceList != null)
            {
                cmbListPrice.ItemsSource = CrearSource(product.priceList);
            }

            descriptionLabel.Text = product.name;
            priceInput.Text = product.price.ToString();
            quantityInput.Text = isEditing ? product.quantity.ToString() : "1";
            discountInput.Text = isEditing ? product.Discount.ToString() : "0";
        }

        private IList CrearSource(List<PriceList> priceList)
        {
            List<string> lista = new List<string>();

            foreach (var item in priceList)
            {
                lista.Add($"{item.description} - US$ {item.price}");
            }

            return lista;
        }

        private void OkButtonClicked(object sender, EventArgs e)
        {
            var languages = CultureInfo.CurrentCulture.Name;
            if (languages == "es-US")
            {
                (double Price, int Quantity, int Discount) result =
                   (Price: double.Parse(priceInput.Text.Replace(",", ".")), Quantity: int.Parse(quantityInput.Text), Discount: int.Parse(discountInput.Text));

                if (result.Discount > 100)
                {
                    toastService.ShowError("El descuento no puede ser mayor a 100%.");
                    return;
                }

                if (result.Quantity <= 0)
                {
                    toastService.ShowError("La cantidad deber ser 1 o mayor.");
                    return;
                }

                OkTapped?.Invoke(this, result);
            }
            else
            {

                (double Price, int Quantity, int Discount) result =
                (Price: double.Parse(priceInput.Text), Quantity: int.Parse(quantityInput.Text), Discount: int.Parse(discountInput.Text));

                if (result.Discount > 100)
                {
                    toastService.ShowError("El descuento no puede ser mayor a 100%.");
                    return;
                }

                if (result.Quantity <= 0)
                {
                    toastService.ShowError("La cantidad deber ser 1 o mayor.");
                    return;
                }

                OkTapped?.Invoke(this, result);
            }
        }

        private async void CancelButtonClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(false);
            NotifyDismiss();
        }

        private void PriceInputTextChanged(object sender, TextChangedEventArgs e)
        {
            if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                subtotalInput.Text = ComputeTotal().ToString("N2", new CultureInfo("es-ES"));
            }
            else
            {
                subtotalInput.Text = ComputeTotal().ToString("N2", new CultureInfo("en-US"));
            }
        }

        private void QuantityInputTextChanged(object sender, TextChangedEventArgs e)
        {
            if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                subtotalInput.Text = ComputeTotal().ToString("N2", new CultureInfo("es-ES"));
            }
            else
            {
                subtotalInput.Text = ComputeTotal().ToString("N2", new CultureInfo("en-US"));
            }
        }

        private void DiscountInputTextChanged(object sender, TextChangedEventArgs e)
        {
            if(data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                subtotalInput.Text = ComputeTotal().ToString("N2", new CultureInfo("es-ES"));
            }
            else
            {
                subtotalInput.Text = ComputeTotal().ToString("N2", new CultureInfo("en-US"));
            }
        }

        private double ComputeTotal()
        {
            try
            {
                if (lblPrice.Text == "Price") //para cuando el idioma esta en ingles
                {

                    var Price = double.Parse(priceInput.Text.Replace(",", "."));
                    var Quantity = int.Parse(quantityInput.Text);
                    var Discount = int.Parse(discountInput.Text);

                    double tempTotal = Price * Quantity;
                    if (Discount == 0)
                    {
                        return tempTotal;
                    }
                    else
                    {
                        return tempTotal - (tempTotal * Discount / 100);
                    }
                }
                else // para cuando el idioma esta en español
                {
                    var languages = CultureInfo.CurrentCulture.Name;
                    if (languages == "es-US")
                    {
                        var labelPrice = priceInput.Text.Replace(",", ".");

                        var Price = double.Parse(labelPrice);
                        var Quantity = int.Parse(quantityInput.Text);
                        var Discount = int.Parse(discountInput.Text);

                        double tempTotal = Price * Quantity;
                        if (Discount == 0)
                        {
                            return tempTotal;
                        }
                        else
                        {
                            return tempTotal - (tempTotal * Discount / 100);
                        }
                    }
                    else
                    {
                        var Price = double.Parse(priceInput.Text);
                        var Quantity = int.Parse(quantityInput.Text);
                        var Discount = int.Parse(discountInput.Text);

                        double tempTotal = Price * Quantity;
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
            catch (Exception e)
            {
                return 0;
            }
        }

        private void cmbListPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cmbListPrice.SelectedIndex;

            var languages = CultureInfo.CurrentCulture.Name;
            if (languages == "es-US")
            {
                priceInput.Text = product.priceList[index].price.ToString().Replace(".",",");
            }
        }
    }
}