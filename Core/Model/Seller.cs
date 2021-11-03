using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Seller
    {
        public int id { get; set; }
        public int idUser { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string abbreviation { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        //public List<Role> roles { get; set; }
        public string language { get; set; }
        public object coordinatorId { get; set; }
        public object coordinator { get; set; }
        public object personalInCharge { get; set; }
    }
}
