using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Enums;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class MenuViewModel : MvxViewModel
    {
        private readonly IOfflineDataService _offlineDataService;

        // Properties
        private User loggedUser;
        public User LoggedUser
        {
            get => loggedUser;
            private set => SetProperty(ref loggedUser, value);
        }

        public List<MenuItems> MenuItems { get; set; }

        // Fields
        private readonly ApplicationData appData;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;
        private readonly INotificationService notificationService;

        public MenuViewModel(ApplicationData appData, IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService, INotificationService notificationService, IOfflineDataService offlineDataService)
        {
            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;
            this.notificationService = notificationService;

            _offlineDataService = offlineDataService;

            MenuItems = new List<MenuItems>();

            this.appData = appData;
            LoggedUser = appData.LoggedUser;


            if (LoggedUser.Language.abbreviation.ToLower() == "es" || LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                MenuItems.Add(new MenuItems(MenuItemType.Opportunities, "Oportunidades", "ic_menu_cuentasic_menu_oportunidades"));
                MenuItems.Add(new MenuItems(MenuItemType.Pedidos, "Pedidos", "ic_menu_pedidos"));
                MenuItems.Add(new MenuItems(MenuItemType.Customers, "Clientes", "ic_menu_cuentas"));
                MenuItems.Add(new MenuItems(MenuItemType.Contacts, "Contactos", "ic_menu_contactos"));
                MenuItems.Add(new MenuItems(MenuItemType.Logout, "Cerrar Sesi??n", "ic_keyboard_backspace"));
            }
            else
            {
                MenuItems.Add(new MenuItems(MenuItemType.Opportunities, "Opportunities", "ic_menu_cuentasic_menu_oportunidades"));
                MenuItems.Add(new MenuItems(MenuItemType.Pedidos, "Orders", "ic_menu_pedidos"));
                MenuItems.Add(new MenuItems(MenuItemType.Customers, "Customers", "ic_menu_cuentas"));
                MenuItems.Add(new MenuItems(MenuItemType.Contacts, "Contacts", "ic_menu_contactos"));
                MenuItems.Add(new MenuItems(MenuItemType.Logout, "Log out", "ic_keyboard_backspace"));
            }

            this.appData.PropertyChanged += OnAppDataPropertyChanged;
        }


        public override async Task Initialize()
        {
            await base.Initialize();
            notificationService.NotificationsUpdated += OnNotificationsUpdated;
        }

        public async Task GoToMenu(MenuItemType menuType)
        {
            switch (menuType)
            {
                case MenuItemType.Logout:
                    await Logout();
                    break;
                case MenuItemType.Customers:
                    await GoToCustomers();
                    break;
                case MenuItemType.Contacts:
                    await GoToContacts();
                    break;
                case MenuItemType.Opportunities:
                    await GoToOpportunities();
                    break;
                case MenuItemType.Pedidos:
                    await GoToPedidos();
                    break;
                case MenuItemType.Sales:
                    await GoToSales();
                    break;
            }
        }

        private async Task GoToSales()
        {

            await navigationService.Navigate<SalesViewModel>();
        }

        private async Task GoToPedidos()
        {
            await navigationService.Navigate<PedidosViewModel>();
        }

        private async Task Logout()
        {
            appData.ClearLoggedUser();
            appData.ClearFilterOrder();
            appData.ClearFiltroAvansado();

            await navigationService.Navigate<LoginViewModel>();
        }

        private async Task GoToCustomers()
        {
            await navigationService.Navigate<CustomersViewModel>();
        }

        private async Task GoToContacts()
        {
            await navigationService.Navigate<ContactsViewModel>();
        }

        private async Task GoToOpportunities()
        {
            await navigationService.Navigate<OpportunitiesViewModel>();
        }

        private void OnAppDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Ac?? tengo un peque??o problema: la p??gina del Men?? es del tipo Master en el layout MasterDetail que tengo definido.
            // Por ende, solo se instancia una ??nica vez (o al menos eso es lo que parece), y adem??s se hace antes de la pantalla de login, lo
            // cual quiere decir que: una vez que yo me logue?? y los datos del usuario logueado se actualizaron (ApplicationData), no tengo forma
            // de poder leer, desde el men??, los datos de login actualizados del usuario. Si bien yo en el constructor de MenuViewModel estoy 
            // guardando el usuario logueado, que lo leo desde el ApplicationData; si posteriormente el usuario logueado en ApplicationData se
            // actualiza, no lo puedo saber desde la propiedad LoggedUser que tengo en MenuViewModel.
            // La forma correcta de haber solucionado esto es la siguiente: la pantalla de Login no debe formar parte del layout MasterDetail, sino
            // que debe ser una p??gina normal. Luego, una vez que me logue??, entonces ah?? s?? creo el layout MasterDetail. Haciendo esto, la p??gina
            // del men?? se instancia una vez que ya me logue??, con lo cual, en el constructor de MenuViewModel, los datos que estoy leyendo de 
            // ApplicationData ya son finales, es decir que no pueden cambiar. Para que cambien, debo desloguearme y volverme a loguear. Pero una
            // vez que me volv?? a loguear, se crea otra instancia de la p??gina del men??, con los datos del usuario logueado nuevamente actualizados.
            // Sin embargo, por m??s que prob?? varias cosas y busqu??, no pude crear el layout de MasterDetail despu??s de haber creado una p??gina
            // normal. No s?? si ser?? una limitaci??n de Xamarin.Forms, o de MvvmCross, o un bugg, o yo no lo estaba haciendo bien: el caso es que
            // de entrada debo crear el layout MasterDetail, caso contrario empiezo a tener problemas con el men??, que no aparece.
            // Por eso es que necesito que se me notifique cuando el usuario logueado cambia en ApplicationData, para yo entonces actualizarlo
            // en el MenuViewModel.
            if (e.PropertyName == nameof(ApplicationData.LoggedUser))
            {
                LoggedUser = appData.LoggedUser;
            }
        }

        private void OnNotificationsUpdated(object sender, bool hasUnreadNotifications)
        {
            MenuItems menu = MenuItems.Single(m => m.Type == MenuItemType.Notifications);
            menu.Icon = hasUnreadNotifications ? "ic_bell_on" : "ic_bell";
        }
    }
}
