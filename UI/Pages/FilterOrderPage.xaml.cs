﻿using Core.ViewModels;
using MvvmCross.Forms.Views;
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
    }
}