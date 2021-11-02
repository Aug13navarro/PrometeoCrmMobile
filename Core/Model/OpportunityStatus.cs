using System;

namespace Core.Model
{
    public class OpportunityStatus
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string nameCacheEsp => DetectarEstado(this.Id);
        public string nameCacheEn => DetectStatus(this.Id);

        private string DetectStatus(int id)
        {
            switch (id)
            {
                case 1:
                    return "Analysis";
                case 2:
                    return "Proposal";
                case 3:
                    return "Negotiation";
                case 4:
                    return "Closed Won";
                case 5:
                    return "losed Loss";
            }

            return "";
        }

        private string DetectarEstado(int id)
        {
            switch (id)
            {
                case 1:
                    return "Analisís";
                case 2:
                    return "Propuesta";
                case 3:
                    return "Negociación";
                case 4:
                    return "Cerrada Ganada";
                case 5:
                    return "Cerrada Perdida";
            }

            return "";
        }
    }
}
