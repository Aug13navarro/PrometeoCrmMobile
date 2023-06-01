namespace Core.Model
{
    public class Company
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public int? ExternalId { get; set; }
        public int? externalErpId { get; set; }
        public bool? ExportPv { get; set; }
    }
}
