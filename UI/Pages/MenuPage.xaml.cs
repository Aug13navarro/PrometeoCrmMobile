using Core.Model;
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

        public MenuItem[] MenuItems { get; } =
        {
            new MenuItem(MenuItemType.Pedidos, LangResources.AppResources.Orders, "ic_menu_pedidos"),
            new MenuItem(MenuItemType.Customers, LangResources.AppResources.Customers, "ic_menu_cuentas"),
            new MenuItem(MenuItemType.Contacts, LangResources.AppResources.Contacts, "ic_menu_contactos"),
            new MenuItem(MenuItemType.Opportunities, LangResources.AppResources.Opportunities, "ic_menu_cuentasic_menu_oportunidades"),
            new MenuItem(MenuItemType.Logout, LangResources.AppResources.CloseSession, "ic_keyboard_backspace"),
        };

        private async void OnMenuItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            string cont = LangResources.AppResources.Contacts;
            var menuItem = (MenuItem)e.Item;
            await ViewModel.GoToMenu(menuItem.Type);

            FormsApp.RootPage.HideMainMenu();
        }
    }
}
