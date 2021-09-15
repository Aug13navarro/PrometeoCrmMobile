using Core.Model.Enums;
using Core.ViewModels;
using Core.ViewModels.Model;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Master)]
    public partial class MenuPage : MvxContentPage<MenuViewModel>
    {
        public MenuPage()
        {
            InitializeComponent();
            menuList.ItemsSource = MenuItems;
        }

        public MenuItems[] MenuItems { get; } =
        {
            //new MenuItems(MenuItemType.Sales, LangResources.AppResources.Sales, "ic_menu_presupuestos.png"),
            new MenuItems(MenuItemType.Opportunities, LangResources.AppResources.Opportunities, "ic_menu_cuentasic_menu_oportunidades"),
            new MenuItems(MenuItemType.Pedidos, LangResources.AppResources.Orders, "ic_menu_pedidos"),
            new MenuItems(MenuItemType.Customers, LangResources.AppResources.Customers, "ic_menu_cuentas"),
            new MenuItems(MenuItemType.Contacts, LangResources.AppResources.Contacts, "ic_menu_contactos"),
            new MenuItems(MenuItemType.Logout, LangResources.AppResources.CloseSession, "ic_keyboard_backspace"),
        };

        private async void OnMenuItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            string cont = LangResources.AppResources.Contacts;
            var menuItem = (MenuItems)e.Item;
            await ViewModel.GoToMenu(menuItem.Type);

            FormsApp.RootPage.HideMainMenu();
        }
    }
}
