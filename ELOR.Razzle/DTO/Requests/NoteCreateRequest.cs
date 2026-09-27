using ELOR.Razzle.Data.Enums;

namespace ELOR.Razzle.DTO.Requests
{
    public sealed class NoteCreateRequest : IIdempotentRequest
    {
        public string Text { get; set; }
        public List<uint> TagIds { get; set; } = new List<uint>();
        public CashFlowType? CashFlowType { get; set; }
        public uint Amount { get; set; }
        public uint TaskId { get; set; }

        public int RandomId { get; set; }
    }
}
