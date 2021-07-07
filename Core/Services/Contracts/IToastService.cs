namespace Core.Services.Contracts
{
    public interface IToastService
    {
        void ShowError(string message);
        void ShowOk(string message);
    }
}
