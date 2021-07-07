using System.ComponentModel;

namespace Core.Model.Enums
{
    public enum ClosedLostStatusCause
    {
        [Description("Competencia")]
        Competence,

        [Description("Sin Fondos")]
        NoFunds,

        [Description("Sin Decisión")]
        NoDecision,

        [Description("Precio")]
        Price,

        [Description("Otros")]
        Other,
    }
}
