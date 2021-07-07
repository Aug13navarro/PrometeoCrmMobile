using PrometeoCrmMobile.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(MasterDetailPage), typeof(CustomMasterDetailPageRenderer))]

namespace PrometeoCrmMobile.Droid
{
    // Necesito un renderer custom para poder ocultar el ícono del menú, dado que solamente quiero un toolbar completamente
    // vacío que yo pueda customizar a gusto.
    public class CustomMasterDetailPageRenderer : MasterDetailPageRenderer
    {
        public CustomMasterDetailPageRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            if (toolbar != null)
            {
                toolbar.NavigationIcon = null;
            }
        }
    }
}
