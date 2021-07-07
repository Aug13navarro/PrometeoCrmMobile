using System.Threading.Tasks;
using MvvmCross.Commands;
using Xamarin.Forms;

namespace UI.Controls
{
    public class BackIconToolbar : Image
    {
        public BackIconToolbar()
        {
            Source = "ic_arrow_back.png";
            GestureRecognizers.Add(new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1,
                Command = new MvxAsyncCommand(GoBack),
            });
        }

        private Task GoBack()
        {
            return Navigation.PopAsync();
        }
    }
}
