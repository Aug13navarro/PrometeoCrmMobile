using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Helpers;
using Core.Model;
using Core.Services.Contracts;
using Core.Services.Exceptions;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class LoginViewModel : MvxViewModel
    {
        // Properties
        //private string userName = "fladino@docworld.com.ar";
        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        //private string password = "123";
        private string password;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private bool isLogging;
        public bool IsLogging
        {
            get => isLogging;
            private set => SetProperty(ref isLogging, value);
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        // Commands
        public IMvxCommand LoginCommand { get; }
        public Command RecoverPasswordCommand { get; }
        // Fields
        private readonly ApplicationData appData;

        // Services
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly INotificationService notificationService;

        public LoginViewModel(IMvxNavigationService navigationService,
                              IPrometeoApiService prometeoApiService,
                              ApplicationData appData,
                              INotificationService notificationService)
        {
            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            this.appData = appData;
            this.notificationService = notificationService;

            RecoverPasswordCommand = new Command(async () => await RecoverPassword());

            LoginCommand = new MvxAsyncCommand(LoginAsync);
        }

        private async Task RecoverPassword()
        {
            await navigationService.Navigate<RecoverPasswordViewModel>();
        }

        private async Task LoginAsync()
        {
            try
            {
                ErrorMessage = "";
                IsLogging = true;

                if (string.IsNullOrWhiteSpace(UserName))
                {
                    ErrorMessage = "Debe ingresar un usuario.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Debe ingresar una contraseña.";
                    return;
                }

                LoginData loginData = await prometeoApiService.Login(UserName, Password);
                UserData userData = await prometeoApiService.GetUserData(loginData.Id);

                var user = new User()
                {
                    Id = loginData.Id,
                    Token = loginData.Token,
                    Expiration = loginData.Expiration,
                    FullName = userData.FullName,
                    Email = userData.Email,
                    Language = loginData.Language == null ? new Language { name = "English", abbreviation = "EN", id = 0 } : loginData.Language,
                };

                user.RolesStr = JsonConvert.SerializeObject(userData.Roles);

                appData.SetLoggedUser(user);
                Identity.LanguageUser = user.Language.abbreviation;



                //notificationService.StartListeningNotifications();

                CultureInfo language;
                string lang = user.Language.abbreviation.ToLower();

                if (lang == "es" || lang.Contains("spanish"))
                    language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("Spanish"));
                else
                    language = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains("English"));

                Thread.CurrentThread.CurrentUICulture = language;
                MessagingCenter.Send(this, "LangChanged", language);

                await navigationService.Navigate<HomeViewModel>();
                await navigationService.Navigate<MenuViewModel>();
            }
            catch (ServiceException ex)
            {
                MessagingCenter.Send(this, "invalidCredentials");
                //ErrorMessage = "El usuario o la contraseña no es válido.";
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "errorLogin");
                //ErrorMessage = "Ocurrió un error al iniciar sesión. Compruebe su conexión a internet.";
            }
            finally
            {
                IsLogging = false;
            }
        }
    }
}
