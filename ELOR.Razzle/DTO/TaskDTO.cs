namespace ELOR.Razzle.DTO
{
    // TODO: is finished and other.
    public sealed class TaskDTO
    {
        public uint Id { get; init; }
        public long CreatedAt { get; init; }
        public string Name { get; init; }
        public bool IsCompleted { get; init; }
        public uint? CompletionNoteId { get; init; }
    }
}
