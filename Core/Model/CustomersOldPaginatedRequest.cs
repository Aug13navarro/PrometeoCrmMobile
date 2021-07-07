namespace Core.Model
{
    public class CustomersOldRequest
    {
        public int UserId { get; set; }
        public string Query { get; set; }
        public int CompanyId { get; set; }
        public bool IsParentCustomer { get; set; }
    }
}
