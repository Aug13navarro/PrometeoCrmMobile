using Core.Model;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChangeCompanyPopupPage : PopupPage
	{
		public delegate void ChangeHandler(int companyId);
		public event ChangeHandler ItChanged;

		public ChangeCompanyPopupPage (List<Company> companies)
		{
			InitializeComponent();
			CargarListaFront(companies);
		}

		private void CargarListaFront(List<Company> companies)
		{
			foreach (var item in companies)
			{
				var frameOption = new Frame
				{
					HasShadow = false,
					Padding = new Thickness(10),
					Content = new Label
					{
						Text = item.BusinessName
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