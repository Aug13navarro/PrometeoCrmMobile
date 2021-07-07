using System;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpportunityOptionsPopup : BasePopupPage
    {
        public event EventHandler EditTapped;

        public OpportunityOptionsPopup()
        {
            InitializeComponent();
        }

        private void EditLabelTapped(object sender, EventArgs e)
        {
            EditTapped?.Invoke(this, EventArgs.Empty);
        }

        private void GeneratePropositionLabelTapped(object sender, EventArgs e)
        {
        }
    }
}
