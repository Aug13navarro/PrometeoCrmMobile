using Core.Model.Enums;
using Core.ViewModels;
using Core.ViewModels.Model;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UI.Popups;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Master)]
    public partial class MenuPage : MvxContentPage<MenuViewModel>
    {
        public List<MenuItems> MenuItems { get; set; }

        public MenuPage()
        {
            InitializeComponent();
            //CargarItems();

            //MenuItems = ViewModel.MenuItems;
            //menuList.ItemsSource = MenuItems;
        }

        //private void CargarItems()
        //{
        //    if (ViewModel.LoggedUser.Language.ToLower() == "es" || ViewModel.LoggedUser.Language.Contains("spanish"))
        //    {
        //        MenuItems.Add(new MenuItems(MenuItemType.Opportunities, "Oportunidades", "ic_menu_cuentasic_menu_oportunidades"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Pedidos, "Pedidos", "ic_menu_pedidos"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Customers, "Clientes", "ic_menu_cuentas"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Contacts, "Contactos", "ic_menu_contactos"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Logout, "Cerrar Sesión", "ic_keyboard_backspace"));
        //    }
        //    else
        //    {
        //        MenuItems.Add(new MenuItems(MenuItemType.Opportunities, "Opportunities", "ic_menu_cuentasic_menu_oportunidades"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Pedidos, "Orders", "ic_menu_pedidos"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Customers, "Customers", "ic_menu_cuentas"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Contacts, "Contacts", "ic_menu_contactos"));
        //        MenuItems.Add(new MenuItems(MenuItemType.Logout, "Log out", "ic_keyboard_backspace"));
        //    }
        //}

        

        private async void OnMenuItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            string cont = LangResources.AppResources.Contacts;
            var menuItem = (MenuItems)e.Item;

            if (menuItem.Type == MenuItemType.ChangeCompany)
            {
                var popup = new ChangeCompanyPopupPage(ViewModel.ListCompanies.ToList());

                popup.ItChanged += async (s) =>
                {
                    await PopupNavigation.Instance.PopAsync(false);

                    ViewModel.SetCurrentCompany(s);
                };

                await PopupNavigation.Instance.PushAsync(popup);
            }
            else
            {
                await ViewModel.GoToMenu(menuItem.Type);
            }

            FormsApp.RootPage.HideMainMenu();
        }
    }
}
