using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System;
using Xamarin.Forms;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = true)]
    public partial class LoginPage : MvxContentPage<LoginViewModel>
    {
        public LoginPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<LoginViewModel>(this, "invalidCredentials", (sender) =>
            {
                (BindingContext.DataContext as LoginViewModel).ErrorMessage = LangResources.AppResources.LoginInvalidMsg;
            });

            MessagingCenter.Subscribe<LoginViewModel>(this, "errorLogin", (sender) =>
            {
                (BindingContext.DataContext as LoginViewModel).ErrorMessage = LangResources.AppResources.ErrorLoginMsg;
            });
        }

        private void RevealPasswordTapped(object sender, EventArgs e)
        {
            passwordEntry.IsPassword = !passwordEntry.IsPassword;
            eyeImage.Source = passwordEntry.IsPassword ? "pw_eye_not" : "pw_eye";
        }
    }
}
