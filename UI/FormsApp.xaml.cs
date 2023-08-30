using AutoMapper;
using Core;
using Core.Data;
using Core.Helpers;
using Core.Model;
using Core.Notification;
using Core.Services;
using Core.Services.Contracts;
using Core.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UI.LangResources;
using UI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace UI
{
    public partial class FormsApp : Application
    {
        private readonly ApplicationData appData;

        public static RootPage RootPage
        {
            get
            {
                Application app = Current;
                return (RootPage)app.MainPage;
            }
        }

        public INotificationManager notificationManager;
        IMapper mapper;

        public FormsApp()
        {
            this.appData = new ApplicationData();
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
            });

            mapper = mapperConfig.CreateMapper();

            MessagingCenter.Subscribe<LoginViewModel, CultureInfo>(this, "LangChanged", (sender, currentCulture) =>
            {
                AppResources.Culture = currentCulture;
            });

            if (this.appData != null && this.appData.LoggedUser != null)
            {
                if (this.appData.LoggedUser.Language.abbreviation.ToLower().Contains("es"))
                {
                    var idiomas = CultureInfo.GetCultures(CultureTypes.NeutralCultures).Where(x => x.DisplayName.ToLower().Contains("español")).ToList();
                    var lang = new CultureInfo("es-ES");
                    AppResources.Culture = new CultureInfo("es-ES");


                    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-ES");
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                }
                else
                {
                    AppResources.Culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault(element => element.EnglishName.Contains("English"));
                }
            }
            else
            {
                AppResources.Culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault(element => element.EnglishName.Contains("English"));
            }

            Thread.CurrentThread.CurrentUICulture = AppResources.Culture;
            notificationManager = DependencyService.Get<INotificationManager>();

            InitializeComponent();
        }

        protected override void OnStart()
        {
            base.OnStart();
            CrossConnectivity.Current.ConnectivityChanged += HendleConnectivityChanged;
            Conectarse();
        }
        protected override void OnResume()
        {
            base.OnResume();
            CrossConnectivity.Current.ConnectivityChanged += HendleConnectivityChanged;
            //Conectarse();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            CrossConnectivity.Current.ConnectivityChanged += HendleConnectivityChanged;
            //Conectarse();
        }

        private async void HendleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                if (e.IsConnected)
                {
                    //ConnectionRed.SeeAlertConnection(e.IsConnected);

                    string title = $"Aviso de Conexión";
                    string message = $"'Prometeo Suite' se conectó a internet.";
                    notificationManager.SendNotification(title, message, 0, false);

                    var s = SincronizacionService();

                    //await s.Sincronizar();
                }
                else
                {
                    //ConnectionRed.SeeAlertConnection(e.IsConnected);

                    string title = $"Aviso de Conexión";
                    string message = $"'Prometeo Suite' perdió conexión a internet, seguira trabajando de manera Offline.";
                    notificationManager.SendNotification(title, message, 0, false);
                }
            }
            catch (Exception ex)
            {
                string title = $"Aviso de Conexión";
                string message = $"Sincronización interrumpida.";
                notificationManager.SendNotification(title, message, 10, true);

                await Application.Current.MainPage.DisplayAlert("Sincronizando", $"Ocurrio un error al sincronizar los dotos locales con el Servicio - {ex.Message}", "Aceptar"); return;
            }
        }

        private async Task<bool> SincronizacionService()
        {
            try
            {
                var pedidos = OfflineDatabase.GetOrderNotes();

                if (pedidos != null && pedidos.Count > 0)
                {
                    string title = $"Aviso de Sincronización";
                    string message = $"'Sincronizando datos almacenados de manera Offline.";
                    notificationManager.SendNotification(title, message, 0, false);

                    var service = new SincronizacionService();

                    var result = await service.SincronizarPedidosOffline(mapper.Map<List<OrderNote>>(pedidos), appData.LoggedUser.Token);

                    if (result)
                    {
                        // notificacion de exito
                        string titleE = $"Aviso de Sincronización";
                        string messageE = $"'Sincronización completada con Exito";
                        notificationManager.SendNotification(titleE, messageE, 0, false);

                        // borrar todos lo PV creados de manera Offline
                        await OfflineDatabase.DeleteAllOrderNote();
                        return true;
                    }

                    return false;
                }

                return true;
            }
            catch(Exception e)
            {
                var mensaje = e.Message;
                //mostrar mensaje de error 
                string title = $"Aviso de Sincronización fallida";
                string message = $"'Ocurrió un error al sincronizar uno o mas registros.";
                notificationManager.SendNotification(title, message, 0, false);

                //borrar aquellos pedidos que fueron sincronizados y dejar solamente aquellos que tiraron error 
                if (message.Contains("["))
                {
                    var list = JsonConvert.DeserializeObject<List<int>>(mensaje);
                    await OfflineDatabase.DeleteOrderNote(list);
                }
                return false;
            }
        }

        private async void Conectarse()
        {
            try
            {
                // create a HubConnection instance
                var cadena = EndpointURL.PrometeoApiEndPoint.ToString().Replace("/api", "") + "echo/";

                var hubConnection = new HubConnectionBuilder()
                    .WithUrl(cadena, options =>
                    {
                        options.Headers.Add("user-device", "mobile");
                    })
                    .Build();

                // register event handlers
                hubConnection.Closed += async (error) =>
                {
                    await Task.Delay(1000);
                    await hubConnection.StartAsync();
                };

                // start the connection
                await hubConnection.StartAsync();

                // register message handlers
                hubConnection.On<object>("GetCurrentCompany", data =>
                {
                    //deserealizar el objeto para guardar nuevos permisos 
                    var dataChange = JsonConvert.DeserializeObject<DataChangeCompany>(data.ToString());

                    if (dataChange.CompanyId != appData.LoggedUser.CompanyId.Value && dataChange.UserId == appData.LoggedUser.Id)
                    {
                        if (!string.IsNullOrEmpty(dataChange.Device))
                        {
                            if (dataChange.Device.Contains("web"))
                            {
                                string title = $"Notificación de Alerta";
                                string message = $"Se cambió la Empresa actual para el usuario logeuado.";
                                //notificationManager.SendNotification(title, message, 0, false);
                                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                                {
                                    var response = await Application.Current.MainPage.DisplayAlert("Alerta", "Se cambió la empresa logueda para el Usuario actual", "", "OK");

                                    if (!response)
                                    {
                                        var user = appData.LoggedUser;

                                        if (user != null)
                                        {
                                            //user.RolesAssignmetn = null;
                                            //user.UserCompanies = user.UserCompaniesTotal.Where(x => x.CompanyId == dataChange.CompanyId).ToList();
                                            //user.Permissions = user.PermissionsTotal.Where(x => x.CompanyId == dataChange.CompanyId).ToList();
                                            user.Token = dataChange.Token.Token;
                                            user.CompanyId = dataChange.CompanyId;
                                            appData.SetLoggedUser(user);

                                            //MainViewModel.GetInstance().Menu = new MenuViewModel(true, dataChange);
                                            //MainPage = new NavigationPage(new HomePage());
                                        }
                                    }
                                });
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                var m = e.Message;
            }
        }
    }
}
