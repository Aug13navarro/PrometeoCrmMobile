﻿using SQLite;
using System;

namespace Core.Model
{
    public class PaymentCondition
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
        public int CompanyId { get; set; }
        public object Company { get; set; }
        public string Abbreviature { get; set; }
        public int Surcharge { get; set; }
        public bool Active { get; set; }

    }
}
