using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model;
using Core.ViewModels.Model;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

namespace Core
{
    public class ApplicationData : MvxNotifyPropertyChanged
    {
        private static User loggedUser;
        private string initialFilter;

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
        public string InitialFilter
        {
            get
            {
                if (initialFilter == null)
                {
                    var dateToStr = Preferences.Get("dateTo", null, "filtroAvansado");

                    if (dateToStr == null || string.IsNullOrWhiteSpace(dateToStr))
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

                        initialFilter = "";
                    }
                }

                return initialFilter;
            }
            private set => SetProperty(ref initialFilter, value);
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
        public void FilterAssignment(string filterOpportunity)
        {
            //var customerId = filterAssignment.customers.FirstOrDefault();
            //var statusId = filterAssignment.status.FirstOrDefault();
            //var productsId = filterAssignment.products.FirstOrDefault();

            //Preferences.Set("dataFrom", filterAssignment.dateFrom.ToString(), "filtroAvanzado");
            //Preferences.Set("dateTo", filterAssignment.dateTo.ToString(), "filtroAvanzado");
            //Preferences.Set("statusId", statusId.id, "filtroAvanzado");
            //Preferences.Set("customerId", customerId.id, "filtroAvanzado");
            //Preferences.Set("productsId", productsId.id, "filtroAvanzado");
            //Preferences.Set("priceFrom", filterAssignment.priceFrom.ToString(), "filtroAvanzado");
            //Preferences.Set("priceTo", filterAssignment.priceTo.ToString(), "filtroAvanzado");

            initialFilter = filterOpportunity;
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
    }
}
