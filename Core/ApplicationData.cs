using System;
using System.Collections.Generic;
using Core.Model;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

namespace Core
{
    public class ApplicationData : MvxNotifyPropertyChanged
    {
        private static User loggedUser;
        public User LoggedUser
        {
            get
            {
                if (loggedUser == null)
                {
                    string token = Preferences.Get("token", null, "loginData");
                    string idStr = Preferences.Get("id", null, "loginData");

                    if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(idStr))
                    {
                        loggedUser = null;
                    }
                    else
                    {
                        var user = new User();

                        int id = int.Parse(idStr);
                        string fullName = Preferences.Get("fullName", null, "loginData");
                        string email = Preferences.Get("email", null, "loginData");
                        string language = Preferences.Get("language", null, "loginData");
                        string tokenExpirationDate = Preferences.Get("tokenExpirationDate", null, "loginData");

                        user.Id = id;
                        user.Email = email;
                        user.FullName = fullName;
                        user.Token = token;
                        user.Language = language;
                        user.Expiration = !string.IsNullOrWhiteSpace(tokenExpirationDate)
                                              ? DateTime.Parse(tokenExpirationDate)
                                              : (DateTime?)null;

                        loggedUser = user;
                    }
                }
                return loggedUser;
            }
            private set => SetProperty(ref loggedUser, value);
        }

        public void SetLoggedUser(User user)
        {
            Preferences.Set("id", user.Id.ToString(), "loginData");
            Preferences.Set("fullName", user.FullName, "loginData");
            Preferences.Set("email", user.Email, "loginData");
            Preferences.Set("token", user.Token, "loginData");
            Preferences.Set("language", user.Language, "loginData");
            Preferences.Set("tokenExpirationDate", user.Expiration.ToString(), "loginData");

            LoggedUser = user;
        }

        public void ClearLoggedUser()
        {
            Preferences.Clear("loginData");
            LoggedUser = null;
        }
    }
}
