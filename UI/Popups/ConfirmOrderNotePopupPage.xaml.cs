using System;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmOrderNotePopupPage : BasePopupPage
    {
        public event EventHandler<(bool confirmed, bool notConfirmed)> OkTapped;
        
        public ConfirmOrderNotePopupPage()
        {
            InitializeComponent();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            (bool confirmed, bool notConfirmed) result = (confirmed: true, notConfirmed: false);
            OkTapped?.Invoke(this, result);
        }

        private async void Button_OnClicked_Cancel(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}