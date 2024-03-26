namespace Core.Model
{
    public class Sequence
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
        public int ActualValue { get; set; }
        public int? SaleTypeId { get; set; }
        public int? CompanyId { get; set; }
    }
}