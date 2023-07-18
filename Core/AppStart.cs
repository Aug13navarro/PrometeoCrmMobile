using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class AppStart<TViewModel> : MvxAppStart<TViewModel> where TViewModel : MvxViewModel
    {
        public AppStart(IMvxApplication application, IMvxNavigationService navigationService) : base(application, navigationService)
        {
        }

        //protected override void Start(TViewModel viewModel)
        //{
        //    // Lógica personalizada al iniciar la aplicación
        //    var s = "";
        //    // Navegar a la primera pantalla o realizar otras acciones
        //}
    }
}
