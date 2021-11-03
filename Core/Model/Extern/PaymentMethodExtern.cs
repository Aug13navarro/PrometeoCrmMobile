using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Extern
{
    [Serializable]
    public class PaymentMethodExtern
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
