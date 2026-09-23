using ELOR.Razzle.Data.Enums;

namespace ELOR.Razzle.Data.Entities
{
    public sealed class CashFlow
    {
        public uint Id { get; set; }
        public uint NoteId { get; set; }
        public CashFlowType? FlowType { get; set; }
        public uint Amount { get; set; }

        public Note Note { get; set; } = null;
    }
}
