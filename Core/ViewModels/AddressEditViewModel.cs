using System.Threading.Tasks;
using Core.Model;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class AddressEditViewModel : MvxViewModel<Customer>
    {
        // Properties
        public CustomerAddress NewAddress { get; } = new CustomerAddress();

        // Commands
        public IMvxAsyncCommand SaveAddressCommand { get; }

        // Fields
        private Customer customer;

        // Services
        private readonly IMvxNavigationService navigationService;

        public AddressEditViewModel(IMvxNavigationService navigationService)
        {
            this.navigationService = navigationService;
            SaveAddressCommand = new MvxAsyncCommand(SaveAddressAsync);
        }

        public override void Prepare(Customer theCustomer)
        {
            customer = theCustomer;
        }

        private async Task SaveAddressAsync()
        {
            customer.Addresses.Add(NewAddress);
            await navigationService.Close(this);
        }
    }
}
