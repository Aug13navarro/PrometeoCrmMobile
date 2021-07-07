using System;
using Rg.Plugins.Popup.Pages;

namespace UI.Popups
{
    public class BasePopupPage : PopupPage
    {
        public event EventHandler Dismissed;

        public BasePopupPage()
        {
            CloseWhenBackgroundIsClicked = true;
            BackgroundClicked += OnBackgroundClicked;
        }

        private void OnBackgroundClicked(object sender, EventArgs e)
        {
            NotifyDismiss();
        }

        public void NotifyDismiss()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
