using System;

namespace Core.Model.Extern
{
    [Serializable]
    public class CompanyExtern
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public int? externalId { get; set; }
    }
}
