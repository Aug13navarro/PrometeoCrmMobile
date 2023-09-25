using Core.Model;
using Rg.Plugins.Popup.Pages;
using System.Collections.Generic;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Core.Services;
using Core.Helpers;
using Core;
using Newtonsoft.Json;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChangeStatusOrderNotePopup : PopupPage
	{
        public delegate void ChangeHandler(int id);
        public event ChangeHandler ItChanged;

        public ChangeStatusOrderNotePopup ()
		{
			InitializeComponent ();
            CargarListaFront();

        }
        private void CargarListaFront()
        {
            var data = new ApplicationData();
            var status = JsonConvert.DeserializeObject<List<StatusOrderNote>>(data.LoggedUser.StatusOrderNotes);

            foreach (var item in status)
            {
                var frameOption = new Frame
                {
                    HasShadow = false,
                    Padding = new Thickness(10),
                    Content = new Label
                    {
                        Text = item.Name
                    },
                    AutomationId = item.Id.ToString()
                };

                var gestureRecognizer = new TapGestureRecognizer();
                gestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

                frameOption.GestureRecognizers.Add(gestureRecognizer);

                StackPrincipal.Children.Add(frameOption);
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var frame = (Frame)sender;

            ItChanged(Convert.ToInt32(frame.AutomationId));
        }
    }
}