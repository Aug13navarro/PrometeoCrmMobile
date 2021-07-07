using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Core.Services.Contracts;

namespace PrometeoCrmMobile.Droid.Services
{
    public class ToastDroidService : IToastService
    {
        public void ShowError(string message)
        {
            ShowToast(message, Color.Red);
        }

        public void ShowOk(string message)
        {
            ShowToast(message, Color.DarkGreen);
        }

        private void ShowToast(string message, Color color)
        {
            Toast toast = Toast.MakeText(Application.Context, message, ToastLength.Long);

            toast.View.SetBackgroundColor(color);
            toast.SetGravity(GravityFlags.Bottom, 0, 50);

            toast.Show();
        }
    }
}
