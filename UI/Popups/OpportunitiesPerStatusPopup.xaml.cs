using Core.Model;
using Core.Model.Enums;
using Core.ViewModels.Model;
using MvvmCross.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpportunitiesPerStatusPopup : BasePopupPage
    {
        public IList<OpportunitiesPerStatusVM> opps { get; set; }

        public OpportunitiesPerStatusPopup(MvxObservableCollection<Opportunity> opportunities)
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = true;

            opps = new List<OpportunitiesPerStatusVM>();

            SepararOportunidades(opportunities);


            try
            {
                //List<OpportunityStatus> status = opportunities.Select(x => x.Status).Distinct().ToList();

                //foreach (OpportunityStatus item in status)
                //{
                //    opps.Add(
                //        new OpportunitiesPerStatusVM()
                //        {
                //            opportunityStatus = item,
                //            Amount = opportunities.Where(x => x.Status == item).Count()
                //        });
                //}

                stateList.ItemsSource = opps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void CancelButtonClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(false);
            NotifyDismiss();
        }

        public void SepararOportunidades(MvxObservableCollection<Opportunity> opportunities)
        {
            var IntAnalisis = new OpportunitiesPerStatusVM
            {
                opportunityStatus = "Analisis"
            };
            var IntPropuesta = new OpportunitiesPerStatusVM
            {
                opportunityStatus = "Propuesta"
            };
            var IntNegociacion = new OpportunitiesPerStatusVM
            {
                opportunityStatus = "Negociacion"
            };
            var IntCerradaGanada = new OpportunitiesPerStatusVM
            {
                opportunityStatus = "Cerrada Ganada"
            };
            var IntCerradaPerdida = new OpportunitiesPerStatusVM
            {
                opportunityStatus = "Cerrada Perdida"
            };

            foreach (var item in opportunities)
            {
                switch (item.opportunityStatus.Id)
                {
                    case 1:
                        IntAnalisis.Amount++;
                        break;
                    case 2:
                        IntPropuesta.Amount++;
                        break;
                    case 3:
                        IntNegociacion.Amount++;
                        break;
                    case 4:
                        IntCerradaGanada.Amount++;
                        break;
                    case 5:
                        IntCerradaPerdida.Amount++;
                        break;
                }
            }

            opps.Add(IntAnalisis); opps.Add(IntPropuesta); opps.Add(IntNegociacion); opps.Add(IntCerradaGanada); opps.Add(IntCerradaPerdida);
        }
    }
}