using ELOR.Razzle.Data.Enums;

namespace ELOR.Razzle.Data.Entities
{
    public sealed class TaskEntity
    {
        public uint Id { get; set; }
        public long CreatedAt { get; set; }
        public TaskFlags Flags { get; set; }
        public string Name { get; set; } = string.Empty;
        public long? CompletedAt { get; set; }
        public uint? CompletionNoteId { get; set; }
        public Note CompletionNote { get; set; }

        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<TagTask> TagTasks { get; set; } = new List<TagTask>();
    }
}
