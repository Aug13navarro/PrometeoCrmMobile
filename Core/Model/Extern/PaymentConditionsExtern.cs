using System;

namespace Core.Model.Extern
{
    [Serializable]
    public class PaymentConditionsExtern
    {
        public int id { get; set; }
        public string description { get; set; }
        public int code { get; set; }
        public int companyId { get; set; }
        public object company { get; set; }
        public string abbreviature { get; set; }
        public int surcharge { get; set; }
        public DateTime? baseDate { get; set; }
        public bool active { get; set; }
    }
}
