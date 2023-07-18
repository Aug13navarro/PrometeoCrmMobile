using System;

namespace Core.Model.Extern
{
    [Serializable]
    public class IncotermExtern
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ExternalId { get; set; }
    }
}
