using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Root, WrapInNavigationPage = false)]
    public partial class RootPage : MvxMasterDetailPage<RootViewModel>
    {
        public bool IsMainMenuVisible => IsPresented;

        public RootPage()
        {
            InitializeComponent();

            // Por defecto no permito que se active el menú deslizando el dedo por el lado izquierdo de la pantalla. Para aquellas páginas
            // donde quiera tener este comportamiento, voy a tener que poner esta propiedad en TRUE en el init/initialize de la página. Pero 
            // ojo, porque al navegar hacia otra página donde nuevamente no quiero este gesture enabled, tengo que de alguna manera restaurar
            // el valor en FALSE: ya sea en la página original, en algún evento OnViewDisappearing o similar; o sino en el init/initialize de
            // cada página, setear siempre el valor por defecto de IsGestureEnabled.
            IsGestureEnabled = false;
        }

        public void ShowMainMenu()
        {
            IsPresented = true;
        }

        public void HideMainMenu()
        {
            IsPresented = false;
        }
    }
}
