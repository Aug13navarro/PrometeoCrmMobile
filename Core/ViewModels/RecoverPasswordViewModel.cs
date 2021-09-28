using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.Presenters.Hints;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class RecoverPasswordViewModel : MvxViewModel
    {
        private string mail;
        public string Mail
        {
            get => mail;
            set => SetProperty(ref mail, value);
        }

        public Command RecoverPassCommand { get; }

        private IPrometeoApiService prometeoApiService;
        private IMvxNavigationService navigationService;

        public RecoverPasswordViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService)
        {
            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;

            RecoverPassCommand = new Command(async () => await RecoverPass());
        }

        private async Task RecoverPass()
        {
            try
            {
                var exito = await prometeoApiService.RecoverPassword(Mail);

                if (CultureInfo.InstalledUICulture.EnglishName.Contains("English"))
                {
                    await Application.Current.MainPage.DisplayAlert("Recover Password", $"Check your email", "Acept");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Recuperar Contraseña", $"Revise su correo electrónico", "Aceptar");
                }

                await navigationService.Navigate<LoginViewModel>();

            }
            catch (Exception )
            {
                if (CultureInfo.InstalledUICulture.EnglishName.Contains("English"))
                {
                    await Application.Current.MainPage.DisplayAlert("Attention", $"The user is incorrect or does not exist", "Acept"); return;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atención", $"El usuario es incorrecto o no existe", "Aceptar"); return;
                }
            }
        }
    }
}
