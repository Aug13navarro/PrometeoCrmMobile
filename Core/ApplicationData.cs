using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model;
using Core.ViewModels.Model;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Core
{
    public class ApplicationData : MvxNotifyPropertyChanged
    {
        private static User loggedUser;
        private string initialFilter;
        private string initialFilterOrder;

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
                        string roles = Preferences.Get("roles", null, "loginData");

                        user.Id = id;
                        user.Email = email;
                        user.FullName = fullName;
                        user.Token = token;
                        user.Language = JsonConvert.DeserializeObject<Language>(language);
                        user.RolesStr = roles;
                        //user.Expiration = !string.IsNullOrWhiteSpace(tokenExpirationDate)
                        //                      ? DateTime.Parse(tokenExpirationDate)
                        //                      : (DateTime?)null;

                        loggedUser = user;
                    }
                }
                return loggedUser;
            }
            private set => SetProperty(ref loggedUser, value);
        }
        public string InitialFilter
        {
            get
            {
                if (initialFilter == null)
                {
                    var json = Preferences.Get("filtroJson", null, "filtroAvanzado");

                    if (json == null || string.IsNullOrWhiteSpace(json))
                    {
                        initialFilter = null;
                    }
                    else
                    {
                        //var filterAdvance = new FilterOportunityModel
                        //{
                        //    dateFrom = DateTime.Parse(Preferences.Get("dateFrom", null, "filtroAvanzado")),
                        //    dateTo = DateTime.Parse(Preferences.Get("dateTo",null,"filtroAvanzado")),

                        //};

                        //var jsonString = Preferences.Get("filtroJson", null, "filtroAvanzado");

                        initialFilter = json;
                    }
                }

                return initialFilter;
            }
            private set => SetProperty(ref initialFilter, value);
        }

        public string InitialFilterOrder
        {
            get
            {
                if(initialFilterOrder == null)
                {
                    var json = Preferences.Get("filtroOrderJson", null, "filtroAvanzadoOrder");

                    if(json == null || string.IsNullOrWhiteSpace(json))
                    {
                        initialFilterOrder = null;
                    }
                    else
                    {
                        initialFilterOrder = json;
                    }
                }

                return initialFilterOrder;
            }
            private set => SetProperty(ref initialFilterOrder, value);
        }

        public void SetLoggedUser(User user)
        {
            Preferences.Set("id", user.Id.ToString(), "loginData");
            Preferences.Set("fullName", user.FullName, "loginData");
            Preferences.Set("email", user.Email, "loginData");
            Preferences.Set("token", user.Token, "loginData");
            Preferences.Set("tokenExpirationDate", user.Expiration.ToString(), "loginData");
            Preferences.Set("roles", user.RolesStr, "loginData");

            Preferences.Set("language", JsonConvert.SerializeObject(user.Language), "loginData");

            LoggedUser = user;
        }
        public void FilterOpportunity(string filterOpportunity)
        {
            Preferences.Set("filtroJson", filterOpportunity, "filtroAvanzado");

            initialFilter = filterOpportunity;
        }

        public void FilterOrder(string filterOrder)
        {
            Preferences.Set("filtroOrderJson", filterOrder, "filtroAvanzadoOrder");

            initialFilterOrder = filterOrder;
        }

        public void ClearLoggedUser()
        {
            Preferences.Clear("loginData");
            LoggedUser = null;
        }
        public void ClearFiltroAvansado()
        {
            Preferences.Clear("filtroAvansado");
            initialFilter = null;
        }

        public void ClearFilterOrder()
        {
            Preferences.Clear("filtroAvanzadoOrder");
            initialFilterOrder = null;
        }
    }
}
