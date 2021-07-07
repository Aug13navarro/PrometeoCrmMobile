using System;

namespace Core.Model
{
    public class TreatmentAlert
    {
        public int Id { get; set; }
        public int TreatmentId { get; set; }
        public int GrdId { get; set; }
        public string Name { get; set; }
        public DateTime InsertDate { get; set; }
        public bool Viewed { get; set; }

        public Assignment Assignment { get; set; }
    }
}
