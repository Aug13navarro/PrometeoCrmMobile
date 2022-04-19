using Core.ViewModels;
using MvvmCross.Forms.Views;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterOrderPage : MvxContentPage<FilterOrdersViewModel>
    {
        public FilterOrderPage()
        {
            InitializeComponent();


        }

        private void txtEndPrice_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var language = CultureInfo.CurrentCulture.Name;
            var entri = (Entry)sender;

            if (!string.IsNullOrWhiteSpace(entri.Text))
            {
                if (language.Contains("US"))
                {
                    var sinCaracter = entri.Text.Replace(",", "");

                    if (sinCaracter.Length > 6)
                    {
                        var millones = sinCaracter.Substring(0, sinCaracter.Length - 6);
                        var miles = sinCaracter.Substring(sinCaracter.Length - 6, 3);
                        var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                        txtEndPrice.Text = $"{millones},{miles},{unidades}";
                    }
                    else
                    {
                        if (sinCaracter.Length > 3)
                        {
                            var miles = sinCaracter.Substring(0, sinCaracter.Length - 3);
                            var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                            txtEndPrice.Text = $"{miles},{unidades}";
                        }
                    }


                    ViewModel.TotalHasta = Convert.ToDouble(sinCaracter);
                }
                else
                {
                    var sinCaracter = entri.Text.Replace(".", "");

                    if (sinCaracter.Length > 6)
                    {
                        var millones = sinCaracter.Substring(0, sinCaracter.Length - 6);
                        var miles = sinCaracter.Substring(sinCaracter.Length - 6, 3);
                        var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                        txtEndPrice.Text = $"{millones}.{miles}.{unidades}";
                    }
                    else if (sinCaracter.Length > 3)
                    {
                        var miles = sinCaracter.Substring(0, sinCaracter.Length - 3);
                        var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                        txtEndPrice.Text = $"{miles}.{unidades}";
                    }


                    ViewModel.TotalHasta = Convert.ToDouble(sinCaracter);
                }
            }
            else
            {
                ViewModel.TotalHasta = 0;
            }
        }

        private void txtFromPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            var language = CultureInfo.CurrentCulture.Name;
            var entri = (Entry)sender;

            if (!string.IsNullOrWhiteSpace(entri.Text))
            {
                if (language.Contains("US"))
                {
                    var sinCaracter = entri.Text.Replace(",", "");

                    if (sinCaracter.Length > 6)
                    {
                        var millones = sinCaracter.Substring(0, sinCaracter.Length - 6);
                        var miles = sinCaracter.Substring(sinCaracter.Length - 6, 3);
                        var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                        txtFromPrice.Text = $"{millones},{miles},{unidades}";
                    }
                    else
                    {
                        if (sinCaracter.Length > 3)
                        {
                            var miles = sinCaracter.Substring(0, sinCaracter.Length - 3);
                            var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                            txtFromPrice.Text = $"{miles},{unidades}";
                        }
                    }


                    ViewModel.TotalDesde = Convert.ToDouble(sinCaracter);
                }
                else
                {
                    var sinCaracter = entri.Text.Replace(".", "");

                    if (sinCaracter.Length > 6)
                    {
                        var millones = sinCaracter.Substring(0, sinCaracter.Length - 6);
                        var miles = sinCaracter.Substring(sinCaracter.Length - 6, 3);
                        var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                        txtFromPrice.Text = $"{millones}.{miles}.{unidades}";
                    }
                    else if (sinCaracter.Length > 3)
                    {
                        var miles = sinCaracter.Substring(0, sinCaracter.Length - 3);
                        var unidades = sinCaracter.Substring(sinCaracter.Length - 3, 3);

                        txtFromPrice.Text = $"{miles}.{unidades}";
                    }


                    ViewModel.TotalDesde = Convert.ToDouble(sinCaracter);
                }
            }
            else
            {
                ViewModel.TotalDesde = 0;
            }
        }
    }
}