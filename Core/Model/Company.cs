using System.Collections.Generic;

namespace Core.Model
{
    public class Company
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public int? externalId { get; set; }
        public List<CompanyOrderType> CompanyOrderTypes{ get; set; }
        public int? externalErpId { get; set; }
    }
}
