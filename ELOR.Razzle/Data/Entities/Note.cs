using ELOR.Razzle.Data.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ELOR.Razzle.Data.Entities
{
    public sealed class Note
    {
        public uint Id { get; set; }
        public long CreatedAt { get; set; }
        public NoteFlags Flags { get; set; }
        public string Text { get; set; } = string.Empty;
        public uint? TaskId { get; set; }
        public TaskEntity Task { get; set; }
        public CashFlow CashFlow { get; set; }

        public ICollection<TagNote> TagNotes { get; set; } = new List<TagNote>();
    }
}
