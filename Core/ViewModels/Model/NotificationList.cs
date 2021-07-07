using Core.Model;
using MvvmCross.ViewModels;

namespace Core.ViewModels.Model
{
    public class NotificationList : MvxObservableCollection<TreatmentAlert>
    {
        public string Heading { get; set; }
    }
}
