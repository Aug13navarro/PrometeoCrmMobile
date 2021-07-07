using Core.Model;
using MvvmCross.ViewModels;

namespace Core.ViewModels.Model
{
    public class CustomerVm : MvxNotifyPropertyChanged
    {
        public Customer Customer { get; set; }

        private bool isContactsVisible;
        public bool IsContactsVisible
        {
            get => isContactsVisible;
            set => SetProperty(ref isContactsVisible, value);
        }
    }
}
