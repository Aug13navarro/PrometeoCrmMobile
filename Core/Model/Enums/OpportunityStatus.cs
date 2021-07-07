using System.ComponentModel;

namespace Core.Model.Enums
{
    public enum OpportunityStatus
    {
        [Description("Análisis")]
        Analysis,

        [Description("Propuesta")]
        Proposition,

        [Description("Negociación")]
        Negotiation,

        [Description("Cerrada Ganada")]
        ClosedWon,

        [Description("Cerrada Perdida")]
        ClosedLost,
    }
}
