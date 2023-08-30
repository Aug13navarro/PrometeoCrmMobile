using Core.Model;
using System.Collections.Generic;

namespace Core.Helpers
{
    public class DataMobileModel
    {
        public List<Company> Companies { get; set; }
        public List<Provider> Providers { get; set; }
        public List<TransportCompany> Transports { get; set; }
        public List<PaymentCondition> PaymentConditions { get; set; }
        public List<User> AssistantComercial { get; set; }
        public List<PaymentMethod> PaymentMethod { get; set; }
        public List<Incoterm> Incoterms { get; set; }
        public List<FreightInCharge> Freights { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Product> ProductsPresentations { get; set; }
    }
}
