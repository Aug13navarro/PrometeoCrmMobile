using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CerrarOportunidadPopup : BasePopupPage
    {
        public event EventHandler GanadaTapped;
        public event EventHandler PerdidaTapped;

        public CerrarOportunidadPopup()
        {
            InitializeComponent();
        }

        private void CerradaGanada_Tapped(object sender, EventArgs e)
        {
            GanadaTapped?.Invoke(this, EventArgs.Empty);
        }
        private void CerradaPerdida_Tapped(object sender, EventArgs e)
        {
            PerdidaTapped?.Invoke(this, EventArgs.Empty);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(false);
        }
    }
}