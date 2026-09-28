namespace ELOR.Razzle.DTO
{
    public sealed class NoteDTO
    {
        public uint Id { get; init; }
        public long CreatedAt { get; init; }
        public string Text { get; init; }
        public bool IsCropped { get; init; }
        public uint? TaskId { get; init; }
        public List<uint> TagIds { get; init; }
        public CashFlowDTO CashFlow { get; init; }
    }
}
