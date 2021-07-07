namespace Core.Model
{
    public class TypeOfCustomer
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CustomerTypeId { get; set; }
        public CustomerType CustomerType { get; set; }
    }
}
