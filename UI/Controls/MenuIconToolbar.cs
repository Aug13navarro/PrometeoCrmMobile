using Xamarin.Forms;

namespace UI.Controls
{
    public class MenuIconToolbar : Image
    {
        public MenuIconToolbar()
        {
            Source = "ic_menu.png";
            GestureRecognizers.Add(new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1,
                Command = new Command(ToggleMenu)
            });
        }

        private void ToggleMenu()
        {
            if (FormsApp.RootPage.IsMainMenuVisible)
            {
                FormsApp.RootPage.HideMainMenu();
            }
            else
            {
                FormsApp.RootPage.ShowMainMenu();
            }
        }
    }
}
